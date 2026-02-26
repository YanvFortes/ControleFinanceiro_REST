using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ControleFinanceiro_REST.BLL.Utils;

public class TokenContexto : ITokenContexto
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenContexto(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ObterToken()
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        return header?.Replace("Bearer ", "");
    }

    public string? ObterClaim(string nome)
    {
        var claim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == nome);
        return claim?.Value;
    }
}
