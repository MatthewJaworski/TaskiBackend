using Microsoft.EntityFrameworkCore;

namespace Taski.Api.Data;
public static class DataExtensions {
  public static void InitializeDatabase (this IServiceProvider serviceProvider){
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskiAppContext>();
        context.Database.Migrate();
    }
}