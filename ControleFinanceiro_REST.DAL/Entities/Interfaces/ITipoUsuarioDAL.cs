namespace ControleFinanceiro_REST.DAL.Entities.Interfaces;

public interface ITipoUsuarioDAL
{
    Task<string?> ObterDescricaoPorIdAsync(int tipoUsuarioId);
}
