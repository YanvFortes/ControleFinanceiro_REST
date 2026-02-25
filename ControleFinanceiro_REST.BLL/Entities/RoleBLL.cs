using AutoMapper;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

public class RoleBLL : IRoleBLL
{
    private readonly IRoleDAL _roleDAL;
    private readonly IMapper _mapper;

    public RoleBLL(IRoleDAL roleDAL, IMapper mapper)
    {
        _roleDAL = roleDAL;
        _mapper = mapper;
    }

    public async Task<PagedResultDTO<RoleDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search)
    {
        var query = _roleDAL.GetQuery();

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

    public async Task<RoleDTO?> ObterPorIdAsync(string id)
    {
        var role = await _roleDAL
            .GetQuery()
            .FirstOrDefaultAsync(r => r.Id == id);

        return role is null
            ? null
            : _mapper.Map<RoleDTO>(role);
    }

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
