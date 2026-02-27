using AutoMapper;
using AutoMapper.QueryableExtensions;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

public class CategoriaBLL : ICategoriaBLL
{
    private readonly ICategoriaDAL _categoriaDAL;
    private readonly ITransacaoDAL _transacaoDAL;
    private readonly IUsuarioContexto _usuarioContexto;
    private readonly IMapper _mapper;

    public CategoriaBLL(
        ICategoriaDAL categoriaDAL,
        ITransacaoDAL transacaoDAL,
        IUsuarioContexto usuarioContexto,
        IMapper mapper)
    {
        _categoriaDAL = categoriaDAL;
        _transacaoDAL = transacaoDAL;
        _usuarioContexto = usuarioContexto;
        _mapper = mapper;
    }

    public async Task<CategoriaDTO?> ObterPorIdAsync(Guid id)
    {
        return await _categoriaDAL.GetQuery()
            .Where(x => x.Id == id)
            .ProjectTo<CategoriaDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResultDTO<CategoriaDTO>> ObterPaginadoAsync(
        int page,
        int pageSize,
        string? search,
        Guid? usuarioId = null)
    {
        var query = _categoriaDAL.GetQuery();

        if (usuarioId.HasValue)
            query = query.Where(x => x.UsuarioId == usuarioId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(x =>
                x.Descricao.ToLower().Contains(s));
        }

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(x => x.Descricao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<CategoriaDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResultDTO<CategoriaDTO>(itens, total, search ?? "");
    }

    public async Task<RetornoDTO<bool>> CriarAsync(CategoriaDTO dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                return new(false, "Descrição é obrigatória.");

            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null)
                return new(false, "Usuário não identificado.");

            var existe = await _categoriaDAL
                .GetQuery()
                .AnyAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.Descricao.ToLower() == dto.Descricao.ToLower());

            if (existe)
                return new(false, "Já existe uma categoria com essa descrição.");

            dto.Id = Guid.NewGuid();
            dto.UsuarioId = usuarioId.Value;
            dto.DataCriacao = DateTime.UtcNow;

            await _categoriaDAL.CreateAsync(dto);

            return new(true, "Categoria criada com sucesso.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
        return new(false, "Erro ao criar categoria.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> AtualizarAsync(CategoriaDTO dto)
    {
        try
        {
            var existente = await _categoriaDAL.GetByIdAsync(dto.Id);
            if (existente is null)
                return new(false, "Categoria não encontrada.");

            dto.DataEdicao = DateTime.UtcNow;

            await _categoriaDAL.EditAsync(dto.Id, dto);

            return new(true, "Categoria atualizada.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao atualizar categoria.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(Guid id)
    {
        try
        {
            var possuiTransacoes = await _transacaoDAL
                .GetQuery()
                .AnyAsync(x => x.CategoriaId == id);

            if (possuiTransacoes)
                return new(false, "Não é possível excluir categoria com transações vinculadas.");

            var ok = await _categoriaDAL.DeleteAsync(id);

            return ok
                ? new(true, "Categoria excluída.")
                : new(false, "Categoria não encontrada.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao excluir categoria.");
#endif
        }
    }
}