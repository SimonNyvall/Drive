using Microsoft.EntityFrameworkCore;
using Context;

namespace Server.Extensions;

public static class DatabaseContextExtension
{
    public static WebApplicationBuilder UseFileDbContext(this WebApplicationBuilder builder, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            // TODO: Make this a logger
            Console.WriteLine("Connection string is not set. No restore data will be available.");

            return builder;
        }

        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        return builder;
    }
}