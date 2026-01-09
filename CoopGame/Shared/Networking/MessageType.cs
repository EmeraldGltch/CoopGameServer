namespace CoopGame.Shared.Networking;

public enum MessageType : byte {
    ClientJoin = 1,
    ServerWelcome = 2,
    ChunkData = 3,
    ChunkRequest = 4,
    PlayerSpawn = 5,
    PlayerMove = 6,
    PlayerUpdate = 7
}
