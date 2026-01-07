using System.Net.Sockets;
using System.Net;
using System.IO;

using CoopGame.Server.Core.Generation;
using CoopGame.Server.World;

using CoopGame.Shared.Networking;
using CoopGame.Shared.Networking.Messages;
using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.Networking;

public class ServerBootstrap {
    private readonly TcpListener listener;
    private readonly List<TcpClient> clients = new();
    private bool running = false;

    private readonly ChunkManager chunkManager = new();

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
        NetworkStream stream = client.GetStream();

        try {
            while (running && client.Connected) {
                var msg = MessageSerializer.read(stream);

                if (msg != null) {
                    handleMessage(client, msg);
                }
            }
        } catch (Exception e) {
            Console.WriteLine($"[Server] Error: {e.Message}");
        } finally {
            lock (clients) {
                clients.Remove(client);
            }

            client.Close();
        }

        Console.WriteLine("[Server] Client disconnected");
    }

    public void sendMessage(TcpClient client, IMessage message) {
        NetworkStream stream = client.GetStream();
        MessageSerializer.send(stream, message);
    }

    private void handleMessage(TcpClient client, IMessage message) {
        switch (message) {
            case ClientJoinMessage join:
                Console.WriteLine($"[Server] {join.playerName} joined");

                sendMessage(client, new ServerWelcomeMessage {
                    welcomeText = $"Welcome, {join.playerName}!"
                });

                break;
            case ChunkRequestMessage chunkRequest:
                Chunk chunk = chunkManager.getChunk(chunkRequest.chunkX, chunkRequest.chunkY);

                // Prepare server's response
                int size = chunk.size;
                TerrainType[,] terrain = new TerrainType[size, size];
                float[,] pollution = new float[size, size];

                for(int x = 0; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        terrain[x, y] = chunk.tiles[x, y].terrainType;
                        pollution[x, y] = chunk.tiles[x, y].pollutionLevel;
                    }
                }

                int[] terrainTypes = TerrainUtils.flattenTerrain(terrain);
                float[] pollutionLevels = TerrainUtils.flattenFloat(pollution);

                sendMessage(client, new ChunkDataMessage {
                    chunkX = chunk.chunkX,
                    chunkY = chunk.chunkY,
                    chunkSize = size,
                    terrainTypes = terrainTypes,
                    pollutionLevels = pollutionLevels
                });

                break;
            default:
                Console.WriteLine($"[Server] Unknown message type received: {message.GetType()}");
                break;
        }
    }

    public void stop() {
        running = false;
        listener.Stop();
    }
}