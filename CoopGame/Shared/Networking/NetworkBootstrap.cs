using CoopGame.Shared.Networking.Messages;

namespace CoopGame.Shared.Networking;

public static class NetworkBootstrap {
    private static bool initialized = false;

    public static void initialize() {
        if (initialized) {
            return;
        }

        initialized = true;

        MessageSerializer.register<ClientJoinMessage>(MessageType.ClientJoin);
        MessageSerializer.register<ServerWelcomeMessage>(MessageType.ServerWelcome);

        Console.WriteLine("[NetworkBootstrap] Message registration complete");
    }
}
