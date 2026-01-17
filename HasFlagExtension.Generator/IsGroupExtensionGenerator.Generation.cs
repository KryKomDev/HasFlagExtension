// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

namespace HasFlagExtension.Generator;

[Generator]
public partial class IsGroupExtensionGenerator : IIncrementalGenerator {
    
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        
        var incrNaming = context.CompilationProvider.Select(static (compilation, _) => {
            var enumNamingAttribute = compilation.Assembly
                .GetAttributes()
                .FirstOrDefault(attr => 
                    attr.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(EnumNamingAttribute)}");
        
            return AnalyzeNaming(enumNamingAttribute);
        });
        
        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is EnumDeclarationSyntax,
                static (ctx, _) => {
                    var enumSyntax    = (EnumDeclarationSyntax)ctx.Node;
                    var semanticModel = ctx.SemanticModel;
                        
                    // Get the symbol for the enum
                    var enumSymbol = semanticModel.GetDeclaredSymbol(enumSyntax);
                    if (enumSymbol is null) return null;
                        
                    // Check if it has the FlagsAttribute
                    var hasFlagsAttribute = enumSymbol.GetMembers()
                        .Any(m => m.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() 
                            == $"{HFNS}.{nameof(FlagGroupAttribute)}"));
                        
                    return hasFlagsAttribute ? enumSyntax : null;
                }
            )
            .Where(static e => e is not null)
            .Combine(context.CompilationProvider)
            .Select(static (d, _) => AnalyzeEnum(d.Left!, d.Right));
        
        var combined = enumDeclarations.Combine(incrNaming);

        context.RegisterSourceOutput(incrNaming, (spc, result) => {
            foreach (var d in result.Diagnostics) {
                spc.ReportDiagnostic(d);
            }
        });
        
        context.RegisterSourceOutput(combined, Generate);
        
        return;

        static void Generate(SourceProductionContext spc, (EnumAnalysisResult Left, EnumNamingAnalysisResult Right) data) {
            var naming  = data.Right;
            var enumRes = data.Left;

            foreach (var d in enumRes.Diagnostics) spc.ReportDiagnostic(d);

            if (enumRes.Data is null) return;

            var enumInfo = enumRes.Data.Value;

            spc.AddSource($"{enumInfo.EnumName}Extensions.g.cs", GenerateSourceFile(enumInfo, naming.Naming));
        }
    }

    private static string GenerateSourceFile(EnumAnalysisData data, EnumNamingInfo naming) {
        var ns           = data.Namespace;
        var enumName     = data.EnumName;
        var fullEnumName = data.FullName;
        var extTypeName  = enumName + "Extensions";
        var am           = data.Accessibility == Accessibility.Public ? "public" : "internal";
        var nm           = data.Naming ?? naming;
        
        var sb = new StringBuilder();
        
        sb.AppendLine(
            $$"""
              {{AUTOGEN_HEADER}}
              
              using System.Diagnostics.Contracts;
              using System.Runtime.CompilerServices;
              using {{ns}};

              namespace {{ns}};

              {{am}} static partial class {{extTypeName}} {
              
              """);

        foreach (var d in data.Data) {
            AddImpl_Method(d);
        }

        sb.AppendLine(
            $$"""
              
              #if DOTNET_10_OR_GREATER
              
                  extension({{fullEnumName}} {{VALUE_NAME}}) {
                  
              """);
        
        foreach (var d in data.Data) {
            AddImpl_Property(d);
        }

        sb.AppendLine(
            """
                }
                
            #endif
                
            }
            """);
        
        return sb.ToString();

        void AddImpl_Method(GroupData groupData) {
            var name = "Get" + groupData.Prefix + NameConvertor.Convert(groupData.Name, nm);
            sb.Append(
                $"""
                     /// <summary>
                     /// Returns true if the enum value is a member of the '{groupData.Name}' group.
                     /// </summary>
                     [Pure]
                     [MethodImpl(MethodImplOptions.AggressiveInlining)]
                     {am} static bool {name}(this {fullEnumName} {VALUE_NAME}) 
                         => {(data.IsFlags 
                             ? CreateImpl_Flags(groupData.Flags, fullEnumName) 
                             : CreateImpl_Normal(groupData.Flags, fullEnumName))};
                 
                 """);
        }
        
        void AddImpl_Property(GroupData groupData) {
            var name = groupData.Prefix + NameConvertor.Convert(groupData.Name, nm);
            sb.Append(
                $"""
                         /// <summary>
                         /// Returns true if the enum value is a member of the '{groupData.Name}' group.
                         /// </summary>
                         [Pure]
                         [MethodImpl(MethodImplOptions.AggressiveInlining)]
                         {am} bool {name} 
                              => {(data.IsFlags 
                                  ? CreateImpl_Flags(groupData.Flags, fullEnumName) 
                                  : CreateImpl_Normal(groupData.Flags, fullEnumName))};
                 
                 """);
        }

        static string CreateImpl_Normal(string[] flags, string enumName) {
            return $"{VALUE_NAME} is {flags.Select(f => $"{enumName}.{f}").Aggregate((a, b) => $"{a} or {b}")}";
        }
        
        static string CreateImpl_Flags(string[] flags, string enumName) {
            return flags.Select(f => $"{VALUE_NAME}.HasFlag({enumName}.{f})").Aggregate((a, b) => $"{a} || {b}");
        }
    }
}