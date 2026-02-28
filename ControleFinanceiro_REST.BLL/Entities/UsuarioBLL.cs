using AutoMapper;
using AutoMapper.QueryableExtensions;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Request;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

public class UsuarioBLL : IUsuarioBLL
{
    private readonly IUsuarioDAL _dal;
    private readonly IUsuarioContexto _usuarioContexto;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuarioBLL(
        IUsuarioDAL dal,
        IUsuarioContexto usuarioContexto,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _dal = dal;
        _usuarioContexto = usuarioContexto;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
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
            return new(new List<UsuarioDTO>(), 0, search ?? "");

        var usuarioLogado = await _dal.GetQuery(false)
            .FirstOrDefaultAsync(x => x.Id == usuarioId);

        if (usuarioLogado == null)
            return new(new List<UsuarioDTO>(), 0, search ?? "");

        var identityUser = await _userManager
            .FindByIdAsync(usuarioLogado.AspNetUserId);

        var isAdmin = identityUser != null &&
                      await _userManager.IsInRoleAsync(identityUser, "administrador");

        var query = _dal.GetQuery(true);

        if (!isAdmin)
        {
            query = query.Where(x => x.Id == usuarioId);
        }

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
            .ProjectTo<UsuarioDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new(itens, total, search ?? "");
    }

    public async Task<UsuarioDTO?> ObterPorIdAsync(Guid id)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return null;

        var usuarioLogado = await _dal.GetQuery(false)
            .FirstOrDefaultAsync(x => x.Id == usuarioId);

        if (usuarioLogado == null)
            return null;

        var identityUser = await _userManager
            .FindByIdAsync(usuarioLogado.AspNetUserId);

        var isAdmin = identityUser != null &&
                      await _userManager.IsInRoleAsync(identityUser, "administrador");

        if (!isAdmin && usuarioId != id)
            return null;

        return await _dal.GetQuery(true)
            .Where(x => x.Id == id)
            .ProjectTo<UsuarioDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<RetornoDTO<bool>> CriarAsync(CriarUsuarioRequestDTO dto)
    {
        try
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _userManager.FindByEmailAsync(email) != null)
                return new(false, "E-mail já cadastrado.");

            var identityUser = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Senha);

            if (!result.Succeeded)
                return new(false, string.Join(" | ", result.Errors.Select(e => e.Description)));

            var roleName = ResolverNomeRole(dto.TipoUsuarioId);

            if (string.IsNullOrWhiteSpace(roleName))
                return new(false, "Tipo de usuário inválido.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(identityUser, roleName);

            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Email = email,
                TipoUsuarioId = dto.TipoUsuarioId,
                AspNetUserId = identityUser.Id,
                DataCriacao = DateTime.UtcNow,
                DataEdicao = DateTime.UtcNow
            };

            await _dal.CreateAsync(usuario);

            return new(true, "Usuário criado com sucesso.");
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

    public async Task<RetornoDTO<bool>> AtualizarAsync(AtualizarUsuarioRequestDTO dto)
    {
        try
        {
            var usuario = await _dal.GetByIdAsync(dto.Id);
            if (usuario == null)
                return new(false, "Usuário não encontrado.");

            var identityUser = await _userManager.FindByIdAsync(usuario.AspNetUserId);
            if (identityUser == null)
                return new(false, "Usuário Identity não encontrado.");

            var email = dto.Email.Trim().ToLowerInvariant();

            identityUser.Email = email;
            identityUser.UserName = email;

            var updateResult = await _userManager.UpdateAsync(identityUser);
            if (!updateResult.Succeeded)
                return new(false, string.Join(" | ", updateResult.Errors.Select(e => e.Description)));

            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                var senhaResult = await _userManager.ResetPasswordAsync(identityUser, token, dto.Senha);

                if (!senhaResult.Succeeded)
                    return new(false, string.Join(" | ", senhaResult.Errors.Select(e => e.Description)));
            }

            usuario.Nome = dto.Nome;
            usuario.Email = email;
            usuario.TipoUsuarioId = dto.TipoUsuarioId;
            usuario.DataEdicao = DateTime.UtcNow;

            await _dal.EditAsync(usuario.Id, usuario);

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
            var usuario = await _dal.GetByIdAsync(id);
            if (usuario == null)
                return new(false, "Usuário não encontrado.");

            var identityUser = await _userManager.FindByIdAsync(usuario.AspNetUserId);
            if (identityUser != null)
                await _userManager.DeleteAsync(identityUser);

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

    private string? ResolverNomeRole(int tipoUsuarioId)
    {
        return tipoUsuarioId switch
        {
            1 => "administrador",
            2 => "usuario",
            _ => null
        };
    }
}