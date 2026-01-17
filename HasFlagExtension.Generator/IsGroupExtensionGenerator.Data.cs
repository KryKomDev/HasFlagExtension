// HasFlagExtension Generator
// Copyright (c) 2025 - 2026 KryKom

namespace HasFlagExtension.Generator;

public partial class IsGroupExtensionGenerator {

    private struct EnumAnalysisResult {
        public EnumAnalysisData? Data { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        
        public EnumAnalysisResult(
            EnumAnalysisData?          data, 
            ImmutableArray<Diagnostic> diagnostics) 
        {
            Data          = data;
            Diagnostics   = diagnostics;
        }
        
        public static EnumAnalysisResult Failure(ImmutableArray<Diagnostic> diagnostics) 
            => new(null, diagnostics);
    }

    private readonly struct EnumAnalysisData {
        public GroupData[] Data { get; }
        public Accessibility Accessibility { get; }
        public string EnumName { get; }
        public string FullName { get; }
        public string Namespace { get; }
        public EnumNamingInfo? Naming { get; }
        public bool IsFlags { get; }

        public EnumAnalysisData(
            GroupData[]     data,
            Accessibility   accessibility,
            string          enumName,
            string          ns,
            string          fullName,
            EnumNamingInfo? naming,
            bool            isFlags)
        {
            Data          = data;
            Accessibility = accessibility;
            EnumName      = enumName;
            Namespace     = ns;
            FullName      = fullName;
            Naming        = naming;
            IsFlags       = isFlags;
        }

        public override string ToString() =>
            $$"""
              EnumAnalysis {
                  Data: {{string.Join(", ", Data)}}
                  Accessibility: {{Accessibility}}
                  EnumName: {{EnumName}}
                  FullName: {{FullName}}
                  Namespace: {{Namespace}}
                  Naming: {{Naming}}
                  IsFlags: {{IsFlags}}
              }
              """;
    }

    private readonly struct GroupData {
        public string Name { get; }
        public string Prefix { get; }
        public string[] Flags { get; }
        
        public GroupData(string name, string[] flags, string? prefix = null) {
            Name    = name;
            Prefix  = prefix ?? "Is";
            Flags   = flags;
        }

        public override string ToString() {
            return $"GroupData {{ Name: {Name} Prefix: {Prefix} Flags: {string.Join(", ")} }}";
        }
    }
    
    private readonly struct GroupDeclarationInfo : IEquatable<GroupDeclarationInfo> {
        public string GroupName { get; }
        public string? Prefix { get; }
        
        public GroupDeclarationInfo(string groupName, string? prefix) {
            GroupName = groupName;
            Prefix    = prefix;
        }

        public bool Equals(GroupDeclarationInfo other) => GroupName == other.GroupName;
        public override bool Equals(object? obj) => obj is GroupDeclarationInfo other && Equals(other);
        public override int GetHashCode() => GroupName.GetHashCode();

        public override string ToString() {
            return $"GroupDeclaration {{ Name: {GroupName} Prefix: {Prefix} }}";
        }
    }

    private readonly record struct GroupedFlagInfo {
        public string FlagName { get; }
        public HashSet<string> Groups { get; }
        
        public GroupedFlagInfo(string flagName, string[] groups) {
            FlagName = flagName;
            Groups   = new HashSet<string>(groups);
        }

        public override string ToString() {
            return $"FlagInfo {{ Name: {FlagName} Groups: {string.Join(", ")} }}";
        }
    }
}