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
    public List<Node> AStar(Node s, Node t) 
    {
        // call InSameComponent() to first to find ccs
        if (!InSameComponent(s, t)) 
        {
            return new List<Node>();
        }

        // need a minheap to evaluate the node with lowest cost ! 
        MinHeap open = new MinHeap(2000); // todo don't hardcode this? 
        open.InsertKey(s); 
        HashSet<Node> closed = new HashSet<Node>(); 

        while (open.heapSize > 0) 
        {
            Node current = open.ExtractMin();
            closed.Add(current);

            if (current.Equals(t)) // path has been found
            {
                return ReconstructPath(s,t); 
            }

            foreach (Node neighbor in current.neighList)
            {
                if (closed.Contains(neighbor)) 
                {
                    // todo: skip to the next neighbor
                    continue; 
                }
                
                // if new path to neighbor is shorter, or neighbor is not in open,
                // set f_cost of neighbor
                float newMvmtCostToNeigh = current.gCost + Vector3.Distance(current.position, neighbor.position); 
                if (newMvmtCostToNeigh < neighbor.gCost || !open.Contains(neighbor))
                {

                    neighbor.gCost = newMvmtCostToNeigh;
                    neighbor.hCost = Vector3.Distance(neighbor.position, t.position);
                    // set parent of neighbor to current node
                    neighbor.comesFrom = current; 

                    if (!open.Contains(neighbor))
                    {
                        open.InsertKey(neighbor); 
                    }
                }
            }
        }
        return null; 
    }

    public List<Node> ReconstructPath(Node s, Node t)
    {
        List<Node> path = new List<Node>();
        path.Add(t); 
        while (!t.Equals(s)) 
        {
            //path.Add(n.comesFrom), could do path.reverse 
            path.Insert(0, t.comesFrom);
            t = t.comesFrom; 
        }
        return path; 
    }
}
