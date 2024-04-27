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
        int[] superTriangleVerts = { graphVerts.Length-3, graphVerts.Length-2, graphVerts.Length-1 };
        triangles.Add(superTriangleVerts[0]);
        triangles.Add(superTriangleVerts[1]);
        triangles.Add(superTriangleVerts[2]);

        while(unusedPoints.Count > 0) {
            int pointIndex = Random.Range(0, unusedPoints.Count);
            Vector2 point = unusedPoints[pointIndex];
            unusedPoints.RemoveAt(pointIndex);
            for(int i = 0; i < triangles.Count; i+=3) {
                Circle circumcircle = getCircumcircle(
                    graphVerts[triangles[i]],
                    graphVerts[triangles[i+1]],
                    graphVerts[triangles[i+2]]);
                for(int j = 0; j < 32; j++) {
                    Debug.DrawLine(new Vector2(Mathf.Cos(j*Mathf.PI/16), Mathf.Sin(j*Mathf.PI/16)) * circumcircle.radius + circumcircle.position, new Vector2(Mathf.Cos((j+1)*Mathf.PI/16), Mathf.Sin((j+1)*Mathf.PI/16)) * circumcircle.radius + circumcircle.position, Color.blue, 30f);
                }
                if((point-circumcircle.position).magnitude <= circumcircle.radius) {
                    if(i != 0) {
                        triangles.RemoveRange(i, 3);
                        i-=3;
                    }
                }
            }
            Dictionary<Vector2Int, int> edgesSeenCount = new Dictionary<Vector2Int, int>();
            for(int i = 0; i < triangles.Count; i+=3) {
                for(int j = 0; j < 3; j++) {
                    int a = triangles[i+j];
                    int b = triangles[i+((j+1)%3)];
                    Vector2Int key1 = new Vector2Int(a, b);
                    Vector2Int key2 = new Vector2Int(b, a);
                    if(edgesSeenCount.ContainsKey(key1)) {
                        edgesSeenCount[key1]++;
                    } else if(edgesSeenCount.ContainsKey(key2)) {
                        edgesSeenCount[key2]++;
                    } else {
                        edgesSeenCount.Add(key1, 1);
                    }
                }
            }
            foreach(Vector2Int edge in edgesSeenCount.Keys) {
                if(edgesSeenCount[edge] == 1) {
                    triangles.Add(edge.x);
                    triangles.Add(edge.y);
                    triangles.Add(pointIndex);
                }
            }
        }
        for(int i = 0; i < triangles.Count; i+=3) {
            for(int j = 0; j < 3; j++) {
                int a = triangles[i+j];
                int b = triangles[i+((j+2)%3)];
                if(!graph.hasEdge(a, b)) {
                    graph.addEdge(a, b);
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
        int a = graph.addVertex(new Vector2((minX+maxX)/2-width/2-wp, minY));
        int b = graph.addVertex(new Vector2((minX+maxX)/2, minY+height+hp));
        int c = graph.addVertex(new Vector2((minX+maxX)/2+width/2+wp, minY));
        graph.addEdge(a, b);
        graph.addEdge(b, c);
        graph.addEdge(c, a);
    }
    private static Circle getCircumcircle(Vector2 a, Vector2 b, Vector2 c) {
        float bisectSlopeA = 0f;
        float bisectSlopeB = 0f;
        Vector2 bisectPointA = Vector2.zero;
        Vector2 bisectPointB = Vector2.zero;

        if(a.y - b.y != 0) {
            bisectSlopeA = -(a.x - b.x) / (a.y - b.y);
            bisectPointA = new Vector2((a.x + b.x)/2, (a.y + b.y)/2);
        }
        if(b.y - c.y != 0) {
            bisectSlopeB = -(b.x - c.x) / (b.y - c.y);
            bisectPointB = new Vector2((b.x + c.x)/2, (b.y + c.y)/2);
        }

        float x, y;
        if(a.y - b.y == 0) {
            x = bisectPointA.x;
            y = bisectSlopeB * (x - bisectPointB.x) + bisectPointB.y;
        } else if(b.y - c.y == 0) {
            x = bisectPointB.x;
            y = bisectSlopeA * (x - bisectPointA.x) + bisectPointA.y;
        } else {
            x = (bisectSlopeA*bisectPointA.x - bisectSlopeB*bisectPointB.x + bisectPointB.y - bisectPointA.y)
                    / (bisectSlopeA - bisectSlopeB);
            y = bisectSlopeA * (x - bisectPointA.x) + bisectPointA.y;
        }

        Vector2 circumcenter = new Vector2(x, y);
        if(circumcenter.x.Equals(float.NaN) || circumcenter.y.Equals(float.NaN)) {
            Debug.Log("FAIL: " + a + " | " + b + " | " + c);
        } else {
            Debug.Log("SUCCESS: " + circumcenter);
        }
        float radius = (a-circumcenter).magnitude;

        return new Circle(circumcenter, radius);
    }

    // private static Circle getCircumcircleOLD(Vector2 a, Vector2 b, Vector2 c) {
    //     float t = Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2) - Mathf.Pow(b.x, 2) - Mathf.Pow(b.y, 2);
    //     float u = Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2) - Mathf.Pow(c.x, 2) - Mathf.Pow(c.y, 2);
    //     float J = (a.x-b.x)*(a.y-c.y)-(a.x-c.x)*(a.y-b.y);
        
    //     float x = (-(a.y-b.y)*u+(a.y-c.y)*t)/(2*J);
    //     float y = (-(a.x-b.x)*u+(a.x-c.x)*t)/(2*J);
    //     Vector2 position = new Vector2(x, y);

    //     return new Circle(position, (position-a).magnitude);
    // }

    private struct Circle {

        public Vector2 position;
        public float radius;
        public Circle(Vector2 position, float radius) {
            this.position = position;
            this.radius = radius;
            Debug.Log("POSITION: " + position);
            Debug.Log("RADIUS: " + radius);
        }

    }
}
