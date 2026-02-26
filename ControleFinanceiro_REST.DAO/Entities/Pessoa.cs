namespace ControleFinanceiro_REST.DAO.Entities;

public partial class Pessoa
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = null!;
    public int Idade { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;


    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
