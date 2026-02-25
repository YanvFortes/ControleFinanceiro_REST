using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface ICategoriaBLL
{
    Task<RetornoDTO<bool>> CriarAsync(CategoriaDTO dto);
    Task<RetornoDTO<bool>> AtualizarAsync(CategoriaDTO dto);
    Task<RetornoDTO<bool>> ExcluirAsync(Guid id);

    Task<CategoriaDTO?> ObterPorIdAsync(Guid id);

    Task<PagedResultDTO<CategoriaDTO>> ObterPaginadoAsync(
        int page,
        int pageSize,
        string? search,
        Guid? usuarioId = null);
}
