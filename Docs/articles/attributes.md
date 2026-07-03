# Attributes Reference

`HasFlagExtension` provides several attributes to customize how code is generated for your enums. These attributes are added automatically to your project's compilation at build time.

---

## 1. `[EnumNaming]`
Configures the casing conversions used when translating enum member names to method or property names.

* **Target**: Assembly, Enum
* **Parameters**:
  - `EnumNamingCase` (`NamingCase`): The casing format of the enum members in source code.
  - `MethodNamingCase` (`NamingCase`): The target casing format for the generated extension methods/properties.
* **Usage**:
  ```csharp
  [EnumNaming(NamingCase.SCREAMING_SNAKE, NamingCase.PASCAL)]
  [Flags]
  public enum StatusFlags
  {
      IS_ACTIVE = 1,
      HAS_ERROR = 2,
  }
  // Generates: GetHasIsActive() / HasIsActive
  ```

---

## 2. `[ExcludeFlagEnum]`
Excludes an entire enum from code generation. No extension methods or properties will be generated for it.

* **Target**: Enum
* **Parameters**:
  - `exclude` (`bool`, default `true`): Whether to exclude this enum.
* **Usage**:
  ```csharp
  [Flags]
  [ExcludeFlagEnum]
  public enum InternalFlags
  {
      Hidden = 1,
      Secret = 2,
  }
  ```

---

## 3. `[ExcludeFlag]`
Excludes a specific enum member (flag) from code generation.

* **Target**: Enum Member (Field)
* **Parameters**:
  - `exclude` (`bool`, default `true`): Whether to exclude this specific member.
* **Usage**:
  ```csharp
  [Flags]
  public enum MyFlags
  {
      None = 0,
      [ExcludeFlag] NoneEquivalent = 0, // Excluded to avoid duplicate generation
      FlagA = 1,
      FlagB = 2,
  }
  ```

---

## 4. `[HasFlagPrefix]`
Overrides the default prefix (`GetHas` or `Has`) for generated methods and properties.

* **Target**: Enum, Enum Member (Field)
* **Parameters**:
  - `prefix` (`string`): The custom prefix to prepend (e.g. `"Allow"`, `"Can"`, `"Is"`).
* **Usage**:
  ```csharp
  [Flags]
  [HasFlagPrefix("Can")]
  public enum DeviceStatus
  {
      Read = 1,
      Write = 2,
  }
  // Generates: GetCanRead() / CanRead, GetCanWrite() / CanWrite
  ```

---

## 5. `[FlagDisplayName]`
Overrides the name of the flag used to construct the method/property name. This ignores the naming case conversion logic and applies the specified name directly.

* **Target**: Enum Member (Field)
* **Parameters**:
  - `displayName` (`string`): The custom name to use for this flag.
* **Usage**:
  ```csharp
  [Flags]
  public enum MyFlags
  {
      [FlagDisplayName("SuperAdministrator")]
      Admin = 1,
  }
  // Generates: GetHasSuperAdministrator() / HasSuperAdministrator
  ```

---

## 6. `[FlagGroup]`
Groups multiple flag members into a named logical set and generates a combined check method/property.

* **Target**: Enum (to define group), Enum Member (Field) (to add to group)
* **Parameters**:
  - `group` (`string`): The name of the group.
  - `prefix` (`string`, optional): Custom prefix for the group check.
* **Usage**:
  ```csharp
  [Flags]
  [FlagGroup("ReadWrite")] // 1. Define group on enum
  public enum FileAccess
  {
      [FlagGroup("ReadWrite")] Read = 1,  // 2. Assign members to group
      [FlagGroup("ReadWrite")] Write = 2,
      Execute = 4,
  }
  // Generates: GetIsReadWrite() / IsReadWrite (checks if either Read or Write is present)
  ```
