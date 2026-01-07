using System;

namespace CoopGame.Server.World;

public class Player {
    public readonly Guid id;

    public float worldX;
    public float worldY;

    public int chunkX;
    public int chunkY;

    private readonly int chunkSize;

    public Player(float startX, float startY, int chunkSize) {
        id = Guid.NewGuid();
        this.chunkSize = chunkSize;

        setPosition(startX, startY);
    }

    public void setPosition(float x, float y) {
        worldX = x;
        worldY = y;

        chunkX = (int)Math.Floor(worldX / chunkSize);
        chunkY = (int)Math.Floor(worldY / chunkSize);
    }
}
