using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator {
    
    public const int VOID_ID = 0;
    public const int WALL_ID = 1;
    public const int FLOOR_ID = 2;
    public const int DOORWAY_ID = 3;

    private const int maxRoomIterations = 20;

    public static DungeonResult GenerateDungeon(int width, int height, int numRooms, Texture2D[] roomTemplates) {
        int[,] dungeon = new int[width, height];
        List<RectInt> roomsBounds = new List<RectInt>();
        int i = 0;
        int roomIterations = 0;
        while(i < numRooms) {
            if(addRoom(new DungeonData(ref dungeon, roomTemplates, ref roomsBounds))) {
                i++;
                roomIterations = 0;
            } else {
                roomIterations++;
                if(roomIterations >= maxRoomIterations) {
                    i++;
                }
            }
        }
        Vector2[] roomCenters = new Vector2[roomsBounds.Count];
        for(i = 0; i < roomCenters.Length; i++) {
            roomCenters[i] = roomsBounds[i].center;
        }
        Graph delauney = Delauney.Triangulate(roomCenters);
        return new DungeonResult(dungeon, roomsBounds, delauney);
    }

    public static bool addRoom(DungeonData dungeonData) {
        int roomIndex = Random.Range(0, dungeonData.roomTemplates.Length);
        Texture2D roomTemplate = dungeonData.roomTemplates[roomIndex];
        Color[] pixels = roomTemplate.GetPixels();
        int roomWidth = roomTemplate.width;
        int roomHeight = roomTemplate.height;
        int minX = Random.Range(0, dungeonData.dungeon.GetLength(0)-roomWidth);
        int minY = Random.Range(0, dungeonData.dungeon.GetLength(1)-roomHeight);
        RectInt roomBounds = new RectInt(minX, minY, roomWidth, roomHeight);
        bool valid = true;
        foreach(RectInt otherRoom in dungeonData.roomsBounds) {
            if(roomBounds.Overlaps(otherRoom)) {
                valid = false;
                break;
            }
        }
        if(valid) {
            Debug.Log("New Room");
            dungeonData.roomsBounds.Add(roomBounds);
            for(int y = 0 ; y < roomHeight; y++) {
                for(int x = 0 ; x < roomWidth; x++) {
                    if(pixels[y*roomWidth + x].Equals(Color.black)) {
                        dungeonData.dungeon[x+minX, y+minY] = WALL_ID;
                    } else if(pixels[y*roomWidth + x].Equals(Color.white)) {
                        dungeonData.dungeon[x+minX, y+minY] = FLOOR_ID;
                    } else if(pixels[y*roomWidth + x].Equals(Color.red)) {
                        dungeonData.dungeon[x+minX, y+minY] = DOORWAY_ID;
                    }
                }
            }
        }
        return valid;
    }

    public struct DungeonData {
        public int[,] dungeon;
        public Texture2D[] roomTemplates;
        public List<RectInt> roomsBounds;
        public DungeonData(ref int[,] dungeon, Texture2D[] roomTemplates, ref List<RectInt> roomsBounds) {
            this.dungeon = dungeon;
            this.roomTemplates = roomTemplates;
            this.roomsBounds = roomsBounds;
        }
    }

}

public struct DungeonResult {
    public int[,] dungeon;
    public List<RectInt> roomsBounds;
    public Graph delauney;
    public DungeonResult(int[,] dungeon, List<RectInt> roomsBounds, Graph delauney) {
        this.dungeon = dungeon;
        this.roomsBounds = roomsBounds;
        this.delauney = delauney;
    }
}
