using System;
using System.IO;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.WebSockets;

namespace Achaia;
using HandlerFunc = Func<Context, Task<byte[]>>;

public class Server
{
    /// <summary>
    /// keys are HTTP methods, values are paths
    /// </summary>
    private readonly Dictionary<string, RouteNode?> routes = [];
    private HttpListener listener = new();
    private bool running = true;

    public ILogger? logger = null;

    public Server()
    {
        routes[Method.DELETE] = null;
        routes[Method.GET] = null;
        routes[Method.POST] = null;
        routes[Method.PATCH] = null;
        routes[Method.PUT] = null;
        routes[Method.HEAD] = null;
        routes[Method.TRACE] = null;
        routes[Method.CONNECT] = null;
        routes[Method.OPTIONS] = null;
    }

    public void POST(string route, HandlerFunc func) => Add(Method.POST, route, func);
    public void PATCH(string route, HandlerFunc func) => Add(Method.PATCH, route, func);
    public void DELETE(string route, HandlerFunc func) => Add(Method.DELETE, route, func);
    public void PUT(string route, HandlerFunc func) => Add(Method.PUT, route, func);
    public void GET(string route, HandlerFunc func) => Add(Method.GET, route, func);
    public void HEAD(string route, HandlerFunc func) => Add(Method.HEAD, route, func);
    public void TRACE(string route, HandlerFunc func) => Add(Method.TRACE, route, func);
    public void OPTIONS(string route, HandlerFunc func) => Add(Method.OPTIONS, route, func);
    public void CONNECT(string route, HandlerFunc func) => Add(Method.CONNECT, route, func);

    public void Add(string method, string route, HandlerFunc func)
    {
        if (routes[method] is null)
        {
            routes[method] = new(null, "/");
            routes[method].SetFunction(func);
        }
        routes[method].AddRoute(route).SetFunction(func);
    }

    public void WebSocket()
    {
    }

    public void Static(string route, string path)
    {
        Add(Method.GET, route + ":file", ctx => StaticHandler(ctx, path));
    }

    public void UseLogger(ILogger logger)
    {
        this.logger = logger;
    }

    private async Task<byte[]> StaticHandler(Context ctx, string path)
    {
        string pwd = Environment.CurrentDirectory;
        string fullpath = $"{pwd}{path}/{ctx.Params["file"]}";
        if (!File.Exists(fullpath))
        {
            return ctx.NoData(404);
        }
        byte[] data = await File.ReadAllBytesAsync(fullpath);
        return data;
    }

    private async Task HandleConnections()
    {
        while (running)
        {
            HttpListenerContext http_ctx = await listener.GetContextAsync();
            Context ctx = new(http_ctx);

            logger?.LogRequest(ctx.Request);
            byte[] data;
            string path = ctx.Request.Url?.AbsolutePath.Substring(1);
            RouteNode? node = routes[ctx.Request.HttpMethod]?.Find(ctx, ctx.Request.Url?.AbsolutePath);

            string body;
            using (StreamReader reader = new(ctx.Request.InputStream))
            {
                body = await reader.ReadToEndAsync();
            }
            ctx.RequestContent = body;

            if (node is null)
            {
                data = ctx.NoData(400);
            }
            else
            {
                data = await node?.func(ctx);
            }
            logger?.LogResponse(ctx.Response);
            await ctx.Send(data);
        }
    }

    public void Listen(ushort port) => Listen("localhost", port);

    private static void DrawIntro()
    {
        Console.WriteLine("===================");
        Console.WriteLine($"{TextFMT.BGColour(Colour.RED)}  {TextFMT.BGColour(Colour.YELLOW)}  {TextFMT.BGColour(Colour.GREEN)}  {TextFMT.Reset()}");
        Console.Write($"{TextFMT.BGColour(Colour.YELLOW)}  {TextFMT.BGColour(Colour.GREEN)}  {TextFMT.BGColour(Colour.BLUE)}  {TextFMT.Reset()}  ");
        Console.WriteLine($"{TextFMT.Bold()}{TextFMT.FGColour(Colour.WHITE)}A C H A I A{TextFMT.Reset()}");
        Console.Write($"{TextFMT.BGColour(Colour.GREEN)}  {TextFMT.BGColour(Colour.BLUE)}  {TextFMT.BGColour(Colour.PURPLE)}  {TextFMT.Reset()}  ");
        Console.WriteLine("v0.0.1");
        Console.WriteLine("===================");
    }

    public void Listen(string address, ushort port)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            Console.WriteLine("closing...");
            running = false;
            listener.Close();
        };
        DrawIntro();
        listener.Prefixes.Add($"http://{address}:{port}/");
        listener.Start();
        Console.WriteLine($"Listening on port {port}...");
        HandleConnections().GetAwaiter().GetResult();

    }

    public void ShowAllRoutes()
    {
        foreach (var pair in routes)
        {
            if (pair.Value is null)
            {
                continue;
            }
            Console.WriteLine(pair.Key);
            pair.Value.PrintTree();
        }
    }

    ~Server()
    {
        Console.WriteLine("closing...");
        listener.Close();
    }
}
