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

> **Important:**
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