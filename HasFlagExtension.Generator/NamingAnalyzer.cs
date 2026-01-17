// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

namespace HasFlagExtension.Generator;

internal static class NamingAnalyzer {

    internal static EnumNamingInfo? GetNaming(INamedTypeSymbol enumSymbol, DiagBuilder diag) {
        var attr = enumSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == $"{HFNS}.{nameof(EnumNamingAttribute)}");
        
        if (attr is null) return null;
        
        var res = AnalyzeNaming(attr);
        diag.AddRange(res.Diagnostics);
        
        return res.Naming;
    }
    
    internal static EnumNamingAnalysisResult AnalyzeNaming(AttributeData? namingAttr) {
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

    internal static NamingCase GetNamingCase(string? name) {
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
    
    internal struct EnumNamingAnalysisResult {
        public EnumNamingInfo Naming { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public EnumNamingAnalysisResult(EnumNamingInfo naming, ImmutableArray<Diagnostic> diagnostics) {
            Naming      = naming;
            Diagnostics = diagnostics;
        }
    }
    
    internal readonly struct EnumNamingInfo {
        public NamingCase Source { get; }
        public NamingCase Target { get; }

        public EnumNamingInfo(NamingCase source, NamingCase target) {
            Source = source;
            Target = target;
        }
        
        public override string ToString() => $"{Source} -> {Target}";
    }
}