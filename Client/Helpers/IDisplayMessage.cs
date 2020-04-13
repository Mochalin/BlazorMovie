using System.Threading.Tasks;

namespace BlazorDemoUdemy.Client.Helpers{
    public interface IDisplayMessage{
        ValueTask DisplayErrorMessage(string message);
        ValueTask DisplaySuccessMessage(string message);

    }
   
}