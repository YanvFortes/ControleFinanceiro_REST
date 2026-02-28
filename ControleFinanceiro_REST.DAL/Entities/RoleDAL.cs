using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO.Context;
using ControleFinanceiro_REST.DTO.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Camada de acesso a dados para Roles.
/// 
/// Diferente das demais entidades,
/// Roles são gerenciadas pelo ASP.NET Identity.
/// 
/// Por isso:
/// - Herdamos de BaseDAL para manter padronização arquitetural.
/// - Sobrescrevemos operações críticas para utilizar RoleManager,
///   garantindo integridade com o Identity.
/// </summary>
public class RoleDAL : BaseDAL<IdentityRole, RoleDTO>, IRoleDAL
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleDAL(
        FinanceDbContext context,
        IMapper mapper,
        RoleManager<IdentityRole> roleManager)
        : base(context, mapper)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Retorna IQueryable de roles.
    /// 
    /// Utiliza Roles do Identity para garantir
    /// consistência com o mecanismo interno do ASP.NET.
    /// </summary>
    public override IQueryable<IdentityRole> GetQuery(
        bool asNoTracking = true,
        params System.Linq.Expressions.Expression<Func<IdentityRole, object>>[] includes)
    {
        return _roleManager.Roles.AsNoTracking();
    }

    /// <summary>
    /// Criação de role via RoleManager.
    /// 
    /// Identity é responsável por:
    /// - Normalização do nome
    /// - Validações internas
    /// - Integridade de dados
    /// </summary>
    public async Task CriarAsync(RoleDTO dto)
    {
        var role = new IdentityRole(dto.Name);

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            throw new Exception(string.Join(" | ",
                result.Errors.Select(e => e.Description)));
    }

    /// <summary>
    /// Atualização de role.
    /// 
    /// Atualiza nome e NormalizedName,
    /// mantendo padrão exigido pelo Identity.
    /// </summary>
    public async Task AtualizarAsync(RoleDTO dto)
    {
        var role = await _roleManager.FindByIdAsync(dto.Id)
                   ?? throw new Exception("Role não encontrada.");

        role.Name = dto.Name;
        role.NormalizedName = dto.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
            throw new Exception(string.Join(" | ",
                result.Errors.Select(e => e.Description)));
    }

    /// <summary>
    /// Exclusão de role.
    /// 
    /// A responsabilidade por verificar
    /// vínculos com usuários deve estar na BLL.
    /// </summary>
    public async Task ExcluirAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id)
                   ?? throw new Exception("Role não encontrada.");

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            throw new Exception(string.Join(" | ",
                result.Errors.Select(e => e.Description)));
    }
}