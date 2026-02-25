using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaBLL _categoriaBll;

    public CategoriasController(ICategoriaBLL categoriaBll)
        => _categoriaBll = categoriaBll;

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<CategoriaDTO>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _categoriaBll.ObterPaginadoAsync(page, pageSize, search);
        return Ok(result);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<CategoriaDTO>> Obter(Guid id)
        => Ok(await _categoriaBll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize]
#endif
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar([FromBody] CategoriaDTO dto)
    {
        var retorno = await _categoriaBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(Guid id, [FromBody] CategoriaDTO dto)
    {
        dto.Id = id;
        return Ok(await _categoriaBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _categoriaBll.ExcluirAsync(id));
}
