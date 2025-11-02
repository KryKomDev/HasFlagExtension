// HasFlagExtension Generator
// Copyright (c) 2025 KryKom
 
// ReSharper disable ClassNeverInstantiated.Global

using System;

namespace HasFlagExtension;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Enum)]
public class EnumNamingAttribute : Attribute {
    public NamingCase EnumNamingCase { get; }
    public NamingCase MethodNamingCase { get; }
    
    public EnumNamingAttribute(NamingCase enumNamingCase, NamingCase methodNamingCase) {
        EnumNamingCase = enumNamingCase;
        MethodNamingCase = methodNamingCase;
    }
}

[AttributeUsage(AttributeTargets.Enum)]
public class ExcludeFlagEnumAttribute : Attribute {
    public bool Exclude { get; }
    
    public ExcludeFlagEnumAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ExcludeFlagAttribute : Attribute {
    public bool Exclude { get; }
    
    public ExcludeFlagAttribute(bool exclude = true) {
        Exclude = exclude;
    }
}

[AttributeUsage(AttributeTargets.Enum)]
public class HasFlagPrefixAttribute : Attribute {
    public string Prefix { get; }
    
    public HasFlagPrefixAttribute(string prefix) {
        Prefix = prefix;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class FlagDisplayNameAttribute : Attribute {
    public string DisplayName { get; }
    
    public FlagDisplayNameAttribute(string displayName) {
        DisplayName = displayName;
    }
}