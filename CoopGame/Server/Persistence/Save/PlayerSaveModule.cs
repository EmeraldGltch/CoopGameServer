using CoopGame.Server.World;
using System.Text.Json;

namespace CoopGame.Server.Persistence.Save;

public class PlayerSaveModule : ISaveModule {
	public string id => "players";

	private readonly PlayerManager playerManager;

	public PlayerSaveModule(PlayerManager playerManager) {
		this.playerManager = playerManager;
		Console.WriteLine("[Save] Registered PlayerSaveModule");
	}

	public void save(SaveContext context) {
		Console.WriteLine("[Save] Saving players...");

		foreach(var player in playerManager.getAllPlayers()) {
			string fileName = $"{player.id.ToString()}.json";
			string fullPath = context.getPath(fileName);

			PlayerSaveData data = new PlayerSaveData {
				id = player.id,
				worldX = player.worldX / 16f,
				worldY = player.worldY / 16f
			};

			Console.WriteLine($"[Save] Saving player {player.id} to {fullPath}");

			var options = new JsonSerializerOptions {
				WriteIndented = true,
				IncludeFields = true
			};

			string json = JsonSerializer.Serialize(data, options);
			File.WriteAllText(fullPath, json);
		}

		Console.WriteLine("[Save] Players saved.");
	}

	public void load(SaveContext context) {
		string playersPath = context.getPath(id);

		if(!Directory.Exists(playersPath)) {
			return;
		}

		foreach(var file in Directory.GetFiles(playersPath, "*.json")) {
			try {
				string json = File.ReadAllText(file);
				PlayerSaveData? data = JsonSerializer.Deserialize<PlayerSaveData>(json)!;

				if(data == null) {
					continue;
				}

				Player player = new Player(data.id, data.worldX * 16f, data.worldY * 16f, chunkSize: 16);

				playerManager.addPlayer(player);
			} catch(Exception e) {
				Console.WriteLine($"[Save] Failed to load player file '{file}': {e.Message}");
			}
		}
	}

	private class PlayerSaveData {
		public Guid id { get; set; }
		public float worldX { get; set; }
		public float worldY { get; set; }
	}
}
