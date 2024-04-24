using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    //cost on node instead of edge
    public float cost;
    public List<Node> edges = new List<Node>();

    public Vector3Int pos;

    public Node(float nodeCost, Vector3Int nodePos){
        cost = nodeCost;
        pos = nodePos;
    }

}