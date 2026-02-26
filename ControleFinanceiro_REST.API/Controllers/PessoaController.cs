using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaBLL _pessoaBll;

    public PessoasController(IPessoaBLL pessoaBll)
        => _pessoaBll = pessoaBll;

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<PessoaDTO>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _pessoaBll.ObterPaginadoAsync(page, pageSize, search);
        return Ok(result);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<PessoaDTO>> Obter(Guid id)
    {
        var pessoa = await _pessoaBll.ObterPorIdAsync(id);

        if (pessoa == null)
            return NotFound();

        return Ok(pessoa);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar([FromBody] PessoaDTO dto)
    {
        var retorno = await _pessoaBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(Guid id, [FromBody] PessoaDTO dto)
    {
        dto.Id = id;
        return Ok(await _pessoaBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
    {
        return Ok(await _pessoaBll.ExcluirAsync(id));
    }
}