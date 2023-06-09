using Base.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

public abstract class BaseEntityCrudControllerMvc<TDbContext, TEntity> : 
    BaseDbControllerMvc<TDbContext, TEntity> where TDbContext : DbContext
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
            BaseEntities.Add(entity);
            await DbContext.SaveChangesAsync();
            // ReSharper disable once Mvc.ActionNotResolved
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
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
                BaseEntities.Update(entity);
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