using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorDemoUdemy.Client.Helpers
{
    public static class JSRuntimeExtensionMethods
    {
        public static async ValueTask<bool> Confirm (this IJSRuntime js, string message){
              await js.InvokeVoidAsync("console.log","example messages");
              return await js.InvokeAsync<bool>("confirm",message);
        }

        public static async ValueTask MyFunction(this IJSRuntime js, string message){
            await js.InvokeVoidAsync("my_function",message);
        }

        public static ValueTask<object> SetInLocalStorage(this IJSRuntime jS, string key, string content)=>
        jS.InvokeAsync<object>("localStorage.setItem", key, content);

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime jS, string key)=>
        jS.InvokeAsync<string>("localStorage.getItem", key);

        public static ValueTask<object> RemoveItem(this IJSRuntime jS, string key)=>
        jS.InvokeAsync<object>("localStorage.removeItem", key);
    }
}