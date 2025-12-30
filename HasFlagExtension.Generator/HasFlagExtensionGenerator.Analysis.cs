// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HasFlagExtension.Generator;

public partial class HasFlagExtensionGenerator {

    private static EnumNamingAnalysisResult AnalyzeNaming(AttributeData? namingAttr) {
        var diag = ImmutableArray.CreateBuilder<Diagnostic>();

        if (namingAttr is null || namingAttr.ConstructorArguments.Length < 2) {
            diag.Add(Diagnostic.Create(NamingNotSpecified, Location.None));
            return new EnumNamingAnalysisResult(new EnumNamingInfo(NamingCase.PASCAL, NamingCase.PASCAL), diag.ToImmutable());
        }

        // extract source naming
        var source = GetNamingCase(namingAttr.ConstructorArguments[0].Value?.ToString());
        
        if (source is NamingCase.KEBAB or NamingCase.SPACED_CAMEL or NamingCase.TRAIN or NamingCase.UNKNOWN) {
            diag.Add(Diagnostic.Create(
                InvalidSourceCase, 
                GetAttributeLocation(namingAttr), 
                source
            ));
        }

        // extract target naming
        var target = GetNamingCase(namingAttr.ConstructorArguments[1].Value?.ToString());
        
        if (target is NamingCase.KEBAB or NamingCase.SPACED_CAMEL or NamingCase.TRAIN or NamingCase.UNKNOWN) {
            diag.Add(Diagnostic.Create(
                InvalidTargetCase, 
                GetAttributeLocation(namingAttr), 
                target
            ));
        }

        // Return the extracted data as a tuple or custom struct
        return new EnumNamingAnalysisResult(new EnumNamingInfo(source, target), diag.ToImmutable());
    }

    private static NamingCase GetNamingCase(string? name) {
        return name switch {
            "0" => NamingCase.CAMEL,
            "1" => NamingCase.PASCAL,
            "2" => NamingCase.SNAKE,
            "3" => NamingCase.SCREAMING_SNAKE,
            "4" => NamingCase.KEBAB,
            "5" => NamingCase.SPACED_CAMEL,
            "6" => NamingCase.TRAIN,
            _   => NamingCase.UNKNOWN
        };
    }
    
    private static EnumGenResult AnalyzeEnum(
        EnumDeclarationSyntax enumDecl,
        Compilation compilation) 
    {
        var diag = ImmutableArray.CreateBuilder<Diagnostic>();
        
        var model = compilation.GetSemanticModel(enumDecl.SyntaxTree);
        if (ModelExtensions.GetDeclaredSymbol(model, enumDecl) is not INamedTypeSymbol enumSymbol) {
            diag.Add(Diagnostic.Create(InvalidEnumSyntax, enumDecl.GetLocation()));
            return EnumGenResult.Failure(diag.ToImmutable());
        }
        
        return new EnumGenResult(GetEnumInfo(enumSymbol, diag), diag.ToImmutable());
    }
    
    private static EnumInfo? GetEnumInfo(INamedTypeSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var access   = symbol.DeclaredAccessibility;

        // skip enums with invalid accessibility
        if (access is not Accessibility.Public and not Accessibility.Internal)
            return null;
        
        var name     = symbol.Name;
        var fullName = symbol.ToDisplayString();
        var ns       = symbol.ContainingNamespace.ToDisplayString();
        
        var prefix  = GetEnumPrefix(symbol, diag);
        var exclude = GetExcludeEnum(symbol, diag);
        var naming  = GetEnumNaming(symbol, diag);
        
        var flags = GetFlags(symbol, diag);

        return new EnumInfo(name, ns, fullName, flags, exclude, access, prefix, naming);
    }

    private static string? GetEnumPrefix(INamedTypeSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(HasFlagPrefixAttribute)}"
            );
        
        if (attr is null) 
            return null;
    
        // not enough arguments
        if (attr.ConstructorArguments.Length < 1) {
            diag.Add(Diagnostic.Create(
                PrefixNotSpecified, 
                GetAttributeLocation(attr)
            ));
            return null;
        }
        
        // invalid type
        if (attr.ConstructorArguments[0].Value is not string prefix) {
            diag.Add(Diagnostic.Create(
                InvalidPrefixType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return null;
        }

        // invalid identifier
        if (!SyntaxFacts.IsValidIdentifier(prefix)) {
            diag.Add(Diagnostic.Create(
                InvalidPrefix,
                GetAttributeLocation(attr), 
                prefix
            ));
            return null;
        }
        
        return prefix;
    }

    private static bool GetExcludeEnum(INamedTypeSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(ExcludeFlagEnumAttribute)}"
            );
        
        if (attr is null) return false;
    
        // not enough arguments
        if (attr.ConstructorArguments.Length < 1)
            return true;

        // invalid type
        if (attr.ConstructorArguments[0].Value is not bool exclude) {
            diag.Add(Diagnostic.Create(
                InvalidExcludeEnumType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return true;
        }
        
        return exclude;
    }

    private static EnumNamingInfo? GetEnumNaming(INamedTypeSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(EnumNamingAttribute)}"
            );

        if (attr is null) return null;

        if (attr.ConstructorArguments.Length < 2) {
            diag.Add(Diagnostic.Create(
                InvalidNamingArguments, 
                GetAttributeLocation(attr)
            ));
            return null;
        }
        
        // extract source naming
        var source = GetNamingCase(attr.ConstructorArguments[0].Value?.ToString());

        if (source is NamingCase.KEBAB or NamingCase.SPACED_CAMEL or NamingCase.TRAIN or NamingCase.UNKNOWN) {
            diag.Add(Diagnostic.Create(
                InvalidSourceCase,
                GetAttributeLocation(attr),
                source
            ));
            return null;
        }

        // extract target naming
        var target = GetNamingCase(attr.ConstructorArguments[1].Value?.ToString());

        if (target is NamingCase.KEBAB or NamingCase.SPACED_CAMEL or NamingCase.TRAIN or NamingCase.UNKNOWN) {
            diag.Add(Diagnostic.Create(
                InvalidTargetCase,
                GetAttributeLocation(attr), 
                target
            ));
            return null;
        }
        
        return new EnumNamingInfo(source, target);
    }
    
    private static FlagInfo[] GetFlags(INamedTypeSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var members = symbol.GetMembers();
        var infos   = new List<FlagInfo>();
        
        foreach (var member in members) {
            if (member is IFieldSymbol field) {
                infos.Add(GetFlagInfo(field, diag));
            }
        }
        
        return infos.ToArray();
    }

    private static FlagInfo GetFlagInfo(IFieldSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var name        = symbol.Name;
        var displayName = GetDisplayName(symbol, diag);
        var exclude     = GetExcludeFlag(symbol, diag);
        var prefix      = GetFlagPrefix (symbol, diag);
        
        return new FlagInfo(name, displayName, exclude, prefix);
    }

    private static bool GetExcludeFlag(IFieldSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(ExcludeFlagAttribute)}"
            );
        
        if (attr is null) return false;
    
        // not enough arguments
        if (attr.ConstructorArguments.Length < 1)
            return true;

        // invalid type
        if (attr.ConstructorArguments[0].Value is not bool exclude) {
            diag.Add(Diagnostic.Create(
                InvalidExcludeFlagType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return true;
        }
        
        return exclude;
    }
    
    private static string? GetDisplayName(IFieldSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(FlagDisplayNameAttribute)}"
            );
        
        if (attr is null) 
            return null;
    
        // not enough arguments
        if (attr.ConstructorArguments.Length < 1) {
            diag.Add(Diagnostic.Create(FlagNameNotSpecified, symbol.Locations.First()));
            return null;
        }
        
        // invalid type
        if (attr.ConstructorArguments[0].Value is not string prefix) {
            diag.Add(Diagnostic.Create(
                InvalidFlagNameType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return null;
        }

        // invalid identifier
        if (!SyntaxFacts.IsValidIdentifier(prefix)) {
            diag.Add(Diagnostic.Create(
                InvalidFlagName,
                GetAttributeLocation(attr), 
                prefix
            ));
            return null;
        }
        
        return prefix;
    }
    
    private static string? GetFlagPrefix(IFieldSymbol symbol, ImmutableArray<Diagnostic>.Builder diag) {
        var attr = symbol
            .GetAttributes()
            .FirstOrDefault(
                a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(HasFlagPrefixAttribute)}"
            );
        
        if (attr is null) 
            return null;
    
        // not enough arguments
        if (attr.ConstructorArguments.Length < 1) {
            diag.Add(Diagnostic.Create(PrefixNotSpecified, symbol.Locations.First()));
            return null;
        }
        
        // invalid type
        if (attr.ConstructorArguments[0].Value is not string prefix) {
            diag.Add(Diagnostic.Create(
                InvalidPrefixType, 
                GetAttributeLocation(attr), 
                attr.ConstructorArguments[0].Value?.GetType().Name
            ));
            return null;
        }

        // invalid identifier
        if (!SyntaxFacts.IsValidIdentifier(prefix)) {
            diag.Add(Diagnostic.Create(
                InvalidPrefix,
                GetAttributeLocation(attr), 
                prefix
            ));
            return null;
        }
        
        return prefix;
    }

    private static Location GetAttributeLocation(AttributeData attr) => 
        attr.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
}