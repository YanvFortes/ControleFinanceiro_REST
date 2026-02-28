using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

/// <summary>
/// Controller responsável por expor os endpoints de Categoria.
/// Atua apenas como camada de orquestração HTTP,
/// delegando toda regra de negócio para a BLL.
/// </summary>
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
    /// <summary>
    /// Retorna categorias de forma paginada,
    /// permitindo filtro por texto na descrição.
    /// </summary>
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
    /// <summary>
    /// Retorna uma categoria específica pelo Id.
    /// </summary>
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<CategoriaDTO>> Obter(Guid id)
        => Ok(await _categoriaBll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Cria uma nova categoria vinculada ao usuário logado.
    /// </summary>
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar([FromBody] CategoriaDTO dto)
    {
        var retorno = await _categoriaBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Atualiza os dados da categoria.
    /// </summary>
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(Guid id, [FromBody] CategoriaDTO dto)
    {
        dto.Id = id;
        return Ok(await _categoriaBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Remove uma categoria caso não possua transações vinculadas.
    /// </summary>
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _categoriaBll.ExcluirAsync(id));
}