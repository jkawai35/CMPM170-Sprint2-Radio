using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{
    public int dungeonWidth, dungeonHeight, numRooms;
    public Tile[] tileset;
    public Texture2D[] roomTemplates;

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
        dungeonResult = DungeonGenerator.GenerateDungeon(dungeonWidth, dungeonHeight, numRooms, roomTemplates);
        ApplyDungeon(dungeonResult.dungeon);
    }

    void Update() {
        Vector2[] vertices = dungeonResult.delauney.getVertices();
        float[,] adjArray = dungeonResult.delauney.getAdjacencyMatrixArray();
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

    private void ApplyDungeon(int[,] dungeon) {
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
        wallTilemap.SetTiles(wallPositions.ToArray(), wallTiles.ToArray());
        walkableTilemap.SetTiles(walkablePositions.ToArray(), walkableTiles.ToArray());
    }
}
