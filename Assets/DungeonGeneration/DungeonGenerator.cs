using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DungeonTiles {
    FLOOR, WALL, DOOR
}
public class DungeonGenerator
{
    public static DungeonTiles[,] GenerateDungeon(int width, int height) {
        DungeonTiles[,] dungeon = new DungeonTiles[width, height];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                dungeon[x, y] = (DungeonTiles)Random.Range(0, 3);
            }
        }

        return dungeon;
    }
}
