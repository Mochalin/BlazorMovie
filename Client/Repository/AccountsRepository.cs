using System;
using System.Threading.Tasks;
using BlazorDemoUdemy.Client.Helpers;
using BlazorDemoUdemy.Shared.DTO;

namespace BlazorDemoUdemy.Client.Repository {
    public class AccountsRepository: IAccountsRepository{
        private readonly IHttpService httpService;
        private readonly string baseURL="api/accounts";

        public AccountsRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<UserToken> Register(UserInfo userInfo){
            var httpResponse = await httpService.Post<UserInfo, UserToken>($"{baseURL}/create", userInfo);
            if (!httpResponse.Success){
                throw new ApplicationException(await httpResponse.GetBody());
            }
            return httpResponse.Response;
            
        }

           public async Task<UserToken> Login(UserInfo userInfo){
            var httpResponse = await httpService.Post<UserInfo, UserToken>($"{baseURL}/login", userInfo);
            if (!httpResponse.Success){
                throw new ApplicationException(await httpResponse.GetBody());
            }
            return httpResponse.Response;
            
        }
    }
}