# Achaia

Achaia is a simple HTTP server for C#. Inspired by Go's Echo and Gin, Achaia aims for simplicity of creating the server and not dictating the code structure.

It is still very early in development, so not only it's still a bug-filled mess with hardly any features, but the API might change in the future as well, so breaking changes
are to be expected.

Example server:
```cs
using Achaia;

Server server = new();

server.Static("/css/", "/css")

server.GET("/", ctx => ctx.Text(200, "Hello, world!"));

server.POST("/user/:id", ctx => {
    if (int.TryParse(ctx.Params["id"], out int id)) {
        return ctx.Text(200, $"Received POST! {id}");
    }
    return ctx.NoData(400);
});

server.Listen(8080);
```

Current features:
- Easy defining of the routes of all HTTP verbs
- Route parameters, starting with colon sign, which can be used to pass data via routes themselves
- JSON requests and responses support

## Obtaining
Simply clone this repository into your project or, if you prefer to use a DLL, clone this repository into a directory and run `dotnet build`, then copy the resulting files to your project.