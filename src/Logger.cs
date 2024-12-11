using System.Net;
using System.Text;

namespace Achaia;

public class Logger : ILogger {
    public void LogRequest(HttpListenerRequest req) {
        StringBuilder builder = new();
        builder.Append($"[{DateTime.Now}]\t");
        builder.Append($"{TextFMT.FormatMethod(req.HttpMethod)}\t");
        builder.Append($"{TextFMT.Foreground(252, 186, 3)}{req.Url?.AbsolutePath,-25}{TextFMT.Reset()}\t");
        Console.Write(builder.ToString());
    }

    public void LogResponse(HttpListenerResponse resp) {
        StringBuilder builder = new();
        builder.Append($"\t| Response: {TextFMT.FormatStatus(resp.StatusCode)}");
        Console.WriteLine(builder.ToString());
    }
}