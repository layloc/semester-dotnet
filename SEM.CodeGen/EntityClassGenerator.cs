using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEM.CodeGen;

public static class EntityClassGenerator
{
    public static string Generate(TypeDefinition model)
    {
        var classDeclaration = SyntaxFactory.ClassDeclaration(model.Name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(model.Properties.Select(CreateProperty).ToArray());

        var unit = SyntaxFactory.CompilationUnit()
            .AddMembers(classDeclaration);

        return RoslynCodeWriter.ToSource(unit);
    }

    private static PropertyDeclarationSyntax CreateProperty(PropertyDefinition property)
    {
        var modifiers = new List<SyntaxToken> { SyntaxFactory.Token(SyntaxKind.PublicKeyword) };
        if (property.IsRequired)
            modifiers.Add(SyntaxFactory.Token(SyntaxKind.RequiredKeyword));

        var accessors = property.IsReadonly
            ? new[] { CreateAccessor(SyntaxKind.GetAccessorDeclaration) }
            : new[]
            {
                CreateAccessor(SyntaxKind.GetAccessorDeclaration),
                CreateAccessor(SyntaxKind.SetAccessorDeclaration)
            };

        return SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.ParseTypeName(property.Type),
                property.Name)
            .WithModifiers(SyntaxFactory.TokenList(modifiers))
            .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));
    }

    private static AccessorDeclarationSyntax CreateAccessor(SyntaxKind kind) =>
        SyntaxFactory.AccessorDeclaration(kind)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
}
