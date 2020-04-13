using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.DTO;

namespace BlazorDemoUdemy.Client.Repository{
    public interface IUsersRepository
    {
        Task<PaginatedResponse<List<UserDTO>>> GetUsers(PaginationDTO paginationDTO);
        Task<List<RoleDTO>> GetRoles();
        Task AssignRole(EditRoleDTO editRoleDTO);
        Task RemoveRole(EditRoleDTO editRoleDTO);

    }
}