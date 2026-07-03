# Getting Started

`HasFlagExtension` is a Roslyn source generator and analyzer that generates type-safe, highly efficient extension methods and properties for C# flag enums.

---

## Installation

# [.NET CLI](#tab/dotnet-cli)
```bash
dotnet add package HasFlagExtension.Generator
```

# [Package Manager](#tab/package-manager)
```powershell
Install-Package HasFlagExtension.Generator
```

# [.csproj](#tab/csproj)
```xml
<ItemGroup>
    <PackageReference Include="HasFlagExtension.Generator" Version="*"/>
</ItemGroup>
```
---

---

## Basic Usage

The generator automatically discovers all enums decorated with the `[Flags]` attribute and generates helpers for them.

### .NET Versions < 10.0 (Methods Only)
For target frameworks older than .NET 10.0, the generator creates extension methods prefixed with `GetHas`.

Given the following enum:
```csharp
using System;

[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
}
```

You can check flag presence using the generated extension methods:
```csharp
Permissions userPerms = Permissions.Read | Permissions.Write;

bool canRead = userPerms.GetHasRead();       // Returns true
bool canExecute = userPerms.GetHasExecute(); // Returns false
```

### .NET 10.0 and Higher (Properties & Pattern Matching)

When targeting .NET 10.0 or higher, the generator leverages extension properties (introduced in C# 14 / .NET 10) to generate properties prefixed with `Has`.

```csharp
Permissions userPerms = Permissions.Read | Permissions.Write;

bool canRead = userPerms.HasRead;       // Returns true
bool canExecute = userPerms.HasExecute; // Returns false
```

#### Property Pattern Matching

With extension properties, you can write clean property pattern matching statements:
```csharp
Permissions userPerms = Permissions.Read | Permissions.Write;

if (userPerms is { HasRead: true, HasWrite: true, HasExecute: false })
{
    Console.WriteLine("User has read/write but not execute permissions.");
}
```