using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Responsável por gerar e validar tokens JWT.
/// 
/// Encapsula:
/// - Geração de claims
/// - Assinatura do token
/// - Validação de integridade
/// - Extração de campos
/// </summary>
public class GeradorToken : IGeradorToken
{
    private readonly string _chaveSecreta;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IEncriptador _encriptador;

    public GeradorToken(IConfiguration configuration, IEncriptador encriptador)
    {
        _chaveSecreta = configuration["Authtoken:ChaveSecreta"];
        _issuer = configuration["Authtoken:Issuer"];
        _audience = configuration["Authtoken:Audience"];
        _encriptador = encriptador;
    }

    /// <summary>
    /// Gera campo "acesso" criptografado.
    /// 
    /// Estrutura:
    /// GUID.random + tipoUsuario + timestamp
    /// 
    /// Isso evita exposição direta do nível de acesso no token.
    /// </summary>
    private string EncriptarAcesso(int tipoUsuario)
    {
        return _encriptador.Encriptar(
            Guid.NewGuid() + "." +
            _encriptador.Encriptar(tipoUsuario.ToString()) + "." +
            _encriptador.Encriptar(DateTime.Now.Millisecond.ToString()));
    }

    /// <summary>
    /// Extrai claim diretamente do token sem validar assinatura.
    /// </summary>
    public string? ObterCampoDiretoDoToken(string token, string campo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
            return null;

        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == campo)?.Value;
    }

    /// <summary>
    /// Descriptografa campo de acesso para obter tipoUsuario.
    /// </summary>
    public int DesencriptarAcesso(string tipoUsuario)
    {
        var temp = _encriptador.Desencriptar(tipoUsuario);
        return int.Parse(_encriptador.Desencriptar(temp.Split('.')[1]));
    }

    /// <summary>
    /// Gera token JWT assinado.
    /// </summary>
    public string GerarJWT(string usuarioId, string usuarioNome, int tipoUsuario)
    {
        var claims = new[]
        {
            new Claim("id", usuarioId),
            new Claim("name", usuarioNome),
            new Claim("acesso", EncriptarAcesso(tipoUsuario))
        };

        var chaveSecreta = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_chaveSecreta));

        var credenciais = new SigningCredentials(
            chaveSecreta,
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credenciais,
            Issuer = _issuer,
            Audience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Valida token JWT verificando:
    /// - Assinatura
    /// - Emissor
    /// - Audiência
    /// - Expiração
    /// </summary>
    public ClaimsPrincipal ValidarToken(string token)
    {
        var chaveSecreta = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_chaveSecreta));

        var parametrosValidacao = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,

            ValidateAudience = true,
            ValidAudience = _audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = chaveSecreta,

            ClockSkew = TimeSpan.Zero,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(token, parametrosValidacao, out _);
    }
}