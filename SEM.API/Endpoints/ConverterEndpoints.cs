using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using SEM.Abstractions;
using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;

namespace WebApplication1.Endpoints;

public static class ConverterEndpoints
{
    public static void MapConverterEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/convert").WithOpenApi().WithTags("Converter").RequireAuthorization();
        group.MapPost("/to-code", async ([FromServices] IJsonToModelParser modelParser, 
            [FromServices] IModelToCodeParser codeParser,
            [FromBody] JsonNode json, [FromQuery] string name, HttpContext httpContext) =>
        
            {   
                var parsed = modelParser.ParseJsonToModel(name, json);
                return codeParser.ParseModelToCode(parsed);
            });
        
        group.MapPut("/create", async ([FromServices] IModelService modelService, 
            [FromServices] IJsonToModelParser parserToModel, 
            [FromBody] JsonNode json, [FromQuery] string name, HttpContext httpContext) =>
        
        {
            var userId = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var model = parserToModel.ParseJsonToModel(name, json);
            var guid = Guid.Parse(userId);
            await modelService.CreateModelAsync(model, guid);
            return Results.Created();
        });

        group.MapPost("/save", async ([FromServices] IModelService modelService, [FromBody] ModelDto savingModel,
            [FromServices] IJsonToModelParser parserToModel, [FromQuery] Guid id) =>
        {
            await modelService.UpdateModelAsync(savingModel);
            return Results.Ok();
        });

    }
}