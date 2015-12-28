using System;

namespace dredd_hooks_dotnet
{
    public class HooksHandler : IHooksHandler
    {
        public IHooksHandler RegisterHandlerFor(string TransactionName, EventType Event, Action<Transaction> Handler)
        {
            System.Console.Out.WriteLine("Oh, someone called me!");
            return this;
        }
    }
}