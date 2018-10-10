using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

    public List<Node> vertexList;

    public Graph()
    {
        vertexList = new List<Node>(); 
    }

    public Graph(List<Node> v)
    {
        vertexList = v;
    }

    public Node GetNodeById(int id)
    {
        // todo: improve runtime? linear 
        foreach (Node node in vertexList)
        {
            if (node.id == id) return node; 
        }
        return null; 
    }

    public bool InSameComponent(Node s, Node t) 
    {
        // using BFS 
        Queue<Node> nextToVisit = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        nextToVisit.Enqueue(s); 

        while (nextToVisit.Count > 0) 
        {
            Node node = nextToVisit.Dequeue();
            if (node == t) return true;
            if (visited.Contains(node)) continue;
            visited.Add(node);
            foreach (Node neigh in node.neighList)
            {
                nextToVisit.Enqueue(neigh);
            }
        }
        return false; 
    }

    // todo: implement astar 
    public List<Node> AStar() 
    {
        return null; 
    }

    public List<Node> ReconstructPath()
    {
        return null; 
    }
}
