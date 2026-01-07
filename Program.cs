using CoopGame.Server;

internal static class Program {
    private static void Main(string[] args) {
        var server = new GameServer();
        server.start();

        Console.WriteLine("Press Ctrl+C to shut down.");
        waitForShutdown();

        server.stop();
    }

    static void waitForShutdown() {
        var exitEvent = new ManualResetEvent(false);

        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true; // Prevent the process from terminating.
            exitEvent.Set();
        };

        exitEvent.WaitOne();
    }
}
