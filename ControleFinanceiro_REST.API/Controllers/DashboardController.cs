using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardBLL _bll;

    public DashboardController(IDashboardBLL bll)
        => _bll = bll;

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("Resumo")]
    public async Task<IActionResult> Resumo([FromQuery] int dias = 7)
        => Ok(await _bll.ObterResumoAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("GastosPorDia")]
    public async Task<IActionResult> GastosPorDia([FromQuery] int dias = 7)
        => Ok(await _bll.ObterGastosPorDiaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("GastosPorPessoa")]
    public async Task<IActionResult> GastosPorPessoa([FromQuery] int dias = 7)
        => Ok(await _bll.ObterGastosPorPessoaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("TotaisPorPessoa")]
    public async Task<IActionResult> TotaisPorPessoa([FromQuery] int dias = 30)
        => Ok(await _bll.ObterTotaisPorPessoaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("TotaisPorCategoria")]
    public async Task<IActionResult> TotaisPorCategoria([FromQuery] int dias = 30)
        => Ok(await _bll.ObterTotaisPorCategoriaAsync(dias));
}