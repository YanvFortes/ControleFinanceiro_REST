using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Dashboard;
using ControleFinanceiro_REST.DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

/// <summary>
/// Responsável por consultas agregadas do Dashboard.
/// 
/// Esta camada executa apenas operações de leitura.
/// Todas as consultas são filtradas pelo usuário autenticado,
/// garantindo isolamento de dados.
/// 
/// As agregações são realizadas diretamente no banco (via LINQ → SQL),
/// evitando processamento desnecessário em memória.
/// </summary>
public class DashboardBLL : IDashboardBLL
{
    private readonly ITransacaoDAL _transacaoDAL;
    private readonly IUsuarioContexto _usuarioContexto;

    public DashboardBLL(
        ITransacaoDAL transacaoDAL,
        IUsuarioContexto usuarioContexto)
    {
        _transacaoDAL = transacaoDAL;
        _usuarioContexto = usuarioContexto;
    }

    /// <summary>
    /// Retorna resumo financeiro do período informado.
    /// Calcula total de receitas e despesas.
    /// </summary>
    public async Task<DashboardResumoDTO> ObterResumoAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new DashboardResumoDTO();

        // Define período dinâmico baseado na data atual
        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        var query = _transacaoDAL.GetQuery()
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.DataCriacao >= dataInicio);

        // Soma receitas diretamente no banco
        var receitas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Receita)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        // Soma despesas diretamente no banco
        var despesas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Despesa)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        return new DashboardResumoDTO
        {
            TotalReceitas = receitas,
            TotalDespesas = despesas
        };
    }

    /// <summary>
    /// Retorna gastos agrupados por dia.
    /// Utilizado para gráficos temporais.
    /// </summary>
    public async Task<List<DashboardGraficoDiaDTO>> ObterGastosPorDiaAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new();

        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        var dados = await _transacaoDAL.GetQuery()
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.Tipo == TipoTransacaoEnum.Despesa &&
                x.DataCriacao >= dataInicio)
            .GroupBy(x => x.DataCriacao.Date)
            .Select(g => new
            {
                Data = g.Key,
                Valor = g.Sum(x => x.Valor)
            })
            .OrderBy(x => x.Data)
            .ToListAsync();

        // Conversão para DTO apenas após execução da query
        return dados.Select(x => new DashboardGraficoDiaDTO
        {
            Data = x.Data.ToString("dd/MM"),
            Valor = x.Valor
        }).ToList();
    }

    /// <summary>
    /// Retorna gastos agrupados por pessoa.
    /// Utilizado para gráficos de distribuição.
    /// </summary>
    public async Task<List<DashboardGraficoPessoaDTO>> ObterGastosPorPessoaAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new();

        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        return await _transacaoDAL.GetQuery(true, x => x.Pessoa)
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.Tipo == TipoTransacaoEnum.Despesa &&
                x.DataCriacao >= dataInicio)
            .GroupBy(x => x.Pessoa.Nome)
            .Select(g => new DashboardGraficoPessoaDTO
            {
                Nome = g.Key,
                Valor = g.Sum(x => x.Valor)
            })
            .OrderByDescending(x => x.Valor)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna totais por pessoa:
    /// - Total de receitas
    /// - Total de despesas
    /// - Saldo implícito (Receita - Despesa)
    /// 
    /// Inclui também total geral consolidado.
    /// </summary>
    public async Task<DashboardTotaisResponseDTO> ObterTotaisPorPessoaAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new();

        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        var query = _transacaoDAL.GetQuery(true, x => x.Pessoa)
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.DataCriacao >= dataInicio);

        var agrupado = await query
            .GroupBy(x => x.Pessoa.Nome)
            .Select(g => new DashboardTotalItemDTO
            {
                Nome = g.Key,

                TotalReceitas = g
                    .Where(x => x.Tipo == TipoTransacaoEnum.Receita)
                    .Sum(x => (decimal?)x.Valor) ?? 0m,

                TotalDespesas = g
                    .Where(x => x.Tipo == TipoTransacaoEnum.Despesa)
                    .Sum(x => (decimal?)x.Valor) ?? 0m
            })
            .OrderBy(x => x.Nome)
            .ToListAsync();

        // Total consolidado calculado após agregação individual
        var totalReceitas = agrupado.Sum(x => x.TotalReceitas);
        var totalDespesas = agrupado.Sum(x => x.TotalDespesas);

        return new DashboardTotaisResponseDTO
        {
            Itens = agrupado,
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas
        };
    }

    /// <summary>
    /// Retorna totais agrupados por categoria,
    /// incluindo consolidação geral.
    /// </summary>
    public async Task<DashboardTotaisResponseDTO> ObterTotaisPorCategoriaAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new();

        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        var query = _transacaoDAL.GetQuery(true, x => x.Categoria)
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.DataCriacao >= dataInicio);

        var agrupado = await query
            .GroupBy(x => x.Categoria.Descricao)
            .Select(g => new DashboardTotalItemDTO
            {
                Nome = g.Key,

                TotalReceitas = g
                    .Where(x => x.Tipo == TipoTransacaoEnum.Receita)
                    .Sum(x => (decimal?)x.Valor) ?? 0m,

                TotalDespesas = g
                    .Where(x => x.Tipo == TipoTransacaoEnum.Despesa)
                    .Sum(x => (decimal?)x.Valor) ?? 0m
            })
            .OrderBy(x => x.Nome)
            .ToListAsync();

        var totalReceitas = agrupado.Sum(x => x.TotalReceitas);
        var totalDespesas = agrupado.Sum(x => x.TotalDespesas);

        return new DashboardTotaisResponseDTO
        {
            Itens = agrupado,
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas
        };
    }
}