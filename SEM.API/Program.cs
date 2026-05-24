using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SEM.Abstractions;
using SEM.API.Config;
using SEM.Domain.Abstractions;
using SEM.Domain.Services;
using SEM.Infrastructure.Repositories;
using SEM.Services;
using SEM.API.Endpoints;
using SEM.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddScoped<SessionAuthMiddleware>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var dataProtectionPath = builder.Configuration["DataProtection:KeysPath"] ?? "keys";
try
{
    Directory.CreateDirectory(dataProtectionPath);
}
catch (UnauthorizedAccessException) when (builder.Environment.IsEnvironment("Docker"))
{
  dataProtectionPath = Path.Combine(Path.GetTempPath(), "sem-dataprotection-keys");
  Directory.CreateDirectory(dataProtectionPath);
}

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
        .UseSnakeCaseNamingConvention());
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "sem:";
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
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });
builder.Services.AddAuthorization();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IJsonToModelParser, JsonToModelParser>();
builder.Services.AddTransient<IModelToCodeParser, ModelToCodeParser>();
builder.Services.AddTransient<IModelService, ModelService>();
builder.Services.AddTransient<IModelRepository, ModelRepository>();
builder.Services.AddTransient<IApiGenerator, ApiGenerator>();
builder.Services.ConfigureValidators();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Configuration.GetValue("ApplyMigrations", app.Environment.IsEnvironment("Docker")))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsEnvironment("Docker"))
    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/login"));
app.MapAuthEndpoints();
app.MapConverterEndpoints();
app.MapFallbackToFile("/login", "login.html");
app.MapFallbackToFile("/converter", "converter.html").RequireAuthorization();
app.MapFallbackToFile("/my-models", "my-models.html").RequireAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
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

