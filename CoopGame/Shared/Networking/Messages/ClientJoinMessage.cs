using CoopGame.Shared.Networking;

namespace CoopGame.Shared.Networking.Messages;

public class ClientJoinMessage : IMessage {
    public string playerName;
}
