using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleFinanceiro_REST.BLL.Utils;

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

    private string EncriptarAcesso(int tipoUsuario)
    {
        return _encriptador.Encriptar(Guid.NewGuid().ToString() + "." + _encriptador.Encriptar(tipoUsuario.ToString()) + "." + _encriptador.Encriptar(DateTime.Now.Millisecond.ToString()));
    }

    public string? ObterCampoDiretoDoToken(string token, string campo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            Console.WriteLine("Token inválido.");
            return null;
        }

        var jwtToken = tokenHandler.ReadJwtToken(token);
        var acessoClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == campo);

        return acessoClaim?.Value;
    }

    public int DesencriptarAcesso(string tipoUsuario)
    {
        var temp = _encriptador.Desencriptar(tipoUsuario);
        return int.Parse(_encriptador.Desencriptar(temp.Split('.')[1]));
    }
    public string GerarJWT(string usuarioId, string usuarioNome, int tipoUsuario)
    {
        try
        {
            var clienteData = new[]
            {
                    new Claim("id", usuarioId.ToString()),
                    new Claim("name", usuarioNome),
                    new Claim("acesso", EncriptarAcesso(tipoUsuario))
                };

            var chaveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_chaveSecreta));
            var credenciais = new SigningCredentials(chaveSecreta, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(clienteData),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = credenciais,
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public ClaimsPrincipal ValidarToken(string token)
    {
        try
        {
            var chaveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_chaveSecreta));

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
            var principal = tokenHandler.ValidateToken(token, parametrosValidacao, out SecurityToken validatedToken);

            return principal;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
