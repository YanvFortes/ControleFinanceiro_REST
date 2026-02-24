using ControleFinanceiro_REST.DTO.Enums;

namespace ControleFinanceiro_REST.DAO.Entities;

public partial class Categoria
{
    public Guid Id { get; set; }

    public string Descricao { get; set; } = null!; 
    public FinalidadeCategoriaEnum Finalidade { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
