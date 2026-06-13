// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System;

namespace HasFlagExtension.Sample;

[Flags]
[HasFlagPrefix("Allow")]
public enum FlagEnum {
    
    [FlagDisplayName("ElementA")]
    ELEMENT_A,
    
    [HasFlagPrefix("Can")]
    B,
    C,
    D,
    E,
    
    [ExcludeFlag]
    EXCLUDED
}