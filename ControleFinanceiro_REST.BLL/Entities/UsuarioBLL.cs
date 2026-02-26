using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

public class UsuarioBLL : IUsuarioBLL
{
    private readonly IUsuarioDAL _dal;
    private readonly IUsuarioContexto _usuarioContexto;

    public UsuarioBLL(
        IUsuarioDAL dal,
        IUsuarioContexto usuarioContexto)
    {
        _dal = dal;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<PagedResultDTO<UsuarioDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search)
    {
        if (page <= 0) page = 1;
        if (size <= 0) size = 10;

        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new PagedResultDTO<UsuarioDTO>(
                new List<UsuarioDTO>(), 0, search ?? "");

        var query = _dal.GetQuery(true)
                        .Where(x => x.Id == usuarioId);

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
                Email = x.Email
            })
            .ToListAsync();

        return new PagedResultDTO<UsuarioDTO>(itens, total, search ?? "");
    }

    public async Task<UsuarioDTO?> ObterPorIdAsync(Guid id)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null || usuarioId != id)
            return null;

        return await _dal.GetQuery(true)
            .Where(x => x.Id == id)
            .Select(x => new UsuarioDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Email = x.Email
            })
            .FirstOrDefaultAsync();
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
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao criar usuário.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> AtualizarAsync(UsuarioDTO dto)
    {
        try
        {
            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null || usuarioId != dto.Id)
                return new(false, "Acesso negado.");

            var existente = await _dal.GetByIdAsync(dto.Id);
            if (existente == null)
                return new(false, "Usuário não encontrado.");

            dto.DataEdicao = DateTime.UtcNow;

            await _dal.EditAsync(dto.Id, dto);
            return new(true, "Usuário atualizado.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao atualizar usuário.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(Guid id)
    {
        try
        {
            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null || usuarioId != id)
                return new(false, "Acesso negado.");

            var usuario = await _dal.GetByIdAsync(id);
            if (usuario == null)
                return new(false, "Usuário não encontrado.");

            await _dal.DeleteAsync(id);
            return new(true, "Usuário excluído.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao excluir usuário.");
#endif
        }
    }
}