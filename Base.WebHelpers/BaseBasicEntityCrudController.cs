using Base.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Base.WebHelpers;

public class BaseBasicEntityCrudController<TDbContext, TEntity> : BaseDbControllerMvc<TDbContext, TEntity>
    where TDbContext : DbContext where TEntity : class, IIdDatabaseEntity
{
    public BaseBasicEntityCrudController(TDbContext dbContext) : base(dbContext)
    {
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
            return RedirectToAction("Index");
        }

        // ReSharper disable once Mvc.ViewNotResolved
        return View(entity);
    }
}