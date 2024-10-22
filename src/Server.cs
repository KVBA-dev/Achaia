using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Achaia;
using HandlerFunc = Func<Context, byte[]>;

internal struct Route {
    public string route;
    public HandlerFunc func;
}

public class Server {
    private readonly Dictionary<string, List<Route>> routes = [];
    /// <summary>
    /// keys are routes, values are paths
    /// </summary>
    private readonly Dictionary<string, Node> routeTrees = [];
    private HttpListener listener = new();
    private bool running = true;
    public const string METHOD_POST = "POST";
    public const string METHOD_GET = "GET";
    public const string METHOD_PUT = "PUT";
    public const string METHOD_PATCH = "PATCH";
    public const string METHOD_DELETE = "DELETE";
    public const string METHOD_HEAD = "HEAD";
    public const string METHOD_TRACE = "TRACE";
    public const string METHOD_OPTIONS = "OPTIONS";
    public const string METHOD_CONNECT = "CONNECT";

    public Logger logger = new();

    public Server() {
        routes[METHOD_DELETE] = [];
        routes[METHOD_GET] = [];
        routes[METHOD_POST] = [];
        routes[METHOD_PATCH] = [];
        routes[METHOD_PUT] = [];
        routes[METHOD_HEAD] = [];
        routes[METHOD_TRACE] = [];
        routes[METHOD_CONNECT] = [];
        routes[METHOD_OPTIONS] = [];
    }

    public void POST(string route, HandlerFunc func) => Add(METHOD_POST, route, func);
    public void PATCH(string route, HandlerFunc func) => Add(METHOD_PATCH, route, func);
    public void DELETE(string route, HandlerFunc func) => Add(METHOD_DELETE, route, func);
    public void PUT(string route, HandlerFunc func) => Add(METHOD_PUT, route, func);
    public void GET(string route, HandlerFunc func) => Add(METHOD_GET, route, func);
    public void HEAD(string route, HandlerFunc func) => Add(METHOD_HEAD, route, func);
    public void TRACE(string route, HandlerFunc func) => Add(METHOD_TRACE, route, func);
    public void OPTIONS(string route, HandlerFunc func) => Add(METHOD_OPTIONS, route, func);
    public void CONNECT(string route, HandlerFunc func) => Add(METHOD_CONNECT, route, func);

    public void Add(string method, string route, HandlerFunc func) {
        routes[method].Add(new Route{
            route = route,
            func = func
        });
    }

    public void Static(string route, string path) {
        Add(METHOD_GET, route + ":file", ctx => StaticHandler(ctx, path));
    }

    private byte[] StaticHandler(Context ctx, string path) {
        string pwd = Environment.CurrentDirectory;
        string fullpath = $"{pwd}{path}\\{ctx.Params["file"]}";
        Console.WriteLine(fullpath);
        if (!File.Exists(fullpath)) {
            return ctx.NoData(404);
        }
        byte[] data = File.ReadAllBytes(fullpath);
        return data;
    }

    private async Task HandleConnections() {
        while (running) {
            HttpListenerContext http_ctx = await listener.GetContextAsync();
            Context ctx = new(http_ctx);

            Console.Write(logger.LogRequest(ctx.Request));
            byte[] data;
            string path = ctx.Request.Url?.AbsolutePath.Substring(1);
            Route? selectedRoute = null;
            foreach (Route route in routes[ctx.Request.HttpMethod]) {
                if (MatchRoutes(ctx, route.route, ctx.Request.Url?.AbsolutePath)) {
                    selectedRoute = route;
                    break;
                }
            }
            if (selectedRoute is null) {
                data = ctx.NoData(400);
            }
            else {
                data = ((Route)selectedRoute).func(ctx);
            }
            Console.WriteLine(logger.LogResponse(ctx.Response));
            await ctx.Send(data);
        }
    }

    private bool MatchRoutes(Context ctx, string route, string path) {
        string[] routeSliced = route.Split('/');
        string[] pathSliced = path.Split('/');

        if (routeSliced.Length != pathSliced.Length) {
            return false;
        }

        for (int i = 0; i < routeSliced.Length; i++) {
            if (routeSliced[i].StartsWith(':')) {
                ctx.Params.Add(routeSliced[i].Substring(1), pathSliced[i]);
                continue;
            }

            if (!routeSliced[i].Equals(pathSliced[i], StringComparison.OrdinalIgnoreCase)) {
                return false;
            }
        }
        return true;
    }

    public void Listen(ushort port) => Listen("localhost", port);
    private void DrawIntro() {
        Console.WriteLine("===================");
        Console.WriteLine($"{TextFMT.BGColour(Colour.RED)}  {TextFMT.BGColour(Colour.YELLOW)}  {TextFMT.BGColour(Colour.GREEN)}  {TextFMT.Reset()}");
        Console.Write($"{TextFMT.BGColour(Colour.YELLOW)}  {TextFMT.BGColour(Colour.GREEN)}  {TextFMT.BGColour(Colour.BLUE)}  {TextFMT.Reset()}  ");
        Console.WriteLine($"{TextFMT.Bold()}{TextFMT.FGColour(Colour.WHITE)}A C H A I A{TextFMT.Reset()}");
        Console.Write($"{TextFMT.BGColour(Colour.GREEN)}  {TextFMT.BGColour(Colour.BLUE)}  {TextFMT.BGColour(Colour.PURPLE)}  {TextFMT.Reset()}  ");
        Console.WriteLine("v0.0.1");
        Console.WriteLine("===================");
    } 
    public void Listen(string address, ushort port) {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => {
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

    ~Server() {
        Console.WriteLine("closing...");
        listener.Close();
    }
}
