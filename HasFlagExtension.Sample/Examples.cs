// HasFlagExtension Generator
// Copyright (c) 2025 KryKom

using HasFlagExtension.Sample.Dir;
using static HasFlagExtension.Sample.Dir.NestedClass;

namespace HasFlagExtension.Sample;

public static class Examples {
    
    public static void Main() {
        var f = FlagEnum.ELEMENT_A | FlagEnum.B;
        var p = InternalEnum.A | InternalEnum.B;
        var i = InnerEnum.A | InnerEnum.C;
        var n = NestedEnum.A | NestedEnum.B;
        var dn = DeeplyNestedClass.DeeplyNestedEnum.A | DeeplyNestedClass.DeeplyNestedEnum.B;
        
        var a = f.GetAllowElementA();
        var b = i.GetHasB();
        var c = n.GetHasC();
        var d = dn.GetHasD();

        var isA = p.HasA;
        
        var allowsA = f.GetAllowElementA(); // returns true
        var allowsC = f.GetAllowC(); // returns false
        
        if (f is { AllowElementA: true, AllowB: true }) {
            
        }
    }
}