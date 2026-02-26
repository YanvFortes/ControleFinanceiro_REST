using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface IPessoaBLL
{
    Task<PagedResultDTO<PessoaDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search);

    Task<PessoaDTO?> ObterPorIdAsync(Guid id);

    Task<RetornoDTO<bool>> CriarAsync(PessoaDTO dto);
    Task<RetornoDTO<bool>> AtualizarAsync(PessoaDTO dto);
    Task<RetornoDTO<bool>> ExcluirAsync(Guid id);
}
