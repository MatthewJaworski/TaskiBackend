using Microsoft.AspNetCore.Mvc;

namespace Taski.Api.Extensions;

public static class MvcBuilderExtensions
{
  public static IMvcBuilder ConfigureTaskiApiBehavior(this IMvcBuilder builder)
  {
    return builder.ConfigureApiBehaviorOptions(options =>
    {
      options.InvalidModelStateResponseFactory = context =>
          {
            var result = new JsonResult(context.ModelState);

            var errorDetails = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());

            result.Value = new
            {
              errors = errorDetails,
              success = false
            };

            result.StatusCode = StatusCodes.Status400BadRequest;

            return result;
          };
    });
  }
}

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddTaskiApiVersioning(this IServiceCollection services)
  {
    services.AddApiVersioning(options =>
    {
      options.ReportApiVersions = true;
    }).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

    return services;
  }
}