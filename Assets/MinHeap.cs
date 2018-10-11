using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap {

    // Minheap is sorted based on f_cost 

    public Node[] heapArr;
    public int capacity;
    public int heapSize; 
    
    public MinHeap(int capacity)
    {
        if (capacity < 1) 
        { 
            throw new System.Exception("invalid minheap capacity"); 
        }
        this.capacity = capacity;
        heapArr = new Node[capacity];
        heapSize = 0; 
    }

    // get index of parent of node at index i
    int Parent(int i) 
    { 
        return (i - 1) / 2; 
    }

    // get index of left child of node at index i 
    int LeftChild(int i) 
    { 
        return 2 * i + 1; 
    }

    // get index of right child  of node at index i
    int RightChild(int i) 
    { 
        return 2 * i + 2; 
    }

    public bool Contains(Node n) // linear time... goodbye forever
    {
        foreach (Node curr in heapArr)
        {
            if (!(curr == null)) {
                if (curr.Equals(n)) return true;
            }
        }
        return false; 
    }

    // insert new key 
    public void InsertKey(Node n) // seems to be working ! 
    {
        if (heapSize == 0) {
            heapSize++;
            heapArr[0] = n;
        }
        else
        { // 10, 1, 5, 0 --> becomes --> 1, 0, 5, 10 
            heapSize++;
            int i = heapSize - 1;
            heapArr[i] = n;

            // comparing the fcost
            while (i > 0 && (heapArr[Parent(i)].Fcost() > heapArr[i].Fcost()))
            {
                // swap 
                Node temp = heapArr[Parent(i)];
                heapArr[Parent(i)] = heapArr[i];
                heapArr[i] = temp;
                i = Parent(i);
            }
        }
    }

    // to extract the root which is the minimum element 
    public Node ExtractMin()
    {
        // todo: test 
        if (heapSize < 1) return null; 
        if (heapSize == 1)
        {
            heapSize -= 1;
            return heapArr[0];
        }
        else 
        {
            Node root = heapArr[0];
            heapArr[0] = heapArr[heapSize - 1]; // replace root with the largest value
            heapSize -= 1;
            MakeMinheap(0);
            return root; 
        }
    }

    // a recursive method, "heapify" subtree at root i
    void MakeMinheap(int i)
    {
        // todo: test 
        int left = LeftChild(i);
        int right = RightChild(i);
        int smallest = i;
        if (left < heapSize && heapArr[left].Fcost() < heapArr[i].Fcost()
            && right < heapSize && heapArr[right].Fcost() < heapArr[i].Fcost())
        {
            if (heapArr[right].Fcost() < heapArr[left].Fcost()) { smallest = right; }
            else { smallest = left; }
        }
        else {
            if (left < heapSize && heapArr[left].Fcost() < heapArr[i].Fcost())
            {
                smallest = left;
            }
            if (right < heapSize && heapArr[right].Fcost() < heapArr[i].Fcost())
            {
                smallest = right;
            }
        }

        if (smallest != i)
        {
            Node temp = heapArr[i];
            heapArr[i] = heapArr[smallest];
            heapArr[smallest] = temp;

            MakeMinheap(smallest); 
        }
    }

    public void PrintHeap()
    {
        Debug.Log("PRINTING HEAP ARRAY");
        string array = "[ "; 
        for (int i = 0; i < heapSize; i++) 
        {
            Node n = heapArr[i];
            array += n.Fcost() + "  "; 

        }
        array += "]"; 
        Debug.Log(array);
    }
}
