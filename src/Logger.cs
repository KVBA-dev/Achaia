using System;
using System.Net;
using System.Text;

namespace Achaia;


public class Logger {
    public string LogRequest(HttpListenerRequest req) {
        StringBuilder builder = new();
        builder.Append($"[{DateTime.Now}]\t");
        builder.Append($"{TextFMT.FormatMethod(req.HttpMethod)}\t");
        builder.Append($"{TextFMT.Foreground(252, 186, 3)}{req.Url?.AbsolutePath}{TextFMT.Reset()}\t");
        return builder.ToString();
    }

    public string LogResponse(HttpListenerResponse resp) {
        StringBuilder builder = new();
        builder.Append($"\t| Response: {TextFMT.FormatStatus(resp.StatusCode)}");
        return builder.ToString();
    }
}