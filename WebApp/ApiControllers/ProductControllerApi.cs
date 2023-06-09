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
}