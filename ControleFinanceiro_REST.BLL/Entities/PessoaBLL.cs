using AutoMapper;
using AutoMapper.QueryableExtensions;
using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Entities;
using ControleFinanceiro_REST.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

/// <summary>
/// Camada responsável pelas regras de negócio da entidade Pessoa.
/// 
/// Pessoa representa um indivíduo vinculado ao usuário logado,
/// utilizado para agrupamento e análise de transações.
/// 
/// Todas as operações garantem isolamento por usuário.
/// </summary>
public class PessoaBLL : IPessoaBLL
{
    private readonly IPessoaDAL _dal;
    private readonly IUsuarioContexto _usuarioContexto;
    private readonly IMapper _mapper;

    public PessoaBLL(
        IPessoaDAL dal,
        IUsuarioContexto usuarioContexto,
        IMapper mapper)
    {
        _dal = dal;
        _usuarioContexto = usuarioContexto;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna pessoas do usuário autenticado de forma paginada.
    /// 
    /// - Aplica filtro textual opcional.
    /// - Utiliza ILike para busca case-insensitive (PostgreSQL).
    /// - Usa ProjectTo para otimizar o mapeamento direto no banco.
    /// </summary>
    public async Task<PagedResultDTO<PessoaDTO>> ObterPaginadoAsync(
        int page,
        int size,
        string? search)
    {
        if (page <= 0) page = 1;
        if (size <= 0) size = 10;

        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new PagedResultDTO<PessoaDTO>(
                new List<PessoaDTO>(), 0, search ?? "");

        var query = _dal.GetQuery(true)
            .Where(p => p.UsuarioId == usuarioId);

        // Filtro por nome
        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = $"%{search.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Nome, like));
        }

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ProjectTo<PessoaDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResultDTO<PessoaDTO>(itens, total, search ?? "");
    }

    /// <summary>
    /// Retorna pessoa específica,
    /// garantindo que pertença ao usuário autenticado.
    /// </summary>
    public async Task<PessoaDTO?> ObterPorIdAsync(Guid id)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return null;

        return await _dal.GetQuery(true)
            .Where(p => p.UsuarioId == usuarioId && p.Id == id)
            .ProjectTo<PessoaDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Cria nova pessoa vinculada ao usuário autenticado.
    /// 
    /// Valida:
    /// - Usuário autenticado
    /// - Idade não negativa
    /// </summary>
    public async Task<RetornoDTO<bool>> CriarAsync(PessoaDTO dto)
    {
        try
        {
            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null)
                return new(false, "Usuário não identificado.");

            if (dto.Idade < 0)
                return new(false, "Idade inválida.");

            dto.Id = Guid.NewGuid();
            dto.UsuarioId = usuarioId.Value;
            dto.DataCriacao = DateTime.UtcNow;

            await _dal.CreateAsync(dto);

            return new(true, "Pessoa criada com sucesso.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao criar pessoa.");
#endif
        }
    }

    /// <summary>
    /// Atualiza dados da pessoa.
    /// 
    /// Regra importante:
    /// Usuário só pode atualizar pessoas que lhe pertencem.
    /// </summary>
    public async Task<RetornoDTO<bool>> AtualizarAsync(PessoaDTO dto)
    {
        try
        {
            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null)
                return new(false, "Usuário não identificado.");

            var existente = await _dal.GetQuery()
                .FirstOrDefaultAsync(p =>
                    p.Id == dto.Id &&
                    p.UsuarioId == usuarioId);

            if (existente == null)
                return new(false, "Pessoa não encontrada.");

            existente.Nome = dto.Nome;
            existente.Idade = dto.Idade;
            existente.DataEdicao = DateTime.UtcNow;

            await _dal.EditAsync(dto.Id, dto);

            return new(true, "Pessoa atualizada.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao atualizar pessoa.");
#endif
        }
    }

    /// <summary>
    /// Remove pessoa do sistema.
    /// </summary>
    public async Task<RetornoDTO<bool>> ExcluirAsync(Guid id)
    {
        try
        {
            var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
            if (usuarioId == null)
                return new(false, "Usuário não identificado.");

            var existente = await _dal.GetQuery()
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.UsuarioId == usuarioId);

            if (existente == null)
                return new(false, "Pessoa não encontrada.");

            await _dal.DeleteAsync(id);

            return new(true, "Pessoa excluída com sucesso.");
        }
        catch (Exception ex)
        {
#if DEBUG
            return new(false, ex.Message);
#else
            return new(false, "Erro ao excluir pessoa.");
#endif
        }
    }
}