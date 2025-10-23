namespace HasFlagExtension.Sample;

public static class Examples {
    public static void Main() {
        var f       = FlagEnum.A | FlagEnum.B;
        
        var allowsA = f.GetAllowA(); // returns true
        var allowsC = f.GetAllowC(); // returns false
        
        if (f is { AllowA: true, AllowB: true }) {
            
        }
    }
}