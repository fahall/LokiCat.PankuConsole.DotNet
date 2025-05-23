using System.Reflection;
using System.Text;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet ConsoleCommandEmitter.dll <outputDir> <assemblyPath>");

    return;
}

var outputDir = Path.GetFullPath(args[0]);
var assemblyPath = args[1];
Directory.CreateDirectory(outputDir);

var assembly = Assembly.LoadFrom(assemblyPath);

string ToGDScriptType(Type type) => type.Name switch
{
    "String" => "String",
    "Int32" => "int",
    "Single" => "float",
    "Boolean" => "bool",
    _ => "Variant"
};

var commands = assembly.GetTypes()
                       .Where(t => t.IsClass)
                       .SelectMany(t =>
                                       t.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                    BindingFlags.DeclaredOnly)
                                        .Where(m => m.GetCustomAttributes()
                                                     .Any(a => a.GetType().Name == "ConsoleCommandAttribute"))
                                        .Select(m => new
                                        {
                                            ClassName = t.Name,
                                            MethodName = m.Name,
                                            IsAsync = m.ReturnType.Name == "Task",
                                            Parameters = m.GetParameters(),
                                            CustomName = m.GetCustomAttributesData()
                                                          .FirstOrDefault(
                                                              a => a.AttributeType.Name == "ConsoleCommandAttribute")
                                                          ?.ConstructorArguments.FirstOrDefault()
                                                          .Value as string
                                        })
                       )
                       .ToList();

var sb = new StringBuilder();
sb.AppendLine("extends Node");
sb.AppendLine("class_name ConsoleBridge\n");

foreach (var cls in commands.Select(c => c.ClassName).Distinct())
{
    sb.AppendLine($"@onready var {cls} = get_node(\"%{cls}\")");
}

foreach (var cmd in commands)
{
    var baseName = string.IsNullOrWhiteSpace(cmd.CustomName)
        ? $"{cmd.ClassName}.{cmd.MethodName}"
        : cmd.CustomName;

    var allParams = cmd.Parameters;
    var requiredCount = allParams.TakeWhile(p => !p.HasDefaultValue).Count();

    for (var i = allParams.Length; i >= requiredCount; i--)
    {
        var slice = allParams.Take(i).ToArray();
        var paramDecl = string.Join(", ", slice.Select(p => $"{p.Name}: {ToGDScriptType(p.ParameterType)}"));
        var argList = string.Join(", ", slice.Select(p => p.Name));

        sb.AppendLine();
        sb.AppendLine("@panku_command");
        sb.AppendLine($"func {baseName}({paramDecl}):");
        sb.Append("    ");

        if (cmd.IsAsync)
        {
            sb.Append("await ");
        }

        sb.AppendLine($"{cmd.ClassName}.{cmd.MethodName}({argList})");
    }
}

var outputPath = Path.Combine(outputDir, "ConsoleBridge.gd");
File.WriteAllText(outputPath, sb.ToString());
Console.WriteLine($"[ConsoleCommandEmitter] ✅ Wrote {outputPath}");