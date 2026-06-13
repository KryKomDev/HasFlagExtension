using System;
// ReSharper disable InconsistentNaming

namespace HasFlagExtension.Sample;

[Flags]
[EnumNaming(NamingCase.PASCAL, NamingCase.SNAKE)]
public enum DifferentlyNamedEnum {
    ThisEnum,
    WillBe,
    Renamed,
    Differently
}