using AutoMapper;
using BLL.Identity;
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.DTO;
using Public.DTO.Identity;

namespace WebApp.ApiControllers.Admin;

[ApiController]
[Route("api/admin/[controller]/[action]")]
[Authorize(Roles = RoleNames.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ManageUsersController : ControllerBase
{
    private readonly IdentityUow _identityUow;
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public ManageUsersController(IdentityUow identityUow, IMapper mapper, AppDbContext dbContext)
    {
        _identityUow = identityUow;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserWithRoles>>> ListAll([FromQuery] UserFilter filter)
    {
        var users = await _identityUow.UserService.GetUsersWithRoles(nameQuery: filter.NameQuery);
        return Ok(users.Select(u => _mapper.Map<UserWithRoles>(u)));
    }

    [HttpGet]
    public ActionResult<List<string>> ListAllRoleNames()
    {
        return Ok(RoleNames.AllAsList);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        if (!User.IsAllowedToManageRole(roleName)) return Forbid();
        await _identityUow.UserService.RemoveUserFromRole(userId, roleName);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(Guid userId, string roleName)
    {
        if (!User.IsAllowedToManageRole(roleName)) return Forbid();
        await _identityUow.UserService.AddUserToRole(userId, roleName);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}