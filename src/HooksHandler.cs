using System;

namespace dredd_hooks_dotnet
{
    public class HooksHandler : IHooksHandler
    {
        public IHooksHandler RegisterHandlerFor(string TransactionName, EventType Event, Action<Transaction> Handler)
        {
            return this;
        }
    }
}