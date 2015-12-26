using System.Collections.Generic;
using Newtonsoft.Json;

namespace dredd_hooks_dotnet
{
  public struct HookTransaction
  {
    public string uuid;
    public string @event;
    [JsonConverter(typeof(SingleOrArrayConverter<Transaction>))] // Temp, as it will pollute the structure.
    public IList<Transaction> data;
  }
}
