using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface IUsuarioBLL
{
    Task<PagedResultDTO<UsuarioDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search);

    Task<UsuarioDTO?> ObterPorIdAsync(Guid id);
    Task<RetornoDTO<bool>> CriarAsync(UsuarioDTO dto);
    Task<RetornoDTO<bool>> AtualizarAsync(UsuarioDTO dto);
    Task<RetornoDTO<bool>> ExcluirAsync(Guid id);
}
