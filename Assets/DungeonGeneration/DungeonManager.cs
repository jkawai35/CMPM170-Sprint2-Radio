using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{
    public int dungeonWidth, dungeonHeight, numRooms, numNonMSTEdges, seed;
    public Tile[] tileset;
    public Texture2D[] roomTemplates;
    public GameObject doorPrefab;
    public GameObject player, monster, goal;

    private Tilemap walkableTilemap, wallTilemap;
    private DungeonResult dungeonResult;

    void Start() {
        foreach(Transform child in transform) {
            switch(child.tag) {
                case "WalkableTilemap":
                walkableTilemap = child.GetComponent<Tilemap>();
                break;
                case "WallsTilemap":
                wallTilemap = child.GetComponent<Tilemap>();
                break;
            }
        }
        seed = Random.Range(423, 140972);
        dungeonResult = DungeonGenerator.GenerateDungeon(dungeonWidth, dungeonHeight, numRooms, roomTemplates, numNonMSTEdges, seed);
        int[] randomNumbers = {Random.Range(0, dungeonResult.rooms.Count), -1, -1};
        for(int i = 1; i < 3; i++) {
            int newRandom = Random.Range(0, dungeonResult.rooms.Count);
            bool valid = true;
            for(int j = 0; j < i; j++) {
                if(randomNumbers[i] == newRandom) {
                    valid = false;
                    break;
                }
            }
            if(valid) {
                randomNumbers[i] = newRandom;
            } else {
                i++;
            }
        }
         Vector2 playerStart = dungeonResult.rooms[randomNumbers[0]].bounds.center;
        Vector2 monsterStart = dungeonResult.rooms[randomNumbers[1]].bounds.center;
        Vector2 goalStart = dungeonResult.rooms[randomNumbers[2]].bounds.center;
        player.transform.position = playerStart - new Vector2(dungeonWidth/2, dungeonHeight/2);
        monster.transform.position = monsterStart - new Vector2(dungeonWidth/2, dungeonHeight/2);
        goal.transform.position = goalStart - new Vector2(dungeonWidth/2, dungeonHeight/2);
        ApplyDungeon(dungeonResult);
    }

    void Update() {
        Vector2[] vertices = dungeonResult.connections.getVertices();
        float[,] adjArray = dungeonResult.connections.getAdjacencyMatrixArray();
        Vector2 offset = new Vector2(-dungeonWidth/2, -dungeonHeight/2);
        for(int y = 0; y < adjArray.GetLength(1); y++) {
            for(int x = 0; x < adjArray.GetLength(0); x++) {
                if(x <= y) continue;
                if(adjArray[x, y] > 0) {
                    Debug.DrawLine(offset + vertices[x], offset + vertices[y], Color.magenta);
                }
            }
        }
    }

    private void ApplyDungeon(DungeonResult dungeonResult) {
        int[,] dungeon = dungeonResult.dungeon;
        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);
        List<Vector3Int> wallPositions = new List<Vector3Int>();
        List<Vector3Int> walkablePositions = new List<Vector3Int>();
        List<Tile> wallTiles = new List<Tile>();
        List<Tile> walkableTiles = new List<Tile>();
        for(int y = 0 ; y < height; y++) {
            for(int x = 0 ; x < width; x++) {
                int dungeonValue = dungeon[x, y];
                Vector3Int position = new Vector3Int(x-width/2, y-height/2, 0);
                Tile tile = tileset[dungeonValue];
                if(dungeonValue == DungeonGenerator.VOID_ID) {
                    continue;
                } else if(dungeonValue == DungeonGenerator.WALL_ID) {
                    wallPositions.Add(position + Vector3Int.forward);
                    wallTiles.Add(tile);
                } else {
                    walkablePositions.Add(position);
                    walkableTiles.Add(tile);
                }
            }
        }
        // foreach(Room room in dungeonResult.rooms) {
        //     foreach(Door door in room.doors) {
        //         Quaternion rotation = Quaternion.identity;
        //         if(door.vertical) {
        //             rotation = Quaternion.Euler(0f, 0f, 90f);
        //         }
        //         Instantiate(doorPrefab, door.position-new Vector2(width/2, height/2), rotation);
        //     }
        // }
        wallTilemap.SetTiles(wallPositions.ToArray(), wallTiles.ToArray());
        walkableTilemap.SetTiles(walkablePositions.ToArray(), walkableTiles.ToArray());
    }
}
