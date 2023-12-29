using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Taski.Api.OpenApi;

public class Swagger : IConfigureOptions<SwaggerGenOptions>
{

  private readonly IApiVersionDescriptionProvider provider;

  public Swagger(IApiVersionDescriptionProvider provider)
  {
    this.provider = provider;
  }
  public void Configure(SwaggerGenOptions options)
  {
    var descriptions = provider.ApiVersionDescriptions;
    foreach (var description in descriptions)
    {
      options.SwaggerDoc(description.GroupName, new OpenApiInfo
      {
        Title = $"My API {description.ApiVersion}",
        Version = description.ApiVersion.ToString()
      });
    }
  }
}
