using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

    private AdjacencyMatrix adjacencyMatrix;
    private List<Vector2> vertices;

    public Graph() {
        vertices = new List<Vector2>();
        adjacencyMatrix = new AdjacencyMatrix();
    }
    public Graph(Vector2[] verts) {
        vertices = new List<Vector2>();
        adjacencyMatrix = new AdjacencyMatrix();
        foreach(Vector2 vert in verts) {
            addVertex(vert);
        }
    }

    public int addVertex(Vector2 vertex) {
        vertices.Add(vertex);
        adjacencyMatrix.addVertex();
        return vertices.Count-1;
    }
    public void removeVertexAt(int index) {
        vertices.RemoveAt(index);
        adjacencyMatrix.removeVertexAt(index);
    }

    public void addEdge(int a, int b) {
        Vector2 vecA = vertices[a];
        Vector2 vecB = vertices[b];
        float dist = (vecA-vecB).magnitude;
        adjacencyMatrix.addAdjacency(a, b, dist);
    }
    public void removeEdge(int a, int b) {
        adjacencyMatrix.clearAdjacency(a, b);
    }
    public bool hasEdge(int a, int b) {
        return adjacencyMatrix.hasAdjacency(a, b);
    }
    public Vector2Int[] getEdges() {
        float[,] adjMat = getAdjacencyMatrixArray();
        List<Vector2Int> edges = new List<Vector2Int>();
        for(int j = 0; j < adjMat.GetLength(1); j++) {
            for(int i = 0; i < adjMat.GetLength(0); i++) {
                if(i <= j) continue;
                if(adjMat[i, j] > 0) {
                    edges.Add(new Vector2Int(i, j));
                }
            }
        }
        return edges.ToArray();
    }

    public Vector2[] getVertices() {
        return vertices.ToArray();
    }
    public float[,] getAdjacencyMatrixArray() {
        return adjacencyMatrix.ToArray();
    }

}

public class AdjacencyMatrix {

    private List<List<float>> matrix;
    private int numVerts;

    public AdjacencyMatrix() {
        matrix = new List<List<float>>();
    }

    public void addVertex() {
        foreach(List<float> colum in matrix) {
            colum.Add(-1f);
        }
        numVerts++;
        matrix.Add(new List<float>());
        for(int i = 0; i < numVerts; i++) {
            matrix[numVerts-1].Add(-1f);
        }
    }
    public void removeVertexAt(int index) {
        matrix.RemoveAt(index);
        foreach(List<float> colum in matrix) {
            colum.RemoveAt(index);
        }
        numVerts--;
    }
    public void addAdjacency(int a, int b, float dist) {
        matrix[a][b] = matrix[b][a] = dist;
    }
    public void clearAdjacency(int a, int b) {
        matrix[a][b] = matrix[b][a] = -1f;
    }
    public bool hasAdjacency(int a, int b) {
        return matrix[a][b] > 0;
    }
    public float[,] ToArray() {
        float[,] array = new float[numVerts, numVerts];
        for(int j = 0; j < numVerts; j++) {
            for(int i = 0; i < numVerts; i++) {
                array[i, j] = matrix[i][j];
            }
        }
        return array;
    }

}