namespace ControleFinanceiro_REST.DTO.Dashboard;

public class DashboardTotalItemDTO
{
    public string Nome { get; set; } = null!;
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
