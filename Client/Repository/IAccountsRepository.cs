using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.DTO;

namespace BlazorDemoUdemy.Client.Repository{
    public interface IAccountsRepository 
    {
       Task<UserToken> Register(UserInfo userInfo); 
       Task<UserToken> Login(UserInfo userInfo);
    }
}