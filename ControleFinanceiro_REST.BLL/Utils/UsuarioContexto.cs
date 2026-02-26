using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ControleFinanceiro_REST.BLL.Utils;

public class UsuarioContexto : IUsuarioContexto
{
    private readonly ITokenContexto _tokenContexto;
    private readonly IUsuarioDAL _usuarioDAL;

    public UsuarioContexto(
        ITokenContexto tokenContexto,
        IUsuarioDAL usuarioDAL)
    {
        _tokenContexto = tokenContexto;
        _usuarioDAL = usuarioDAL;
    }

    public string? ObterAspNetUserId()
        => _tokenContexto.ObterClaim(ClaimTypes.NameIdentifier);

    public async Task<Guid?> ObterUsuarioIdAsync()
    {
        var aspId = ObterAspNetUserId();

        if (string.IsNullOrWhiteSpace(aspId))
            return null;

        return await _usuarioDAL.GetQuery()
            .Where(u => u.AspNetUserId == aspId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();
    }
}