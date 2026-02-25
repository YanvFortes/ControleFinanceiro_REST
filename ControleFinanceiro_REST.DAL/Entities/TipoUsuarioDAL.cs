using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO.Context;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.DAL.Entities;

public class TipoUsuarioDAL
    : BaseDAL<Tipousuario, TipoUsuarioDTO>, ITipoUsuarioDAL
{
    public TipoUsuarioDAL(FinanceDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    public async Task<string?> ObterDescricaoPorIdAsync(int tipoUsuarioId)
    {
        return await GetQuery(true)
                .Where(t => t.Id == tipoUsuarioId)
                .Select(t => t.Descricao)
                .FirstOrDefaultAsync();
    }
}
