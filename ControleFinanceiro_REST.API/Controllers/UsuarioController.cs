using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Request;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro_REST.API.Controllers;

/// <summary>
/// Controller responsável pela gestão de usuários do sistema.
/// 
/// Responsabilidades:
/// - Expor endpoints HTTP.
/// - Aplicar regras de autorização.
/// - Delegar regras de negócio para a BLL.
/// 
/// Não contém lógica de negócio.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioBLL _usuarioBll;

    public UsuariosController(IUsuarioBLL usuarioBll)
        => _usuarioBll = usuarioBll;

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Lista usuários de forma paginada.
    /// Administradores podem visualizar todos.
    /// Usuários comuns visualizam apenas seus próprios dados.
    /// </summary>
    [HttpGet("Get")]
    public async Task<ActionResult<PagedResultDTO<UsuarioDTO>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _usuarioBll.ObterPaginadoAsync(page, pageSize, search);
        return Ok(result);
    }

#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// Retorna um usuário específico.
    /// A validação de permissão é tratada na camada BLL.
    /// </summary>
    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<UsuarioDTO>> Obter(Guid id)
        => Ok(await _usuarioBll.ObterPorIdAsync(id));

#if !DEBUG
    [Authorize(Roles = "administrador, suporte, gerente, gerente master")]
#endif
    /// <summary>
    /// Cria novo usuário e integra com ASP.NET Identity.
    /// Apenas perfis autorizados podem executar.
    /// </summary>
    [HttpPost("Cadastrar")]
    public async Task<ActionResult<RetornoDTO<bool>>> Cadastrar(
        [FromBody] CriarUsuarioRequestDTO dto)
    {
        var retorno = await _usuarioBll.CriarAsync(dto);
        return Ok(retorno);
    }

#if !DEBUG
    [Authorize(Roles = "administrador, suporte")]
#endif
    /// <summary>
    /// Atualiza dados cadastrais e, se informado, redefine senha.
    /// </summary>
    [HttpPut("Editar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Editar(
        Guid id,
        [FromBody] AtualizarUsuarioRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await _usuarioBll.AtualizarAsync(dto));
    }

#if !DEBUG
    [Authorize(Roles = "administrador")]
#endif
    /// <summary>
    /// Remove usuário do sistema e da base Identity.
    /// Restrito a administradores.
    /// </summary>
    [HttpDelete("Deletar/{id:guid}")]
    public async Task<ActionResult<RetornoDTO<bool>>> Delete(Guid id)
        => Ok(await _usuarioBll.ExcluirAsync(id));
}