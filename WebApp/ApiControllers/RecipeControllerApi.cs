using AutoMapper;
using Base.WebHelpers;
using BLL;
using BLL.DTO;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[Route("api/recipes/[action]")]
public class RecipeControllerApi : BaseDbControllerApi<AppDbContext, Recipe>
{
    public RecipeControllerApi(AppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<ActionResult<List<Public.DTO.RecipeWithIngredients>>> GetRecipes([FromQuery] Public.DTO.RecipeSearch? search)
    {
        var recipes = await DbContext.GetRecipes(
            userId: User.GetUserIdIfExists(),
            nameQuery: search?.NameQuery,
            includesIngredientQuery: search?.IncludesIngredientQuery,
            excludesIngredientQuery: search?.ExcludesIngredientQuery,
            minPrepareTime: search?.MinPrepareTime,
            maxPrepareTime: search?.MaxPrepareTime,
            privacyFilter: search?.PrivacyFilter ?? ERecipePrivacyFilter.All,
            filterServable: search?.FilterServable ?? false,
            servingsAmount: search?.Servings
        );
        var result = new List<Public.DTO.RecipeWithIngredients>();
        foreach (var recipe in recipes)
        {
            var dtoRecipe = new Public.DTO.RecipeWithIngredients
            {
                Creator = Mapper.Map<Public.DTO.Identity.User>(recipe.Creator),
                Id = recipe.Id,
                Instructions = recipe.Instructions,
                IsPrivate = recipe.IsPrivate,
                Name = recipe.Name,
                PrepareTimeMinutes = recipe.PrepareTimeMinutes,
                Servings = recipe.Servings,
                RecipeProducts = new List<Public.DTO.RecipeProduct>(),
            };
            foreach (var recipeProduct in recipe.RecipeProducts!)
            {
                dtoRecipe.RecipeProducts.Add(new Public.DTO.RecipeProduct
                {
                    Amount = recipeProduct.Amount,
                    Id = recipeProduct.Id,
                    Product = Mapper.Map<Public.DTO.Product>(recipeProduct.Product),
                });
            }
            result.Add(dtoRecipe);
        }

        return Ok(result);
    }
}