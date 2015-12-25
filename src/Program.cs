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
          if (args.Length != 1)
          {
            Console.WriteLine("Hooks file name not specified.");
            return;
          }
          
          IHooksHandler handler = null;

#if DNXCORE50
          Console.WriteLine(
            @"Unfortunately DNXCORE50 does not support assembly loading
            yet. The issue is tracked here: https://github.com/dotnet/coreclr/issues/2095
            I will load the current in-project class.
            Please refer to project documentation for more informations.");
            
            handler = new HooksHandler();
            // Add your handlers here.
            
#else          
          if (!File.Exists(args[0]))
          {
            Console.WriteLine("Specified hook file does not exist");
            return;
          }
          
          Assembly assembly = null;
          
          try
          {
            assembly = Assembly.LoadFile(args[0]);
          }
          catch
          {
            // Not a DLL, let's try with code itself.
            
            string code = File.ReadAllText(args[0]);
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

            Type hookType = assembly.GetType("HooksHandler");
            handler = (IHooksHandler)Activator.CreateInstance(hookType, null);
#endif                  
          
          Server s = new Server();
          s.Run(handler).Wait();
        }
    }
}
