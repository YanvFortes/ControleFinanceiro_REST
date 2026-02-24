namespace ControleFinanceiro_REST.DAO.Entities;

public partial class Tipousuario
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
