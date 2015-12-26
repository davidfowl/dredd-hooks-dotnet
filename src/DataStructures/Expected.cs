using System.Collections.Generic;

namespace dredd_hooks_dotnet
{
  public struct Expected 
  {
    string statusCode;
    IDictionary<string, string> headers;
    string body;
    string schema;    
  }
}