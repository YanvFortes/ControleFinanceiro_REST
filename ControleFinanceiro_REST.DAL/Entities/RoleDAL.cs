using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.DAL.Entities;

public class RoleDAL : IRoleDAL
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleDAL(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IQueryable<IdentityRole> GetQuery()
        => _roleManager.Roles.AsNoTracking();

    public async Task CriarAsync(RoleDTO dto)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole(dto.Name));

        if (!result.Succeeded)
            throw new Exception(string.Join(" | ",
                result.Errors.Select(e => e.Description)));
    }

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
