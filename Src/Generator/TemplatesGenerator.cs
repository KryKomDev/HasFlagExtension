// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System.IO;
using System.Reflection;

namespace HasFlagExtension.Generator;

[Generator]
public class TemplatesGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterPostInitializationOutput(ctx => {
            RegSrc(ctx, "EnumNamingAttribute");
            RegSrc(ctx, "ExcludeFlagAttribute");
            RegSrc(ctx, "ExcludeFlagEnumAttribute");
            RegSrc(ctx, "FlagDisplayNameAttribute");
            RegSrc(ctx, "HasFlagPrefixAttribute");
            RegSrc(ctx, "FlagGroupAttribute");
            RegSrc(ctx, "NamingCase");
        });
    }

    private static void RegSrc(IncrementalGeneratorPostInitializationContext context, string name) {
        var assembly     = Assembly.GetExecutingAssembly();
        var resourceName = $"HasFlagExtension.Generator.Templates.{name}.cs";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            return;

        using var reader = new StreamReader(stream);
        var       source = reader.ReadToEnd();

        context.AddSource($"{name}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

}