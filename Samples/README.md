# HasFlagExtension Samples

This directory contains sample enums and a console application demonstrating the features of the **HasFlagExtension** source generator.

## How to Run the Samples

To run the sample console application, execute the following command from the repository root:

```bash
dotnet run --project Samples/HasFlagExtension.Sample.csproj
```

---

## Demonstrated Features

### 1. Basic Extension Methods & Prefixes
* **File:** [FlagEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/FlagEnum.cs)
* **Explanation:**
  * Uses `[HasFlagPrefix("Allow")]` on the enum to prefix generated methods with `GetAllow...` (e.g., `GetAllowC()`).
  * Uses `[FlagDisplayName("ElementA")]` to override the suffix name for `ELEMENT_A` to `ElementA`, resulting in `GetAllowElementA()`.
  * Uses `[HasFlagPrefix("Can")]` on member `B` to override the prefix specifically for that member (`GetCanB()`).
  * Uses `[ExcludeFlag]` on member `EXCLUDED` to completely skip generating extension methods for it.

### 2. Extension Properties (.NET 10.0+)
* **File:** [InternalEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/InternalEnum.cs)
* **Explanation:**
  * Demonstrates support for internal enums.
  * In `.NET 10.0` and above, the generator produces experimental C# extension properties (e.g., `p.HasA`, `p.HasC`) using the `extension` keyword.
  * This allows you to check flags cleanly without calling method parentheses.

### 3. Nested Enums
* **File:** [NestedClass.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/Dir/NestedClass.cs)
* **Explanation:**
  * Demonstrates that the source generator successfully handles enums nested within classes (`NestedEnum`) as well as deeply nested classes (`DeeplyNestedEnum`).

### 4. Naming Case Conversions
* **Files:** 
  * [AssemblyInfo.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/AssemblyInfo.cs) (Global naming configuration)
  * [AutomaticallyRenamedEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/AutomaticallyRenamedEnum.cs)
  * [DifferentlyNamedEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/DifferentlyNamedEnum.cs)
* **Explanation:**
  * By default, the assembly specifies `[assembly: EnumNaming(NamingCase.SCREAMING_SNAKE, NamingCase.PASCAL)]`, which automatically translates SCREAMING_SNAKE enum fields (like `THIS_ENUM`) to PascalCase methods (`GetHasThisEnum()`).
  * You can override naming conventions locally on specific enums. For example, `DifferentlyNamedEnum` uses `[EnumNaming(NamingCase.PASCAL, NamingCase.SNAKE)]` to transform PascalCase members into snake_case methods (`GetHasthis_enum()`).

### 5. Exclude Enum Generation
* **File:** [ExcludedEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/ExcludedEnum.cs)
* **Explanation:**
  * Adding `[ExcludeFlagEnum]` on the enum level tells the generator not to produce any extensions for it whatsoever.

### 6. Group Extensions
* **Files:**
  * [GroupedEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/GroupedEnum.cs) (Normal Enum)
  * [GroupedFlagEnum.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/GroupedFlagEnum.cs) (Flag Enum)
* **Explanation:**
  * You can declare flag groups on an enum using `[FlagGroup("GroupName", "Prefix")]` (e.g., `[FlagGroup("Group1", "IsIn")]`).
  * Associate fields/flags with those groups using `[FlagGroup("GroupName")]` on the fields.
  * For normal enums, the generator produces methods verifying if the value matches one of the group's members (`val is GroupedEnum.A or GroupedEnum.B`).
  * For flag enums, it produces bitwise checks using `HasFlag` (e.g., `val.HasFlag(GroupedFlagEnum.A) || val.HasFlag(GroupedFlagEnum.B)`).
  * If no prefix is specified, the generator defaults to `"Is"` (e.g., `GetIsGroup1()`).

### 7. Extension Properties Pattern Matching
* **File:** [Examples.cs](file:///C:/Users/krystof/Desktop/projects/HasFlagExtension/Samples/Examples.cs)
* **Explanation:**
  * Demonstrates using property pattern matching syntax on generated extension properties:
    ```csharp
    if (f is { AllowElementA: true, CanB: true }) { ... }
    ```
