using CoopGame.Shared.Networking;

namespace CoopGame.Shared.Networking.Messages;

public class PlayerSpawnMessage : IMessage {
    public string playerId;
    public float worldX;
    public float worldY;
    public bool isLocalPlayer;
}
