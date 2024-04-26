using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{

    public Tilemap tilemap;
    public Tile[] tiles;
    public Texture[] roomTemplates;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();

        DungeonTiles[,] dungeon = DungeonGenerator.GenerateDungeon(40, 40);

        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                tilemap.SetTile(new Vector3Int(x-width/2, y-height/2, 0), tiles[(int)dungeon[x, y]]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
