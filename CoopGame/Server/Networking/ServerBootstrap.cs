using System.Net.Sockets;
using System.Net;

using CoopGame.Shared.Networking;
using CoopGame.Shared.Networking.Messages;

namespace CoopGame.Server.Networking;

public class ServerBootstrap {
    private readonly TcpListener listener;
    private readonly List<TcpClient> clients = new();
    private bool running = false;

    public ServerBootstrap(int port = 7777) {
        listener = new TcpListener(IPAddress.Any, port);
    }

    public void start() {
        listener.Start();
        running = true;

        Console.WriteLine("[Server] Listening for connections...");

        Thread acceptThread = new Thread(acceptLoop);
        acceptThread.Start();
    }

    private void acceptLoop() {
        while (running) {
            TcpClient client = listener.AcceptTcpClient();

            lock (clients) {
                clients.Add(client);
            }

            Console.WriteLine($"[Server] Client connected");

            Thread clientThread = new Thread(() => handleClient(client));
            clientThread.Start();
        }
    }

    private void handleClient(TcpClient client) {
        using (NetworkStream stream = client.GetStream()) {
            try {
                var msg = MessageSerializer.read(stream);

                if (msg is ClientJoinMessage join) {
                    Console.WriteLine($"[Server] Player Joined: {join.playerName}");

                    // Send welcome
                    MessageSerializer.send(stream, new ServerWelcomeMessage {
                        welcomeText = $"Welcome {join.playerName}!"
                    });
                }
            } catch (Exception e) {
                Console.WriteLine($"[Server] Error: {e.Message}");
            }
        }

        lock (clients) {
            clients.Remove(client);
        }

        Console.WriteLine("[Server] Client disconnected");
    }

    public void stop() {
        running = false;
        listener.Stop();
    }
}