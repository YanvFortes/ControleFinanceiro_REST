using ControleFinanceiro_REST.DTO.Dashboard;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface IDashboardBLL
{
    Task<DashboardResumoDTO> ObterResumoAsync(int dias);
    Task<List<DashboardGraficoDiaDTO>> ObterGastosPorDiaAsync(int dias);
    Task<List<DashboardGraficoPessoaDTO>> ObterGastosPorPessoaAsync(int dias);
    Task<DashboardTotaisResponseDTO> ObterTotaisPorPessoaAsync(int dias);
    Task<DashboardTotaisResponseDTO> ObterTotaisPorCategoriaAsync(int dias);
}
