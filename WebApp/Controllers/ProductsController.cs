using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

public class ProductsController : BaseEntityCrudControllerMvc<AppDbContext, Product>
{
    public ProductsController(AppDbContext dbContext) : base(dbContext)
    {
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Edit(Guid id)
    {
        return base.Edit(id);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Create()
    {
        return base.Create();
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Delete(Guid id)
    {
        return base.Delete(id);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> DeleteConfirmed(Guid id)
    {
        return base.DeleteConfirmed(id);
    }

    [HttpPost]
    [ActionName(nameof(Create))]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> CreatePost([FromForm] Product entity)
    {
        return await CreateInternal(entity);
    }

    [HttpPost]
    [ActionName(nameof(Edit))]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> EditPost([FromForm] Product entity)
    {
        return await EditInternal(entity.Id, entity);
    }

    protected override IQueryable<Product> Entities
    {
        get
        {
            var userId = User.GetUserIdIfExists();
            return BaseEntities.Include(p => p.ProductExistences!.Where(e => e.UserId == userId));
        }
    }
}