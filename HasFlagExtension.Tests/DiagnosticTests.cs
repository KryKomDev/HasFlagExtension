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
            public NamingCase EnumNamingCase { get; }
            public NamingCase MethodNamingCase { get; }
            
            public EnumNamingAttribute(NamingCase enumNamingCase, NamingCase methodNamingCase) {
                EnumNamingCase = enumNamingCase;
                MethodNamingCase = methodNamingCase;
            }
        }

        [AttributeUsage(AttributeTargets.Enum)]
        internal class ExcludeFlagEnumAttribute : Attribute {
            public bool Exclude { get; }
            public ExcludeFlagEnumAttribute(bool exclude = true) {
                Exclude = exclude;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class ExcludeFlagAttribute : Attribute {
            public bool Exclude { get; }
            public ExcludeFlagAttribute(bool exclude = true) {
                Exclude = exclude;
            }
        }

        [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
        internal class HasFlagPrefixAttribute : Attribute {
            public string Prefix { get; }
            public HasFlagPrefixAttribute(string prefix) {
                Prefix = prefix;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class FlagDisplayNameAttribute : Attribute {
            public string DisplayName { get; }
            public FlagDisplayNameAttribute(string displayName) {
                DisplayName = displayName;
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
}
