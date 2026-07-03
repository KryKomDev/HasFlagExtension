# Diagnostic Rules

`HasFlagExtension` includes a Roslyn compiler analyzer that runs inside the editor to check for incorrect attributes, naming conventions, and grouping configurations.

---

## Naming and Case Conversion (HFE001 - HFE004)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE001` | Invalid Source Naming Case | Error | The source naming case configured in `[EnumNaming]` is not supported (e.g. kebab-case or space-camel) because it cannot be reliably converted. | Choose a supported case (e.g., `NamingCase.CAMEL`, `NamingCase.PASCAL`, `NamingCase.SNAKE`, `NamingCase.SCREAMING_SNAKE`). |
| `HFE002` | Invalid Target Naming Case | Error | The target naming case configured in `[EnumNaming]` is invalid for C# identifiers (e.g. kebab-case containing hyphens, or train-case). | Ensure target casing translates to a valid C# identifier (e.g., Pascal case or Camel case). |
| `HFE003` | Naming Case not specified | Warning | Neither the enum nor the assembly has a specified `[EnumNaming]` configuration. | The generator defaults to Pascal Case. Specifying `[EnumNaming]` suppresses this warning. |
| `HFE004` | Invalid Naming Arguments | Error | The `[EnumNaming]` constructor arguments could not be processed, or the wrong number of arguments was supplied. | Check constructor usage. |

---

## Prefixes (HFE010 - HFE012)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE010` | Invalid Prefix | Error | The prefix supplied in `[HasFlagPrefix]` contains characters that are invalid for C# identifiers (e.g. starting with numbers, containing spaces or hyphens). | Change the prefix to a valid C# identifier (e.g., `"Can"`, `"Is"`, `"Allow"`). |
| `HFE011` | Prefix not specified in attribute | Error | `[HasFlagPrefix]` was declared without a constructor argument. | Provide a non-empty string prefix. |
| `HFE012` | Invalid Prefix Type | Error | The prefix parameter provided to `[HasFlagPrefix]` is not a string. | Pass a string literal to the constructor. |

---

## Exclusion Attributes (HFE020 - HFE021)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE020` | Invalid ExcludeFlagEnumAttribute argument type | Warning | `[ExcludeFlagEnum]` was called with an argument that is not a boolean. | Use a boolean literal (`true` or `false`) or omit the argument. |
| `HFE021` | Invalid ExcludeFlagAttribute argument type | Warning | `[ExcludeFlag]` was called with an argument that is not a boolean. | Use a boolean literal (`true` or `false`) or omit the argument. |

---

## Custom Flag Display Names (HFE030 - HFE032)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE030` | Invalid Flag Name | Error | The custom flag name in `[FlagDisplayName]` is not a valid C# identifier. | Choose an identifier name that contains only alphanumeric characters and underscores. |
| `HFE031` | Flag name not specified in attribute | Error | `[FlagDisplayName]` was declared without a constructor argument. | Provide a custom string for the display name. |
| `HFE032` | Invalid Flag Name Type | Error | The argument provided to `[FlagDisplayName]` is not a string. | Pass a string literal to the constructor. |

---

## Group Configuration (HFE0040 - HFE0045)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE0040` | Invalid Group Name | Error | The group name supplied in `[FlagGroup]` is not a valid C# identifier. | Change the group name to a valid C# identifier. |
| `HFE0041` | Invalid Group Name Type | Error | The group name parameter is not a string. | Pass a string literal to the constructor. |
| `HFE0042` | Invalid Group Prefix | Error | The custom group prefix supplied in `[FlagGroup(group, prefix)]` is not a valid C# identifier. | Change the prefix to a valid C# identifier. |
| `HFE0043` | Invalid Group Prefix Type | Error | The group prefix parameter is not a string. | Pass a string literal for the prefix. |
| `HFE0044` | Unknown Group Name | Warning | A flag member was decorated with `[FlagGroup("GroupName")]`, but `GroupName` was never defined on the parent enum declaration. | Add `[FlagGroup("GroupName")]` to the enum declaration itself. |
| `HFE0045` | Invalid Group Addition | Error | An invalid constructor combination of `[FlagGroup]` was used. | Follow constructor patterns. |

---

## General Syntax (HFE1000)

| ID | Title | Severity | Cause | Fix |
|:---|:---|:---|:---|:---|
| `HFE1000` | Invalid Enum Syntax | Error | The enum has syntax errors that prevent the compiler/semantic analyzer from parsing its properties or structure. | Address compiler syntax errors first. |
