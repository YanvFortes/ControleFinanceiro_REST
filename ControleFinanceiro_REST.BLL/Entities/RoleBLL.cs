using AutoMapper;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

/// <summary>
/// Camada responsável pela gestão de Roles (perfis de acesso).
/// 
/// Atua como intermediária entre a Controller e a DAL,
/// delegando operações ao Identity através da RoleDAL.
/// 
/// Não contém regras complexas de domínio,
/// pois Role é uma entidade estrutural de controle de acesso.
/// </summary>
public class RoleBLL : IRoleBLL
{
    private readonly IRoleDAL _roleDAL;
    private readonly IMapper _mapper;

    public RoleBLL(IRoleDAL roleDAL, IMapper mapper)
    {
        _roleDAL = roleDAL;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna roles de forma paginada.
    /// Permite filtro opcional por nome.
    /// </summary>
    public async Task<PagedResultDTO<RoleDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search)
    {
        var query = _roleDAL.GetQuery();

        // Filtro simples por nome
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name!.Contains(search));

        var total = await query.CountAsync();

        var roles = await query
            .OrderBy(r => r.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var itens = _mapper.Map<List<RoleDTO>>(roles);

        return new PagedResultDTO<RoleDTO>(itens, total, search ?? "");
    }

    /// <summary>
    /// Retorna role específica pelo Id.
    /// </summary>
    public async Task<RoleDTO?> ObterPorIdAsync(string id)
    {
        var role = await _roleDAL
            .GetQuery()
            .FirstOrDefaultAsync(r => r.Id == id);

        return role is null
            ? null
            : _mapper.Map<RoleDTO>(role);
    }

    /// <summary>
    /// Cria novo perfil no sistema.
    /// Delegado para RoleDAL, que integra com ASP.NET Identity.
    /// </summary>
    public async Task<RetornoDTO<bool>> CriarAsync(RoleDTO dto)
    {
        try
        {
            await _roleDAL.CriarAsync(dto);
            return new(true, "Criado.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }

    /// <summary>
    /// Atualiza dados de uma role existente.
    /// </summary>
    public async Task<RetornoDTO<bool>> AtualizarAsync(RoleDTO dto)
    {
        try
        {
            await _roleDAL.AtualizarAsync(dto);
            return new(true, "Atualizado.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }

    /// <summary>
    /// Remove role do sistema.
    /// Regras adicionais (ex: impedir exclusão se houver usuários vinculados)
    /// podem ser implementadas futuramente nesta camada.
    /// </summary>
    public async Task<RetornoDTO<bool>> ExcluirAsync(string id)
    {
        try
        {
            await _roleDAL.ExcluirAsync(id);
            return new(true, "Excluído.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }
}