namespace CoopGame.Shared.Networking;

public enum MessageType : byte {
    ClientJoin = 1,
    ServerWelcome = 2,
    ChunkData = 3,
    ChunkRequest = 4
}
