using System;

namespace HasFlagExtension.Sample.Dir;

public class NestedClass {
    
    [Flags]
    public enum NestedEnum {
        A,
        B,
        C,
        D
    }

    public class DeeplyNestedClass {
        
        [Flags]
        public enum DeeplyNestedEnum {
            A,
            B,
            C,
            D
        }
    }
}