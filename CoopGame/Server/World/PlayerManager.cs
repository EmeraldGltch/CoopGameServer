namespace CoopGame.Server.World;

public class PlayerManager {
    private readonly Dictionary<Guid, Player> players = new();
    private readonly object lockObj = new();

    //////////////////////
    // Player Lifecycle //
    //////////////////////

    public void addPlayer(Player player) {
        lock(lockObj) {
            players[player.id] = player;
        }
    }

    public void removePlayer(Guid playerId) {
        lock(lockObj) {
            players.Remove(playerId);
        }
    }

    ///////////////////
    // Player Access //
    ///////////////////
    
    public bool tryGetPlayer(Guid playerId, out Player? player) {
        lock (lockObj) {
            return players.TryGetValue(playerId, out player);
        }
    }

    public List<Player> getAllPlayers() {
        lock(lockObj) {
            return new List<Player>(players.Values);
        }
    }

    public int playerCount() {
        return players.Count;
    }
}
