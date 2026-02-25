using System.Security.Claims;

namespace ControleFinanceiro_REST.BLL.Utils.Interfaces;

public interface IGeradorToken
{
        public int DesencriptarAcesso(string tipoUsuario);
        string? ObterCampoDiretoDoToken(string token, string campo);
        public string GerarJWT(string usuarioId, string usuarioNome, int tipoUsuario);
        public ClaimsPrincipal ValidarToken(string token);
}
