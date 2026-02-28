using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Request;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioBLL _usuarioBll;
    public UsuariosController(IUsuarioBLL usuarioBll) => _usuarioBll = usuarioBll;

#if !DEBUG
        [Authorize]
#endif
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<UsuarioDTO>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
        )
    {
        var result = await _usuarioBll.ObterPaginadoAsync(page, pageSize, search);

        return Ok(result);
    }

#if !DEBUG
        [Authorize]
#endif
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<UsuarioDTO>> Obter(Guid id)
        => Ok(await _usuarioBll.ObterPorIdAsync(id));


#if !DEBUG
        [Authorize(Roles = "administrador, suporte, gerente, gerente master")]
#endif
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar([FromBody] CriarUsuarioRequestDTO dto)
    {
        var retorno = await _usuarioBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
             [Authorize(Roles = "administrador, suporte")]
#endif
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(Guid id, [FromBody] AtualizarUsuarioRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await _usuarioBll.AtualizarAsync(dto));
    }

#if !DEBUG
            [Authorize(Roles = "administrador")]
#endif
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id) =>
        Ok(await _usuarioBll.ExcluirAsync(id));

}