using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delauney : MonoBehaviour
{
    public static Graph Triangulate(Vector2[] points) {
        Graph graph = new Graph(points);
        addSuperTriangle(ref graph);
        List<Vector2> unusedPoints = new List<Vector2>(points);
        List<int> triangles = new List<int>();
        Vector2[] graphVerts = graph.getVertices();
        triangles.Add(graphVerts.Length-3);
        triangles.Add(graphVerts.Length-2);
        triangles.Add(graphVerts.Length-1);

        while(unusedPoints.Count > 0) {
            int pointIndex = Random.Range(0, unusedPoints.Count);
            Vector2 point = unusedPoints[pointIndex];
            unusedPoints.RemoveAt(pointIndex);
            for(int i = 0; i < triangles.Count; i+=3) {
                Circle circumcircle = getCircumcircle(
                    graphVerts[triangles[i]],
                    graphVerts[triangles[i+1]],
                    graphVerts[triangles[i+2]]);
                if((point-circumcircle.position).magnitude <= circumcircle.radius) {
                    Debug.Log("IN THE CIRCLE");
                }
            }
        }

        return graph;
    }

    private static void addSuperTriangle(ref Graph graph) {
        Vector2[] verts = graph.getVertices();
        float minX, minY, maxX, maxY;
        minX = maxX = verts[0].x;
        minY = maxY = verts[0].y;
        for(int i = 1; i < verts.Length; i++) {
            Vector2 vert = verts[i];
            if(vert.x < minX) {
                minX = vert.x;
            } else if(vert.x > maxX) {
                maxX = vert.x;
            }
            if(vert.y < minY) {
                minY = vert.y;
            } else if(vert.y > maxY) {
                maxY = vert.y;
            }
        }
        minX -= 2;
        maxX += 2;
        minY -= 2;
        maxY += 2;
        float width = maxX-minX;
        float height = maxY-minY;
        float triTan = Mathf.Tan(Mathf.PI/3);
        float wp = height/triTan;
        float hp = width*triTan/2;
        int a = graph.addVertex(new Vector2((minX+maxX)/2, minY+height+hp));
        int b = graph.addVertex(new Vector2((minX+maxX)/2-width/2-wp, minY));
        int c = graph.addVertex(new Vector2((minX+maxX)/2+width/2+wp, minY));
        graph.addEdge(a, b);
        graph.addEdge(b, c);
        graph.addEdge(c, a);
    }

    private static Circle getCircumcircle(Vector2 a, Vector2 b, Vector2 c) {
        float t = Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2) - Mathf.Pow(b.x, 2) - Mathf.Pow(b.y, 2);
        float u = Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2) - Mathf.Pow(c.x, 2) - Mathf.Pow(c.y, 2);
        float J = (a.x-b.x)*(a.y-c.y)-(a.x-c.x)*(a.y-b.y);
        
        float x = (-(a.y-b.y)*u+(a.y-c.y)*t)/(2*J);
        float y = (-(a.x-b.x)*u+(a.x-c.x)*t)/(2*J);
        Vector2 position = new Vector2(x, y);

        return new Circle(position, (position-a).magnitude);
    }

    private struct Circle {

        public Vector2 position;
        public float radius;
        public Circle(Vector2 position, float radius) {
            this.position = position;
            this.radius = radius;
        }

    }
}
