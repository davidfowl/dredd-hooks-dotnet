namespace dredd_hooks_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
          Server s = new Server();
          s.Run().Wait();
        }
    }
}
