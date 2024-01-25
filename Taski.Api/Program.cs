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

builder.Services.AddTaskiApiVersioning();

builder.Services.AddScoped(typeof(IRepository<Project>), typeof(Repository<Project>));
builder.Services.AddScoped(typeof(IRepository<Story>), typeof(Repository<Story>));
builder.Services.AddScoped(typeof(IRepository<ProjectTag>), typeof(Repository<ProjectTag>));
builder.Services.AddScoped(typeof(IRepository<ProjectTagAssociation>), typeof(Repository<ProjectTagAssociation>));
builder.Services.AddScoped(typeof(IRepository<UserProjectAssociation>), typeof(Repository<UserProjectAssociation>));
builder.Services.AddScoped(typeof(IRepository<StoryTag>), typeof(Repository<StoryTag>));
builder.Services.AddScoped(typeof(IRepository<User>), typeof(Repository<User>));
builder.Services.AddScoped(typeof(IRepository<Comment>), typeof(Repository<Comment>));

var connectionString = builder.Configuration.GetConnectionString("TaskiContext");

builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<TaskiAppContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSqlServer<TaskiAppContext>(connectionString);


AuthenticationBuilder.AddAuthentication(builder);

builder.Services.AddControllers().ConfigureTaskiApiBehavior();

builder.Services.AddPolicies();

builder.Services.AddSwaggerGen().AddTransient<IConfigureOptions<SwaggerGenOptions>, Swagger>().AddEndpointsApiExplorer();

var app = builder.Build();

app.Services.InitializeDatabase();

app.MapProjectsEndpoint();
app.MapStoriesEndpoint();
app.MapUsersEndpoint();
app.MapCommentsEndpoint();

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