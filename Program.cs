using CoopGame.Server;
using CoopGame.Server.World;

internal static class Program {
    private static void Main(string[] args) {
        var server = new GameServer();

        // Triggers every server layer needed for gameplay
        server.start();

        Thread cliThread = new Thread(() => commandLoop(server));
        cliThread.IsBackground = true;
        cliThread.Start();

        waitForShutdown();

        server.stop();
        cliThread.Join();
    }

    static void waitForShutdown() {
        var exitEvent = new ManualResetEvent(false);

        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true; // Prevent the process from terminating.
            exitEvent.Set();
        };

        exitEvent.WaitOne();
    }

    static void commandLoop(GameServer server) {
        while(true) {
            string? input = Console.ReadLine()?.Trim().ToLower();
            if(input == null) {
                continue;
            }

            switch (input) {
                case "stop":
                    Environment.Exit(0);
                    break;
                case "players":
                    var players = server.playerManager.getAllPlayers();

                    Console.WriteLine($"[Server] {players.Count} Players online:");

                    foreach (var p in players) {
                        Console.WriteLine($"    - {p.id} at ({(p.worldX / 16):F1}, {(p.worldY / 16):F1})");
                    }

                    break;
                default:
                    Console.WriteLine("[Server] Unknown command");
                    break;
            }
        }
    }
}
