using System;
using System.Collections.Generic;

namespace Achaia;
using HandlerFunc = Func<Context, byte[]>;

internal class Node {
    public readonly List<Node> children = [];
    public Node? parent = null;
    public bool IsLeaf => children.Count == 0;
    public string route = string.Empty;
    public string? staticContent = null;
    public HandlerFunc func;

    public void Insert(Span<string> route) {
        if (Find(route) is not null) {
            return;
        }

    }

    public Node? Find(Span<string> route) {
        if (route[0] != this.route) {
            return null;
        }
        if (route.Length == 1) {
            return this;
        }
        Node? result = null;
        foreach (Node n in children) {
            result = n.Find(route[1..]);
            if (result is not null) {
                break;
            }
        }
        return result;
    }
}

internal class RouteTree {
    public Node? root = null;

    public void Insert(Route route) {
        if (route.route == "/") {
            if (root is not null) {
                return;
            }
            root = new() {
                route = route.route,
                func = route.func
            };
            return;
        }

        Span<string> splitRoute = route.route.Split('/').AsSpan();
        if (root.Find(splitRoute) is not null) {
            return;
        }
        root.Insert(splitRoute);
        
    }
    
}