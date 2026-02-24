using ControleFinanceiro_REST.DTO.Enums;

namespace ControleFinanceiro_REST.DTO.Entities;

public class CategoriaDTO
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = null!;
    public FinalidadeCategoriaEnum Finalidade { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
