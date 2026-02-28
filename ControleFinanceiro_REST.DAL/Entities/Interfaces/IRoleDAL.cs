using Microsoft.AspNetCore.Identity;
using ControleFinanceiro_REST.DTO.Entities;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface IRoleDAL
{
    IQueryable<IdentityRole> GetQuery(
        bool asNoTracking = true,
        params System.Linq.Expressions.Expression<Func<IdentityRole, object>>[] includes);
    Task CriarAsync(RoleDTO dto);
    Task AtualizarAsync(RoleDTO dto);
    Task ExcluirAsync(string id);
}
