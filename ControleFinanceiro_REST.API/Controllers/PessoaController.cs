using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

/// <summary>
/// Controller responsável pela gestão de Pessoas.
/// 
/// Pessoa representa um indivíduo vinculado a um usuário,
/// utilizado para agrupamento de transações.
/// </summary>
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
    /// <summary>
    /// Lista pessoas de forma paginada.
    /// </summary>
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
    /// <summary>
    /// Retorna pessoa pelo Id.
    /// </summary>
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
    /// <summary>
    /// Cria nova pessoa vinculada ao usuário autenticado.
    /// </summary>
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar(
        [FromBody] PessoaDTO dto)
    {
        var retorno = await _pessoaBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Atualiza dados da pessoa.
    /// </summary>
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(
        Guid id,
        [FromBody] PessoaDTO dto)
    {
        dto.Id = id;
        return Ok(await _pessoaBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Remove pessoa do sistema.
    /// </summary>
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _pessoaBll.ExcluirAsync(id));
}