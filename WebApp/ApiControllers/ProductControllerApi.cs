using AutoMapper;
using AutoMapper.QueryableExtensions;
using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers;

[Route("api/products/[action]")]
public class ProductControllerApi : BaseDbControllerApi<AppDbContext, Product>
{
    public ProductControllerApi(AppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<Public.DTO.Product>>> GetAll()
    {
        return await Entities.ProjectTo<Public.DTO.Product>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(Public.DTO.ProductCreateData product)
    {
        var newProduct = new Product
        {
            Name = product.Name,
            Unit = product.Unit,
        };
        BaseEntities.Add(newProduct);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<Public.DTO.Product>> GetById(Guid id)
    {
        var result = await Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (result == null) return NotFound();
        return Mapper.Map<Public.DTO.Product>(result);
    }

    [HttpDelete]
    [Authorize(Roles = RoleNames.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await BaseEntities.Where(e => e.Id == id).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = RoleNames.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit(Public.DTO.Product product)
    {
        if (!await BaseEntities.AnyAsync(e => e.Id == product.Id)) return NotFound();
        var mapped = Mapper.Map<Product>(product);
        BaseEntities.Update(mapped);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<ActionResult<List<Public.DTO.ProductExistence>>> GetExistences()
    {
        var userId = User.GetUserId();
        var result = await DbContext.ProductExistences
            .Include(e => e.Product)
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ProjectTo<Public.DTO.ProductExistence>(Mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<ActionResult<Public.DTO.ProductExistence>> GetExistenceById(Guid id)
    {
        var userId = User.GetUserId();
        var result = await DbContext.ProductExistences
            .Include(e => e.Product)
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ProjectTo<Public.DTO.ProductExistence>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> CreateExistence(Public.DTO.ProductExistenceData data)
    {
        if (!await DbContext.Products.AnyAsync(e => e.Id == data.ProductId)) return NotFound();
        DbContext.ProductExistences.Add(new ProductExistence
        {
            ProductId = data.ProductId,
            Amount = data.Amount,
            Location = data.Location,
            UserId = User.GetUserId(),
        });
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdateExistence([FromQuery] Guid id, [FromBody] Public.DTO.ProductExistenceData data)
    {
        Console.WriteLine("HLLEOLOEFOL");
        Console.WriteLine(id);
        if (!await DbContext.ProductExistences.AnyAsync(e => e.Id == id)) return NotFound();
        var userId = User.GetUserId();
        if (!await DbContext.ProductExistences.AnyAsync(e => e.Id == id && e.UserId == userId)) return Forbid();
        if (!await DbContext.Products.AnyAsync(e => e.Id == data.ProductId)) return NotFound();
        DbContext.ProductExistences.Update(new ProductExistence
        {
            Id = id,
            Amount = data.Amount,
            Location = data.Location,
            ProductId = data.ProductId,
            UserId = userId,
        });
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteExistence(Guid id)
    {
        var userId = User.GetUserId();
        await DbContext.ProductExistences
            .Where(e => e.Id == id && e.UserId == userId)
            .ExecuteDeleteAsync();
        return Ok();
    }
}