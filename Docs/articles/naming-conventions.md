# Naming Conventions

The generator translates your enum members into clean C# method and property identifiers. To do this, it supports standard naming conversions configured via the `NamingCase` enum.

---

## The NamingCase Enum

The following cases are supported:

| Casing Value | Code Example | Output Example |
|---|---|---|
| `NamingCase.CAMEL` | `helloWorld` | `helloWorld` |
| `NamingCase.PASCAL` | `HelloWorld` | `HelloWorld` |
| `NamingCase.SNAKE` | `hello_world` | `hello_world` |
| `NamingCase.SCREAMING_SNAKE` | `HELLO_WORLD` | `HELLO_WORLD` |
| `NamingCase.KEBAB` | `hello-world` | Invalid (causes HFE001 or HFE002 error) |
| `NamingCase.SPACED_CAMEL` | `hello world` | Invalid (causes HFE001 or HFE002 error) |
| `NamingCase.TRAIN` | `HELLO-WORLD` | Invalid (causes HFE001 or HFE002 error) |

> [!WARNING]
> Kebab case, spaced camel case, and train case are not valid identifier types in C# and will trigger diagnostic errors if configured as a target naming case.

---

## Scope of Configuration

### Assembly-Level
You can set a default naming convention for all enums in your assembly by adding the `[assembly: EnumNaming(...)]` attribute to any file (e.g. `AssemblyInfo.cs`):

```csharp
using HasFlagExtension;

[assembly: EnumNaming(NamingCase.SCREAMING_SNAKE, NamingCase.PASCAL)]
```
This configurations tells the generator that all enums in this assembly are written in SCREAMING_SNAKE casing and should have their extension methods/properties generated in PASCAL casing.

### Enum-Level Override
You can override the assembly-wide convention for individual enums by applying the `[EnumNaming]` attribute directly to the enum:

```csharp
[Flags]
[EnumNaming(NamingCase.CAMEL, NamingCase.PASCAL)]
public enum camelCasedFlags
{
    firstFlag = 1,
    secondFlag = 2,
}
```

---

## Prefix Formatting

Generated members combine a Prefix and the Name of the flag.

1. **Default Prefix**:
   * For methods (<= .NET 9.0): `GetHas` + `Name` (e.g. `GetHasFlagA`)
   * For properties (>= .NET 10.0): `Has` + `Name` (e.g. `HasFlagA`)
2. **Custom Prefix**:
   * Prepend the custom prefix to the converted name.
   * If `[HasFlagPrefix("Allow")]` is used on `Read` (Pascal case target):
     * Method generates as `AllowRead()`
     * Property generates as `AllowRead`
3. **Name Override**:
   * `[FlagDisplayName("SuperAdmin")]` on a flag member overrides naming conversions entirely. The result is `GetHasSuperAdmin()` / `HasSuperAdmin`.
