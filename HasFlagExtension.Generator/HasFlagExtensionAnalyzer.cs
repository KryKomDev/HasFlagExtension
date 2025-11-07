using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HasFlagExtension.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class HasFlagExtensionAnalyzer : DiagnosticAnalyzer {
    
    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(Analyze | ReportDiagnostics);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
        InvalidSourceCase,
        InvalidTargetCase,
        NamingNotSpecified,
        InvalidNamingArguments,
        InvalidPrefix,
        PrefixNotSpecified,
        InvalidPrefixType,
        InvalidExcludeEnumType,
        InvalidExcludeFlagType,
        InvalidFlagName,
        FlagNameNotSpecified,
        InvalidFlagNameType,
        InvalidEnumSyntax
    ];
    
    // naming

    internal static readonly DiagnosticDescriptor InvalidSourceCase = new(
        id: "HFE001",
        title: "Invalid Source Naming Case",
        messageFormat: "Source Naming case {0} is not valid in C#", 
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Source Naming case {0} is not valid in C#, because it could result in invalid identifiers."
    );
    
    internal static readonly DiagnosticDescriptor InvalidTargetCase = new(
        id: "HFE002",
        title: "Invalid Target Naming Case",
        messageFormat: "Target Naming case {0} is not valid in C#", 
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Target Naming case {0} is not valid in C#, because it could result in invalid identifiers."
    );
    
    internal static readonly DiagnosticDescriptor NamingNotSpecified = new(
        id: "HFE003",
        title: "Naming Case not specified",
        messageFormat: "Naming case is not specified for assembly", 
        category: "HasFlagExtension",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Naming Case is not specified for assembly, using default (Pascal Case)."
    );
    
    internal static readonly DiagnosticDescriptor InvalidNamingArguments = new( 
        id: "HFE004",
        title: "Invalid Naming Arguments",
        messageFormat: "Naming arguments are not valid", 
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Naming arguments are not valid. Expected 2 NamingCase arguments."
    );
    
    // enum analysis
    
    internal static readonly DiagnosticDescriptor InvalidPrefix = new(
        id: "HFE010",
        title: "Invalid Prefix",
        messageFormat: "HasFlag method prefix '{0}' is not valid",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method prefix '{0}' is not valid, because it cannot result in a valid identifier."
    );
    
    internal static readonly DiagnosticDescriptor PrefixNotSpecified = new(
        id: "HFE011",
        title: "Prefix not specified in attribute",
        messageFormat: "HasFlag method prefix is not specified in attribute",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method prefix is not specified in attribute constructor."
    );
    
    internal static readonly DiagnosticDescriptor InvalidPrefixType = new(
        id: "HFE012",
        title: "Invalid Prefix Type",
        messageFormat: "HasFlag method prefix is not of valid type",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method prefix is not of valid type. Expected string, got {0}."
    );
    
    // exclude enum and flag
    
    internal static readonly DiagnosticDescriptor InvalidExcludeEnumType = new(
        id: "HFE020",
        title: "Invalid ExcludeFlagEnumAttribute argument type",
        messageFormat: "ExcludeFlagEnumAttribute argument is not of valid type",
        category: "HasFlagExtension",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "ExcludeFlagEnumAttribute argument is not of valid type. Expected bool, got {0}."
    );
    
    internal static readonly DiagnosticDescriptor InvalidExcludeFlagType = new(
        id: "HFE021",
        title: "Invalid ExcludeFlagAttribute argument type",
        messageFormat: "ExcludeFlagAttribute argument is not of valid type",
        category: "HasFlagExtension",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "ExcludeFlagAttribute argument is not of valid type. Expected bool, got {0}."
    );
    
    // flag analysis
    
    internal static readonly DiagnosticDescriptor InvalidFlagName = new(
        id: "HFE030",
        title: "Invalid Flag Name",
        messageFormat: "HasFlag method name '{0}' is not valid",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method name '{0}' is not valid, because it cannot result in a valid identifier."
    );
    
    internal static readonly DiagnosticDescriptor FlagNameNotSpecified = new(
        id: "HFE031",
        title: "Flag name not specified in attribute",
        messageFormat: "HasFlag method name is not specified in attribute",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method name is not specified in attribute constructor."
    );
    
    internal static readonly DiagnosticDescriptor InvalidFlagNameType = new(
        id: "HFE032",
        title: "Invalid Flag Name Type",
        messageFormat: "HasFlag method name is not of valid type",
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "HasFlag method name is not of valid type. Expected string, got {0}."
    );
    
    // internal shit
    
    internal static readonly DiagnosticDescriptor InvalidEnumSyntax = new(
        id: "HFE1000",
        title: "Invalid Enum Syntax",
        messageFormat: "Cannot generate HasFlag extension, invalid syntax",   
        category: "HasFlagExtension",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot generate HasFlag extension, invalid syntax."
    );
}