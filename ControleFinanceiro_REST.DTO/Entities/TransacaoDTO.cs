using ControleFinanceiro_REST.DTO.Enums;

namespace ControleFinanceiro_REST.DTO.Entities;

public class TransacaoDTO
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public TipoTransacaoEnum Tipo { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid PessoaId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
