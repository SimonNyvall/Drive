using Server.Middleware.Hateoas;

namespace Server.Extensions;

public static class HateoasExtension
{
    public static IApplicationBuilder UseHateoas(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HateoasMiddleware>();
    }
}
