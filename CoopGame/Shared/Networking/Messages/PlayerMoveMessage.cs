namespace CoopGame.Shared.Networking.Messages;

public class PlayerMoveMessage : IMessage {
    public string playerId;
    public float deltaX;
    public float deltaY;
}
