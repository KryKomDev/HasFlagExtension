// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using Xunit;

namespace HasFlagExtension.Tests;

public class NameConvertorTests {
    
    [Theory]
    [InlineData("myAwesomeFlag", 0, 1, "MyAwesomeFlag")]
    [InlineData("myAwesomeFlag", 0, 2, "my_awesome_flag")]
    [InlineData("myAwesomeFlag", 0, 3, "MY_AWESOME_FLAG")]
    [InlineData("myAwesomeFlag", 0, 4, "my-awesome-flag")]
    [InlineData("myAwesomeFlag", 0, 5, "my Awesome Flag")]
    [InlineData("myAwesomeFlag", 0, 6, "MY-AWESOME-FLAG")]
    
    [InlineData("MyAwesomeFlag", 1, 0, "myAwesomeFlag")]
    [InlineData("MyAwesomeFlag", 1, 2, "my_awesome_flag")]
    [InlineData("MyAwesomeFlag", 1, 3, "MY_AWESOME_FLAG")]
    [InlineData("MyAwesomeFlag", 1, 4, "my-awesome-flag")]
    
    [InlineData("my_awesome_flag", 2, 1, "MyAwesomeFlag")]
    [InlineData("my_awesome_flag", 2, 0, "myAwesomeFlag")]
    [InlineData("my_awesome_flag", 2, 3, "MY_AWESOME_FLAG")]
    
    [InlineData("MY_AWESOME_FLAG", 3, 0, "myAwesomeFlag")]
    [InlineData("MY_AWESOME_FLAG", 3, 1, "MyAwesomeFlag")]
    
    [InlineData("my-awesome-flag", 4, 1, "MyAwesomeFlag")]
    
    [InlineData("my Awesome Flag", 5, 1, "MyAwesomeFlag")]
    
    [InlineData("MY-AWESOME-FLAG", 6, 0, "myAwesomeFlag")]
    public void Convert_ShouldTransformNamesCorrectly(string input, int source, int target, string expected) {
        var result = NameConvertor.Convert(input, (NamingCase)source, (NamingCase)target);
        Assert.Equal(expected, result);
    }
}
