using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator {
    
    public const int VOID_ID = 0;
    public const int WALL_ID = 1;
    public const int FLOOR_ID = 2;
    public const int DOORWAY_ID = 3;
    public const int TEMP_FLOOR_ID = 4;

    public const int VOID_MOVE_COST = 25;
    public const int WALL_MOVE_COST = -1;
    public const int FLOOR_MOVE_COST = 5;
    public const int DOORWAY_MOVE_COST = -2;

    private const int maxRoomIterations = 20;

    public static DungeonResult GenerateDungeon(int width, int height, int numRooms, Texture2D[] roomTemplates, int numNonMSTEdges, int seed) {
        int[,] dungeon = new int[width, height];
        List<Room> rooms = new List<Room>();
        Random.InitState(seed);

        placeRooms(numRooms, new DungeonData(ref dungeon, roomTemplates, ref rooms));
        Graph roomConnections = createHallways(new DungeonData(ref dungeon, roomTemplates, ref rooms), numNonMSTEdges);
        
        return new DungeonResult(dungeon, rooms, roomConnections);
    }

    private static void placeRooms(int numRooms, DungeonData dungeonData) {
        int i = 0;
        int roomIterations = 0;
        while(i < numRooms) {
            if(addRoom(new DungeonData(ref dungeonData.dungeon, dungeonData.roomTemplates, ref dungeonData.rooms))) {
                i++;
                roomIterations = 0;
            } else {
                roomIterations++;
                if(roomIterations >= maxRoomIterations) {
                    i++;
                }
            }
        }
    }
    private static Graph createHallways(DungeonData dungeonData, int numNonMSTEdges) {
        Vector2[] roomCenters = new Vector2[dungeonData.rooms.Count];
        for(int i = 0; i < roomCenters.Length; i++) {
            roomCenters[i] = dungeonData.rooms[i].bounds.center;
        }
        Graph delauney = Delauney.Triangulate(roomCenters);
        Graph connections = delauney.getMST();
        List<(int, int)> notInMST = new List<(int, int)>(delauney.getEdges());
        foreach((int, int) edge in connections.getEdges()) {
            if(notInMST.Contains(edge)) {
                notInMST.Remove(edge);
            } else if(notInMST.Contains((edge.Item2, edge.Item1))) {
                notInMST.Remove((edge.Item2, edge.Item1));
            }
        }
        for(int i = 0; i < numNonMSTEdges; i++) {
            if(notInMST.Count < 1) break;
            int randI = Random.Range(0, notInMST.Count);
            (int, int) edge = notInMST[randI];
            notInMST.RemoveAt(randI);
            connections.addEdge(edge.Item1, edge.Item2);
        }
        Vector2[] verts = connections.getVertices();
        Room[] rearangedRooms = new Room[dungeonData.rooms.Count];
        for(int i = 0; i < rearangedRooms.Length; i++) {
            for(int j = 0; j < rearangedRooms.Length; j++) {
                if(roomCenters[j] == verts[i]) {
                    rearangedRooms[i] = dungeonData.rooms[j];
                }
            }
        }
        int[,] dungeonTiles = dungeonData.dungeon;
        int[,] moveCosts = new int[dungeonTiles.GetLength(0), dungeonTiles.GetLength(1)];
        for(int y = 0; y < moveCosts.GetLength(1); y++) {
            for(int x = 0; x < moveCosts.GetLength(0); x++) {
                switch(dungeonTiles[x, y]) {
                    case VOID_ID:
                    moveCosts[x, y] = VOID_MOVE_COST;
                    break;
                    case WALL_ID:
                    moveCosts[x, y] = WALL_MOVE_COST;
                    break;
                    case FLOOR_ID:
                    moveCosts[x, y] = FLOOR_MOVE_COST;
                    break;
                    case DOORWAY_ID:
                    moveCosts[x, y] = DOORWAY_MOVE_COST;
                    break;
                }
            }
        }
        (int, int)[] edges = connections.getEdges();
        foreach((int, int) edge in edges) {
            Room roomA = rearangedRooms[edge.Item1];
            Room roomB = rearangedRooms[edge.Item2];

            Vector2Int doorACellPos = Vector2Int.FloorToInt(roomA.doors[0].position);
            Vector2Int doorBCellPos = Vector2Int.FloorToInt(roomB.doors[0].position);
            (Vector2Int, Vector2Int) closestPair = (doorACellPos, doorBCellPos);
            float closestPairSqrLength = (closestPair.Item1-closestPair.Item2).sqrMagnitude;
            for(int i = 0; i < roomB.doors.Count; i++) {
                for(int j = 0; j < roomA.doors.Count; j++) {
                    doorACellPos = Vector2Int.FloorToInt(roomA.doors[j].position);
                    doorBCellPos = Vector2Int.FloorToInt(roomB.doors[i].position);
                    float sqrLen = (doorACellPos-doorBCellPos).sqrMagnitude;
                    if(sqrLen < closestPairSqrLength) {
                        closestPairSqrLength = sqrLen;
                        closestPair = (doorACellPos, doorBCellPos);
                    }
                }
            }
            DungeonAStar.addPath(closestPair.Item1, closestPair.Item2, ref moveCosts, 3);
        }
        for(int y = 0; y < moveCosts.GetLength(1); y++) {
            for(int x = 0; x < moveCosts.GetLength(0); x++) {
                switch(moveCosts[x, y]) {
                    case VOID_MOVE_COST:
                        dungeonTiles[x, y] = VOID_ID;
                        break;
                    case WALL_MOVE_COST:
                        dungeonTiles[x, y] = WALL_ID;
                        break;
                    case FLOOR_MOVE_COST:
                        dungeonTiles[x, y] = FLOOR_ID;
                        break;
                    case DOORWAY_MOVE_COST:
                        dungeonTiles[x, y] = DOORWAY_ID;
                        break;
                }
            }
        }
        Vector2Int[] expandDirections = {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };
        for(int y = 0; y < dungeonTiles.GetLength(1); y++) {
            for(int x = 0; x < dungeonTiles.GetLength(0); x++) {
                int tile = dungeonTiles[x, y];
                if(tile == FLOOR_ID) {
                    foreach(Vector2Int expandDir in expandDirections) {
                        if(dungeonTiles[x+expandDir.x, y+expandDir.y]==VOID_ID) {
                            dungeonTiles[x+expandDir.x, y+expandDir.y] = TEMP_FLOOR_ID;
                        }
                    }
                }
            }
        }
        for(int y = 0; y < dungeonTiles.GetLength(1); y++) {
            for(int x = 0; x < dungeonTiles.GetLength(0); x++) {
                int tile = dungeonTiles[x, y];
                if(tile == TEMP_FLOOR_ID) {
                    dungeonTiles[x, y] = FLOOR_ID;
                    foreach(Vector2Int expandDir in expandDirections) {
                        if(dungeonTiles[x+expandDir.x, y+expandDir.y]==VOID_ID) {
                            dungeonTiles[x+expandDir.x, y+expandDir.y] = WALL_ID;
                        }
                    }
                }
            }
        }

        return connections;
    }

    private static bool addRoom(DungeonData dungeonData) {
        int roomIndex = Random.Range(0, dungeonData.roomTemplates.Length);
        Texture2D roomTemplate = dungeonData.roomTemplates[roomIndex];
        Color[] pixels = roomTemplate.GetPixels();
        int roomWidth = roomTemplate.width;
        int roomHeight = roomTemplate.height;
        int minX = Random.Range(4, dungeonData.dungeon.GetLength(0)-roomWidth-4);
        int minY = Random.Range(4, dungeonData.dungeon.GetLength(1)-roomHeight-4);
        RectInt roomBounds = new RectInt(minX, minY, roomWidth, roomHeight);
        bool valid = true;
        foreach(Room otherRoom in dungeonData.rooms) {
            if(roomBounds.Overlaps(otherRoom.bounds)) {
                valid = false;
                break;
            }
        }
        if(valid) {
            List<Door> doors = new List<Door>();
            List<Vector2> doorPositions = new List<Vector2>();
            for(int y = 0 ; y < roomHeight; y++) {
                for(int x = 0 ; x < roomWidth; x++) {
                    if(pixels[y*roomWidth + x].Equals(Color.black)) {
                        dungeonData.dungeon[x+minX, y+minY] = WALL_ID;
                    } else if(pixels[y*roomWidth + x].Equals(Color.white)) {
                        dungeonData.dungeon[x+minX, y+minY] = FLOOR_ID;
                    } else if(pixels[y*roomWidth + x].Equals(Color.red)) {
                        Vector2 doorPosition;
                        bool vertical = false;
                        if(pixels[y*roomWidth + x + 1].Equals(Color.red)) {
                            doorPosition = new Vector2(x+1f, y+0.5f);
                        } else if(pixels[y*roomWidth + x - 1].Equals(Color.red)) {
                            doorPosition = new Vector2(x, y+0.5f);
                        } else if(pixels[(y+1)*roomWidth + x].Equals(Color.red)) {
                            doorPosition = new Vector2(x+0.5f, y+1f);
                            vertical = true;
                        } else {
                            doorPosition = new Vector2(x+0.5f, y);
                            vertical = true;
                        }
                        if(!doorPositions.Contains(doorPosition)) {
                            doorPositions.Add(doorPosition);
                            doors.Add(new Door(new Vector2(minX, minY) + doorPosition, vertical));
                        }
                        dungeonData.dungeon[x+minX, y+minY] = DOORWAY_ID;
                    }
                }
            }
            dungeonData.rooms.Add(new Room(roomBounds, doors));
        }
        return valid;
    }

    private struct DungeonData {
        public int[,] dungeon;
        public Texture2D[] roomTemplates;
        public List<Room> rooms;
        public DungeonData(ref int[,] dungeon, Texture2D[] roomTemplates, ref List<Room> rooms) {
            this.dungeon = dungeon;
            this.roomTemplates = roomTemplates;
            this.rooms = rooms;
        }
    }

}
public struct Room {
    public RectInt bounds;
    public List<Door> doors;
    public Room(RectInt bounds, List<Door> doors) {
        this.bounds = bounds;
        this.doors = doors;
    }
}
public struct Door {
    public Vector2 position;
    public bool vertical;
    public Door(Vector2 position, bool vertical = false) {
        this.position = position;
        this.vertical = vertical;
    }
}
public struct DungeonResult {
    public int[,] dungeon;
    public List<Room> rooms;
    public Graph connections;
    public DungeonResult(int[,] dungeon, List<Room> rooms, Graph connections) {
        this.dungeon = dungeon;
        this.rooms = rooms;
        this.connections = connections;
    }
}
