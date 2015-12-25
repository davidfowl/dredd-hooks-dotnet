using System;
using System.IO;

namespace dredd_hooks_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
          if (args.Length == 0)
          {
            Console.WriteLine("Hooks file name not specified.");
            return;
          }
          
          if (!File.Exists(args[0]))
          {
            Console.WriteLine("Specified hook file does not exist");
            return;
          }
          
          Server s = new Server();
          s.Run().Wait();
        }
    }
}
