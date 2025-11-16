// HasFlagExtension Generator
// Copyright (c) 2025 KryKom
 
// ReSharper disable ClassNeverInstantiated.Global

using System;

namespace HasFlagExtension;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Enum)]
internal class EnumNamingAttribute : Attribute {
    public NamingCase EnumNamingCase { get; }
    public NamingCase MethodNamingCase { get; }
    
    public EnumNamingAttribute(NamingCase enumNamingCase, NamingCase methodNamingCase) {
        EnumNamingCase = enumNamingCase;
        MethodNamingCase = methodNamingCase;
    }
}

[AttributeUsage(AttributeTargets.Enum)]
internal class ExcludeFlagEnumAttribute : Attribute {
    public bool Exclude { get; }
    
    public ExcludeFlagEnumAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}

[AttributeUsage(AttributeTargets.Field)]
internal class ExcludeFlagAttribute : Attribute {
    public bool Exclude { get; }
    
    public ExcludeFlagAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}

[AttributeUsage(AttributeTargets.Enum)]
internal class HasFlagPrefixAttribute : Attribute {
    public string Prefix { get; }
    
    public HasFlagPrefixAttribute(string prefix) {
        Prefix = prefix;
    }
}

[AttributeUsage(AttributeTargets.Field)]
internal class FlagDisplayNameAttribute : Attribute {
    public string DisplayName { get; }
    
    public FlagDisplayNameAttribute(string displayName) {
        DisplayName = displayName;
    }
}