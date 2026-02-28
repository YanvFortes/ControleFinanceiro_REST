using AutoMapper;
using ControleFinanceiro_REST.DAO.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Base;

public class BaseDAL<TEntity, TDTO>
    where TEntity : class
    where TDTO : class
{
    protected readonly FinanceDbContext DataContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IMapper Mapper;

    public BaseDAL(FinanceDbContext context, IMapper mapper)
    {
        DataContext = context;
        DbSet = context.Set<TEntity>();
        Mapper = mapper;
    }

    public virtual IQueryable<TEntity> GetQuery(
        bool asNoTracking = true,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return query;
    }

    public virtual async Task<List<TDTO>> GetAsync()
    {
        var entities = await DbSet.AsNoTracking().ToListAsync();
        return Mapper.Map<List<TDTO>>(entities);
    }

    public virtual async Task<TDTO?> GetByIdAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        return entity == null ? null : Mapper.Map<TDTO>(entity);
    }

    public virtual async Task<TDTO> CreateAsync(TDTO dto)
    {
        try
        {
            var entity = Mapper.Map<TEntity>(dto);

            await DbSet.AddAsync(entity);
            await DataContext.SaveChangesAsync();

            return Mapper.Map<TDTO>(entity);
        }
        catch (DbUpdateException ex)
        {
            var root = ex.GetBaseException();
            throw new Exception(root.Message, root);
        }
    }

    public virtual async Task<TDTO?> EditAsync(Guid id, TDTO dto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            return null;

        Mapper.Map(dto, entity);

        await DataContext.SaveChangesAsync();

        return Mapper.Map<TDTO>(entity);
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            return false;

        DbSet.Remove(entity);
        await DataContext.SaveChangesAsync();

        return true;
    }
}