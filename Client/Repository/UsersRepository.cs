using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Client.Helpers;
using BlazorDemoUdemy.Shared.DTO;

namespace BlazorDemoUdemy.Client.Repository{
    public class UsersRepository : IUsersRepository{

        private readonly IHttpService httpService;
        private readonly string url = "api/users";

        public UsersRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<PaginatedResponse<List<UserDTO>>> GetUsers(PaginationDTO paginationDTO){
           return await httpService.GetHelper<List<UserDTO>>(url, paginationDTO);
        }

        public async Task<List<RoleDTO>> GetRoles(){
           return await httpService.GetHelper<List<RoleDTO>>($"{url}/roles");
        }

        public async Task AssignRole(EditRoleDTO editRoleDTO){
            var response = await httpService.Post($"{url}/assignRole",editRoleDTO);
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }

        }

          public async Task RemoveRole(EditRoleDTO editRoleDTO){
            var response = await httpService.Post($"{url}/removeRole",editRoleDTO);
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }

        }
    }
}