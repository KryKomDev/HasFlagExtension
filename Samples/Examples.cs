// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System;
using HasFlagExtension.Sample.Dir;
using static HasFlagExtension.Sample.Dir.NestedClass;
using static HasFlagExtension.Sample.Dir.NestedClass.DeeplyNestedClass;

namespace HasFlagExtension.Sample;

public static class Examples {

    public static void Main() {
        Console.WriteLine("==================================================");
        Console.WriteLine("     HasFlagExtension Source Generator Demo       ");
        Console.WriteLine("==================================================");
        Console.WriteLine();

        // 1. Basic Extension Methods & Prefix Customization
        var f = FlagEnum.ELEMENT_A | FlagEnum.B;
        Console.WriteLine("1. Basic Extension Methods & Custom Prefixes:");
        Console.WriteLine("   Value: ELEMENT_A | B");
        Console.WriteLine($"   f.GetAllowElementA() (Custom DisplayName & Prefix): {f.GetAllowElementA()}");
        Console.WriteLine($"   f.GetCanB() (Custom Prefix override): {f.GetCanB()}");
        Console.WriteLine($"   f.GetAllowC() (Default Prefix): {f.GetAllowC()}");
        Console.WriteLine("   - Note: EXCLUDED member is excluded via [ExcludeFlag].");
        Console.WriteLine("   - Note: ExcludedEnum is completely excluded via [ExcludeFlagEnum].");
        Console.WriteLine();

        // 2. C# Extension Properties (available in .NET 10.0+)
        Console.WriteLine("2. C# Extension Properties:");
        var p = InternalEnum.A | InternalEnum.B;
        Console.WriteLine("   Value: A | B");
        Console.WriteLine($"   p.HasA: {p.HasA}");
        Console.WriteLine($"   p.HasC: {p.HasC}");
        Console.WriteLine();

        // 3. Nested Enums
        Console.WriteLine("3. Nested Enums:");
        var n = NestedEnum.A | NestedEnum.B;
        var dn = DeeplyNestedEnum.A | DeeplyNestedEnum.B;
        Console.WriteLine($"   n.GetHasC() (NestedEnum): {n.GetHasC()}");
        Console.WriteLine($"   dn.GetHasD() (DeeplyNestedEnum): {dn.GetHasD()}");
        Console.WriteLine();

        // 4. Automatic & Custom Naming Cases
        Console.WriteLine("4. Naming Case Conversions:");
        var ar = AutomaticallyRenamedEnum.THIS_ENUM | AutomaticallyRenamedEnum.WILL_BE;
        var df = DifferentlyNamedEnum.ThisEnum | DifferentlyNamedEnum.WillBe;
        Console.WriteLine($"   ar.GetHasThisEnum() (Auto ScreamingSnake -> Pascal): {ar.GetHasThisEnum()}");
        Console.WriteLine($"   df.GetHasthis_enum() (Custom Pascal -> Snake): {df.GetHasthis_enum()}");
        Console.WriteLine();

        // 5. Group Extensions
        Console.WriteLine("5. Group Extensions:");
        var ge = GroupedEnum.A;
        var gfe = GroupedFlagEnum.A | GroupedFlagEnum.B;
        Console.WriteLine($"   ge.GetIsInGroup1() (Normal Enum with custom prefix): {ge.GetIsInGroup1()}");
        Console.WriteLine($"   ge.GetContainedInGroup2() (Normal Enum with custom prefix): {ge.GetContainedInGroup2()}");
        Console.WriteLine($"   gfe.GetIsGroup1() (Flag Enum with default prefix): {gfe.GetIsGroup1()}");
        Console.WriteLine($"   gfe.GetIsGroup2(): {gfe.GetIsGroup2()}");
        Console.WriteLine($"   gfe.GetIsGroup3(): {gfe.GetIsGroup3()}");
        Console.WriteLine();

        // 6. Property Pattern Matching (Extension properties)
        Console.WriteLine("6. Extension Properties Pattern Matching:");
        
        #if NET10_0_OR_GREATER

        Console.WriteLine(
            f is { AllowElementA: true, CanB: true } 
                ? "   Success: Pattern matched { AllowElementA: true, CanB: true } on FlagEnum!" 
                : "   Failed to match pattern."
        );

        #else
        
        Console.WriteLine("   Not supported on this compiler version.");
        
        #endif
        
        Console.WriteLine("==================================================");
    }
}
