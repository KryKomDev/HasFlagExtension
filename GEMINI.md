# Gemini Project Context: HasFlagExtensions

## Project Overview

This project is a C#/.NET source generator named `HasFlagExtensions`. Its purpose is to automatically generate highly efficient `HasFlag` extension methods and properties for enums that are decorated with the `[Flags]` attribute. This avoids the overhead of the traditional `Enum.HasFlag()` method which can cause boxing allocations.

The generator is built using the .NET Compiler Platform (Roslyn) and targets `.NET Standard 2.0`, making it compatible with a wide range of .NET projects.

The core logic involves:
1.  **Analysis:** The generator analyzes the user's source code, looking for enums marked with `[Flags]`.
2.  **Model Creation:** It inspects the enum's members and attributes (`[HasFlagPrefix]`, `[FlagDisplayName]`, `[ExcludeFlag]`, etc.) to build a model of the code to be generated.
3.  **Code Generation:** It then generates the corresponding extension methods and properties as a new C# source file that is added to the user's compilation.

## Building and Running

The project uses the standard `dotnet` CLI for building and testing.

### Key Commands:

*   **Restore Dependencies:**
    ```bash
    dotnet restore ./HasFlagExtension.sln
    ```

*   **Build Project:**
    ```bash
    dotnet build ./HasFlagExtension.sln --no-restore
    ```

*   **Run Tests:**
    ```bash
    dotnet test ./HasFlagExtension.Tests/HasFlagExtension.Tests.csproj --no-build
    ```

## Development Conventions

*   **Language:** The project is written in C# using modern language features (up to C# 14).
*   **Style:** The code is consistently formatted and follows standard C# naming conventions (e.g., `PascalCase` for methods and properties, `camelCase` for local variables).
*   **Testing:** The project has a dedicated test suite (`HasFlagExtension.Tests`) that uses `Microsoft.VisualStudio.TestTools.UnitTesting`. The tests appear to cover the generator's core functionality, including attribute handling and diagnostic error reporting.
*   **CI/CD:** Continuous integration is set up using GitHub Actions (`.github/workflows/test.yml`) to automatically build and test the project on every push to the main branches.
*   **Project Structure:** The solution is divided into three main projects:
    *   `HasFlagExtension.Generator`: The source generator itself.
    *   `HasFlagExtension.Sample`: A sample project demonstrating the generator's usage.
    *   `HasFlagExtension.Tests`: Unit tests for the generator.
