using AutoMapper;
using AutoMapper.QueryableExtensions;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
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
    private readonly IPessoaDAL _pessoaDAL;
    private readonly ICategoriaDAL _categoriaDAL;
    private readonly IUsuarioContexto _usuarioContexto;
    private readonly IMapper _mapper;

    public TransacaoBLL(
        ITransacaoDAL transacaoDAL,
        IUsuarioDAL usuarioDAL,
        ICategoriaDAL categoriaDAL,
        IPessoaDAL pessoaDAL,
        IMapper mapper,
        IUsuarioContexto usuarioContexto)
    {
        _transacaoDAL = transacaoDAL;
        _usuarioDAL = usuarioDAL;
        _categoriaDAL = categoriaDAL;
        _pessoaDAL = pessoaDAL;
        _mapper = mapper;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<PagedResultDTO<TransacaoDTO>> ObterPaginadoAsync(
        int page,
        int pageSize,
        string? search)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new(new List<TransacaoDTO>(), 0, search ?? "");

        var query = _transacaoDAL.GetQuery(true,
            x => x.Usuario,
            x => x.Categoria,
            x => x.Pessoa)
            .Where(x => x.UsuarioId == usuarioId);

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
            .ProjectTo<TransacaoDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new(itens, total, search ?? "");
    }

    public async Task<TransacaoDTO?> ObterPorIdAsync(Guid id)
    {
        return await _transacaoDAL.GetQuery(true,
                x => x.Usuario,
                x => x.Categoria,
                x => x.Pessoa)
            .Where(x => x.Id == id)
            .ProjectTo<TransacaoDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<RetornoDTO<bool>> CriarAsync(TransacaoDTO dto)
    {
        try
        {
            if (dto.Valor <= 0)
                return new(false, "Valor deve ser maior que zero.");

            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null)
                return new(false, "Usuário não identificado.");

            var pessoa = await _pessoaDAL.GetQuery()
                .FirstOrDefaultAsync(p =>
                    p.Id == dto.PessoaId &&
                    p.UsuarioId == usuarioId);

            if (pessoa == null)
                return new(false, "Pessoa inválida.");

            var categoria = await _categoriaDAL.GetQuery()
                .FirstOrDefaultAsync(c =>
                    c.Id == dto.CategoriaId &&
                    c.UsuarioId == usuarioId);

            if (categoria == null)
                return new(false, "Categoria inválida.");

            if (pessoa.Idade < 18 && dto.Tipo == TipoTransacaoEnum.Receita)
                return new(false, "Menores de idade só podem registrar despesas.");

            dto.Id = Guid.NewGuid();
            dto.UsuarioId = usuarioId.Value;
            dto.Tipo = (TipoTransacaoEnum)categoria.Finalidade;
            var dataCriacao = dto.DataCriacao;

            if (dataCriacao == default)
            {
                dataCriacao = DateTime.Now;
            }
            else
            {
                if (dataCriacao > DateTime.Now)
                    return new(false, "Data da transação não pode ser futura.");
            }

            dto.DataCriacao = dataCriacao.AddHours(3);

            await _transacaoDAL.CreateAsync(dto);

            return new(true, "Transação criada com sucesso.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
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

            dto.DataEdicao = DateTime.Now;

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

}
