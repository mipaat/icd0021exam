using Base.WebHelpers;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProductExistencesController : BaseDbControllerMvc<AppDbContext, ProductExistence>
{
    public ProductExistencesController(AppDbContext dbContext) : base(dbContext)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create(Guid productId, string? returnUrl = null)
    {
        var product = await DbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return NotFound();
        var productExistence = new ProductExistence
        {
            ProductId = productId,
        };
        return View(new ProductExistencesCreateEditViewModel
        {
            Product = product,
            ReturnUrl = returnUrl,
            ProductExistence = productExistence,
        });
    }

    [Authorize]
    [HttpPost]
    [ActionName(nameof(Create))]
    public async Task<IActionResult> CreatePost([FromForm] ProductExistencesCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var product = await DbContext.Products.FirstOrDefaultAsync(p => p.Id == model.ProductExistence.ProductId);
            if (product == null) return NotFound();
            model.Product = product;
            return View(model);
        }

        model.ProductExistence.UserId = User.GetUserId();
        BaseEntities.Add(model.ProductExistence);
        await DbContext.SaveChangesAsync();
        return Redirect(model.ReturnUrl ?? Url.Content("~/"));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
    {
        var userId = User.GetUserId();
        var productExistence = await Entities.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (productExistence == null) return NotFound();
        return View(new ProductExistencesCreateEditViewModel
        {
            Product = productExistence.Product!,
            ReturnUrl = returnUrl,
            ProductExistence = productExistence,
        });
    }

    [Authorize]
    [HttpPost]
    [ActionName(nameof(Edit))]
    public async Task<IActionResult> EditPost([FromForm] ProductExistencesCreateEditViewModel model)
    {
        var userId = User.GetUserId();
        var productExistence =
            await Entities.FirstOrDefaultAsync(e => e.Id == model.ProductExistence.Id && e.UserId == userId);
        if (productExistence == null) return NotFound();
        productExistence.Amount = model.ProductExistence.Amount;
        productExistence.Location = model.ProductExistence.Location;

        if (!ModelState.IsValid)
        {
            model.ProductExistence = productExistence;
            return View(model);
        }

        BaseEntities.Update(productExistence);
        await DbContext.SaveChangesAsync();
        return Redirect(model.ReturnUrl ?? Url.Content("~/"));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
    {
        var userId = User.GetUserId();
        await BaseEntities.Where(e => e.Id == id && e.UserId == userId)
            .ExecuteDeleteAsync();
        return Redirect(returnUrl ?? Url.Content("~/"));
    }

    protected override IQueryable<ProductExistence> Entities
    {
        get
        {
            return BaseEntities.Include(e => e.Product);
        }
    }
}