using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using static BlazorDemoUdemy.Client.Shared.MainLayout;

namespace BlazorDemoUdemy.Client.Pages
{

    public partial class Counter
    {
       private int currentCount = 0;
       [CascadingParameter] private Task<AuthenticationState> authenticationState {get; set;}
      public async Task IncrementCount()
    {
        var authState = await authenticationState;
        var user = authState.User;
        if (user.Identity.IsAuthenticated){
        currentCount++;            
        } else {
            currentCount--;
        }
    }
    }
}