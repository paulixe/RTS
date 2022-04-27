using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node:IHeapItem<Node>
{
    public bool Walkable { get; }


    public int X { get; }  // there is no need to get the pos for the program in PathFinding since they are already
                             // stored in the matrix. We created these variables to have a code easier to read
    public int Y { get; }
    public int gCost { get; set; } //distance from the start
    public int hCost { get; set; } // distance from the goal
    public Node parent;
    public int fCost { get { return gCost+hCost; } }


    public Node(int x,int y)
    {
        X = x;
        Y = y;

    }
    public int heapIndex { get; set; }

    public int CompareTo(Node other)
    {
        int compare=fCost.CompareTo(other.fCost);
        if (compare==0)
        {
            compare=hCost.CompareTo(other.hCost);
        }
        return -compare;  // " - " because lower values have higher priority
    }
    public override string ToString()
    {
        return "pos :(" + X + "," + Y + ")"+ " heapindex: "+heapIndex+"  fcost:"+fCost;
    }
}
