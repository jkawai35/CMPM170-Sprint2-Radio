using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //cost on node instead of edge
    public float cost;
    public List<Node> edges;

    public List<Node> GetEdges(){
        return edges;
    }

}