namespace HasFlagExtension.Sample;

public class Examples {
    public static void Test() {
        var f       = FlagEnum.A | FlagEnum.B;
        // var allowsA = f.AllowA(); // returns true
        // var allowsC = f.AllowC(); // returns false

        if (f is { AllowA: true, AllowB: true }) {
            
        }
    }
}