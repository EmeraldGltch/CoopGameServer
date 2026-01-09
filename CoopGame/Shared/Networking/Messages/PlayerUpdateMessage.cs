namespace CoopGame.Shared.Networking.Messages;

public class PlayerUpdateMessage : IMessage {
    public string playerId;
    public float worldX;
    public float worldY;
}
