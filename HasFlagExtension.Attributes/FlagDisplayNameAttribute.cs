// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;

namespace HasFlagExtension;

/// <summary>
/// Specifies the name that will be used for the HasFlag extension method for the specified enum member.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class FlagDisplayNameAttribute : Attribute {
    public string DisplayName { get; }

    public FlagDisplayNameAttribute(string displayName) {
        DisplayName = displayName;
    }
}