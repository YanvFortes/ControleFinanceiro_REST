using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface IUsuarioDAL
{
    IQueryable<Usuario> GetQuery(bool asNoTracking = true, params Expression<Func<Usuario, object>>[] includes);
    Task<List<UsuarioDTO>> GetAsync();
    Task<UsuarioDTO?> GetByIdAsync(Guid id);
    Task<Usuario?> GetUsuarioPorEmailAsync(string email);
    Task<UsuarioDTO> CreateAsync(UsuarioDTO dto);
    Task<UsuarioDTO?> EditAsync(Guid id, UsuarioDTO dto);
    Task<bool> DeleteAsync(Guid id);
}
