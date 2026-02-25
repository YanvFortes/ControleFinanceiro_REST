using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface IRoleBLL
{
    Task<PagedResultDTO<RoleDTO>> ObterPaginadoAsync(int page, int size, string? search);
    Task<RoleDTO?> ObterPorIdAsync(string id);
    Task<RetornoDTO<bool>> CriarAsync(RoleDTO dto);
    Task<RetornoDTO<bool>> AtualizarAsync(RoleDTO dto);
    Task<RetornoDTO<bool>> ExcluirAsync(string id);
}
