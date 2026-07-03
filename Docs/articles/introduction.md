# Introduction

HasFlagExtension is a lightweight, high-performance Roslyn source generator and analyzer designed to simplify working with C# flag enums.

---

## Why HasFlagExtension?

In standard C#, checking for flag combinations is historically done using the `Enum.HasFlag` method or bitwise operations:

```csharp
// Bitwise check (harder to read)
if ((userPerms & Permissions.Read) == Permissions.Read) { ... }

// Enum.HasFlag (safe but historically carried boxing overhead, and verbose)
if (userPerms.HasFlag(Permissions.Read)) { ... }
```

HasFlagExtension addresses this by automatically generating type-safe, highly optimized extension methods and properties directly in your assembly:

```csharp
// Using HasFlagExtension
if (userPerms.HasRead) { ... }
```

---

## Key Features

*   **Automatic Generation**: Automatically scans your codebase for enums decorated with the `[Flags]` attribute and generates appropriate extension helpers.
*   **Version-Specific Optimizations**: 
    *   Generates **extension methods** (e.g. `GetHasFlagA()`) for target frameworks older than .NET 10.0.
    *   Generates **extension properties** (e.g. `HasFlagA`) for .NET 10.0 and C# 14 projects.
*   **Property Pattern Matching**: Leverage modern C# features to check flag values within pattern matching blocks (e.g., `userPerms is { HasRead: true, HasWrite: false }`).
*   **Logical Flag Grouping**: Define groups of flags on the enum (e.g., `ReadWrite` or `Admin`) to generate composite checks that verify if any or all flags in the group are set.
*   **Flexible Naming**: Configure default naming case conversions for enum members at the assembly or individual enum level, or override display names entirely.
*   **Active Analyzer Rules**: Includes an integrated Roslyn analyzer that catches invalid prefix settings, naming conflicts, or malformed grouping configurations at design time.

---

## Architecture and Performance

Since HasFlagExtension is a source generator, all helpers are compiled directly into your target project. 

*   **Zero Runtime Overhead**: There are no additional DLL dependencies, reflection calls, or runtime overhead.
*   **Aggressive Inlining**: All generated methods and properties are annotated with `[MethodImpl(MethodImplOptions.AggressiveInlining)]` and `[Pure]`, allowing the compiler to optimize the calls into simple bitwise checks.