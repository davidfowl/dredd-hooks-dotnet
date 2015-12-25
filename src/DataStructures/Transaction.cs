namespace dredd_hooks_dotnet
{
  public struct Transaction 
  {
    string name;
    string host;
    int port;
    string protocol;
    string fullPath;
    Request Request;
    Origin Origin;
  }
}