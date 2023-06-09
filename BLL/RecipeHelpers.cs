using BLL.DTO;
using BLL.DTO.Exceptions;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace BLL;

public static class RecipeHelpers
{
    public static float TotalAmount(this Product product) =>
        product.ProductExistences!.Sum(e => e.Amount);

    public static float TotalAmount(this RecipeProduct recipeProduct) =>
        recipeProduct.Product!.TotalAmount();

    public static bool IsMissingIngredient(this RecipeProduct recipeProduct)
    {
        return recipeProduct.TotalAmount() < recipeProduct.Amount;
    }

    public static float MissingAmount(this RecipeProduct recipeProduct) =>
        recipeProduct.Amount - recipeProduct.TotalAmount();

    public static List<MissingIngredient> GetMissingIngredients(this Recipe recipe)
    {
        var result = new List<MissingIngredient>();
        foreach (var recipeProduct in recipe.RecipeProducts!)
        {
            var totalAmount = recipeProduct.Product!.ProductExistences!.Sum(e => e.Amount);
            if (totalAmount < recipeProduct.Amount)
            {
                result.Add(new MissingIngredient(recipeProduct.Product, recipeProduct.Amount - totalAmount));
            }
        }

        return result;
    }

    public static bool IsPreparable(this Recipe recipe)
    {
        return recipe.GetMissingIngredients().Count == 0;
    }

    public static void PrepareRecipe(AppDbContext dbContext, Recipe recipe)
    {
        var missing = new List<MissingIngredient>();
        foreach (var recipeProduct in recipe.RecipeProducts!)
        {
            var amount = recipeProduct.Amount;
            foreach (var productExistence in recipeProduct.Product!.ProductExistences!)
            {
                if (amount <= 0) break;
                var amountToRemove = Math.Min(amount, productExistence.Amount);
                productExistence.Amount -= amountToRemove;
                amount -= amountToRemove;
                dbContext.Entry(productExistence).Property(e => e.Amount).IsModified = true;
            }

            if (amount > 0)
            {
                missing.Add(new MissingIngredient(recipeProduct.Product, amount));
            }
        }

        if (missing.Count > 0) throw new NotEnoughIngredientsException(missing);
    }

    public static async Task<List<Recipe>> GetRecipes(this AppDbContext dbContext,
        Guid? userId = null,
        string? nameQuery = null,
        string? includesIngredientQuery = null,
        string? excludesIngredientQuery = null,
        int? minPrepareTime = null,
        int? maxPrepareTime = null)
    {
        IQueryable<Recipe> query = dbContext.Recipes.Include(e => e.Creator)
            .Include(e => e.RecipeProducts!)
            .ThenInclude(e => e.Product!)
            .ThenInclude(e => e.ProductExistences!.Where(p => p.UserId == userId));
        if (nameQuery != null)
        {
            query = query.Where(e => EF.Functions.Like(e.Name, $"%{nameQuery}%"));
        }

        if (includesIngredientQuery != null)
        {
            var requiredIngredients = includesIngredientQuery.Split(", ");
            query = query.Where(e =>
                e.RecipeProducts!.Select(rp => rp.Product)
                    .Any(p => requiredIngredients.Any(i => EF.Functions.ILike(p!.Name, i))));
        }

        if (excludesIngredientQuery != null)
        {
            var excludedIngredients = excludesIngredientQuery.Split(", ");
            query = query.Where(e => 
                e.RecipeProducts!.Select(rp => rp.Product!)
                    .All(p => !excludedIngredients.Any(i => EF.Functions.ILike(p!.Name, i))));
        }

        if (minPrepareTime != null)
        {
            query = query.Where(e => e.PrepareTimeMinutes >= minPrepareTime);
        }

        if (maxPrepareTime != null)
        {
            query = query.Where(e => e.PrepareTimeMinutes <= maxPrepareTime);
        }

        return await query.ToListAsync();
    }
}