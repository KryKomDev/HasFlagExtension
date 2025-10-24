// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;

namespace HasFlagExtension;

/// <summary>
/// Defines what prefix should be used for the HasFlag extension method.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class HasFlagPrefixAttribute : Attribute {
    
    public string Prefix { get; }

    public HasFlagPrefixAttribute(string prefix) {
        Prefix = prefix;
    }
}