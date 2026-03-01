using ControleFinanceiro_REST.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_REST.DTO.Entities;

public class TransacaoDTO
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Descrição é obrigatória.")]
    [MaxLength(400, ErrorMessage = "Descrição deve ter no máximo 400 caracteres.")]
    public string Descricao { get; set; } = null!;

    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Tipo é obrigatório.")]
    public TipoTransacaoEnum Tipo { get; set; }

    [Required(ErrorMessage = "Categoria é obrigatória.")]
    public Guid CategoriaId { get; set; }

    [Required(ErrorMessage = "Pessoa é obrigatória.")]
    public Guid PessoaId { get; set; }

    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
