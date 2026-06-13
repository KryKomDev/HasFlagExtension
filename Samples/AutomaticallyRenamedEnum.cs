using System;

namespace HasFlagExtension.Sample;

[Flags]
public enum AutomaticallyRenamedEnum {
    THIS_ENUM,
    WILL_BE,
    RENAMED,
    AUTOMATICALLY
}