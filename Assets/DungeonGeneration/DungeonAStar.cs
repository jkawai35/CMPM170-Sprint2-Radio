using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonAStar {

    private static Vector2Int[] propDirs = new Vector2Int[]{new Vector2Int(0,1), new Vector2Int(0,-1), new Vector2Int(1,0), new Vector2Int(-1,0)};

    public static void addPath(Vector2Int start, Vector2Int end, ref int[,] moveCosts, int turnMultiplier) {
        int width = moveCosts.GetLength(0);
        int height = moveCosts.GetLength(1);

        DungeonAStarCell[,] cells = new DungeonAStarCell[width,height];
        MinimumBinaryHeap<Vector2Int> nextCells = new MinimumBinaryHeap<Vector2Int>(width * height);

        propagate(start, end, cells, moveCosts, nextCells, turnMultiplier);

        while(nextCells.Count > 0) {
            Vector2Int nextCellPos = nextCells.ExtractMin();
            if(closeCell(nextCellPos, end, cells, moveCosts, nextCells, turnMultiplier)) {
                List<Vector2Int> path = new List<Vector2Int>();
                while(nextCellPos != start) {
                    path.Add(nextCellPos);
                    if(moveCosts[nextCellPos.x, nextCellPos.y] == DungeonGenerator.VOID_MOVE_COST) {
                        moveCosts[nextCellPos.x, nextCellPos.y] = DungeonGenerator.FLOOR_MOVE_COST;
                    }
                    nextCellPos = getCell(nextCellPos, cells).getSrc();
                }
                return;
            }
        }
    }

    public static void propagate(Vector2Int pos, Vector2Int end, DungeonAStarCell[,] cells, int[,] moveCosts, MinimumBinaryHeap<Vector2Int> nextCells, int turnMultiplier) {
		int x = pos.x;
		int y = pos.y;
        DungeonAStarCell centerCell = getCell(pos, cells);
        int centerCellG = centerCell == null ? 0 : centerCell.getG();
        foreach(Vector2Int propDir in propDirs) {
            Vector2Int workingCellPos = new Vector2Int(x + propDir.x, y + propDir.y);
            DungeonAStarCell currentCell = getCell(workingCellPos, cells);

            int moveCost = getMoveCost(workingCellPos, moveCosts);
            int moveMultiplier = 1;
            if(centerCell != null) {
                Vector2Int src = centerCell.getSrc();
                Vector2Int movingDirection = new Vector2Int(x - src.x, y - src.y);
                if(propDir != movingDirection) {
                    moveMultiplier = turnMultiplier;
                }
            }

            DungeonAStarCell proposedCell = moveCost < 0 ? null : new DungeonAStarCell(centerCellG+moveCost*moveMultiplier, calculateH(workingCellPos, end), pos);
            if(proposedCell != null) {
                if(currentCell == null || proposedCell.getF() < currentCell.getF()) {
                    setCell(workingCellPos, proposedCell, cells);
                }
                if(currentCell == null) {
                    nextCells.Add(proposedCell.getF(), workingCellPos);
                }
            }
        }
    }

    public static bool closeCell(Vector2Int pos, Vector2Int end, DungeonAStarCell[,] cells, int[,] moveCosts, MinimumBinaryHeap<Vector2Int> nextCells, int turnMultiplier) {
        DungeonAStarCell closingCell = cells[pos.x, pos.y];
        closingCell.close();
        if(closingCell.getH() <= 1) {
            return true;
        }
        propagate(pos, end, cells, moveCosts, nextCells, turnMultiplier);
        return false;
    }

    private static int calculateH(Vector2Int pos, Vector2Int end) {
        return Mathf.Abs(end.x-pos.x)+Mathf.Abs(end.y-pos.y);
    }

    private static DungeonAStarCell getCell(Vector2Int pos, DungeonAStarCell[,] cells) {
        if(pos.x < 0 || pos.x >= cells.GetLength(0) || pos.y < 0 || pos.y >= cells.GetLength(1)) return null;
        return cells[pos.x, pos.y];
    }
    private static void setCell(Vector2Int pos, DungeonAStarCell cell, DungeonAStarCell[,] cells) {
        if(pos.x < 0 || pos.x >= cells.GetLength(0) || pos.y < 0 || pos.y >= cells.GetLength(1)) return;
        cells[pos.x, pos.y] = cell;
    }

    private static int getMoveCost(Vector2Int pos, int[,] moveCosts) {
        if(pos.x < 0 || pos.x >= moveCosts.GetLength(0) || pos.y < 0 || pos.y >= moveCosts.GetLength(1)) return -1;
        return moveCosts[pos.x, pos.y];
    }

    
    public static void addPath(ref int[,] dungeon, Room[] rooms, Graph connections) {
        
    }

}
