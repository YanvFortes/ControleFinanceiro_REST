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

public class AutenticacaoBLL : IAutenticacaoBLL
{
    private readonly IUsuarioDAL _usuarioDAL;
    private readonly IEncriptador _encriptador;
    private readonly IGeradorToken _geradorToken;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    public AutenticacaoBLL(IUsuarioDAL usuarioDAL, IEncriptador encriptador,
        IGeradorToken geradorToken, SignInManager<ApplicationUser>
        signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _usuarioDAL = usuarioDAL;
        _encriptador = encriptador;
        _geradorToken = geradorToken;
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<RetornoDTO<string>> Login(AutenticacaoDTO entrada)
    {
        try
        {

            var user = await _userManager.FindByEmailAsync(entrada.Usuario);
            if (user == null)
                return RetornoDTO<string>.Fail("Usuário não encontrado.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, entrada.Senha, false);
            if (!result.Succeeded)
                return RetornoDTO<string>.Fail("Credenciais inválidas.");

            var usuario = await _usuarioDAL.GetUsuarioPorEmailAsync(user.Email);
            if (usuario == null)
                return RetornoDTO<string>.Fail("Usuário não encontrado no cadastro.");

            if (usuario?.TipoUsuario == null)
                return RetornoDTO<string>.Fail("Tipo de usuário não configurado.");

            var jwt = await GenerateJwtToken(user, usuario.TipoUsuario.Descricao);
            return new RetornoDTO<string>(jwt, "ok");

        }
        catch (Exception ex)
        {
            return RetornoDTO<string>.Fail(ex.Message);
        }
    }

    private async Task<string> GenerateJwtToken(IdentityUser user, string role)
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthToken:ChaveSecreta"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["AuthToken:Issuer"],
            audience: _configuration["AuthToken:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_configuration["AuthToken:TokenExpiration"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
