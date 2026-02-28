namespace ControleFinanceiro_REST.DTO.Request;

public class AtualizarUsuarioRequestDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TipoUsuarioId { get; set; }
    public string? Senha { get; set; }
}
