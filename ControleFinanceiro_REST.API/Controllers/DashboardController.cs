using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

/// <summary>
/// Controller responsável por consultas agregadas para o dashboard.
/// 
/// Contém apenas consultas (não altera dados).
/// Todas as agregações são calculadas na BLL.
/// </summary>
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
    /// <summary>
    /// Retorna resumo financeiro do período informado.
    /// </summary>
    [HttpGet("Resumo")]
    public async Task<IActionResult> Resumo([FromQuery] int dias = 7)
        => Ok(await _bll.ObterResumoAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna gastos agrupados por dia.
    /// </summary>
    [HttpGet("GastosPorDia")]
    public async Task<IActionResult> GastosPorDia([FromQuery] int dias = 7)
        => Ok(await _bll.ObterGastosPorDiaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna gastos agrupados por pessoa.
    /// </summary>
    [HttpGet("GastosPorPessoa")]
    public async Task<IActionResult> GastosPorPessoa([FromQuery] int dias = 7)
        => Ok(await _bll.ObterGastosPorPessoaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna totais (receitas, despesas e saldo)
    /// agrupados por pessoa, incluindo total geral.
    /// </summary>
    [HttpGet("TotaisPorPessoa")]
    public async Task<IActionResult> TotaisPorPessoa([FromQuery] int dias = 30)
        => Ok(await _bll.ObterTotaisPorPessoaAsync(dias));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna totais agrupados por categoria,
    /// incluindo total geral consolidado.
    /// </summary>
    [HttpGet("TotaisPorCategoria")]
    public async Task<IActionResult> TotaisPorCategoria([FromQuery] int dias = 30)
        => Ok(await _bll.ObterTotaisPorCategoriaAsync(dias));
}