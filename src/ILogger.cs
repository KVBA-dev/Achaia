using System.Net;

namespace Achaia;

public interface ILogger {
    public void LogRequest(HttpListenerRequest req);
    public void LogResponse(HttpListenerResponse res);
}