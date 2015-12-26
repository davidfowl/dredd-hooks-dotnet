using System.Collections.Generic;

namespace dredd_hooks_dotnet
{
  public struct Real 
  {
    string statusCode;
    IDictionary<string, string> headers;
    string body;    
  }
}