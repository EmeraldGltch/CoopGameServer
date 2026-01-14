namespace CoopGame.Server;

using CoopGame.Server.Core.Generation;
using CoopGame.Server.Networking;
using CoopGame.Server.Persistence;
using CoopGame.Server.Persistence.Save;
using CoopGame.Server.Simulation;
using CoopGame.Server.Simulation.Simulator;
using CoopGame.Server.World;
using CoopGame.Shared.Networking;
using CoopGame.Shared.Networking.Messages;
using System.Diagnostics;

public class GameServer {
    private ServerBootstrap network;

    private int lastReportedDay = -1;

	public PlayerManager playerManager = new PlayerManager();
    public ChunkManager chunkManager = new ChunkManager();

	private SimulationScheduler simulationScheduler;
    private ServerSimulator serverSimulator;
	private Thread simulationThread;

	public SaveManager saveManager;
    private Thread autoSaveThread;
    private bool autoSaveRunning = false;
    private int autoSaveIntervalSeconds = 5; // In seconds

	private bool isRunning = false;

    public void start() {
        NetworkBootstrap.initialize();

        network = new ServerBootstrap(this, 7777);
        network.start();

        saveManager = new SaveManager("saves/dev");
        saveManager.registerModule(new PlayerSaveModule(playerManager));
		saveManager.loadAll();

		serverSimulator = new ServerSimulator();

		simulationScheduler = new SimulationScheduler(chunkManager, playerManager);

		isRunning = true;
        simulationThread = new Thread(simulationLoop);
        simulationThread.IsBackground = true;
        simulationThread.Start();

        autoSaveRunning = true;
        autoSaveThread = new Thread(autoSaveLoop);
        autoSaveThread.IsBackground = true;
        autoSaveThread.Start();

		Console.WriteLine("[Server] Server startup complete");
    }

    private void autoSaveLoop() {
        while(autoSaveRunning) {
            Thread.Sleep(autoSaveIntervalSeconds * 1000);

            Console.WriteLine("[Server] Auto-saving...");
			saveManager?.saveAll();
            Console.WriteLine("[Server] Save complete");
		}
	}

	private void simulationLoop() {
        const float ticksPerSecond = 24f;
        const double tickInterval = 1.0 / ticksPerSecond;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        double lastTime = stopwatch.Elapsed.TotalSeconds;

        Console.WriteLine("[Server] Simulation loop started");

        while(isRunning) {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double delta = currentTime - lastTime;

            if(delta >= tickInterval) {
                serverSimulator.tick(delta);
				simulationScheduler.tick(delta);
                lastTime = currentTime;

                int currentDay = serverSimulator.getDayCount();

				if (currentDay != lastReportedDay) {
                    Console.WriteLine($"[Server] Day {currentDay} started");
                    lastReportedDay = currentDay;
				}
            }

            Thread.Sleep(1); // To reduce CPU usage
        }
    }

    public void stop() {
        isRunning = false;

        simulationThread.Join();
        network?.stop();

        saveManager?.saveAll();

		Console.WriteLine("[Server] Shutdown complete");
    }
}
