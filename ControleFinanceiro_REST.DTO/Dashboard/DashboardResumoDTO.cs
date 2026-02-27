namespace ControleFinanceiro_REST.DTO.Dashboard;

public class DashboardResumoDTO
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
