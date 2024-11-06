using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Achaia;

public class Context {
    private readonly HttpListenerContext ctx;
    public HttpListenerRequest Request => ctx.Request;
    public HttpListenerResponse Response => ctx.Response;
    public Dictionary<string, string> Params { get; init; }
    internal string RequestContent {private get; set; }
    public Context(HttpListenerContext ctx) {
        this.ctx = ctx;
        Params = new();
    }

    public T? GetJsonBody<T>() {
        T? obj;
        try {
            obj = JsonSerializer.Deserialize<T>(RequestContent);
        }
        catch {
            obj = default;
        }
        return obj;
    }

    public byte[] Text(int status, string text) {
        Response.ContentType = "text/plain";
        Response.StatusCode = status;
        return Encoding.UTF8.GetBytes(text);
    }

    public byte[] HTML(int status, string html) {
        Response.ContentType = "text/html";
        Response.StatusCode = status;
        return Encoding.UTF8.GetBytes(html);
    }

    public byte[] NoData(int status) {
        Response.StatusCode = status;
        return [];
    }

    public async Task Send(byte[] data) {
        Response.ContentEncoding = Encoding.UTF8;
        Response.ContentLength64 = data.LongLength;
        await Response.OutputStream.WriteAsync(data);
        Response.Close();
    }

    public byte[] JSON<T>(int status, T obj) {
        string json = JsonSerializer.Serialize(obj);
        Response.ContentType = "text/json";
        Response.StatusCode = status;
        return Encoding.UTF8.GetBytes(json);
    }
}