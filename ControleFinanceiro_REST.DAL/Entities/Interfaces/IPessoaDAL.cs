using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface IPessoaDAL
{
    IQueryable<Pessoa> GetQuery(bool asNoTracking = true, params Expression<Func<Pessoa, object>>[] includes);
    Task<List<PessoaDTO>> GetAsync();
    Task<PessoaDTO?> GetByIdAsync(Guid id);
    Task<PessoaDTO> CreateAsync(PessoaDTO dto);
    Task<PessoaDTO?> EditAsync(Guid id, PessoaDTO dto);
    Task<bool> DeleteAsync(Guid id);
}
