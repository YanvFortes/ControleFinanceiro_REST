using AutoMapper;
using ControleFinanceiro_REST.DAL.Base;
using ControleFinanceiro_REST.DAL.Entities.Interfaces;
using ControleFinanceiro_REST.DAO.Context;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.DAL.Entities;

public class UsuarioDAL
    : BaseDAL<Usuario, UsuarioDTO>, IUsuarioDAL
{
    public UsuarioDAL(FinanceDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    public async Task<Usuario?> GetUsuarioPorEmailAsync(string email)
    {
        return await GetQuery(true, x => x.TipoUsuario)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
