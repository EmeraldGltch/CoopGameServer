using CoopGame.Shared.Networking;

namespace CoopGame.Shared.Networking.Messages;
public class ServerWelcomeMessage : IMessage {
    public string welcomeText;
}
