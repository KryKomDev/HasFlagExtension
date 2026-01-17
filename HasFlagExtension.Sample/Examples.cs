// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System;
using HasFlagExtension.Sample.Dir;
using static HasFlagExtension.Sample.Dir.NestedClass;
using static HasFlagExtension.Sample.Dir.NestedClass.DeeplyNestedClass;

namespace HasFlagExtension.Sample;

public static class Examples {

    public static void Main() {
        var f = FlagEnum.ELEMENT_A | FlagEnum.B;
        var p = InternalEnum.A | InternalEnum.B;
        var i = InnerEnum.A | InnerEnum.C;
        var n = NestedEnum.A | NestedEnum.B;
        var dn = DeeplyNestedEnum.A | DeeplyNestedEnum.B;
        var ar = AutomaticallyRenamedEnum.THIS_ENUM | AutomaticallyRenamedEnum.WILL_BE;
        var df = DifferentlyNamedEnum.ThisEnum | DifferentlyNamedEnum.WillBe;
        var ge = GroupedEnum.A;
        var gfe = GroupedFlagEnum.A | GroupedFlagEnum.B;
        // var ex = ExcludedEnum.A | ExcludedEnum.B;

        var a = f.GetAllowElementA();
        var b = i.GetHasB();
        var c = n.GetHasC();
        var d = dn.GetHasD();
        // var e = ar.GetHasThisEnum();
        var g = df.GetHasthis_enum();
        // var e = ex.GetHasE();
        var ger = ge.GetIsInGroup1();
        var get = ge.GetContainedInGroup2();
        
        Console.WriteLine(gfe.GetIsGroup1());
        Console.WriteLine(gfe.GetIsGroup2());
        Console.WriteLine(gfe.GetIsGroup3());
        
        var isA = p.HasA;

        var allowsA = f.GetAllowElementA(); // returns true
        var allowsC = f.GetAllowC(); // returns false

        if (f is { AllowElementA: true, CanB: true }) {

        }
    }
}
