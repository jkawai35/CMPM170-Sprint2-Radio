using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delauney : MonoBehaviour
{
    // Creates a Delauney Triangulation of the points
    // Returns the triangulation as a graph
    public static Graph Triangulate(Vector2[] points) {
        //  Initialize the triangle list with the super triangle
        List<Triangle> triangles = new List<Triangle>();
        Triangle superTriangle = getSuperTriangle(points);
        triangles.Add(superTriangle);

        //  Create the list of unchecked points (starts as the entire point list)
        List<Vector2> uncheckedPoints = new List<Vector2>(points);
        // Check each point
        while(uncheckedPoints.Count > 0) {
            int pointI = Random.Range(0, uncheckedPoints.Count);
            Vector2 point = uncheckedPoints[pointI];
            uncheckedPoints.RemoveAt(pointI);
            List<Triangle> badTriangles = new List<Triangle>();
            for(int i = 1; i < triangles.Count; i++) {
                Triangle triangle = triangles[i];
                Circle circumcircle = triangle.getCircumcircle();
                if((point-circumcircle.position).magnitude <= circumcircle.radius) {
                    badTriangles.Add(triangle);
                }
            }
            foreach(Triangle triangle in badTriangles) {
                triangles.Remove(triangle);
            }
            (Vector2, Vector2)[] unsharedEdges = getUnsharedEdges(triangles.ToArray());
            foreach((Vector2, Vector2) edge in unsharedEdges) {
                Triangle tri = new Triangle(edge.Item1, edge.Item2, point);
                triangles.Add(tri);
            }
        }
        // Convert to graph and remove super triange verts
        Graph graph = trianglesToGraph(triangles.ToArray());
        for(int i = 0; i < 3; i++) {
            graph.removeVertexAt(0);
        }

        return graph;
    }

    // Returns a Triangle that encloses all vetices
    private static Triangle getSuperTriangle(Vector2[] vertices) {
        float minX, minY, maxX, maxY;
        minX = maxX = vertices[0].x;
        minY = maxY = vertices[0].y;
        for(int i = 1; i < vertices.Length; i++) {
            Vector2 vert = vertices[i];
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
        minY -= 100;
        maxY += 2;
        float width = maxX-minX;
        float height = maxY-minY;
        float triTan = Mathf.Tan(Mathf.PI/3);
        float wp = height/triTan;
        float hp = width*triTan/2;
        Vector2 a = Vector2Int.FloorToInt(new Vector2((minX+maxX)/2-width/2-wp, minY)+Vector2.left);
        Vector2 b = Vector2Int.FloorToInt(new Vector2((minX+maxX)/2, minY+height+hp)+Vector2.up);
        Vector2 c = Vector2Int.FloorToInt(new Vector2((minX+maxX)/2+width/2+wp, minY)+Vector2.right);
        return new Triangle(a, b, c);
    }

    // Returns a list of edges not shared by any triangles
    private static (Vector2, Vector2)[] getUnsharedEdges(Triangle[] triangles) {
        List<(Vector2, Vector2)> seenEdges = new List<(Vector2, Vector2)>();
        List<(Vector2, Vector2)> unsharedEdges = new List<(Vector2, Vector2)>();

        foreach(Triangle triangle in triangles) {
            Vector2[] triArray = triangle.getTriArray();
            for(int i = 0; i < 3; i++) {
                Vector2 a = triArray[i];
                Vector2 b = triArray[(i+1)%3];
                (Vector2, Vector2) edge1 = (a, b);
                (Vector2, Vector2) edge2 = (b, a);
                if(seenEdges.Contains(edge1) || seenEdges.Contains(edge2)) {
                    if(unsharedEdges.Contains(edge1)) {
                        unsharedEdges.Remove(edge1);
                    } else if(unsharedEdges.Contains(edge2)) {
                        unsharedEdges.Remove(edge2);
                    }
                } else {
                    seenEdges.Add(edge1);
                    unsharedEdges.Add(edge1);
                }
            }
        }

        return unsharedEdges.ToArray();
    }

    // Converts a list of Triangles to a Graph
    private static Graph trianglesToGraph(Triangle[] triangles) {
        Dictionary<Vector2, int> vertToI = new Dictionary<Vector2, int>();
        List<Vector2> verts = new List<Vector2>();
        List<(int, int)> edges = new List<(int, int)>();
        foreach(Triangle triangle in triangles) {
            Vector2[] triArray = triangle.getTriArray();
            /* Make sure all vertices in this triangle
                Have been added to the verts list */
            for(int i = 0; i < 3; i++) {
                Vector2 vert = triArray[i];
                if(!vertToI.ContainsKey(vert)) {
                    vertToI.Add(vert, verts.Count);
                    verts.Add(vert);
                }
            }
            /* Add all vertices in this triangle
                into the edges dictionary */
            for(int i = 0; i < 3; i++) {
                int a = vertToI[triArray[i]];
                int b = vertToI[triArray[(i+1)%3]];
                if(!edges.Contains((a, b)) && !edges.Contains((b, a))) {
                    edges.Add((a, b));
                }
            }
        }
        Graph graph = new Graph(verts.ToArray());
        foreach((int, int) edge in edges) {
            graph.addEdge(edge.Item1, edge.Item2);
        }
        return graph;
    }
}
public struct Circle {
    public Vector2 position;
    public float radius;
    public Circle(Vector2 position, float radius) {
        this.position = position;
        this.radius = radius;
    }
}
public struct Triangle {
    public Vector2 a;
    public Vector2 b;
    public Vector2 c;
    public Triangle(Vector2 a, Vector2 b, Vector2 c) {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    public Vector2[] getTriArray() {
        return new Vector2[]{ a, b, c };
    }
    public Circle getCircumcircle() {
        Vector2 Mab = (a+b)/2;
        Vector2 Mbc = (b+c)/2;
        Vector2 Mca = (c+a)/2;
        float mab = -(a.x-b.x)/(a.y-b.y);
        float mbc = -(b.x-c.x)/(b.y-c.y);
        float mca = -(c.x-a.x)/(c.y-a.y);
        Vector2 center;
        if(a.y!=b.y && b.y!=c.y) {
            center = getIntersection(Mab, Mbc, mab, mbc);
        } else if(b.y!=c.y && c.y!=a.y) {
            center = getIntersection(Mbc, Mca, mbc, mca);
        } else {
            center = getIntersection(Mca, Mab, mca, mab);
        }
        float radius = (center-a).magnitude;
        return new Circle(center, radius);
    }
    private Vector2 getIntersection(Vector2 pointA, Vector2 pointB, float slopeA, float slopeB) {
        float x = (pointB.y-pointA.y-slopeB*pointB.x+slopeA*pointA.x)/(slopeA-slopeB);
        float y = pointA.y-slopeA*(pointA.x-x);
        return new Vector2(x, y);
    }
    public override string ToString()
    {
        return "A: " + a + " | B: " + b + " | C: " + c;
    }
}
