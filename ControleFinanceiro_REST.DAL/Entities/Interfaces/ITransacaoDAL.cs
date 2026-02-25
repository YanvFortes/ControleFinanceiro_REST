using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface ITransacaoDAL
{
    IQueryable<Transacao> GetQuery(bool asNoTracking = true, params Expression<Func<Transacao, object>>[] includes);
    Task<List<TransacaoDTO>> GetAsync();
    Task<TransacaoDTO?> GetByIdAsync(Guid id);
    Task<TransacaoDTO> CreateAsync(TransacaoDTO dto);
    Task<TransacaoDTO?> EditAsync(Guid id, TransacaoDTO dto);
    Task<bool> DeleteAsync(Guid id);
}
