using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorDemoUdemy.Server.Helpers;
using BlazorDemoUdemy.Shared.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemoUdemy.Server.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase{
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get ([FromQuery] PaginationDTO paginationDTO){
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO)
            .Select(x=> new UserDTO(){Email = x.Email, UserId = x.Id}).ToListAsync();

        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<RoleDTO>>> Get(){
            return await context.Roles.Select(e=> new RoleDTO(){RoleName = e.Name}).ToListAsync();
        }

        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO){
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        }
        
        [HttpPost("removeRole")]
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO){
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        }
    }
}