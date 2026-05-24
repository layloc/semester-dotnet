using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SEM.CodeGen;

namespace SEM.Generators;

[Generator]
public sealed class SemSchemaSourceGenerator : IIncrementalGenerator
{
    private const string SemSchemaSuffix = ".sem.json";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var schemaFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(SemSchemaSuffix, StringComparison.OrdinalIgnoreCase))
            .Select(static (file, cancellationToken) =>
            {
                var text = file.GetText(cancellationToken)?.ToString();
                return (Path: file.Path, Text: text);
            })
            .Where(static item => !string.IsNullOrWhiteSpace(item.Text));

        context.RegisterSourceOutput(schemaFiles, static (sourceProductionContext, item) =>
        {
            TypeDefinition typeDefinition;
            try
            {
                typeDefinition = TypeDefinitionMapper.FromJson(item.Text!);
            }
            catch (Exception exception)
            {
                sourceProductionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "SEM001",
                        "Invalid SEM schema",
                        "Failed to parse '{0}': {1}",
                        "SEM.Generators",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None,
                    item.Path,
                    exception.Message));
                return;
            }

            var hintName = SanitizeHintName(typeDefinition.Name);
            sourceProductionContext.AddSource(
                $"{hintName}.g.cs",
                SourceText.From(EntityClassGenerator.Generate(typeDefinition), Encoding.UTF8));
        });
    }

    private static string SanitizeHintName(string name) =>
        string.Concat(name.Select(static c => char.IsLetterOrDigit(c) ? c : '_'));
}
