using System;

namespace LokiCat.PankuConsole.DotNet.Attributes;
/// <summary>
/// Marks a method to be exposed in PankuConsole.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ConsoleCommandAttribute(string commandName = "") : Attribute
{
    public string CommandName { get; } = commandName;
}