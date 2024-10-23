using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Achaia;

public class RouteNode {
    public readonly List<RouteNode> children = new();
    public readonly string path;
    public Func<Context, byte[]> func = (ctx) => Array.Empty<byte>();
    public readonly bool isRoot;

    public RouteNode(RouteNode? parent, string path) {
        parent?.children.Add(this);
        this.path = path;
        isRoot = parent is null;
    }

    public RouteNode? Find(Context ctx, string path) {
        var splitPath = path.Split('/').Where(s => s.Length > 0).ToArray();

        if (splitPath.Length == 0) {
            return this;
        }

        foreach(RouteNode n in children) {
            if (n.path.Equals(splitPath[0])) {
                StringBuilder builder = new();
                builder.AppendJoin('/', splitPath.Skip(1));
                return n.Find(ctx, builder.ToString());
            }

            if (n.path.StartsWith(':')) {                
                ctx.Params.Add(n.path.Substring(1), splitPath[0]);
                StringBuilder builder = new();
                builder.AppendJoin('/', splitPath.Skip(1));
                return n.Find(ctx, builder.ToString());
            }
        }
        return null;
    }

    public RouteNode AddRoute(string route) {
        if (!isRoot) {
            return this;
        }
        return AddRoute(route.Split('/').Where(s => s.Length > 0).ToArray().AsSpan());
    }

    private RouteNode AddRoute(Span<string> route) {
        if (route.Length == 0) {
            return this;
        }
        foreach (RouteNode n in children) {
            if (n.path == route[0]) {
                return n.AddRoute(route[1..]);
            }
        }
        RouteNode newNode = new(this, route[0]);
        return newNode.AddRoute(route[1..]);
    }

    public void SetFunction(Func<Context, byte[]> func) => this.func = func;

    public void PrintTree(int indent = 0) {
        for (int i = 0; i < indent; i++) { 
            Console.Write(' '); 
        }
        Console.WriteLine(path);
        foreach(RouteNode n in children) {
            n.PrintTree(indent + 4);
        }
    }
}