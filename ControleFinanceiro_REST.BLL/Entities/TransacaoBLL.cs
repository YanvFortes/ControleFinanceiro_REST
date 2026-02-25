using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Enums;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

public class TransacaoBLL : ITransacaoBLL
{
    private readonly ITransacaoDAL _transacaoDAL;
    private readonly IUsuarioDAL _usuarioDAL;
    private readonly ICategoriaDAL _categoriaDAL;

    public TransacaoBLL(
        ITransacaoDAL transacaoDAL,
        IUsuarioDAL usuarioDAL,
        ICategoriaDAL categoriaDAL)
    {
        _transacaoDAL = transacaoDAL;
        _usuarioDAL = usuarioDAL;
        _categoriaDAL = categoriaDAL;
    }

    public async Task<RetornoDTO<bool>> CriarAsync(TransacaoDTO dto)
    {
        try
        {
            if (dto.Valor <= 0)
                return new(false, "Valor deve ser maior que zero.");

            var usuario = await _usuarioDAL.GetByIdAsync(dto.UsuarioId);
            if (usuario is null)
                return new(false, "Usuário não encontrado.");

            var categoria = await _categoriaDAL.GetByIdAsync(dto.CategoriaId);
            if (categoria is null)
                return new(false, "Categoria não encontrada.");

            if (usuario.Idade < 18 && dto.Tipo == TipoTransacaoEnum.Receita)
                return new(false, "Menores de idade só podem registrar despesas.");

            if (categoria.Finalidade != FinalidadeCategoriaEnum.Ambas &&
                categoria.Finalidade != (FinalidadeCategoriaEnum)dto.Tipo)
                return new(false, "Categoria incompatível com o tipo da transação.");

            dto.Id = Guid.NewGuid();
            dto.DataCriacao = DateTime.UtcNow;

            await _transacaoDAL.CreateAsync(dto);

            return new(true, "Transação criada com sucesso.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, $"Erro ao criar transação: {ex.Message}");
#else
            return new(false, "Erro ao criar transação.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> AtualizarAsync(TransacaoDTO dto)
    {
        try
        {
            var existente = await _transacaoDAL.GetByIdAsync(dto.Id);
            if (existente is null)
                return new(false, "Transação não encontrada.");

            dto.DataEdicao = DateTime.UtcNow;

            await _transacaoDAL.EditAsync(dto.Id, dto);

            return new(true, "Transação atualizada.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao atualizar transação.");
#endif
        }
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(Guid id)
    {
        try
        {
            var ok = await _transacaoDAL.DeleteAsync(id);
            return ok
                ? new(true, "Transação excluída.")
                : new(false, "Transação não encontrada.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao excluir transação.");
#endif
        }
    }

    public async Task<PagedResultDTO<TransacaoDTO>> ObterPaginadoAsync(
        int page,
        int pageSize,
        string? search,
        Guid? usuarioId = null)
    {
        var query = _transacaoDAL.GetQuery(true,
            x => x.Usuario,
            x => x.Categoria);

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
            .OrderByDescending(x => x.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransacaoDTO
            {
                Id = x.Id,
                Descricao = x.Descricao,
                Valor = x.Valor,
                Tipo = x.Tipo,
                UsuarioId = x.UsuarioId,
                CategoriaId = x.CategoriaId,
                DataCriacao = x.DataCriacao,
                DataEdicao = x.DataEdicao
            })
            .ToListAsync();

        return new PagedResultDTO<TransacaoDTO>(itens, total, search ?? "");
    }

    public async Task<TotaisTransacaoDTO> ObterTotaisAsync(Guid? usuarioId = null)
    {
        var query = _transacaoDAL.GetQuery();

        if (usuarioId.HasValue)
            query = query.Where(x => x.UsuarioId == usuarioId.Value);

        var totalReceitas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Receita)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        var totalDespesas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Despesa)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        return new TotaisTransacaoDTO
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas
        };
    }

    public async Task<TransacaoDTO?> ObterPorIdAsync(Guid id)
        => await _transacaoDAL.GetByIdAsync(id);
}
