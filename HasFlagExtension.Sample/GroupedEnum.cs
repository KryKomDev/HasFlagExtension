namespace HasFlagExtension.Sample;

[FlagGroup("Group1", "IsIn")]
[FlagGroup("Group2", "ContainedIn")]
public enum GroupedEnum {
    
    [FlagGroup("Group1")] 
    A,
    
    [FlagGroup("Group1")] 
    B,

    [FlagGroup("Group1"), FlagGroup("Group2")] 
    C,
    
    [FlagGroup("Group2")]
    D,
    
    [FlagGroup("Group2")]
    E
}