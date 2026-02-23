using System.Net;
using System.Security.Claims;
using System.Text.Json.Nodes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication.TestDecoding;
using SEM.Abstractions;
using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;
using SEM.Domain.Entities;

namespace SEM.API.Endpoints;

public static class ConverterEndpoints
{
    public static void MapConverterEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/convert").WithOpenApi().WithTags("Converter").RequireAuthorization();
        group.MapPost("/to-code", async ([FromServices] IJsonToModelParser modelParser, 
            [FromServices] IModelToCodeParser codeParser,
            [FromBody] JsonNode json, 
            [FromQuery] string name,
            HttpContext context) => 
            {
                if (string.IsNullOrEmpty(name)) return Results.BadRequest();
                var parsed = await Task.Run(() => modelParser.ParseJsonToModel(name, json));
                context.Response.ContentType = "text/plain";
                return Results.Ok(await Task.Run(() => codeParser.ParseModelToCode(parsed)));
            });
        
        group.MapPut("/create", async ([FromServices] IModelService modelService, 
            [FromServices] IJsonToModelParser parserToModel, 
            [FromBody] JsonNode json, 
            [FromQuery] string name, 
            HttpContext httpContext) =>
        
        {
            var userId = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var model = parserToModel.ParseJsonToModel(name, json);
            var guid = Guid.Parse(userId);
            await modelService.CreateModelAsync(model, guid);
            return Results.Created(model.Id.ToString(), model);
        });

        group.MapPut("/save", async ([FromServices] IModelService modelService,
            [FromServices] IJsonToModelParser modelParser,
            [FromBody] JsonNode json,
            [FromQuery] Guid id,
            [FromQuery] string name,
            HttpContext context) =>
        {
            var uid = Guid.Parse(context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var newModel = modelParser.ParseJsonToModel(name, json);
            newModel.Id = id;
            try
            {
                await modelService.UpdateModelAsync(newModel, uid);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
            return Results.Ok("Successfully updated model");
        }); 
        group.MapPost("/from-url", async (
            [FromServices] IHttpClientFactory httpClientFactory,
            [FromServices] IJsonToModelParser modelParser,
            [FromServices] IModelToCodeParser codeParser,
            [FromBody] ConvertJsonFromUrl request,
            HttpContext context) =>
        {
            var client = httpClientFactory.CreateClient();
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(request.Url);
            }
            catch (Exception)
            {
                return Results.BadRequest("Failed to reach target API");
            }

            if (!response.IsSuccessStatusCode)
                return Results.BadRequest($"Remote API returned {response.StatusCode}");

            var jsonString = await response.Content.ReadAsStringAsync();

            JsonNode? json;
            try
            {
                json = JsonNode.Parse(jsonString);
            }
            catch (Exception)
            {
                return Results.BadRequest("Response is not valid JSON");
            }

            if (json is null)
                return Results.BadRequest("Empty JSON");

            var model = modelParser.ParseJsonToModel(request.Name, json);
            var code = codeParser.ParseModelToCode(model);

            context.Response.ContentType = "text/plain";
            return Results.Ok(code);
        });
        group.MapPost("/generate-api", (
            [FromServices] IApiGenerator apiGenerator,
            [FromServices] IJsonToModelParser parser,
            [FromQuery] string name,
            [FromQuery] string typeOfGen,
            [FromBody] JsonNode json,
            HttpContext context) =>
        {
            var model = parser.ParseJsonToModel(name, json);
            var code = typeOfGen == "api" ? apiGenerator.GenerateCrudEndpoints(model) : apiGenerator.GenerateDbContext(model);
            context.Response.ContentType = "text/plain";
            return Results.Ok(code);
        });
        group.MapGet("/models", async ([FromServices] IModelRepository modelRepository, HttpContext httpContext) =>
        {
            var userId = Guid.Parse(httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)
                .Value);
            var modelList = await modelRepository.GetModelsOfUserAsync(userId);
            var result = modelList
                .Where(m => m != null)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    properties = m.Properties.Select(p => new
                    {
                        name = p.Name,
                        type = p.Type,
                        isReadonly = p.IsReadonly,
                        isRequired = p.IsRequired
                    }).ToList()
                });
            return Results.Ok(result);
        });
        
        group.MapGet("/models/{id:guid}", async ([FromServices] IModelService modelService, Guid id) =>
        {
            var model = await modelService.GetModelAsync(id);
            var result = new
            {
                id = model.Id,
                name = model.Name,
                properties = model.Properties.Select(p => new
                {
                    name = p.Name,
                    type = p.Type,
                    isReadonly = p.IsReadonly,
                    isRequired = p.IsRequired
                }).ToList()
            };

            return Results.Ok(result);        });
        group.MapPost("/to-code-from-structure", async ([FromServices] IModelToCodeParser codeParser,
            [FromBody] ModelDto dto,
            HttpContext context) =>
        {
            var model = new Model
            {
                Name = dto.Name,
                Properties = dto.Properties.Select(p => new PropertyInfo
                {
                    Name = p.Name,
                    Type = p.Type,
                    IsReadonly = p.IsReadonly,
                    IsRequired = p.IsRequired
                }).ToList()
            };
            context.Response.ContentType = "text/plain";
            return Results.Ok(await Task.Run(() => codeParser.ParseModelToCode(model)));
        });
        group.MapPut("/create-from-structure", async ([FromServices] IModelService modelService,
            [FromBody] ModelDto dto,
            HttpContext httpContext) =>
        {
            if (string.IsNullOrEmpty(dto.Name) || dto.Properties == null || dto.Properties.Count == 0)
                return Results.BadRequest("Name and Properties are required");
            
            var userId = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var guid = Guid.Parse(userId);
            var model = new Model
            {
                Name = dto.Name,
                Properties = dto.Properties.Select(p => new PropertyInfo
                {
                    Name = p.Name,
                    Type = p.Type,
                    IsReadonly = p.IsReadonly,
                    IsRequired = p.IsRequired
                }).ToList()
            };
            await modelService.CreateModelAsync(model, guid);
            return Results.Created(model.Id.ToString(), model);
        });
        group.MapPut("/save-from-structure", async ([FromServices] IModelService modelService,
            [FromBody] ModelDto dto,
            HttpContext context) =>
        {
            var uid = Guid.Parse(context.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var newModel = new Model
            {
                Id = dto.Id,
                Name = dto.Name,
                Properties = dto.Properties.Select(p => new PropertyInfo
                {
                    Name = p.Name,
                    Type = p.Type,
                    IsReadonly = p.IsReadonly,
                    IsRequired = p.IsRequired
                }).ToList()
            };
            try
            {
                await modelService.UpdateModelAsync(newModel, uid);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }

            return Results.Ok("Successfully updated model");
        });
        group.MapPost("/generate-api-from-structure", (
            [FromServices] IApiGenerator apiGenerator,
            [FromQuery] string typeOfGen,
            [FromBody] ModelDto dto,
            HttpContext context) =>
        {
            var model = new Model
            {
                Name = dto.Name,
                Properties = dto.Properties.Select(p => new PropertyInfo
                {
                    Name = p.Name,
                    Type = p.Type,
                    IsReadonly = p.IsReadonly,
                    IsRequired = p.IsRequired
                }).ToList()
            };
            var code = typeOfGen == "api" ? apiGenerator.GenerateCrudEndpoints(model) : apiGenerator.GenerateDbContext(model);
            context.Response.ContentType = "text/plain";
            return Results.Ok(code);
        });
        
    }
}