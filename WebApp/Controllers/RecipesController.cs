using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

public class Recipes : BaseEntityCrudControllerMvc<AppDbContext, Recipe>
{
    public Recipes(AppDbContext dbContext) : base(dbContext)
    {
    }
    
    [Authorize]
    public override async Task<IActionResult> Edit(Guid id)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();
        return await base.Edit(id);
    }

    [Authorize]
    public override Task<IActionResult> Create()
    {
        return base.Create();
    }

    [Authorize]
    public override async Task<IActionResult> Delete(Guid id)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();
        return await base.Delete(id);
    }

    [Authorize]
    public override async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (!await IsAdminOrAuthor(id)) return Forbid();
        return await base.DeleteConfirmed(id);
    }

    [HttpPost]
    [ActionName(nameof(Create))]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromForm] Recipe entity)
    {
        entity.CreatorId = User.GetUserId();
        return await CreateInternal(entity);
    }

    [HttpPost]
    [ActionName(nameof(Edit))]
    [Authorize]
    public async Task<IActionResult> EditPost([FromForm] Recipe entity)
    {
        if (!await IsAdminOrAuthor(entity.Id)) return Forbid();
        return await EditInternal(entity.Id, entity);
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
            return BaseEntities.Include(e => e.Creator);
        }
    }
}