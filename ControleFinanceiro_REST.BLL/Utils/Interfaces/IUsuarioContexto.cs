namespace ControleFinanceiro_REST.BLL.Utils.Interfaces;

public interface IUsuarioContexto
{
    string? ObterAspNetUserId();
    Task<Guid?> ObterUsuarioIdAsync();
}
