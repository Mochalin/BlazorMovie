using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorDemoUdemy.Client.Helpers;
using Blazor.FileReader;
using BlazorDemoUdemy.Client.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorDemoUdemy.Client.Auth;

namespace BlazorDemoUdemy.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddTransient<IRepository, RepositoryInMemory>();   
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();  
            builder.Services.AddScoped<IPersonRepository,PersonRepository>();
            builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
            builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<IDisplayMessage, DisplayMessage>();
            builder.Services.AddScoped<IUsersRepository,UsersRepository>();

            builder.Services.AddFileReaderService(options=> options.InitializeOnFirstCall = true);

            builder.Services.AddScoped<JWTAuthenticationStateProvider>();

            builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>(
                provider => provider.GetRequiredService<JWTAuthenticationStateProvider>()
            );  
            builder.Services.AddScoped<ILoginService, JWTAuthenticationStateProvider>(
                provider => provider.GetRequiredService<JWTAuthenticationStateProvider>()
            );  


            builder.Services.AddOptions();      
            builder.Services.AddAuthorizationCore();    
            await builder.Build().RunAsync();
        }
    }
}
