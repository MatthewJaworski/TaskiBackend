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
        Title = $"Taski api {description.ApiVersion}",
        Version = description.ApiVersion.ToString()
      });
    }
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
      Name = "JWT Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
  }
}
