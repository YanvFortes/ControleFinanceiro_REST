using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO.Context;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;

namespace ControleFinanceiro_REST.DAL.Entities;

public class TransacaoDAL
    : BaseDAL<Transacao, TransacaoDTO>, ITransacaoDAL
{
    public TransacaoDAL(FinanceDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }
}
