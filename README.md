# HasFlagExtension

HasFlagExtension is a simple source generator that creates HasFlag extension methods for flag enums.

## Basic Usage

For dotnet versions lower than 10.0, the generator only generates the extension methods.
For dotnet versions 10.0 and higher, it also generates extension properties that can be
used for pattern matching.

Consider the following enum:

```c#
[Flags]
public enum MyFlags
{
    None = 0,
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
}
```

You can then use the extension methods like this:

```c#
MyFlags flags = MyFlags.Flag1 | MyFlags.Flag2;

var a = flags.GetHasFlag1(); // true
var b = flags.GetHasFlag3(); // false
```

or when using dotnet 10.0 and higher:

```c#
MyFlags flags = MyFlags.Flag1 | MyFlags.Flag2;

var a = flags.HasFlag1; // true
var b = flags.HasFlag3; // false
```

### Pattern matching

If you are using dotnet 10.0 or higher, you can also use the extension
properties for pattern matching:

```c#
MyFlags flags = MyFlags.Flag1 | MyFlags.Flag2;

if (flags is { HasFlag1: true, HasFlag2: false })
{
    // ...
}
```

## HasFlagPrefixAttribute

The `HasFlagPrefixAttribute` can be used to specify a prefix for the generated 
extension methods.

For instance, if we consider the enum from earlier and add the following line:

```c#
[HasFlagPrefix("Allow")]
```

then the generated extension methods will be named `AllowHasFlag1`, `AllowHasFlag2`...

## Installation

### DotNet CLI

```bash
dotnet add package HasFlagExtension
```

### NuGet CLI

```bash
Package-Install HasFlagExtension
```

### .csproj

```xml
<ItemGroup>
    <PackageReference Include="HasFlagExtension" Version="1.0.0"/>
</ItemGroup>
```