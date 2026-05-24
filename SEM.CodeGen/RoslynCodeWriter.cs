using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEM.CodeGen;

internal static class RoslynCodeWriter
{
    public static string ToSource(CompilationUnitSyntax unit) =>
        unit.NormalizeWhitespace().ToFullString();
}
