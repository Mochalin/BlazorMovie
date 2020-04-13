using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorDemoUdemy.Client.Helpers {

    public class HttpService: IHttpService
    {
        private readonly HttpClient httpClient;
        private JsonSerializerOptions defaultJsonSerializerOptions =>
            new JsonSerializerOptions() {PropertyNameCaseInsensitive = true };
        public HttpService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseWrapper<T>> Get<T>(string url){
            var responseHttp = await httpClient.GetAsync(url);
            if (responseHttp.IsSuccessStatusCode){
                var response = await Desirialize<T>(responseHttp, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<T>(response,true, responseHttp);
            } else {
                return new HttpResponseWrapper<T>(default, false, responseHttp);
            }


        }

        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T data){
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, stringContent);
            return new HttpResponseWrapper<object> (null, response.IsSuccessStatusCode, response);
        }

        
        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T data){
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(url, stringContent);
            return new HttpResponseWrapper<object> (null, response.IsSuccessStatusCode, response);
        }

          public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data){
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, stringContent);
            if (response.IsSuccessStatusCode){
                var responseDeserialized = await Desirialize<TResponse>(response, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
            } else {
               return new HttpResponseWrapper<TResponse>(default, false, response);
            }          
        }

        private async Task<T> Desirialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options){
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, options);  
        }

        public async Task<HttpResponseWrapper<object>> Delete (string url){
            var responseHTTP = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, responseHTTP.IsSuccessStatusCode, responseHTTP ); 
        }
        
    }
}