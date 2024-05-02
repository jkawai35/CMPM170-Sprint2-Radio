using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour
{
    public Tilemap walkable;
    public GameObject player;
    public GameObject monster;
    public GameObject end;



    public int monsterPathLength=0;
    

    public static GridGenerator _instance;
    public static GridGenerator Instance {get{return _instance;}}
    private void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }
        else{
            _instance =  this;
        }
    }

    public Tilemap GetWalkableMap(){
        return walkable;
    }

    void Start(){
        GenerateGrid();
        StartCoroutine(RadioInstruction(5f));
    }

    IEnumerator RadioInstruction(float delay){
        Pathfinder pathfinder = new Pathfinder();
        while(true){
            Node startNode = nodes[walkable.WorldToCell(player.transform.position)];
            Node nextNode = startNode;
            Node endNode = nodes[walkable.WorldToCell(end.transform.position)];
            if(nextNode!=endNode){
                nextNode = pathfinder.CalculateNextNodeInput(nextNode,endNode);
            }

            Vector3Int direction = nextNode.pos-startNode.pos;
            if(direction==Vector3Int.up){
                Debug.Log("move up");
                Radio.Instance.RadioDirection(0);
            }
            else if(direction==Vector3Int.down){
                Debug.Log("move down");
                Radio.Instance.RadioDirection(1);
            }
            else if(direction==Vector3Int.left){
                Debug.Log("move left");
                Radio.Instance.RadioDirection(2);
            }
            else if(direction==Vector3Int.right){
                Debug.Log("move right");
                Radio.Instance.RadioDirection(3);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    public Vector2 MonsterInstructions(){
        Pathfinder pathfinder = new Pathfinder();
        while(true){
            Node startNode = nodes[walkable.WorldToCell(monster.transform.position)];
            Node nextNode = startNode;
            Node endNode = nodes[walkable.WorldToCell(player.transform.position)];
            if(nextNode!=endNode){
                nextNode = pathfinder.CalculateNextNodeInput(nextNode,endNode);
            }
            monsterPathLength = pathfinder.GetPathLength();
            return walkable.GetCellCenterWorld(nextNode.pos);
        }
    }

    public Dictionary<Vector3Int,Node> nodes = new Dictionary<Vector3Int, Node>();

    public void GenerateGrid(){
        nodes.Clear();
        Debug.Log("Generating Grid");
        walkable.CompressBounds();
        BoundsInt bounds = walkable.cellBounds;
        for(int x = bounds.min.x; x< bounds.max.x; x++){
            for(int y = bounds.min.y; y<bounds.max.y; y++){
                Vector3Int currentPos = new Vector3Int(x,y,0);
                if(walkable.HasTile(currentPos)){
                    Node node = new Node(1,currentPos);
                    nodes.Add(currentPos,node);
                }
            }
        }
        foreach(Node n in nodes.Values){
            findNeighbors(n);
        }

    }

    void findNeighbors(Node n){
        Vector3Int currentPos = n.pos;
        //check above
        Vector3Int abovePos = currentPos + Vector3Int.up;
        if(nodes.ContainsKey(abovePos)){
            n.edges.Add(nodes[abovePos]);
        }
        //check below
        Vector3Int belowPos = currentPos + Vector3Int.down;
        if(nodes.ContainsKey(belowPos)){
            n.edges.Add(nodes[belowPos]);
        }
        //check left
        Vector3Int leftPos = currentPos + Vector3Int.left;
        if(nodes.ContainsKey(leftPos)){
            n.edges.Add(nodes[leftPos]);
        }
        //check right
        Vector3Int rightPos = currentPos + Vector3Int.right;
        if(nodes.ContainsKey(rightPos)){
            n.edges.Add(nodes[rightPos]);
        }
    }
}