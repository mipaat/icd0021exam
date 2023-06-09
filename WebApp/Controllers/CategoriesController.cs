using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class CategoriesController : BaseEntityCrudControllerMvc<AppDbContext, Category>
{
    public CategoriesController(AppDbContext dbContext) : base(dbContext)
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
    public async Task<IActionResult> CreatePost([FromForm] Category category)
    {
        return await CreateInternal(category);
    }

    [HttpPost]
    [ActionName(nameof(Edit))]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> EditPost([FromForm] Category category)
    {
        return await EditInternal(category.Id, category);
    }
}