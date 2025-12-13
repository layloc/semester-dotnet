using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SEM.Abstractions;
using SEM.Domain.Abstractions;
using SEM.Domain.Services;
using SEM.Infrastructure.Repositories;
using SEM.Services;
using WebApplication1.Endpoints;
using WebApplication1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddScoped<SessionAuthMiddleware>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("D:\\keys"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
        .UseSnakeCaseNamingConvention());
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "sem:";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("session")
    .AddCookie("session", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.Name = "sem";
    });
builder.Services.AddAuthorization();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IJsonToModelParser, JsonToModelParser>();
builder.Services.AddTransient<IModelToCodeParser, ModelToCodeParser>();
builder.Services.AddTransient<IModelService, ModelService>();
builder.Services.AddTransient<IModelRepository, ModelRepository>();



var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapConverterEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseReDoc(options => options.SpecUrl("/openapi/v1.json"));
    app.UseSwaggerUI(options =>
    {
        options.SwaggerDocumentUrlsPath = "/openapi/v1.json";
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
app.Run();

