namespace CoopGame.Server;

using CoopGame.Server.Networking;

using CoopGame.Shared.Networking;

public class GameServer {
    private ServerBootstrap network;

    public void start() {
        NetworkBootstrap.initialize();

        network = new ServerBootstrap(7777);
        network.start();

        Console.WriteLine("[Server] Server startup complete");
    }

    public void stop() {
        network?.stop();
        Console.WriteLine("[Server] Shutdown complete");
    }
}
