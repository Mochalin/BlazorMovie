using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorDemoUdemy.Client.Auth {
    public class DummyAuthenticationStateProvider : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
          //  await Task.Delay(3000);
            var anonymus = new ClaimsIdentity(new List<Claim>{
                new Claim("key1","value1"),
                new Claim(ClaimTypes.Name,"Fillipe"),
                new Claim(ClaimTypes.Role, "Admin")
            });
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymus)));
        }
    }
}