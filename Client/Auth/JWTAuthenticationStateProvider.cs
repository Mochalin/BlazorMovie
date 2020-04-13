using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using BlazorDemoUdemy.Client.Helpers;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace BlazorDemoUdemy.Client.Auth{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime js;
        private readonly string TOKENKEY = "TOKENKEY";
        private AuthenticationState Anonymus=>
        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        private readonly HttpClient httpClient;

        public JWTAuthenticationStateProvider(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await js.GetFromLocalStorage(TOKENKEY);
            if (string.IsNullOrEmpty(token)){
                return Anonymus;
            }
            return BuildAuthenticationState(token);
        }

        public AuthenticationState BuildAuthenticationState(string token){
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token),"jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt){
            var claims = new List<Claim>();
            var payload = jwt.Split(".")[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string,object>>(jsonBytes);
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
            if (roles != null){
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                    foreach (var parsedRole in parsedRoles){
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                } else {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }
            claims.AddRange(keyValuePairs.Select(e=>new Claim(e.Key, e.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64){
            switch(base64.Length %4){
                case 2: base64+= "=="; break;
                case 3: base64+= "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async Task Login(string token)
        {
           await js.SetInLocalStorage(TOKENKEY, token);
           var authState = BuildAuthenticationState(token);
           NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await js.RemoveItem(TOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymus));
        }
    }
}