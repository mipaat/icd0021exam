using Base.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

public abstract class BaseEntityCrudControllerMvc<TDbContext, TEntity> : 
    BaseBasicEntityCrudController<TDbContext, TEntity> where TDbContext : DbContext
    where TEntity : class, IIdDatabaseEntity
{
    protected BaseEntityCrudControllerMvc(TDbContext dbContext) : base(dbContext)
    {
    }

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

    // GET: Entity/Create
    public virtual Task<IActionResult> Create()
    {
        return Task.FromResult<IActionResult>(View());
    }

    // GET: Entity/Edit/5
    public virtual async Task<IActionResult> Edit(Guid id)
    {
        var entity = await Entities.Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    // GET: Entity/Delete/5
    public virtual async Task<IActionResult> Delete(Guid id)
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
    public virtual async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await Entities.Where(e => e.Id == id).ExecuteDeleteAsync();

        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction(nameof(Index));
    }
}