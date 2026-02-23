using System.Text;
using SEM.Domain.Abstractions;
using SEM.Domain.Entities;

namespace SEM.Domain.Services;

public class ApiGenerator : IApiGenerator
{
    public string GenerateCrudEndpoints(Model model)
    {
        var code = $$"""
            using System.Security.Claims;
            using Microsoft.AspNetCore.Http;

            public static class {{model.Name}}Endpoints
            {
                public static void Map{{model.Name}}Endpoints(this WebApplication app)
                {
                    var group = app.MapGroup("/api/{{model.Name}}")
                                   .RequireAuthorization();

                    group.MapPost("/", Create);
                    group.MapGet("/{id:guid}", GetById);
                    group.MapGet("/", GetAll);
                    group.MapDelete("/{id:guid}", Delete);
                }

                static async Task<IResult> Create(
                    {{model.Name}}Dto dto,
                    I{{model.Name}}Service service,
                    HttpContext ctx)
                {
                    var userId = Guid.Parse(
                        ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    await service.CreateFromDtoAsync(dto, userId);
                    return Results.Created();
                }

                static async Task<IResult> GetById(
                    Guid id,
                    I{{model.Name}}Service service)
                {
                    var model = await service.GetModelAsync(id);
                    return model is null
                        ? Results.NotFound()
                        : Results.Ok(model);
                }

                static async Task<IResult> GetAll(
                    I{{model.Name}}Service service,
                    HttpContext ctx)
                {
                    var userId = Guid.Parse(
                        ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    var models = await service.GetUserModelsAsync(userId);
                    return Results.Ok(models);
                }

                static async Task<IResult> Delete(
                    Guid id,
                    I{{model.Name}}Service service)
                {
                    await service.DeleteModelAsync(id);
                    return Results.NoContent();
                }
            }
            """;

        return code;
    }

    public string GenerateDbContext(Model? model)
    {
        return $$"""
                 using Microsoft.EntityFrameworkCore;
                 using SEM.Domain.Entities;
                 
                 namespace SEM.Infrastructure.Repositories;
                 
                 public class AppDbContext : DbContext
                 {
                     public DbSet<{{model.Name}}> {{model.Name}}s { get; set; }
                     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
                     {
                         
                     }
                 
                     protected override void OnModelCreating(ModelBuilder modelBuilder)
                     {
                         base.OnModelCreating(modelBuilder);
                         
                     }
                 }
                 """;
    }
}