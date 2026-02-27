using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

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
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<TransacaoDTO>> Obter(Guid id)
        => Ok(await _transacaoBll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize]
#endif
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar([FromBody] TransacaoDTO dto)
    {
        var retorno = await _transacaoBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(Guid id, [FromBody] TransacaoDTO dto)
    {
        dto.Id = id;
        return Ok(await _transacaoBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _transacaoBll.ExcluirAsync(id));
}
