namespace ControleFinanceiro_REST.DAO.Entities;

public partial class Usuario
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = null!;
    public int Idade { get; set; }

    public int TipoUsuarioId { get; set; }
    public Tipousuario TipoUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;

    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }

    public string AspNetUserId { get; set; } = null!;
    public virtual AspNetUser? User { get; set; }

    public ICollection<Pessoa> Pessoas { get; set; } = new List<Pessoa>();
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
