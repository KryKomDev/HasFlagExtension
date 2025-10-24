// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;

namespace HasFlagExtension;

/// <summary>
/// Specifies that a field within a flag enum should be excluded from HasFlag
/// method generation, typically in code generation or utility functions that
/// work with enumerations marked with the <see cref="FlagsAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ExcludeFlagAttribute : Attribute {
    public bool Exclude { get; }

    public ExcludeFlagAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}