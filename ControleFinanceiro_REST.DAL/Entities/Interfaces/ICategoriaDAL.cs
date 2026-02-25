using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface ICategoriaDAL
{
    IQueryable<Categoria> GetQuery( bool asNoTracking = true, params Expression<Func<Categoria, object>>[] includes);
    Task<List<CategoriaDTO>> GetAsync();
    Task<CategoriaDTO?> GetByIdAsync(Guid id);
    Task<CategoriaDTO> CreateAsync(CategoriaDTO dto);
    Task<CategoriaDTO?> EditAsync(Guid id, CategoriaDTO dto);
    Task<bool> DeleteAsync(Guid id);
}
