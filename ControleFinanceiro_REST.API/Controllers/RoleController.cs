using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;


/// <summary>
/// Controller responsável pela gestão de perfis (roles).
/// Integrado ao ASP.NET Identity.
/// </summary>
[ApiController, Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleBLL _bll;

    public RolesController(IRoleBLL bll)
        => _bll = bll;

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Lista roles de forma paginada.
    /// </summary>
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<RoleDTO>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
        => Ok(await _bll.ObterPaginadoAsync(page, pageSize, search));

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna role específica.
    /// </summary>
    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<RoleDTO?>> GetById(string id)
        => Ok(await _bll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize(Roles = "administrador")]
#endif
    /// <summary>
    /// Cria novo perfil no sistema.
    /// </summary>
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Post(RoleDTO dto)
        => Ok(await _bll.CriarAsync(dto));

#if !DEBUG
    [Authorize(Roles = "administrador")]
#endif
    /// <summary>
    /// Atualiza nome da role.
    /// </summary>
    [HttpPut("Editar/{id}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Put(string id, RoleDTO dto)
    {
        dto.Id = id;
        return Ok(await _bll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize(Roles = "administrador")]
#endif
    /// <summary>
    /// Remove role do sistema.
    /// </summary>
    [HttpDelete("Deletar/{id}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(string id)
        => Ok(await _bll.ExcluirAsync(id));
}
