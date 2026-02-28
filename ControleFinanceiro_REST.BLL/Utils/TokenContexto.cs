using ControleFinanceiro_REST.BLL.Utils.Interfaces;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Responsável por acessar informações do token
/// diretamente do HttpContext atual.
/// 
/// Permite recuperar:
/// - Token bruto
/// - Claims específicas
/// </summary>
public class TokenContexto : ITokenContexto
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenContexto(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ObterToken()
    {
        var header = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers["Authorization"]
            .FirstOrDefault();

        return header?.Replace("Bearer ", "");
    }

    public string? ObterClaim(string nome)
    {
        return _httpContextAccessor
            .HttpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == nome)?.Value;
    }
}