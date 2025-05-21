using Microsoft.CodeAnalysis;

namespace LokiCat.PankuConsole.DotNet.CommandGenerator.Features.SyntaxHelpers;

public static class Types
{
    public static string GetFullTypeName(this ITypeSymbol? symbol) =>
        symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "object";

}