using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class Recipes : BaseBasicEntityCrudController<AppDbContext, Recipe>
{
    public Recipes(AppDbContext dbContext) : base(dbContext)
    {
    }

    private async Task<List<Product>> GetProducts(string? productNameQuery = null)
    {
        IQueryable<Product> productQuery = DbContext.Products;
        if (productNameQuery != null)
        {
            var newProductNameQuery = "%" + productNameQuery + "%";
            productQuery = productQuery.Where(e => EF.Functions.ILike(e.Name, newProductNameQuery));
        }

        return await productQuery.ToListAsync();
    }

    [Authorize]
    public async Task<IActionResult> Edit(Guid id, string? productNameQuery = null)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();
        var entity = await Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return NotFound();

        return View(new RecipeEditViewModel
        {
            Recipe = entity,
            Products = await GetProducts(productNameQuery),
            ProductNameQuery = productNameQuery,
        });
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        return View(await Entities.ToListAsync());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await Entities.Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();
        var entity = await Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [Authorize]
    [HttpPost, ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();

        await Entities.Where(e => e.Id == id).ExecuteDeleteAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ActionName(nameof(Create))]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromForm] Recipe entity)
    {
        entity.CreatorId = User.GetUserId();
        if (!ModelState.IsValid) return View(entity);
        entity.Id = Guid.NewGuid();
        BaseEntities.Add(entity);
        await DbContext.SaveChangesAsync();
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction(nameof(Edit), new { entity.Id });
    }

    [HttpPost]
    [ActionName(nameof(Edit))]
    [Authorize]
    public async Task<IActionResult> EditPost([FromForm] RecipeEditViewModel model)
    {
        if (!await IsAdminOrAuthor(model.Recipe.Id)) return Forbid();

        if (!ModelState.IsValid)
        {
            var recipe = await Entities.FirstOrDefaultAsync(e => e.Id == model.Recipe.Id);
            if (recipe == null) return NotFound();
            model.Recipe.RecipeProducts = recipe.RecipeProducts;
            model.Recipe.Creator = recipe.Creator;
            model.Products = await GetProducts(model.ProductNameQuery);
            return View(model);
        }

        try
        {
            BaseEntities.Update(model.Recipe);
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await Entities.AnyAsync(e => e.Id == model.Recipe.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Edit), new { model.Recipe.Id, model.ProductNameQuery });
    }

    private async Task<bool> IsAdminOrAuthor(Guid id)
    {
        if (User.IsInRole(RoleNames.Admin)) return true;
        var userId = User.GetUserIdIfExists();
        if (userId == null) return false;
        return await Entities.AnyAsync(r => r.Id == id && r.CreatorId == userId);
    }

    protected override IQueryable<Recipe> Entities
    {
        get
        {
            return BaseEntities.Include(e => e.Creator)
                .Include(e => e.RecipeProducts!)
                .ThenInclude(e => e.Product);
        }
    }
}