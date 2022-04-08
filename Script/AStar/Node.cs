using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable<Node>
{

    public Vector2Int position;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public bool isObstacle = false;
    public bool isBeing = false;
    public Node parentnode;

    public Node(Vector2Int pos)
    {
        position = pos;
        parentnode = null;
    }

    public int CompareTo(Node other)
    {
        int res = fCost.CompareTo(other.fCost);
        if (res == 0)
        {
            res = hCost.CompareTo(other.hCost);
        }
        return res;
    }
}
