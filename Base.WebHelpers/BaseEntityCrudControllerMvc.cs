using Base.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

public abstract class BaseEntityCrudControllerMvc<TDbContext, TEntity> : Controller
    where TDbContext : DbContext
    where TEntity : class, IIdDatabaseEntity
{
    protected readonly TDbContext DbContext;

    public BaseEntityCrudControllerMvc(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected DbSet<TEntity> Entities =>
        DbContext
            .GetType()
            .GetProperties()
            .FirstOrDefault(pi => pi.PropertyType == typeof(DbSet<TEntity>))
            ?.GetValue(DbContext) as DbSet<TEntity> ??
        throw new ApplicationException(
            $"Failed to fetch DbSet for Entity type {typeof(TEntity)} from {typeof(TDbContext)}");

    // GET: Entity
    public virtual async Task<IActionResult> Index()
    {
        return View(await Entities.ToListAsync());
    }

    // GET: Entity/Details/5
    public virtual async Task<IActionResult> Details(Guid id)
    {
        var entity = await Entities.Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    protected virtual Task SetupViewData(TEntity? entity = null)
    {
        return Task.CompletedTask;
    }

    // GET: Entity/Create
    public async Task<IActionResult> Create()
    {
        await SetupViewData();
        return View();
    }

    // POST: Entity/Create
    // Requires creation of separate public Create method in derived class
    // That method should have [HttpPost] [ValidateAntiForgeryToken] attributes
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    protected async Task<IActionResult> CreateInternal(TEntity entity)
    {
        if (ModelState.IsValid)
        {
            entity.Id = Guid.NewGuid();
            Entities.Add(entity);
            await DbContext.SaveChangesAsync();
            // ReSharper disable once Mvc.ActionNotResolved
            return RedirectToAction(nameof(Index));
        }

        await SetupViewData(entity);
        return View(entity);
    }

    // GET: Entity/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await Entities.Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity == null)
        {
            return NotFound();
        }

        await SetupViewData(entity);
        return View(entity);
    }

    // POST: Entity/Edit/5
    // Requires creation of separate public Edit method in derived class
    // That method should have [HttpPost] [ValidateAntiForgeryToken] attributes
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    public async Task<IActionResult> EditInternal(Guid id, TEntity entity)
    {
        if (!id.Equals(entity.Id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                Entities.Update(entity);
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await Entities.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            // ReSharper disable once Mvc.ActionNotResolved
            return RedirectToAction(nameof(Index));
        }

        await SetupViewData(entity);
        return View(entity);
    }

    // GET: Entity/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // POST: Entity/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await Entities.Where(e => e.Id == id).ExecuteDeleteAsync();

        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction(nameof(Index));
    }
}