using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

[ApiController]
public class BaseDbControllerApi<TDbContext> : ControllerBase
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    protected readonly IMapper Mapper;

    public BaseDbControllerApi(TDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
    }
}

public class BaseDbControllerApi<TDbContext, TEntity> : BaseDbControllerApi<TDbContext>
    where TDbContext : DbContext
    where TEntity : class
{
    protected virtual IQueryable<TEntity> Entities => BaseEntities;

    protected DbSet<TEntity> BaseEntities =>
        DbContext
            .GetType()
            .GetProperties()
            .FirstOrDefault(pi => pi.PropertyType == typeof(DbSet<TEntity>))
            ?.GetValue(DbContext) as DbSet<TEntity> ??
        throw new ApplicationException(
            $"Failed to fetch DbSet for Entity type {typeof(TEntity)} from {typeof(TDbContext)}");

    public BaseDbControllerApi(TDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}