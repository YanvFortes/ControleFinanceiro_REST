using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ControleFinanceiro_REST.BLL.Utils;

/// <summary>
/// Responsável por resolver o usuário da aplicação
/// com base no AspNetUserId presente no token.
/// 
/// Atua como ponte entre:
/// - ASP.NET Identity
/// - Tabela Usuario do domínio
/// </summary>
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

    /// <summary>
    /// Obtém ID do usuário no Identity.
    /// </summary>
    public string? ObterAspNetUserId()
        => _tokenContexto.ObterClaim(ClaimTypes.NameIdentifier);

    /// <summary>
    /// Converte AspNetUserId para Id da entidade Usuario.
    /// 
    /// Isso permite isolar dados por usuário no domínio.
    /// </summary>
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