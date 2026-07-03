# Flag Grouping

`HasFlagExtension` allows you to group multiple enum members (flags) into logical sets. The generator then produces a consolidated helper method or property that checks if any of the flags in the group are set.

---

## Defining and Populating Groups

To group flags:
1. Declare the group name on the enum definition using `[FlagGroup("GroupName")]`.
2. Decorate each participating enum member with `[FlagGroup("GroupName")]`.

### Basic Example
```csharp
using System;
using HasFlagExtension;

[Flags]
[FlagGroup("ReadWrite")]
[FlagGroup("Management")]
public enum FilePermissions
{
    [FlagGroup("ReadWrite")]
    Read = 1,

    [FlagGroup("ReadWrite")]
    Write = 2,

    [FlagGroup("Management")]
    Delete = 4,

    [FlagGroup("Management")]
    ModifyAccess = 8,
}
```

This configuration generates the following checks:
* **Methods** (for < .NET 10): `GetIsReadWrite()` and `GetIsManagement()`.
* **Properties** (for >= .NET 10): `IsReadWrite` and `IsManagement`.

Under the hood, `GetIsReadWrite()` compiles to:
```csharp
[Pure]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool GetIsReadWrite(this FilePermissions val)
{
    return (val & (FilePermissions.Read | FilePermissions.Write)) != 0;
}
```

---

## Custom Group Prefixes

By default, group checks are prefixed with `GetIs` (methods) or `Is` (properties). You can customize this prefix on the enum declaration by providing a second argument:

```csharp
[Flags]
[FlagGroup("Management", "Allows")]
public enum FilePermissions
{
    [FlagGroup("Management")]
    Delete = 4,
    
    [FlagGroup("Management")]
    ModifyAccess = 8,
}
```

* **Method generated**: `GetAllowsManagement()`
* **Property generated**: `AllowsManagement`

---

## Validation and Diagnostics

The analyzer enforces the following rules for group configuration:
1. **Prior Declaration**: A flag member cannot be added to a group unless that group is first declared on the parent enum. Failing to do so triggers warning `HFE0044` (Unknown Group Name).
2. **Identifier Safety**: Group names and prefixes must form valid C# identifiers. Failing this triggers errors `HFE0040` or `HFE0042`.
