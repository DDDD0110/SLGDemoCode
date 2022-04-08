using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes
{
    public int width;
    public int hight;
    public Node[,] nodes;

    public Vector2Int Origin;

    public GridNodes(int w, int h)
    {
        width = w;
        hight = h;
        nodes = new Node[width, hight];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                nodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < width && y < hight)
            return nodes[x, y];
        else
            return null;
    }

}
