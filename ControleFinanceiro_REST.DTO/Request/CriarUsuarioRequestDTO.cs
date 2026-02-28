namespace ControleFinanceiro_REST.DTO.Request;

public class CriarUsuarioRequestDTO
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public int TipoUsuarioId { get; set; }
}
