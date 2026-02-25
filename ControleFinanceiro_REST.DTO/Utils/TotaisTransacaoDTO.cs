namespace ControleFinanceiro_REST.DTO.Utils;

public class TotaisTransacaoDTO
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
