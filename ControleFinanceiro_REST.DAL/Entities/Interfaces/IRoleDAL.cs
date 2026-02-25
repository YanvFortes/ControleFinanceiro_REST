using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using ControleFinanceiro_REST.DTO.Entities;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface IRoleDAL
{
    IQueryable<IdentityRole> GetQuery();
    Task CriarAsync(RoleDTO dto);
    Task AtualizarAsync(RoleDTO dto);
    Task ExcluirAsync(string id);
}
