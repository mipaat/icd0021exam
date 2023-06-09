using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class RecipeProductsController : BaseDbControllerMvc<AppDbContext, RecipeProduct>
{
    public RecipeProductsController(AppDbContext dbContext) : base(dbContext)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Remove(Guid id, string? returnUrl = null)
    {
        var recipeProduct = await Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (recipeProduct == null) return NotFound();
        if (!User.IsAllowedToManageRecipe(recipeProduct.Recipe!)) return Forbid();
        BaseEntities.Remove(recipeProduct);
        await DbContext.SaveChangesAsync();
        return Redirect(returnUrl ?? Url.Content("~/"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Set([FromForm] SetRecipeProductData data)
    {
        var recipe = await DbContext.Recipes.FirstOrDefaultAsync(e => e.Id == data.RecipeProduct.RecipeId);
        if (recipe == null) return NotFound();
        if (!User.IsAllowedToManageRecipe(recipe)) return Forbid();
        if (!await DbContext.Products.AnyAsync(e => e.Id == data.RecipeProduct.ProductId)) return NotFound();
        if (!await DbContext.Recipes.AnyAsync(e => e.Id == data.RecipeProduct.RecipeId)) return NotFound();

        var entityDoesNotExist = data.RecipeProduct.Id == Guid.Empty ||
                           !await BaseEntities.AnyAsync(e => e.Id == data.RecipeProduct.Id);
        var amountTooSmall = data.RecipeProduct.Amount <= 0;
        if (entityDoesNotExist)
        {
            if (!amountTooSmall)
            {
                BaseEntities.Add(data.RecipeProduct);
            }
        } else if (amountTooSmall)
        {
            BaseEntities.Remove(data.RecipeProduct);
        }
        else
        {
            BaseEntities.Update(data.RecipeProduct);
        }

        await DbContext.SaveChangesAsync();
        return Redirect(data.ReturnUrl ?? Url.Content("~/"));
    }

    protected override IQueryable<RecipeProduct> Entities
    {
        get
        {
            return BaseEntities.Include(e => e.Recipe);
        }
    }
}