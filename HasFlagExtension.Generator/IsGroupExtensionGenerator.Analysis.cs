// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

namespace HasFlagExtension.Generator;

public partial class IsGroupExtensionGenerator {
    
    // ENTRY POINT
    private static EnumAnalysisResult AnalyzeEnum(
        EnumDeclarationSyntax enumDecl, 
        Compilation           compilation) 
    {
        var diag = ImmutableArray.CreateBuilder<Diagnostic>();
        
        var model = compilation.GetSemanticModel(enumDecl.SyntaxTree);
        if (ModelExtensions.GetDeclaredSymbol(model, enumDecl) is not INamedTypeSymbol enumSymbol) {
            diag.Add(Diagnostic.Create(InvalidEnumSyntax, enumDecl.GetLocation()));
            return EnumAnalysisResult.Failure(diag.ToImmutable());
        }

        return new EnumAnalysisResult(GenerateGroupData(enumSymbol, diag), diag.ToImmutable());
    }

    private static EnumAnalysisData? GenerateGroupData(
        INamedTypeSymbol symbol, 
        DiagBuilder      diagnostics) 
    {
        var access = symbol.DeclaredAccessibility;

        // skip enums with invalid accessibility
        if (access is not Accessibility.Public and not Accessibility.Internal)
            return null;
        
        var name     = symbol.Name;
        var fullName = symbol.ToDisplayString();
        var ns       = symbol.ContainingNamespace.ToDisplayString();
        var isFlags  = symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");
        
        var groupDecls = GetGroupDecls(symbol, diagnostics);
        var flags      = GetFlagGroupAdditions(symbol, groupDecls.Select(g => g.GroupName).ToHashSet(), diagnostics);
        
        var groups = new GroupData[groupDecls.Length];

        for (int i = 0; i < groups.Length; i++) {
            var decl = groupDecls[i];
            var gn   = decl.GroupName;

            var fs = new HashSet<string>();
            
            foreach (var f in flags) {
                if (f.Groups.Contains(gn))
                    fs.Add(f.FlagName);
            }
            
            groups[i] = new GroupData(gn, fs.ToArray(), decl.Prefix);
        }
        
        return new EnumAnalysisData(groups, access, name, ns, fullName, GetNaming(symbol, diagnostics), isFlags);
    }
    
    private static GroupDeclarationInfo[] GetGroupDecls(INamedTypeSymbol symbol, DiagBuilder diag) {
        var decls = symbol.GetAttributes()
            .Where(a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(FlagGroupAttribute)}")
            .ToArray();
        
        List<GroupDeclarationInfo?> di = [];
        
        foreach (var decl in decls) {
            di.Add(AnalyzeGroupDecl(decl, diag));
        }
        
        return di.Where(static d => d is not null).Select(static d => (GroupDeclarationInfo)d!).ToArray();
    }
    
    private static GroupDeclarationInfo? AnalyzeGroupDecl(AttributeData attr, DiagBuilder diag) {
        
        // no parameters specified
        if (attr.ConstructorArguments.Length < 1) {
            diag.Add(Diagnostic.Create(
                PrefixNotSpecified, 
                GetAttributeLocation(attr)
            ));
            return null;
        }
        
        // invalid type
        if (attr.ConstructorArguments[0].Value is not string name) {
            diag.Add(Diagnostic.Create(
                InvalidGroupNameType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return null;
        }
        
        // invalid identifier
        if (!SyntaxFacts.IsValidIdentifier(name)) {
            diag.Add(Diagnostic.Create(
                InvalidGroupName,
                GetAttributeLocation(attr), 
                name
            ));
            return null;
        }

        if (attr.ConstructorArguments.Length < 2)
            return new GroupDeclarationInfo(name, null);
        
        // invalid prefix type
        if (attr.ConstructorArguments[1].Value is not string prefix) {
            diag.Add(Diagnostic.Create(
                InvalidGroupPrefixType,
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return null;
        }

        // invalid identifier
        if (!SyntaxFacts.IsValidIdentifier(prefix)) {
            diag.Add(Diagnostic.Create(
                InvalidGroupPrefix,
                GetAttributeLocation(attr), 
                prefix
            ));
            return null;
        }
        
        return new GroupDeclarationInfo(name, prefix);
    }

    private static GroupedFlagInfo[] GetFlagGroupAdditions(
        INamedTypeSymbol symbol, 
        HashSet<string>  groups, 
        DiagBuilder      diag)
    {
        var members = symbol.GetMembers();
        var infos   = new List<GroupedFlagInfo>();
        
        foreach (var member in members) {
            if (member is IFieldSymbol field && HasGroupAttribute(field)) {
                infos.Add(GetFlagInfo(field, groups, diag));
            }
        }
        
        return infos.ToArray();
        
        static bool HasGroupAttribute(IFieldSymbol field) 
            => field.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(FlagGroupAttribute)}");
    }

    private static GroupedFlagInfo GetFlagInfo(IFieldSymbol symbol, HashSet<string> groups, DiagBuilder diag) {
        var attrs = symbol.GetAttributes()
            .Where(a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(FlagGroupAttribute)}");
        
        var flagName = symbol.Name;

        var added = new HashSet<string>();

        foreach (var a in attrs) {
            var success = TryGetGroupName(a, out var groupName, groups, diag);
            if (!success) continue;
            
            added.Add(groupName!);
        }
        
        return new GroupedFlagInfo(flagName, added.ToArray());
    }

    private static bool TryGetGroupName(
        AttributeData   attr, 
        out string?     name, 
        HashSet<string> groups,
        DiagBuilder     diag) 
    {

        // invalid constructor
        if (attr.ConstructorArguments.Length != 1) {
            diag.Add(Diagnostic.Create(
                InvalidGroupAddition,
                GetAttributeLocation(attr) 
            ));
            
            name = null;
            return false;
        }
        
        // invalid prefix type
        if (attr.ConstructorArguments[0].Value is not string group) {
            diag.Add(Diagnostic.Create(
                InvalidGroupPrefixType,
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            
            name = null;
            return false;
        }

        // is group valid
        if (!groups.Contains(group)) {
            diag.Add(Diagnostic.Create(
                UnknownGroupName,
                GetAttributeLocation(attr), 
                group
            ));
            
            name = null;
            return false;
        }

        name = group;
        return true;
    }
}