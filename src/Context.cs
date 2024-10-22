using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Achaia;

public class Context {
    private readonly HttpListenerContext ctx;
    public HttpListenerRequest Request => ctx.Request;
    public HttpListenerResponse Response => ctx.Response;
    public Dictionary<string, string> Params { get; init; }
    public Context(HttpListenerContext ctx) {
        this.ctx = ctx;
        Params = new();
    }
    public byte[] Text(int status, string text) {
        ctx.Response.StatusCode = status;
        byte[] data = Encoding.UTF8.GetBytes(text);
        return data;
    }

    public byte[] NoData(int status) {
        ctx.Response.StatusCode = status;
        return [];
    }

    public async Task Send(byte[] data) {
        ctx.Response.ContentType = "text/html";
        ctx.Response.ContentEncoding = Encoding.UTF8;
        ctx.Response.ContentLength64 = data.LongLength;
        await ctx.Response.OutputStream.WriteAsync(data);
        ctx.Response.Close();
    }
}