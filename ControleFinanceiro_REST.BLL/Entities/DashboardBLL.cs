using ControleFinanceiro_REST.BLL.Entities.Interfaces;
using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DTO.Dashboard;
using ControleFinanceiro_REST.DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.BLL.Entities;

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

    public async Task<DashboardResumoDTO> ObterResumoAsync(int dias)
    {
        var usuarioId = await _usuarioContexto.ObterUsuarioIdAsync();
        if (usuarioId == null)
            return new DashboardResumoDTO();

        var dataInicio = DateTime.UtcNow.Date.AddDays(-dias);

        var query = _transacaoDAL.GetQuery()
            .Where(x =>
                x.UsuarioId == usuarioId &&
                x.DataCriacao >= dataInicio);

        var receitas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Receita)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        var despesas = await query
            .Where(x => x.Tipo == TipoTransacaoEnum.Despesa)
            .SumAsync(x => (decimal?)x.Valor) ?? 0m;

        return new DashboardResumoDTO
        {
            TotalReceitas = receitas,
            TotalDespesas = despesas
        };
    }

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

        return dados.Select(x => new DashboardGraficoDiaDTO
        {
            Data = x.Data.ToString("dd/MM"),
            Valor = x.Valor
        }).ToList();
    }

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
}
