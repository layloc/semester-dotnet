using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEM.CodeGen;

public static class CrudEndpointsGenerator
{
    public static string Generate(TypeDefinition model)
    {
        var unit = SyntaxFactory.CompilationUnit()
            .AddUsings(
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Security.Claims")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.AspNetCore.Http")))
            .AddMembers(CreateEndpointsClass(model));

        return RoslynCodeWriter.ToSource(unit);
    }

    private static ClassDeclarationSyntax CreateEndpointsClass(TypeDefinition model)
    {
        var mapMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                $"Map{model.Name}Endpoints")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("app"))
                    .WithType(SyntaxFactory.ParseTypeName("WebApplication")))
            .WithBody(SyntaxFactory.Block(
                ParseStatements($$"""
                    var group = app.MapGroup("/api/{{model.Name}}")
                                   .RequireAuthorization();

                    group.MapPost("/", Create);
                    group.MapGet("/{id:guid}", GetById);
                    group.MapGet("/", GetAll);
                    group.MapDelete("/{id:guid}", Delete);
                    """)));

        var createMethod = CreateHandlerMethod(
            "Create",
            $$"""
            {{model.Name}}Dto dto,
            I{{model.Name}}Service service,
            HttpContext ctx
            """,
            $$"""
            var userId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await service.CreateFromDtoAsync(dto, userId);
            return Results.Created();
            """);

        var getByIdMethod = CreateHandlerMethod(
            "GetById",
            $$"""
            Guid id,
            I{{model.Name}}Service service
            """,
            $$"""
            var entity = await service.GetModelAsync(id);
            return entity is null ? Results.NotFound() : Results.Ok(entity);
            """);

        var getAllMethod = CreateHandlerMethod(
            "GetAll",
            $$"""
            I{{model.Name}}Service service,
            HttpContext ctx
            """,
            $$"""
            var userId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var models = await service.GetUserModelsAsync(userId);
            return Results.Ok(models);
            """);

        var deleteMethod = CreateHandlerMethod(
            "Delete",
            $$"""
            Guid id,
            I{{model.Name}}Service service
            """,
            $$"""
            await service.DeleteModelAsync(id);
            return Results.NoContent();
            """);

        return SyntaxFactory.ClassDeclaration($"{model.Name}Endpoints")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddMembers(mapMethod, createMethod, getByIdMethod, getAllMethod, deleteMethod);
    }

    private static MethodDeclarationSyntax CreateHandlerMethod(
        string name,
        string parameters,
        string bodyStatements)
    {
        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName("Task")
                    .AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName("IResult")),
                name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(ParseParameters(parameters))
            .WithBody(SyntaxFactory.Block(ParseStatements(bodyStatements)));
    }

    private static ParameterSyntax[] ParseParameters(string parameters) =>
        string.IsNullOrWhiteSpace(parameters)
            ? Array.Empty<ParameterSyntax>()
            : SyntaxFactory.ParseParameterList($"({parameters})").Parameters.ToArray();

    private static StatementSyntax[] ParseStatements(string statements)
    {
        var method = (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration($"void M() {{ {statements} }}")!;
        return method.Body!.Statements.ToArray();
    }
}
