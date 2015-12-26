namespace dredd_hooks_dotnet
{
  public struct Transaction 
  {
    string name;
    string id;
    string host;
    int port;
    string protocol;
    string fullPath;
    Request request;
    Expected expected;
    Real real;
    Origin origin;
    bool skip;
    string fail;
    Test test;
    Results results;    
  }
}