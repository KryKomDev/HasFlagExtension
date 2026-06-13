using System.Collections.Immutable;
using HasFlagExtension.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace HasFlagExtension.Tests;

public class DiagnosticTests {
    
    private readonly ITestOutputHelper _testOutputHelper;
    public DiagnosticTests(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    private const string ATTRIBUTE_SOURCE = 
        """
        using System;

        namespace HasFlagExtension;

        internal enum NamingCase : byte {
            CAMEL = 0,
            PASCAL = 1,
            SNAKE = 2,
            SCREAMING_SNAKE = 3,
            KEBAB = 4,
            SPACED_CAMEL = 5,
            TRAIN = 6,
            UNKNOWN
        }

        [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Enum)]
        internal class EnumNamingAttribute : Attribute {
            public object EnumNamingCase { get; }
            public object MethodNamingCase { get; }
            
            public EnumNamingAttribute(object enumNamingCase, object methodNamingCase) {
                EnumNamingCase = enumNamingCase;
                MethodNamingCase = methodNamingCase;
            }
        }

        [AttributeUsage(AttributeTargets.Enum)]
        internal class ExcludeFlagEnumAttribute : Attribute {
            public object Exclude { get; }
            public ExcludeFlagEnumAttribute(object exclude) {
                Exclude = exclude;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class ExcludeFlagAttribute : Attribute {
            public object Exclude { get; }
            public ExcludeFlagAttribute(object exclude) {
                Exclude = exclude;
            }
        }

        [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
        internal class HasFlagPrefixAttribute : Attribute {
            public object Prefix { get; }
            public HasFlagPrefixAttribute(object prefix) {
                Prefix = prefix;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class FlagDisplayNameAttribute : Attribute {
            public object DisplayName { get; }
            public FlagDisplayNameAttribute(object displayName) {
                DisplayName = displayName;
            }
        }

        [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
        internal class FlagGroupAttribute : Attribute {
            public object Group { get; }
            public object Prefix { get; }
            
            public FlagGroupAttribute(object group) {
                Group = group;
                Prefix = string.Empty;
            }
            
            public FlagGroupAttribute(object group, object prefix) {
                Group = group;
                Prefix = prefix;
            }
        }
        """;

    [Fact]
    public void InvalidPrefix_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [HasFlagPrefix("123Invalid")]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE010"); // InvalidPrefix
    }

    [Fact]
    public void InvalidSourceNaming_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [EnumNaming(NamingCase.KEBAB, NamingCase.PASCAL)]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE001"); // InvalidSourceCase
    }

    [Fact]
    public void InvalidTargetNaming_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [EnumNaming(NamingCase.PASCAL, NamingCase.KEBAB)]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE002"); // InvalidTargetCase
    }

    [Fact]
    public void InvalidNamingArguments_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [EnumNaming(NamingCase.PASCAL)]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE004"); // InvalidNamingArguments
    }

    [Fact]
    public void PrefixNotSpecified_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [HasFlagPrefix]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE011"); // PrefixNotSpecified
    }

    [Fact]
    public void InvalidPrefixType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [HasFlagPrefix(123)]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE012"); // InvalidPrefixType
    }

    [Fact]
    public void InvalidExcludeEnumArgumentType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            [ExcludeFlagEnum("not-a-bool")]
            public enum TestEnum {
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE020"); // InvalidExcludeEnumType
    }

    [Fact]
    public void InvalidExcludeFlagArgumentType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            public enum TestEnum {
                [ExcludeFlag("not-a-bool")]
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE021"); // InvalidExcludeFlagType
    }

    [Fact]
    public void InvalidFlagName_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            public enum TestEnum {
                [FlagDisplayName("123Invalid")]
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE030"); // InvalidFlagName
    }

    [Fact]
    public void FlagNameNotSpecified_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            public enum TestEnum {
                [FlagDisplayName]
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE031"); // FlagNameNotSpecified
    }

    [Fact]
    public void InvalidFlagNameType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [Flags]
            public enum TestEnum {
                [FlagDisplayName(123)]
                A
            }
            """;

        RunGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE032"); // InvalidFlagNameType
    }

    [Fact]
    public void InvalidGroupName_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup("123Invalid")]
            public enum TestEnum {
                [FlagGroup("123Invalid")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0040"); // InvalidGroupName
    }

    [Fact]
    public void InvalidGroupNameType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup(123)]
            public enum TestEnum {
                [FlagGroup("Read")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0041"); // InvalidGroupNameType
    }

    [Fact]
    public void InvalidGroupPrefix_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup("Read", "123Invalid")]
            public enum TestEnum {
                [FlagGroup("Read")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0042"); // InvalidGroupPrefix
    }

    [Fact]
    public void InvalidGroupPrefixType_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup("Read", 123)]
            public enum TestEnum {
                [FlagGroup("Read")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0043"); // InvalidGroupPrefixType
    }

    [Fact]
    public void UnknownGroupName_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup("Read")]
            public enum TestEnum {
                [FlagGroup("Write")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0044"); // UnknownGroupName
    }

    [Fact]
    public void InvalidGroupAddition_ShouldReportDiagnostic() {
        var source = """
            using System;
            using HasFlagExtension;

            namespace TestNamespace;

            [FlagGroup("Read")]
            public enum TestEnum {
                [FlagGroup("Read", "Prefix")]
                A
            }
            """;

        RunGroupGenerator(source, out var diagnostics);
        
        Assert.Contains(diagnostics, d => d.Id == "HFE0045"); // InvalidGroupAddition
    }

    private void RunGenerator(string source, out ImmutableArray<Diagnostic> diagnostics) {
        var generator = new HasFlagExtensionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        
        var compilation = CSharpCompilation.Create("TestCompilation",
            [
                CSharpSyntaxTree.ParseText(ATTRIBUTE_SOURCE),
                CSharpSyntaxTree.ParseText(source),
            ],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ]
        );
        
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out diagnostics);
    }

    private void RunGroupGenerator(string source, out ImmutableArray<Diagnostic> diagnostics) {
        var generator = new IsGroupExtensionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        
        var compilation = CSharpCompilation.Create("TestCompilation",
            [
                CSharpSyntaxTree.ParseText(ATTRIBUTE_SOURCE),
                CSharpSyntaxTree.ParseText(source),
            ],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ]
        );
        
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out diagnostics);
    }
}
