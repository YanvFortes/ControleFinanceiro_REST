namespace ControleFinanceiro_REST.DTO.Entities;

public class UsuarioDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public int Idade { get; set; }
    public int TipoUsuarioId { get; set; }
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
    public string AspNetUserId { get; set; } = null!;
}
