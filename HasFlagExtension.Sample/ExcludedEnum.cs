using System;

namespace HasFlagExtension.Sample;

[Flags]
[ExcludeFlagEnum]
public enum ExcludedEnum {
    A,
    B,
    C,
    D,
    E
}