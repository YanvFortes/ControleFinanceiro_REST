using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoBLL BLL;
    public AutenticacaoController(IAutenticacaoBLL bll)
    {
        BLL = bll;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(AutenticacaoDTO entrada)
    {
        var ret = await BLL.Login(entrada);

        return ret.Mensagem switch
        {
            "ok" => Ok(ret),
            _ => Unauthorized(ret)
        };
    }
}
