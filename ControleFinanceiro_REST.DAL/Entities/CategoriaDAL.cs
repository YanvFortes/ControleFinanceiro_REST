using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO.Context;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;

namespace ControleFinanceiro_REST.DAL.Entities;

public class CategoriaDAL : BaseDAL<Categoria, CategoriaDTO>, ICategoriaDAL
{
    public CategoriaDAL(FinanceDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }
}
