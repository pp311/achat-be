using Microsoft.EntityFrameworkCore;
using AChat.Infrastructure.Data;

namespace AChat.Extensions;

public static class MigrateExtension
{
    public static async Task MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    } 
}
