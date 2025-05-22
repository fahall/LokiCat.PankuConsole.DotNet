# ConsoleCommandBridgeGenerator

A Roslyn incremental source generator that bridges C# methods to the [PankuConsole](https://github.com/Ark2000/PankuConsole) in Godot via GDScript wrappers.

It automatically generates a `ConsoleBridge.gd` file with `@panku_command` functions, exposing decorated C# methods to the in-game developer console.

---

## ✨ Features

* ✔️ Auto-generates `@panku_command` wrappers for `[ConsoleCommand]` methods
* ✔️ Supports async methods (`Task`)
* ✔️ Supports optional/default parameters with arity overloads
* ✔️ Groups commands by class name using `ClassName.MethodName()`
* ✔️ Emits PascalCase if custom name is given
* ✔️ Self-contained — no need to define the attribute manually

---

## 🚀 Quick Start

1. **Install the generator** (via NuGet or project reference)

2. **Add the attribute to your method:**

```csharp
public partial class PlayerDebug : Node {
  [ConsoleCommand] // exposed as: PlayerDebug.Kill()
  public void Kill(string reason = "test") => GD.Print(reason);
}
```

3. **Or override the name completely:**

```csharp
[ConsoleCommand("kill_now")] // exposed as: kill_now()
public void Kill(string reason = "test") => GD.Print(reason);
```

4. **Use it in Godot via PankuConsole:**

```
> PlayerDebug.Kill("fall")
> kill_now("fall")
```

---

## 🧾 Command Name Mapping

| C# Declaration                                         | Exposed Console Command |
| ------------------------------------------------------ | ----------------------- |
| `[ConsoleCommand]` on `PlayerDebug.Kill()`             | `PlayerDebug.Kill()`    |
| `[ConsoleCommand("kill_now")]` on `PlayerDebug.Kill()` | `kill_now()`            |
| `[ConsoleCommand]` on `GameDebug.ResetGame()`          | `GameDebug.ResetGame()` |

---

## 📄 Generated Output

For `[ConsoleCommand]` method:

```csharp
public class GameDebug : Node {
  [ConsoleCommand]
  public void ResetGame(int delay = 5) => GD.Print("Reset in", delay);
}
```

Generates GDScript:

```gdscript
@panku_command
func game_debug.ResetGame(delay: int):
    GameDebug.ResetGame(delay)

@panku_command
func game_debug.ResetGame():
    GameDebug.ResetGame(5)
```

---

## 🧐 Notes

* Only methods on `Node`-derived types are supported
* Custom command name disables grouping and uses it verbatim
* All generated commands live in `ConsoleBridge.gd`

---

## 📦 Integration

Make sure `ConsoleBridge.gd` is added to your scene and ready at runtime. It will `@onready` bind to your C# nodes using the `%NodeName` syntax.

---

## 💪 License

MIT
