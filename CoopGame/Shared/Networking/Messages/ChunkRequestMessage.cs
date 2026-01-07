using CoopGame.Shared.Networking;

namespace CoopGame.Shared.Networking.Messages;

public class ChunkRequestMessage : IMessage {
    public int chunkX { get; set; }
    public int chunkY { get; set; }
}
