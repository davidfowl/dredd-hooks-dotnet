using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dredd_hooks_dotnet
{
    public class Program
    {        
        public static void Main(string[] args)
        {          
          IHooksHandler handler = new HooksHandler();
#if DNXCORE50
          Console.Out.WriteLine(
            @"Unfortunately DNXCORE50 does not support assembly loading
            yet. The issue is tracked here: https://github.com/dotnet/coreclr/issues/2095
            I will load the current in-project class.
            Please refer to project documentation for more informations.");
            
            // handler.RegisterHandlerFor()...
            // Add your handlers here.
            
#else          
          foreach (var file in args)
          {
            if (!File.Exists(file))
            {
              Console.Out.WriteLine("Can't find {0}, skipping", file);
              continue;
            }
            
            Assembly assembly = null;
            
            try
            {
              assembly = Assembly.LoadFile(file);
            }
            catch
            {
              Console.Out.WriteLine(Microsoft.Extensions.CompilationAbstractions.Default.LibraryExporter);
              // Not a DLL, let's try with code itself.
              string code = File.ReadAllText(file);
                    
              var syntaxTree = SyntaxFactory.ParseSyntaxTree(code);
              var compilation = CSharpCompilation.Create(
                "hooks.dll",
                syntaxTrees: new[] { syntaxTree },
                references: new[] { 
                  MetadataReference.CreateFromFile(typeof(object).Assembly.Location), //mscorlib
                  MetadataReference.CreateFromFile(typeof(IHooksHandler).GetTypeInfo().Assembly.Location) //dredd_hooks_dotnet
                },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

              Console.Out.WriteLine(typeof(IHooksHandler).GetTypeInfo().Assembly.Location);
              IEnumerable<Diagnostic> failures = compilation.GetDiagnostics();

              foreach (Diagnostic diagnostic in failures)
              {
                  Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
              }              
              
              using (var stream = new MemoryStream())
              {
                  var compileResult = compilation.Emit(stream);                  
                  stream.Flush();
                  assembly = Assembly.Load(stream.GetBuffer());
              }
                              
            }

            if (assembly == null)
            {
              throw new Exception("Unable to find an assembly with dll and code mode.");            
            }
            
              Console.Out.WriteLine("Loaded, invoking");
              assembly.GetExportedTypes()[0] // This isn't a great way...
                      .GetMethod("Configure", BindingFlags.Public | BindingFlags.Static)
                      .Invoke(null, new object[] { handler });      
              Console.Out.WriteLine("Loaded, invoked");
               /*
               
               // The template class should be something like:
               
               public static class Whatever
               {
                 public static void Configure(IHooksHandler handler)
                 {
                   
                 }
               }
               */      
          }

#endif                  
          
          Server s = new Server(handler);
          s.Run().Wait();
        }
    }
}
