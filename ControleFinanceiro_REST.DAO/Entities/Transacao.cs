using ControleFinanceiro_REST.DTO.Enums;

namespace ControleFinanceiro_REST.DAO.Entities;

public partial class Transacao
{
    public Guid Id { get; set; }

    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public TipoTransacaoEnum Tipo { get; set; }

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
