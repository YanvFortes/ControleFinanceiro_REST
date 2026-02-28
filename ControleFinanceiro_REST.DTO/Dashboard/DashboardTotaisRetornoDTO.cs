namespace ControleFinanceiro_REST.DTO.Dashboard;

public class DashboardTotaisResponseDTO
{
    public List<DashboardTotalItemDTO> Itens { get; set; } = new();
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
