// HasFlagExtension Generator
// Copyright (c) 2026 KryKom

using System.IO;
using System.Reflection;

namespace HasFlagExtension.Generator;

[Generator]
public class NamingCaseGenerator : IIncrementalGenerator {
    
    public void Initialize(IncrementalGeneratorInitializationContext context) {

        context.RegisterPostInitializationOutput(ctx => {
            var          assembly     = Assembly.GetExecutingAssembly();
            const string resourceName = $"{HFNS}.Generator.Templates.NamingCase.cs";
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return;

            using var reader  = new StreamReader(stream);
            var       content = reader.ReadToEnd();
            
            // Add the source to the compilation
            ctx.AddSource("HasFlagExtension.NamingCase.g.cs", SourceText.From(content, Encoding.UTF8));
        });
    }
}