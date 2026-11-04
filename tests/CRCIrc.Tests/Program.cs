using Meebey.SmartIrc4net;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
var listener = new TcpListener(IPAddress.Loopback, 0);
listener.Start();
int port = ((IPEndPoint)listener.LocalEndpoint).Port;
var irc = new IrcClient { Encoding = Encoding.UTF8, SendDelay = 0 };
var received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
irc.OnConnected += (_, _) =>
{
    irc.Login("CrcTest", "CRC Test");
    irc.RfcJoin("#test");
};
irc.OnChannelMessage += (_, e) => received.TrySetResult(e.Data.Message);

try
{
    Task listening = Task.Run(() =>
    {
        irc.Connect("127.0.0.1", port);
        irc.Listen();
    });

    using var connection = await listener.AcceptTcpClientAsync(timeout.Token);
    using var stream = connection.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8);
    using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\r\n", AutoFlush = true };
    bool nick = false, user = false, welcome = false, joined = false;
    while (!joined)
    {
        string line = await ReadLine(reader, timeout.Token);
        nick |= line.StartsWith("NICK CrcTest", StringComparison.OrdinalIgnoreCase);
        user |= line.StartsWith("USER ", StringComparison.OrdinalIgnoreCase);
        if (nick && user && !welcome)
        {
            await writer.WriteLineAsync(":server 001 CrcTest :Welcome");
            welcome = true;
        }
        joined = line.StartsWith("JOIN #test", StringComparison.OrdinalIgnoreCase);
    }

    await writer.WriteLineAsync(":CrcTest!user@localhost JOIN :#test");
    await writer.WriteLineAsync(":Other!user@localhost PRIVMSG #test :hello");
    await writer.WriteLineAsync("PING :local-test");
    if (await received.Task.WaitAsync(timeout.Token) != "hello")
        throw new Exception("IRC channel message was not parsed correctly.");
    irc.SendMessage(SendType.Message, "#test", "reply");

    bool pong = false, reply = false;
    while (!pong || !reply)
    {
        string line = await ReadLine(reader, timeout.Token);
        pong |= line.StartsWith("PONG", StringComparison.OrdinalIgnoreCase) && line.Contains("local-test");
        reply |= line == "PRIVMSG #test :reply";
    }
    irc.RfcQuit("Safe");
    string quit;
    do { quit = await ReadLine(reader, timeout.Token); }
    while (!quit.StartsWith("QUIT", StringComparison.OrdinalIgnoreCase));
    if (!quit.Contains("Safe"))
        throw new Exception("IRC quit reason was not sent.");

    Console.WriteLine("Local IRC connection, message, PING/PONG and quit tests passed. Checking disconnect cleanup...");
    connection.Close();
    await listening.WaitAsync(timeout.Token);
    Console.WriteLine("IRC disconnect cleanup passed.");
}
finally
{
    listener.Stop();
}

static async Task<string> ReadLine(StreamReader reader, CancellationToken cancellationToken) =>
    await reader.ReadLineAsync(cancellationToken) ?? throw new IOException("IRC connection closed unexpectedly.");
