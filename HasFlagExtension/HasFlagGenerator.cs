// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HasFlagExtension;

[Generator]
public class HasFlagGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        
        // Gather all enum declarations
        var enumDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is EnumDeclarationSyntax,
            static (ctx, _) => (EnumDeclarationSyntax)ctx.Node
        ).Where(static e => e is not null);

        // Combine with compilation to inspect attributes and symbols
        var flaggedEnums = context.CompilationProvider.Combine(enumDeclarations.Collect())
            .Select(static (pair, _) => {
                var (compilation, enums) = pair;
                var result = ImmutableArray.CreateBuilder<FlagEnumInfo>();

                foreach (var ed in enums.Distinct()) {
                    var model = compilation.GetSemanticModel(ed.SyntaxTree);
                    if (model.GetDeclaredSymbol(ed) is not { } enumSymbol)
                        continue;

                    // Must be an enum with [Flags]
                    if (enumSymbol.TypeKind != TypeKind.Enum)
                        continue;
                    
                    var hasFlags = enumSymbol.GetAttributes().Any(
                        a => a.AttributeClass?.ToDisplayString() is 
                            nameof(FlagsAttribute) or 
                            $"{nameof(System)}.{nameof(FlagsAttribute)}"
                    );

                    var access = enumSymbol.DeclaredAccessibility;
                    
                    // skip non-public or non-internal enums
                    if (access is not Accessibility.Public and not Accessibility.Internal) 
                        continue;
                    
                    if (!hasFlags)
                        continue;

                    // Detect an exclude attribute
                    var excludeAttribute = enumSymbol
                        .GetAttributes()
                        .FirstOrDefault(
                            static a => a.AttributeClass?.ToDisplayString() is
                                nameof(ExcludeFlagEnumAttribute) or
                                $"{nameof(HasFlagExtension)}.{nameof(ExcludeFlagEnumAttribute)}"
                        );

                    if (excludeAttribute is not null) {
                        if (excludeAttribute.ConstructorArguments.Length == 0) continue;
                        
                        var exclude = excludeAttribute.ConstructorArguments[0].Value as string ?? null;
                        
                        if (exclude == "true") continue;
                    }

                    // Detect a prefix attribute
                    var prefixAttribute = enumSymbol
                        .GetAttributes()
                        .FirstOrDefault(
                            static a => a.AttributeClass?.ToDisplayString() is 
                                nameof(HasFlagPrefixAttribute) or 
                                $"{nameof(HasFlagExtension)}.{nameof(HasFlagPrefixAttribute)}"
                        );

                    string? prefix = null;
                    if (prefixAttribute is not null && prefixAttribute.ConstructorArguments.Length > 0) {
                        var rawPrefix = prefixAttribute.ConstructorArguments[0].Value;
                        prefix = rawPrefix as string ?? null;
                    }
                    
                    // Collect enum members (exclude special ones without constant value)
                    var members = enumSymbol.GetMembers()
                        .OfType<IFieldSymbol>()
                        // .Where(f => f.HasConstantValue && f.ConstantValue is not null)
                        .Where(static f => {
                            var excludeAttribute = f.GetAttributes().FirstOrDefault(
                                a => a.AttributeClass?.ToDisplayString() is
                                    nameof(ExcludeFlagAttribute) or 
                                    $"{nameof(HasFlagExtension)}.{nameof(ExcludeFlagAttribute)}"
                            );

                            if (excludeAttribute is null) return true;
                            
                            if (excludeAttribute.ConstructorArguments.Length == 0) return false;
                            var exclude = excludeAttribute.ConstructorArguments[0].Value as string ?? null;
                        
                            return exclude != "true";
                        })
                        .Select(static f => {
                            var displayNameAttribute = f.GetAttributes().FirstOrDefault(
                                a => a.AttributeClass?.ToDisplayString() is
                                    nameof(FlagDisplayNameAttribute) or
                                    $"{nameof(HasFlagExtension)}.{nameof(FlagDisplayNameAttribute)}"
                            );

                            if (displayNameAttribute is null || displayNameAttribute.ConstructorArguments.Length <= 0)
                                return new EnumMemberInfo(f.Name, f.ConstantValue);
                            
                            var displayNameObj = displayNameAttribute.ConstructorArguments[0].Value;
                            var displayName    = displayNameObj as string ?? null;

                            return new EnumMemberInfo(f.Name, f.ConstantValue, displayName);
                        })
                        .ToImmutableArray();

                    if (members.Length == 0)
                        continue;

                    result.Add(new FlagEnumInfo(enumSymbol, members, access, prefix));
                }

                return result.ToImmutable();
            });

        // Generate one file per enum
        context.RegisterSourceOutput(flaggedEnums, static (spc, enums) => {
            foreach (var e in enums) {
                var source = GenerateExtensionsSource(e);
                spc.AddSource($"{e.SymbolName}Extensions.g.cs", source);
            }
        });
    }

    private static string GenerateExtensionsSource(FlagEnumInfo info) {
        var ns           = info.Namespace;
        var enumName     = info.SymbolName;
        var fullEnumName = info.FullEnumName;
        var extTypeName  = enumName + "Extensions";
        var am           = info.Accessibility == Accessibility.Public ? "public" : "internal";

        var sb = new StringBuilder();
        sb.AppendLine($"""
                       // <auto-generated/>
                       
                       using System; 
                       using System.Diagnostics.Contracts;
                       using {ns};
                       """);

        // // Add static import for nested enum's containing class
        // if (!string.IsNullOrEmpty(info.ContainingClassFullName)) {
        //     sb.AppendLine($"using static {info.ContainingClassFullName};");
        // }
        
        if (!string.IsNullOrEmpty(ns)) {
            sb.AppendLine();
            sb.AppendLine($"namespace {ns} {{");
            sb.AppendLine();
        }

        sb.AppendLine($$"""
                            {{am}} static partial class {{extTypeName}} {
                        """);
        
        // Generate methods: public static bool HasFlag{Name}(this EnumType value)
        foreach (var m in info.Members) {
            var methodName = info.Prefix + (m.DisplayName ?? SafeIdentifier(m.Name));
            
            sb.AppendLine($"""
                           
                                   /// <summary>
                                   /// Returns true if any of the bits for {m.Name} are set in the value.
                                   /// </summary>
                                   [Pure]
                                   {am} static bool Get{methodName}(this {fullEnumName} value) => value.HasFlag({fullEnumName}.{m.Name});
                           """);
        }

        // Generate Extension Members
        sb.AppendLine($$"""
                        
                        #if NET10_0_OR_GREATER
                        
                                extension({{fullEnumName}} value) {
                        """);

        foreach (var m in info.Members) {
            var propertyName = info.Prefix + (m.DisplayName ?? SafeIdentifier(m.Name));

            sb.AppendLine($"""
                           
                                       /// <summary>
                                       /// Returns true if any of the bits for {m.Name} are set in the value.
                                       /// </summary>
                                       {am} bool {propertyName} => value.HasFlag({fullEnumName}.{m.Name});
                           """);
        }

        sb.AppendLine("""
                              }
                      
                      #endif
                      
                      """);
        
        sb.AppendLine("    }");
        if (!string.IsNullOrEmpty(ns)) {
            sb.Append("}");
        }

        return sb.ToString();
    }

    private static string SafeIdentifier(string name) {
        // Basic sanitization for identifiers conflicting with keywords or containing invalid chars
        // Keywords list is limited; prepend @ if needed and strip invalid chars.
        if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
            SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None)
        {
            return "@" + name;
        }
        
        // Replace invalid identifier chars with underscore
        var sb = new StringBuilder(name.Length);
        for (int i = 0; i < name.Length; i++) {
            var ch = name[i];
            if (i == 0 ? SyntaxFacts.IsIdentifierStartCharacter(ch) : SyntaxFacts.IsIdentifierPartCharacter(ch))
                sb.Append(ch);
            else
                sb.Append('_');
        }
        
        var id = sb.ToString();
        if (string.IsNullOrEmpty(id) || !SyntaxFacts.IsIdentifierStartCharacter(id[0]))
            id = "_" + id;
        return id;
    }

    private readonly record struct EnumMemberInfo(string Name, object? ConstantValue, string? DisplayName = null) {
        public string Name { get; } = Name;
        public object? ConstantValue { get; } = ConstantValue;
        public string? DisplayName { get; } = DisplayName is null ? null : SafeIdentifier(DisplayName);
    }

    private readonly record struct FlagEnumInfo(
        INamedTypeSymbol Symbol,
        ImmutableArray<EnumMemberInfo> Members,
        Accessibility Accessibility, string? Prefix = null) 
    {
        private readonly string? _prefix = Prefix;

        public string Namespace => Symbol.ContainingNamespace?.IsGlobalNamespace == false
            ? Symbol.ContainingNamespace.ToDisplayString()
            : string.Empty;
        
        public string SymbolName => Symbol.Name;
        
        public string FullEnumName {
            get {
                var parts   = new List<string>();
                var current = Symbol;
                
                while (current != null) {
                    parts.Insert(0, current.Name);
                    current = current.ContainingType;
                }
                
                return string.Join(".", parts);
            }
        }
        
        // public string? ContainingClassFullName {
        //     get {
        //         if (Equals(Symbol.ContainingType, null)) return null;
        //         
        //         var parts   = new List<string>();
        //         var current = Symbol.ContainingType;
        //         
        //         while (current != null) {
        //             parts.Insert(0, current.Name);
        //             current = current.ContainingType;
        //         }
        //         
        //         var namespacePart = !string.IsNullOrEmpty(Namespace) ? Namespace + "." : "";
        //         return namespacePart + string.Join(".", parts);
        //     }
        // }
        
        public INamedTypeSymbol Symbol { get; } = Symbol;
        public ImmutableArray<EnumMemberInfo> Members { get; } = Members;
        public Accessibility Accessibility { get; } = Accessibility;

        public string Prefix => _prefix ?? "Has";
    }
}