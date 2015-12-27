using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

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
              continue; // Not existing file, skipping.
            }
            
            Assembly assembly = null;
            
            try
            {
              assembly = Assembly.LoadFile(file);
            }
            catch
            {
              // Not a DLL, let's try with code itself.
              
              string code = File.ReadAllText(file);
              var syntaxTree = SyntaxFactory.ParseSyntaxTree(code);
              var compilation = CSharpCompilation
                  .Create("hooks.dll")
                  .AddSyntaxTrees(syntaxTree)
                  .WithAssemblyName("hooks");
              
              using (var stream = new MemoryStream())
              {
                  var compileResult = compilation.Emit(stream);
                  assembly = Assembly.Load(stream.GetBuffer());
              }
                              
            }

            if (assembly == null)
            {
              throw new Exception("Unable to find an assembly with dll and code mode.");            
            }
              
              assembly.GetExportedTypes()[0] // This isn't a great way...
                      .GetMethod("Configure", BindingFlags.Public | BindingFlags.Static)
                      .Invoke(null, new object[] { handler });            
          }

#endif                  
          
          Server s = new Server(handler);
          s.Run().Wait();
        }
    }
}
