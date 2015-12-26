using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dredd_hooks_dotnet
{
    public class Server
    {
      private readonly Encoding MessageEncoding = Encoding.UTF8;
      private const int dreddServerPort = 61321;
      private const int readSize = 10;
      private const char messageSeparator = '\n';
      private byte[] buffer = new byte[readSize];
      private readonly IHooksHandler _hooksHandler;
      public Server(IHooksHandler hooksHandler)
      {
        _hooksHandler = hooksHandler;
      }

      public HookTransaction ProcessMessage(HookTransaction transaction)
      {
        switch (transaction.@event)
        {
        }
        
        return transaction;
      }

      public async Task Run()
      {

        if (_hooksHandler == null)
        {
          throw new ArgumentNullException("hooksHandler cannot be null");  
        }

        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), dreddServerPort);
        listener.Start();

        var stringBuffer = new StringBuilder(100);
        while (true)
        {
          var clientConnection = await listener.AcceptTcpClientAsync();

          var dataStream = clientConnection.GetStream();
          
          int readBytes = 0;

          do {
            readBytes = await dataStream.ReadAsync(buffer, 0, readSize);
            var messagePart = MessageEncoding.GetString(buffer,0, readBytes);

            if (messagePart.Contains(Environment.NewLine))
            {
              var split = messagePart.Split(messageSeparator);
              var message = string.Concat(stringBuffer.ToString(), split[0]);
              stringBuffer.Clear().Append(split[1]);
              try 
              {
                var deserializedMessage = JsonConvert.DeserializeObject<HookTransaction>(message);
                var response = ProcessMessage(deserializedMessage);
                var serializedResponse = JsonConvert.SerializeObject(response);
                Console.Out.WriteLine(string.Concat(serializedResponse, messageSeparator));
                var responseBytes = MessageEncoding.GetBytes(string.Concat(serializedResponse, messageSeparator));
                await dataStream.WriteAsync(responseBytes, 0, responseBytes.Length);                
              }
              catch (JsonSerializationException e) 
              {
                Console.Out.WriteLine("Invalid message. Skipped.");
                Console.Out.WriteLine(e.Message);
                continue;
              }
            }
            else
            {
              stringBuffer.Append(messagePart);
            }
          } while (readBytes > 0);
          
        }
      }
    }
}
