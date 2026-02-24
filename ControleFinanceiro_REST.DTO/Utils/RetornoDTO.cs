namespace ControleFinanceiro_REST.DTO.Utils;

public class RetornoDTO<T>
{
    public string Mensagem { get; set; } = string.Empty;
    public T Conteudo { get; set; }

    public RetornoDTO() { }

    public RetornoDTO(T conteudo)
    {
        Conteudo = conteudo;
        Mensagem = "ok";
    }

    public RetornoDTO(T conteudo, string mensagem)
    {
        Conteudo = conteudo;
        Mensagem = mensagem;
    }

    public static RetornoDTO<T> Fail(string erro) =>
        new RetornoDTO<T>(default!, erro);
}
