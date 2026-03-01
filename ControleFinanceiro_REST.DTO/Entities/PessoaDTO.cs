using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro_REST.DTO.Entities;

public class PessoaDTO
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
    public string Nome { get; set; } = null!;

    [Range(0, 150, ErrorMessage = "Idade inválida.")]
    public int Idade { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
