using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEM.CodeGen;

public static class DbContextGenerator
{
    public static string Generate(TypeDefinition model)
    {
        var unit = SyntaxFactory.CompilationUnit()
            .AddUsings(
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.EntityFrameworkCore")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("SEM.Domain.Entities")))
            .AddMembers(
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("SEM.Infrastructure.Repositories"))
                    .AddMembers(CreateDbContextClass(model)));

        return RoslynCodeWriter.ToSource(unit);
    }

    private static ClassDeclarationSyntax CreateDbContextClass(TypeDefinition model)
    {
        var dbSetProperty = SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.GenericName("DbSet")
                    .AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName(model.Name)),
                SyntaxFactory.Identifier($"{model.Name}s"))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithAccessorList(SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                })));

        var constructor = SyntaxFactory.ConstructorDeclaration("AppDbContext")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("options"))
                    .WithType(SyntaxFactory.GenericName("DbContextOptions")
                        .AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName("AppDbContext"))))
            .WithInitializer(SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("options"))))))
            .WithBody(SyntaxFactory.Block());

        var onModelCreating = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                "OnModelCreating")
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("modelBuilder"))
                    .WithType(SyntaxFactory.ParseTypeName("ModelBuilder")))
            .WithBody(SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.BaseExpression(),
                            SyntaxFactory.IdentifierName("OnModelCreating")))
                        .WithArgumentList(SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("modelBuilder"))))))));

        return SyntaxFactory.ClassDeclaration("AppDbContext")
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("DbContext")))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(dbSetProperty, constructor, onModelCreating);
    }
}
