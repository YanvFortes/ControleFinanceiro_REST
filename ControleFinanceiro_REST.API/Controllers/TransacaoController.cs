using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

/// <summary>
/// Controller responsável pela gestão de transações financeiras.
/// 
/// Todas as operações são vinculadas ao usuário autenticado.
/// As regras de validação (ex: idade, categoria, saldo)
/// são tratadas na BLL.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoBLL _transacaoBll;

    public TransacoesController(ITransacaoBLL transacaoBll)
        => _transacaoBll = transacaoBll;

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna transações paginadas do usuário logado.
    /// </summary>
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<TransacaoDTO>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _transacaoBll.ObterPaginadoAsync(page, pageSize, search);
        return Ok(result);
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna uma transação específica.
    /// </summary>
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<TransacaoDTO>> Obter(Guid id)
        => Ok(await _transacaoBll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Cria nova transação vinculada ao usuário autenticado.
    /// </summary>
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar(
        [FromBody] TransacaoDTO dto)
    {
        var retorno = await _transacaoBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Atualiza dados da transação.
    /// </summary>
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(
        Guid id,
        [FromBody] TransacaoDTO dto)
    {
        dto.Id = id;
        return Ok(await _transacaoBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Remove transação do sistema.
    /// </summary>
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _transacaoBll.ExcluirAsync(id));
}