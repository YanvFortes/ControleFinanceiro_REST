using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface ITransacaoBLL
{
    Task<RetornoDTO<bool>> CriarAsync(TransacaoDTO dto);
    Task<RetornoDTO<bool>> AtualizarAsync(TransacaoDTO dto);
    Task<RetornoDTO<bool>> ExcluirAsync(Guid id);

    Task<TransacaoDTO?> ObterPorIdAsync(Guid id);
    Task<PagedResultDTO<TransacaoDTO>> ObterPaginadoAsync(
        int page,
        int pageSize,
        string? search,
        Guid? usuarioId = null);

    Task<TotaisTransacaoDTO> ObterTotaisAsync(Guid? usuarioId = null);
}
