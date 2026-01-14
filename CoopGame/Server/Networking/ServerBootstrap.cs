using System.Net.Sockets;
using System.Net;

using CoopGame.Server.Core.Generation;
using CoopGame.Server.World;

using CoopGame.Shared.Networking;
using CoopGame.Shared.Networking.Messages;
using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.Networking;

public class ServerBootstrap {
    private GameServer gameServer;

    private readonly TcpListener listener;
    private readonly List<TcpClient> clients = new();
    private readonly object clientLock = new();
    private readonly Dictionary<TcpClient, Player> clientPlayers = new();
    private bool running = false;

    private readonly ChunkManager chunkManager = new();

    public ServerBootstrap(GameServer gameServer, int port = 7777) {
        this.gameServer = gameServer;
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

            Console.WriteLine($"[Server] Client connected. Clients Connected: {clients.Count}");

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

            Player? associatedPlayer = null;

            lock(clientPlayers) {
                if (clientPlayers.TryGetValue(client, out var player)) {
                    associatedPlayer = player;
                    clientPlayers.Remove(client);
                }
            }

            if (associatedPlayer != null) {
                gameServer.playerManager.removePlayer(associatedPlayer.id);
            }

            // TODO: Broadcast player despawn to other clients

            client.Close();
            Console.WriteLine($"[Server] Client disconnected. Clients Connected: {clients.Count}");
        }
    }

    public void sendMessage(TcpClient client, IMessage message) {
        if (client == null || !client.Connected) {
            return;
        }

        try {
            NetworkStream stream = client.GetStream();
            MessageSerializer.send(stream, message);
        } catch (Exception e) {
            Console.WriteLine($"[Server] Error sending message to client: {e.Message}");
        }
    }

    public void broadcast(IMessage message, TcpClient? excludeClient = null) {
        lock (clients) {
            foreach (var client in clients) {
                if (client == excludeClient || !client.Connected) {
                    continue;
                }

                try {
                    sendMessage(client, message);
                } catch (Exception e) {
                    Console.WriteLine($"[Server] Error broadcasting to client: {e.Message}");
                }
            }
        }
    }

    private void handleMessage(TcpClient client, IMessage message) {

        switch (message) {
            case ClientJoinMessage join:
                sendMessage(client, new ServerWelcomeMessage {
                    welcomeText = $"Welcome, {join.playerName}!"
                });

                var player = new Player(0, 0, 16);

                gameServer.playerManager.addPlayer(player);

                lock(clientPlayers) {
                    clientPlayers[client] = player;
                }

                sendMessage(client, new PlayerSpawnMessage {
                    playerId = player.id.ToString(),
                    worldX = player.worldX,
                    worldY = player.worldY,
                    isLocalPlayer = true
                });

                Console.WriteLine($"[Server] {join.playerName} joined");

                foreach(var kvp in gameServer.playerManager.getAllPlayers()) {
                    var otherPlayer = kvp;

                    if (otherPlayer.id != player.id) {
                        sendMessage(client, new PlayerSpawnMessage {
                            playerId = otherPlayer.id.ToString(),
                            worldX = otherPlayer.worldX,
                            worldY = otherPlayer.worldY,
                            isLocalPlayer = false
                        });

                        foreach(var otherClient in clients) {
                            if (otherClient != client) {
                                sendMessage(otherClient, new PlayerSpawnMessage {
                                    playerId = player.id.ToString(),
                                    worldX = player.worldX,
                                    worldY = player.worldY,
                                    isLocalPlayer = false
                                });
                            }
                        }
                    }
                }

                break;
            case ChunkRequestMessage chunkRequest:
                Chunk chunk = chunkManager.getChunk(chunkRequest.chunkX, chunkRequest.chunkY);

                // Prepare server's response
                int size = chunk.size;
                TerrainType[,] terrain = new TerrainType[size, size];
                float[,] elevation = new float[size, size];

                for(int x = 0; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        elevation[x, y] = chunk.tiles[x, y].elevation;
                        terrain[x, y] = chunk.tiles[x, y].terrainType;
                    }
                }

                int[] terrainTypes = TerrainUtils.flattenTerrain(terrain);
                float[] elevations = TerrainUtils.flattenFloat(elevation);

                sendMessage(client, new ChunkDataMessage {
                    chunkX = chunk.chunkX,
                    chunkY = chunk.chunkY,
                    chunkSize = size,
                    elevations = elevations,
                    terrainTypes = terrainTypes
                });

                break;
            case PlayerMoveMessage move:
                if(!Guid.TryParse(move.playerId, out var playerId)) {
                    Console.WriteLine($"[Server] Invalid player ID in PlayerMoveMessage: {move.playerId}");
                    break;
                }

                if(gameServer.playerManager.tryGetPlayer(playerId, out var movingPlayer)) {
                    movingPlayer.setPosition(movingPlayer.worldX + move.deltaX, movingPlayer.worldY + move.deltaY);

                    broadcast(new PlayerUpdateMessage {
                        playerId = movingPlayer.id.ToString(),
                        worldX = movingPlayer.worldX,
                        worldY = movingPlayer.worldY
                    }, excludeClient: client);
                }

                break;
            default:
                Console.WriteLine($"[Server] Unknown message type received: {message.GetType()}");
                break;
        }
    }

    public bool hasConnectedClients() {
        lock (clients) {
            return clients.Count > 0;
        }
	}

	public void stop() {
        running = false;
        listener.Stop();
    }
}