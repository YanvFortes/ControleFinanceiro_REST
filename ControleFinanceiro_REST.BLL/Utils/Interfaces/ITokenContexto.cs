namespace ControleFinanceiro_REST.BLL.Utils.Interfaces;

public interface ITokenContexto
{
    public string? ObterToken();
    public string? ObterClaim(string nome);
}
