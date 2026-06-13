// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace HasFlagExtension.CodeFixes;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HfeCodeFixes))]
public class HfeCodeFixes : CodeFixProvider {
    
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [HasFlagExtensionAnalyzer.UnknownGroupName.Id];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root == null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var attributeSyntax = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
            .OfType<AttributeSyntax>().FirstOrDefault();

        if (attributeSyntax == null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add group to enum",
                createChangedDocument: c => AddGroupToEnumAsync(context.Document, attributeSyntax, c),
                equivalenceKey: "AddGroupToEnum"
            ),
            diagnostic
        );
            
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Remove group from field",
                createChangedDocument: c => RemoveGroupFromFieldAsync(context.Document, attributeSyntax, c),
                equivalenceKey: "RemoveGroupFromField"
            ),
            diagnostic
        );
    }

    private static async Task<Document> AddGroupToEnumAsync(
        Document          document, 
        AttributeSyntax   attributeSyntax, 
        CancellationToken cancellationToken) 
    {
        // get the group name from the attribute argument
        var groupNameExpr = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
        if (groupNameExpr is null) return document; 
        
        // find the enum declaration
        var enumMember = attributeSyntax.Ancestors().OfType<EnumMemberDeclarationSyntax>().FirstOrDefault();
        var enumDecl = enumMember?.Ancestors().OfType<EnumDeclarationSyntax>().FirstOrDefault();
        
        if (enumDecl == null) return document;

        // create the new attribute for the enum
        var nameSyntax = attributeSyntax.Name;
        
        var argList = SyntaxFactory.AttributeArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(
                    groupNameExpr.WithoutTrivia()
                )
            )
        );
        
        var newAttribute = SyntaxFactory.Attribute(nameSyntax, argList);
        
        var newAttributeList = SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(newAttribute)
        );

        // add the attribute to the enum
        var newEnumDecl = enumDecl.AddAttributeLists(newAttributeList);
        
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null) return document;
        
        var newRoot = root.ReplaceNode(enumDecl, newEnumDecl);
        
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> RemoveGroupFromFieldAsync(
        Document          document, 
        AttributeSyntax   attributeSyntax, 
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null || attributeSyntax.Parent is not AttributeListSyntax attributeList) return document;

        SyntaxNode newRoot;
        
        if (attributeList.Attributes.Count == 1) {
            newRoot = root.RemoveNode(attributeList, SyntaxRemoveOptions.KeepNoTrivia)!;
        }
        else {
            var newAttributeList = attributeList.Attributes.Remove(attributeSyntax);
            var updatedAttributeList = attributeList.WithAttributes(newAttributeList);
            newRoot = root.ReplaceNode(attributeList, updatedAttributeList);
        }
        
        return document.WithSyntaxRoot(newRoot);
    }
}
