using Base.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

public class BaseDbControllerMvc<TDbContext> : Controller
{
    protected readonly TDbContext DbContext;

    public BaseDbControllerMvc(TDbContext dbContext)
    {
        DbContext = dbContext;
    }
}

public class BaseDbControllerMvc<TDbContext, TEntity> : BaseDbControllerMvc<TDbContext>
    where TDbContext : DbContext
    where TEntity : class, IIdDatabaseEntity
{
    public BaseDbControllerMvc(TDbContext dbContext) : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> Entities => BaseEntities;

    protected DbSet<TEntity> BaseEntities =>
        DbContext
            .GetType()
            .GetProperties()
            .FirstOrDefault(pi => pi.PropertyType == typeof(DbSet<TEntity>))
            ?.GetValue(DbContext) as DbSet<TEntity> ??
        throw new ApplicationException(
            $"Failed to fetch DbSet for Entity type {typeof(TEntity)} from {typeof(TDbContext)}");
}