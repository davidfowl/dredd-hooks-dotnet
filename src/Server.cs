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

      public HookTransaction ProcessMessage(HookTransaction transaction)
      {
        switch (transaction.Event)
        {
        }
        
        return transaction;
      }

      public async Task Run()
      {
        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), dreddServerPort);
        listener.Start();

        var stringBuffer = new StringBuilder(100);

        var clientConnection = await listener.AcceptTcpClientAsync();
        var dataStream = clientConnection.GetStream();

        int offset = 0;
        int readBytes = 0;

        do {
          readBytes = await dataStream.ReadAsync(buffer, offset, readSize);
          var messagePart = MessageEncoding.GetString(buffer,0, readBytes);

          if (messagePart.Contains(Environment.NewLine))
          {
            var split = messagePart.Split(messageSeparator);
            var message = string.Concat(stringBuffer.ToString(), split[0]);
            stringBuffer.Clear().Append(split[1]);
            var response = ProcessMessage(JsonConvert.DeserializeObject<HookTransaction>(message));
            var responseBytes = MessageEncoding.GetBytes(string.Concat(JsonConvert.SerializeObject(response),messageSeparator));
            await dataStream.WriteAsync(responseBytes, 0, responseBytes.Length);
          }
          else
          {
            stringBuffer.Append(messagePart);
          }

          offset += readBytes;
        } while (readBytes > 0);
      }
    }
}
