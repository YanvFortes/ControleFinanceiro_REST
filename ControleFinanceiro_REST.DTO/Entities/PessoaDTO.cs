namespace ControleFinanceiro_REST.DTO.Entities;

public class PessoaDTO
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = null!;
    public int Idade { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataEdicao { get; set; }
}
