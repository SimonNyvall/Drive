

using System.Text;
using System.Text.Json;

namespace Server.Middleware.Hateoas;

public class HateoasMiddleware
{
    private readonly RequestDelegate _next;

    public HateoasMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stream originalBodyStream = context.Response.Body;

        string originalResponseBody = await GetResponseBody(context);

        IEnumerable<Endpoint> callableEndpoints = GetCallableEndpoints(context);

        if (callableEndpoints.Any(e => e.DisplayName is null))
        {
            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(originalResponseBody, Encoding.UTF8);
            return;
        }

        string json = JsonSerializer.Serialize(callableEndpoints.Select(e => e.DisplayName));

        JsonElement parsedOriginalResponseBody = JsonSerializer.Deserialize<JsonElement>(originalResponseBody);

        var combinedResponseBody = new
        {
            Data = parsedOriginalResponseBody,
            Endpoints = callableEndpoints.Select(e => new ApiEndpoint(
                e.DisplayName!.Contains("file") ? "file" : "directory",
                e.DisplayName.Substring(e.DisplayName.IndexOf(' ', 6) + 1),
                e.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.First() ?? "ERROR"
            ))
        };

        var jsonResponse = JsonSerializer.Serialize(combinedResponseBody, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        context.Response.ContentType = "application/json";
        context.Response.Body = originalBodyStream;
        await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
    }

    private async Task<string> GetResponseBody(HttpContext context)
    {
        using MemoryStream bodyStream = new();

        context.Response.Body = bodyStream;

        await _next(context);

        bodyStream.Seek(0, SeekOrigin.Begin);

        return await new StreamReader(bodyStream).ReadToEndAsync();
    }

    private IEnumerable<Endpoint> GetCallableEndpoints(HttpContext context)
    {
        var endpoints = context.RequestServices.GetService<EndpointDataSource>();

        if (endpoints is null) return [];

        IEnumerable<Endpoint> endpointList = endpoints.Endpoints;

        var currentEndpoint = context.GetEndpoint();

        if (currentEndpoint is not null)
        {
            endpointList = endpointList
                .Where(e => e.DisplayName is not null && e.DisplayName.Contains(currentEndpoint.DisplayName!));
        }

        return endpointList;
    }
}

