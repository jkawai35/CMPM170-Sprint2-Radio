using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
//Used https://www.redblobgames.com/pathfinding/a-star/introduction.html as a tutorial
public class Pathfinder : MonoBehaviour
{
    public static Pathfinder _instance;
    public static Pathfinder Instance {get{return _instance;}}
    private void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }
        else{
            _instance =  this;
        }
    }

    Node start;
    Node end;

    Dictionary<Node,Node> cameFrom;

    public Node CalculateNextNodeInput(Node startNode, Node endNode){
        start = startNode;
        end = endNode;
        
        Node next = CalculateNextNode();
        //PrintShortestPath();
        return next;
    }

    Node CalculateNextNode(){
        CalculateShortestPath();
        return GetNextNode();
    }

    void CalculateShortestPath(){
        PriorityQueue<Node,float> pq = new PriorityQueue<Node,float>();
        pq.Enqueue(start,0f);
        cameFrom = new Dictionary<Node,Node>();
        Dictionary<Node,float> costSoFar = new Dictionary<Node,float>();
        cameFrom[start]=null;
        costSoFar[start] = 0f;

        while(pq.Count>0){
            Node current = pq.Dequeue();
            if (current == end){
                break;
            }
            foreach (Node next in current.GetEdges()){
                float newCost = costSoFar[current] + next.cost;
                if(!costSoFar.ContainsKey(next)||newCost<costSoFar[next]){
                    costSoFar[next] = newCost;
                    pq.Enqueue(next,newCost+CalculateHeuristic(end,next));
                    cameFrom[next]=current;
                }
            }

        }

    }

    float CalculateHeuristic(Node a, Node b){
        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;
        return Mathf.Abs(aPos.x-bPos.x)+Mathf.Abs(aPos.y-bPos.y);
    }

    public Node GetNextNode(){
        Node current = end;
        Node next = end;
        while(next!=start){
            next = cameFrom[current];
            if(next!=start){
                current = next;
            }
        }
        return current;
    }

    void PrintShortestPath(){
        Node current = end;
        List<Node> nodeList = new List<Node>();
        while(current!=null){
            nodeList.Add(current);
            current = cameFrom[current];
        }
        nodeList.Reverse();
        foreach (Node node in nodeList){
            Debug.Log(node);
        }
    }
}
