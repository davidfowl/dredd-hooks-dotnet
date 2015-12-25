using System;

namespace dredd_hooks_dotnet
{
  public interface IHooksHandler
  {
    IHooksHandler RegisterHandlerFor(string TransactionName, EventType Event, Action<Transaction> Handler);
  }
}