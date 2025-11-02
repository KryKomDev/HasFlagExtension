// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using System;

namespace HasFlagExtension.Sample;

[Flags]
[HasFlagPrefix("Allow")]
public enum FlagEnum {
    
    [FlagDisplayName("ElementA")]
    ELEMENT_A,
    B,
    C,
    D,
    E,
    
    [ExcludeFlag]
    EXCLUDED
}