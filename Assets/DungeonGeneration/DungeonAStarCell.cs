using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonAStarCell {
    //g from start
    private int g, h, f;
    private Vector2Int src;
    private bool closed;

    public DungeonAStarCell(int g, int h, Vector2Int src) {
        this.g = g;
        this.h = h;
        this.f = g + h;
        this.src = src;
        closed = false;
    }

    public int getG() {
        return g;
    }
    public int getH() {
        return h;
    }
    public int getF() {
        return f;
    }

    public Vector2Int getSrc() {
         return src;
    }

    public bool isClosed() {
         return closed;
    }

    public void close() {
        closed = true;
    }
}
