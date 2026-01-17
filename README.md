<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Google+Sans+Code:ital,wght@0,300..800;1,300..800&family=Space+Grotesk:wght@300..700&family=Space+Mono:ital,wght@0,400;0,700;1,400;1,700&display=swap" rel="stylesheet">

<div align="center">
    <h1 style="font-family: 'Space Grotesk', monospace">HasFlagExtension</h1>
    <p style="font-family: 'Space Grotesk', monospace"><b>by KryKom</b></p>
</div>

<div align="center">
    <p>
        <img src="https://img.shields.io/github/license/KryKomDev/HasFlagExtension?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%23F2A0A0" alt="GitHub License" />
        <a href="https://www.nuget.org/packages/HasFlagExtension.Generator"><img src="https://img.shields.io/nuget/v/HasFlagExtension.Generator?color=F0CA95&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet" /></a>
        <img src="https://img.shields.io/nuget/dt/HasFlagExtension.Generator?color=E3ED8A&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet Downloads" />
        <img src="https://img.shields.io/github/actions/workflow/status/KryKomDev/HasFlagExtension/test.yml?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%2395EC7D" alt="GitHub Actions Workflow Status" />
        <img src="https://img.shields.io/badge/.NET-Standard2.0-7ACFDC?style=for-the-badge&amp;labelColor=5F6473" alt=".NET Standard" />
    </p>
</div>

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
    FlagA = 1,
    FlagB = 2,
    FlagC = 4,
}
```

You can then use the extension methods like this:

```c#
MyFlags flags = MyFlags.FlagA | MyFlags.FlagB;

var a = flags.GetHasFlagA(); // true
var b = flags.GetHasFlagC(); // false
```

or when using dotnet 10.0 and higher:

```c#
MyFlags flags = MyFlags.FlagA | MyFlags.FlagB;

var a = flags.HasFlagA; // true
var b = flags.HasFlagC; // false
```

### Pattern matching

If you are using dotnet 10.0 or higher, you can also use the extension
properties for pattern matching:

```c#
MyFlags flags = MyFlags.FlagA | MyFlags.FlagB;

if (flags is { HasFlagA: true, HasFlagB: false })
{
    // ...
}
```

### HasFlagPrefixAttribute

The `HasFlagPrefixAttribute` can be used to specify a prefix for the generated
extension methods.

For instance, if we consider the enum from earlier and add the following line:

```c#
[HasFlagPrefix("Allow")]
```

then the generated extension methods will be named `AllowFlagA`, `AllowFlagB`...

### FlagDisplayNameAttribute

The `FlagDisplayNameAttribute` can be used to specify what will the name of the flag be
when generating the extension methods.

For instance, if we consider the following enum with a different naming convention:
```c#
[Flags]
public enum MyFlags 
{
    [FlagDisplayName("None")]
    NONE = 0,
    
    [FlagDisplayName("FlagA")]
    FLAG_A = 1,
    
    [FlagDisplayName("FlagB")]
    FLAG_B = 2,
    
    [FlagDisplayName("FlagC")]
    FLAG_C = 4,
}
```

then the generated extension methods will be named `HasFlagA`, `HasFlagB`...

### EnumNamingAttribute

The `EnumNamingAttribute` can be used to specify what will the name of the flags be
when generating the extension methods. It can be used in combination with the
`FlagDisplayNameAttribute` and can be applied to the whole assembly and / or to
specific enums.

For instance, if we consider the following enum with a different naming convention:
```c#
[Flags]
[EnumNaming(NamingCase.TRAIN, NamingCase.Pascal)]
public enum MyFlags 
{
    FLAG_A,
    FLAG_B,
    FLAG_C,
    FLAG_D
}
```

then the generated extension properties will be automatically named `HasFlagA`, `HasFlagB`...

### Other Attributes

You can also exclude different flags from the generated extension methods by using the
`ExcludeFlagAttribute` attribute or even whole enums by using the `ExcludeFlagEnumAttribute`.

## Enum Item Groups

You can also group flags and enum items into different groups using the
`FlagGroupAttribute`.

```c#
[FlagGroup("GroupA")]
[FlagGroup("GroupB")]
enum MyFlags
{
    [FlagGroup("GroupA")]
    FlagA,
    
    [FlagGroup("GroupA")]
    [FlagGroup("GroupB")]
    FlagB,
    
    [FlagGroup("GroupB")]
    FlagC,
}
```

`GetIsGroupA` and `GetIsGroupB` extension methods will be generated for the enum (as 
well as the extension properties for dotnet 10.0 and higher). This method will return 
`true` if the value of the enum contains any of the items from the group. Example usage
can be seen below:

```c#
var a = MyFlags.FlagA;
Console.WriteLine(a.GetIsGroupA()); // true
Console.WriteLine(a.GetIsGroupB()); // false
```

> [!IMPORTANT]
> You need to first declare the group on the enum declaration for it to be recognized.

You can also specify a custom prefix for the generated extension methods:

```c#
[FlagGroup("GroupA", "Allows")]
[FlagGroup("GroupB")]
enum MyFlags
{
    // ...
```

The code above will change the name of the `GetIsGroupA` extension 
method to `GetAllowsGroupA`.

## Installation

### DotNet CLI

```bash
dotnet add package HasFlagExtension.Generator
```

### NuGet CLI

```bash
Package-Install HasFlagExtension.Generator
```

### .csproj

```xml
<ItemGroup>
    <PackageReference Include="HasFlagExtension.Generator" Version="*"/>
</ItemGroup>
```