using ControleFinanceiro_REST.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_REST.DTO.Entities;

public class CategoriaDTO
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Descrição é obrigatória.")]
    [MaxLength(400, ErrorMessage = "Descrição deve ter no máximo 400 caracteres.")]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "Finalidade é obrigatória.")]
    public FinalidadeCategoriaEnum Finalidade { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
