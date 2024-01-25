namespace Taski.Api.Extensions
{
  public static class PolicyExtension
  {
    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
      return services.AddAuthorization(options =>
      {
        options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
      });
    }
  }
}