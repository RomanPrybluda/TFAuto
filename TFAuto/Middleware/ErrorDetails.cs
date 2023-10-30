using Newtonsoft.Json;

namespace TFAuto.WebApp.Middleware;

public class ErrorDetails
{
    public int StatusCode { get; set; }

    public string Message { get; set; }

    public string DisplayMessage { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}