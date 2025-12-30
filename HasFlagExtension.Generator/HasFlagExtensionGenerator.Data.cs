// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HasFlagExtension.Generator;

public partial class HasFlagExtensionGenerator {

    private struct EnumGenResult {
        public EnumInfo? Info { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public EnumGenResult(EnumInfo? info, ImmutableArray<Diagnostic> diagnostics) {
            Info        = info;
            Diagnostics = diagnostics;
        }

        public static EnumGenResult Failure(ImmutableArray<Diagnostic> diagnostics) => new(null, diagnostics);
    }
    
    private struct EnumInfo {
        public string Name { get; }
        public string Namespace { get; }
        public string FullName { get; }
        public string Prefix { get; }
        public bool Exclude { get; }
        public Accessibility Access { get; }
        public FlagInfo[] Members { get; }
        public EnumNamingInfo? Naming { get; }

        public EnumInfo(
            string          name,
            string          ns,
            string          fullName,
            FlagInfo[]      members,
            bool            exclude = false,
            Accessibility   access  = Accessibility.Internal,
            string?         prefix  = null,
            EnumNamingInfo? naming  = null) 
        {
            Name      = name;
            Namespace = ns;
            FullName  = fullName;
            Members   = members;
            Exclude   = exclude;
            Access    = access;
            Prefix    = prefix ?? "Has";
            Naming    = naming;
        }
    }
    
    private readonly struct FlagInfo {
        public string Name { get; }

        private readonly string? _displayName;
        private readonly string? _customPrefix;
        
        public string DisplayName => _displayName ?? Name;
        public bool HasCustomName => _displayName != null;
        
        public string CustomPrefix => _customPrefix ?? throw new NullReferenceException();
        public bool HasCustomPrefix => _customPrefix != null;

        public bool Exclude { get; }

        public FlagInfo(string name, string? displayName, bool exclude = false, string? customPrefix = null) {
            Name         = name;
            _displayName = displayName;
            Exclude      = exclude;
            _customPrefix = customPrefix;
        }
    }

    private struct EnumNamingAnalysisResult {
        public EnumNamingInfo Naming { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public EnumNamingAnalysisResult(EnumNamingInfo naming, ImmutableArray<Diagnostic> diagnostics) {
            Naming      = naming;
            Diagnostics = diagnostics;
        }
    }
    
    internal struct EnumNamingInfo {
        public NamingCase Source { get; }
        public NamingCase Target { get; }

        public EnumNamingInfo(NamingCase source, NamingCase target) {
            Source = source;
            Target = target;
        }
    }
}