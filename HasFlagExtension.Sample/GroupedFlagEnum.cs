using System;

namespace HasFlagExtension.Sample;

[Flags]
[FlagGroup("Group1")]
[FlagGroup("Group2")]
[FlagGroup("Group3")]
public enum GroupedFlagEnum {
    [FlagGroup("Group1")]
    A = 1, 
    
    [FlagGroup("Group1")]
    [FlagGroup("Group2")]
    B = 1 << 1, 
    
    [FlagGroup("Group2")]
    C = 1 << 2, 
    
    [FlagGroup("Group3")]
    D = 1 << 3, 
    
    [FlagGroup("Group3")]
    E = 1 << 4
}