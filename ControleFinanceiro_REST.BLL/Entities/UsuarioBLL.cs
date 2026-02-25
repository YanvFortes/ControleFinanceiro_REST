using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ControleFinanceiro_REST.BLL.Entities;

public class UsuarioBLL : IUsuarioBLL
{
    private readonly IUsuarioDAL _dal;
    private readonly ITokenContexto _tokenContexto;

    public UsuarioBLL(
        IUsuarioDAL dal,
        ITokenContexto tokenContexto)
    {
        _dal = dal;
        _tokenContexto = tokenContexto;
    }

    private string? GetAspNetUserId()
        => _tokenContexto.ObterClaim(ClaimTypes.NameIdentifier);

    public async Task<PagedResultDTO<UsuarioDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search)
    {
        var query = _dal.GetQuery(true);


        var aspId = GetAspNetUserId();

        query = query.Where(x => x.AspNetUserId == aspId);


        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = $"%{search.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Nome, like) ||
                EF.Functions.ILike(x.Email, like));
        }

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new UsuarioDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Email = x.Email,
                Idade = x.Idade
            })
            .ToListAsync();

        return new PagedResultDTO<UsuarioDTO>(itens, total, search ?? "");
    }

    public async Task<UsuarioDTO?> ObterPorIdAsync(Guid id)
    {
        var query = _dal.GetQuery(true);


        var aspId = GetAspNetUserId();
        query = query.Where(x => x.AspNetUserId == aspId);


        var usuario = await query
            .Where(x => x.Id == id)
            .Select(x => new UsuarioDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Email = x.Email,
                Idade = x.Idade
            })
            .FirstOrDefaultAsync();

        return usuario;
    }

    public async Task<RetornoDTO<bool>> CriarAsync(UsuarioDTO dto)
    {
        try
        {
            dto.Id = Guid.NewGuid();
            dto.DataCriacao = DateTime.UtcNow;

            await _dal.CreateAsync(dto);
            return new(true, "Usuário criado.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }

    public async Task<RetornoDTO<bool>> AtualizarAsync(UsuarioDTO dto)
    {
        try
        {
            var existente = await _dal.GetByIdAsync(dto.Id);
            if (existente == null)
                return new(false, "Usuário não encontrado.");


            var aspId = GetAspNetUserId();
            if (existente.AspNetUserId != aspId)
                return new(false, "Acesso negado.");


            dto.DataEdicao = DateTime.UtcNow;

            await _dal.EditAsync(dto.Id, dto);
            return new(true, "Usuário atualizado.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(Guid id)
    {
        try
        {
            var usuario = await _dal.GetByIdAsync(id);
            if (usuario == null)
                return new(false, "Usuário não encontrado.");

            await _dal.DeleteAsync(id);
            return new(true, "Usuário excluído.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }
}
