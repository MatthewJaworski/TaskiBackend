using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Taski.Api.Data;
using Taski.Api.Endpoints;
using Taski.Api.Entities;
using Taski.Api.Extensions;
using Taski.Api.OpenApi;
using Taski.Api.Repositiories;
using Taski.Api.Cors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.TaskiCors(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
}).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");



builder.Services.AddScoped(typeof(IRepository<Project>), typeof(Repository<Project>));
builder.Services.AddScoped(typeof(IRepository<Story>), typeof(Repository<Story>));


var connectionString = builder.Configuration.GetConnectionString("TaskiContext");

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<TaskiAppContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSqlServer<TaskiAppContext>(connectionString);


AuthenticationBuilder.AddAuthentication(builder, new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidIssuer = "https://localhost:5001",
    ValidAudience = "https://localhost:5001",
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("P5BsNuJR8hgfAx7ap9ZkW3jmGnC6rMDe")),
    ClockSkew = TimeSpan.Zero
});
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        {
            var result = new JsonResult(context.ModelState);

            // Remove unwanted properties
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


builder.Services.AddSwaggerGen().AddTransient<IConfigureOptions<SwaggerGenOptions>, Swagger>().AddEndpointsApiExplorer();

var app = builder.Build();

app.Services.InitializeDatabase();

app.MapProjectsEndpoint();
app.MapStoriesEndpoint();
app.MapUsersEndpoint();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var descirption in app.DescribeApiVersions())
        {
            var url = $"/swagger/{descirption.GroupName}/swagger.json";
            var name = descirption.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}


app.Run();