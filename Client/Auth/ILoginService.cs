using System.Threading.Tasks;

namespace BlazorDemoUdemy.Client.Auth{
    public interface ILoginService
    {
        Task Login(string token);
        Task Logout();
    }
}