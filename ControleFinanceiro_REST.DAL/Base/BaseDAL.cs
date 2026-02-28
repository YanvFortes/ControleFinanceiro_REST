using AutoMapper;
using ControleFinanceiro_REST.DAO.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleFinanceiro_REST.DAL.Base;

/// <summary>
/// Classe base genérica para operações CRUD.
/// 
/// Objetivo:
/// - Centralizar operações comuns de persistência.
/// - Evitar repetição de código entre DALs específicas.
/// - Garantir padronização no uso de AutoMapper e EF Core.
/// 
/// Cada entidade concreta herda desta classe,
/// podendo sobrescrever métodos caso precise de comportamento específico.
/// </summary>
public class BaseDAL<TEntity, TDTO>
    where TEntity : class
    where TDTO : class
{
    /// <summary>
    /// Contexto principal da aplicação (EF Core).
    /// </summary>
    protected readonly FinanceDbContext DataContext;

    /// <summary>
    /// DbSet da entidade concreta.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// AutoMapper utilizado para conversão entre Entity e DTO.
    /// </summary>
    protected readonly IMapper Mapper;

    public BaseDAL(FinanceDbContext context, IMapper mapper)
    {
        DataContext = context;
        DbSet = context.Set<TEntity>();
        Mapper = mapper;
    }

    /// <summary>
    /// Retorna IQueryable configurável.
    /// 
    /// Permite:
    /// - Uso opcional de AsNoTracking para melhor performance em consultas.
    /// - Inclusão dinâmica de propriedades de navegação (Include).
    /// 
    /// Este método é a base para consultas mais complexas na BLL.
    /// </summary>
    public virtual IQueryable<TEntity> GetQuery(
        bool asNoTracking = true,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet.AsQueryable();

        // Permite carregar relações quando necessário
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return query;
    }

    /// <summary>
    /// Retorna todos os registros da entidade,
    /// aplicando mapeamento automático para DTO.
    /// 
    /// Utiliza AsNoTracking para evitar overhead de tracking
    /// em cenários de leitura.
    /// </summary>
    public virtual async Task<List<TDTO>> GetAsync()
    {
        var entities = await DbSet.AsNoTracking().ToListAsync();
        return Mapper.Map<List<TDTO>>(entities);
    }

    /// <summary>
    /// Busca um registro pelo Id.
    /// 
    /// Utiliza FindAsync por ser mais eficiente
    /// quando a chave primária é conhecida.
    /// </summary>
    public virtual async Task<TDTO?> GetByIdAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        return entity == null ? null : Mapper.Map<TDTO>(entity);
    }

    /// <summary>
    /// Cria um novo registro.
    /// 
    /// Fluxo:
    /// 1. Mapeia DTO → Entity.
    /// 2. Adiciona ao DbSet.
    /// 3. Persiste via SaveChangesAsync.
    /// 
    /// O SaveChanges é executado por operação,
    /// garantindo atomicidade por padrão.
    /// </summary>
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
            // Extrai a exceção raiz para facilitar diagnóstico
            var root = ex.GetBaseException();
            throw new Exception(root.Message, root);
        }
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// 
    /// Fluxo:
    /// 1. Localiza entidade pelo Id.
    /// 2. Aplica mapeamento DTO → entidade existente.
    /// 3. Persiste alterações.
    /// 
    /// Retorna null caso o registro não exista.
    /// </summary>
    public virtual async Task<TDTO?> EditAsync(Guid id, TDTO dto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            return null;

        Mapper.Map(dto, entity);

        await DataContext.SaveChangesAsync();

        return Mapper.Map<TDTO>(entity);
    }

    /// <summary>
    /// Remove registro pelo Id.
    /// 
    /// Não contém regras de negócio.
    /// Validações devem ocorrer na camada BLL.
    /// </summary>
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