using Services.FileService;
using Models;
using Services.RestoreService;

namespace Server.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection UseFileService(this IServiceCollection service)
    {
        service.AddSingleton<IFileService, FileService>();
        return service;
    }

    public static WebApplication MapFileService(this WebApplication app)
    {
        app.MapGet("/api/file/{path}", (IFileService fileServie, string path) => {
            var nodes = fileServie.GetNodes(path);

            var files = nodes
                .Where(n => n is Models.File)
                .Select(n => (Models.File)n)
                .ToArray();

            var file = files.Where(f => f.Path == path).FirstOrDefault();

            if (file is null) return Results.NotFound();

            return Results.Ok(file);
        });

        app.MapPost("/api/file", async (IFileService fileService, IRestoreService restoreService, Models.File file) => 
        {
            fileService.CreateFile(file);

            await restoreService.SyncData(file, RestoreType.Creation);

            return Results.Created($"/api/node/{file.Path}", file);
        });

        app.MapPut("/api/file/{path}", async (IFileService fileService, IRestoreService restoreService, string path, Models.File file) =>
        {
            fileService.UpdateFile(path, file);

            await restoreService.SyncData(file, RestoreType.Update);

            return Results.Ok(file);
        });

        app.MapDelete("/api/file/{path}", async (IFileService fileService, IRestoreService restoreService, string path) =>
        {
            fileService.DeleteFile(path);

            await restoreService.SyncData(new Models.File(path, string.Empty, System.Array.Empty<byte>()), RestoreType.Deletion);

            return Results.NoContent();
        });

        app.MapGet("/api/directory/{path}/children", (IFileService fileService, IRestoreService restoreService, string path) =>
        {
            path = path.Replace("#", "/"); // Can make it diffecult to use api

            var nodes = fileService.GetNodes(path);

            if (nodes.Length == 0)
            {
                restoreService.GetRestoration();
                nodes = fileService.GetNodes(path);
            }

            try
            {
                var dtos = nodes.Select<SystemNode, object>(n => n switch
                {
                    Models.File file => file.ToDto(),
                    Models.Directory directory => directory.ToDto(),
                    _ => throw new Exception("Unknown node type")
                }).ToArray();

                return Results.Ok(dtos);
            }
            catch
            {
                return Results.Problem();
            }
        });

        app.MapPost("/api/directory", async (IFileService fileService, IRestoreService restoreService, Models.Directory directory) =>
        {
            fileService.CreateDirectory(directory);

            await restoreService.SyncData(directory, RestoreType.Creation);

            return Results.Created($"/api/node/{directory.Path}", directory);
        });

        app.MapDelete("/api/directory/{path}", async (IFileService fileService, IRestoreService restoreService, string path) =>
        {
            fileService.DeleteDirectory(path);

            await restoreService.SyncData(new Models.Directory(path), RestoreType.Deletion);

            return Results.NoContent();
        });

        return app;
    }
}