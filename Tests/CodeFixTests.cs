// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HasFlagExtension.CodeFixes;
using HasFlagExtension.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace HasFlagExtension.Tests;

public class CodeFixTests {
    
    private const string ATTRIBUTE_SOURCE = 
        """
        using System;

        namespace HasFlagExtension;

        [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
        internal class FlagGroupAttribute : Attribute {
            public string Group { get; }
            public string Prefix { get; }
            
            public FlagGroupAttribute(string group) {
                Group = group;
                Prefix = string.Empty;
            }
            
            public FlagGroupAttribute(string group, string prefix) {
                Group = group;
                Prefix = prefix;
            }
        }
        """;

    [Fact]
    public async Task UnknownGroupName_CodeFix_ShouldApplyAddGroupFix() {
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

        var (document, diagnostic) = GetRoslynDocumentAndDiagnostic(source, "HFE0044");

        // Instantiate our code fix provider
        var codeFixProvider = new HfeCodeFixes();

        // Register code fixes
        var actions = new List<CodeAction>();
        var context = new CodeFixContext(
            document, 
            diagnostic, 
            (action, _) => actions.Add(action), 
            CancellationToken.None
        );

        await codeFixProvider.RegisterCodeFixesAsync(context);

        // Verify we got the expected number of fixes
        Assert.Equal(2, actions.Count);
        Assert.Equal("Add group to enum", actions[0].Title);
        Assert.Equal("Remove group from field", actions[1].Title);

        // Apply "Add group to enum" fix
        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
        var applyChangesOperation = operations.OfType<ApplyChangesOperation>().Single();
        var newSolution = applyChangesOperation.ChangedSolution;
        var newDocument = newSolution.GetDocument(document.Id)!;
        var newText = (await newDocument.GetTextAsync()).ToString();

        // Verify that FlagGroup("Write") has been added to the enum declaration
        Assert.Contains("[FlagGroup(\"Write\")]", newText);
        
        // Let's assert the exact resulting code structure contains both attributes on the enum
        Assert.Contains("[FlagGroup(\"Read\")]", newText);
        Assert.Contains("public enum TestEnum", newText);
    }

    [Fact]
    public async Task UnknownGroupName_CodeFix_ShouldApplyRemoveGroupFix() {
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

        var (document, diagnostic) = GetRoslynDocumentAndDiagnostic(source, "HFE0044");

        var codeFixProvider = new HfeCodeFixes();
        var actions = new List<CodeAction>();
        var context = new CodeFixContext(
            document, 
            diagnostic, 
            (action, _) => actions.Add(action), 
            CancellationToken.None
        );

        await codeFixProvider.RegisterCodeFixesAsync(context);

        Assert.Equal(2, actions.Count);
        Assert.Equal("Remove group from field", actions[1].Title);

        // Apply "Remove group from field" fix
        var operations = await actions[1].GetOperationsAsync(CancellationToken.None);
        var applyChangesOperation = operations.OfType<ApplyChangesOperation>().Single();
        var newSolution = applyChangesOperation.ChangedSolution;
        var newDocument = newSolution.GetDocument(document.Id)!;
        var newText = (await newDocument.GetTextAsync()).ToString();

        // Verify that FlagGroup("Write") has been removed from the field A
        Assert.DoesNotContain("[FlagGroup(\"Write\")]", newText);
    }

    private static (Document Document, Diagnostic Diagnostic) GetRoslynDocumentAndDiagnostic(string source, string diagnosticId) {
        // Run generator to get compiler diagnostics
        var generator = new IsGroupExtensionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        
        var compSourceTree = CSharpSyntaxTree.ParseText(source);
        var compAttrTree = CSharpSyntaxTree.ParseText(ATTRIBUTE_SOURCE);
        
        var compilation = CSharpCompilation.Create("TestCompilation",
            [compAttrTree, compSourceTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]
        );
        
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        
        var diag = diagnostics.FirstOrDefault(d => d.Id == diagnosticId);
        if (diag == null) {
            throw new Exception($"Diagnostic {diagnosticId} was not reported by the generator.");
        }

        // Set up AdhocWorkspace
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        // We add both files (Attribute definition and source file) to match the compilation structure
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "TestProj", "TestProj", LanguageNames.CSharp)
            .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddDocument(DocumentId.CreateNewId(projectId), "Attributes.cs", SourceText.From(ATTRIBUTE_SOURCE))
            .AddDocument(documentId, "Source.cs", SourceText.From(source));

        var document = solution.GetDocument(documentId)!;

        // Map the diagnostic to the document's syntax tree
        // The driver ran on a compilation constructed from `compSourceTree`. 
        // We find the syntax node in the new document matching the start/end coordinates of the diagnostic location.
        var location = diag.Location;
        var sourceSpan = location.SourceSpan;
        
        // Re-create the diagnostic location mapped to the workspace document's syntax tree
        var docSyntaxTree = document.GetSyntaxTreeAsync().Result!;
        var mappedLocation = Location.Create(docSyntaxTree, sourceSpan);
        var mappedDiagnostic = Diagnostic.Create(diag.Descriptor, mappedLocation, diag.GetMessage());

        return (document, mappedDiagnostic);
    }
}
