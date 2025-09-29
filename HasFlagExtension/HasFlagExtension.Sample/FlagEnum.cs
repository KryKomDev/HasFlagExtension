using System;

namespace HasFlagExtension.Sample;

[Flags]
[HasFlagPrefix("Allow")]
public enum FlagEnum {
    A,
    B,
    C,
    D,
    E
}