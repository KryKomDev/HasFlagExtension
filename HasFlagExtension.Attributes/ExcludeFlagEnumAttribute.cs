// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;

namespace HasFlagExtension;

/// <summary>
/// Marks an enum type to be excluded from the generation process of the `HasFlag` extension methods.
/// This attribute is intended to be applied to enums and is typically used in conjunction with a source generator
/// to conditionally skip code generation for flagged enums.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class ExcludeFlagEnumAttribute : Attribute {
    
    public bool Exclude { get; }
    
    public ExcludeFlagEnumAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}