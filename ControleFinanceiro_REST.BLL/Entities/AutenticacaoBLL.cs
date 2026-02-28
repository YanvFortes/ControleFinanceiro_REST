using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleFinanceiro_REST.BLL.Entities;

/// <summary>
/// Responsável pelo fluxo de autenticação do sistema.
/// 
/// Integra:
/// - ASP.NET Identity (validação de credenciais)
/// - Tabela interna de Usuário (regras de domínio)
/// - Geração de Token JWT
/// 
/// Esta camada centraliza toda lógica de login,
/// mantendo o controller apenas como ponto de entrada HTTP.
/// </summary>
public class AutenticacaoBLL : IAutenticacaoBLL
{
    private readonly IUsuarioDAL _usuarioDAL;
    private readonly IEncriptador _encriptador;
    private readonly IGeradorToken _geradorToken;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AutenticacaoBLL(
        IUsuarioDAL usuarioDAL,
        IEncriptador encriptador,
        IGeradorToken geradorToken,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _usuarioDAL = usuarioDAL;
        _encriptador = encriptador;
        _geradorToken = geradorToken;
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Realiza autenticação do usuário.
    /// 
    /// Fluxo:
    /// 1. Localiza usuário no Identity pelo e-mail.
    /// 2. Valida senha utilizando SignInManager.
    /// 3. Busca usuário na tabela de domínio (Usuario).
    /// 4. Valida se possui tipo/perfil configurado.
    /// 5. Gera JWT contendo claims de identidade e role.
    /// </summary>
    public async Task<RetornoDTO<string>> Login(AutenticacaoDTO entrada)
    {
        try
        {
            // 1. Verifica se usuário existe no Identity
            var user = await _userManager.FindByEmailAsync(entrada.Usuario);
            if (user == null)
                return RetornoDTO<string>.Fail("Usuário não encontrado.");

            // 2. Valida senha
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, entrada.Senha, false);

            if (!result.Succeeded)
                return RetornoDTO<string>.Fail("Credenciais inválidas.");

            // 3. Busca usuário na tabela de domínio
            var usuario = await _usuarioDAL
                .GetUsuarioPorEmailAsync(user.Email);

            if (usuario == null)
                return RetornoDTO<string>.Fail("Usuário não encontrado no cadastro.");

            // 4. Garante que possui perfil configurado
            if (usuario?.TipoUsuario == null)
                return RetornoDTO<string>.Fail("Tipo de usuário não configurado.");

            // 5. Gera token JWT com role vinculada
            var jwt = await GenerateJwtToken(user, usuario.TipoUsuario.Descricao);

            return new RetornoDTO<string>(jwt, "ok");
        }
        catch (Exception ex)
        {
            // Retorna erro controlado (não expõe stacktrace em produção)
            return RetornoDTO<string>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Gera token JWT contendo:
    /// - Id do usuário
    /// - Email
    /// - Nome
    /// - Role
    /// - JTI único
    /// 
    /// Configurações são carregadas via IConfiguration.
    /// </summary>
    private async Task<string> GenerateJwtToken(
        IdentityUser user,
        string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Chave simétrica definida no appsettings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["AuthToken:ChaveSecreta"]));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["AuthToken:Issuer"],
            audience: _configuration["AuthToken:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                int.Parse(_configuration["AuthToken:TokenExpiration"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}