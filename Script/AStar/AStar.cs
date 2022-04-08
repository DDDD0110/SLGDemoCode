using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public GridNodes gridNodes;
    private Node startnode;
    private Node targetnode;
    private List<Node> opennodelist;
    private HashSet<Node> closenodelist;
    private Stack<MovementStep> movementSteps;

    public bool FoundPath;
    //调用该方法生成路径
    //start target为grid坐标
    public Stack<MovementStep> BuildPath(Vector2Int start, Vector2Int target)
    {
        if (gridNodes == null)
            Debug.Log("未加载GridNode");
        start -= gridNodes.Origin;
        target -= gridNodes.Origin;
        FoundPath = false;
        movementSteps = new Stack<MovementStep>();
        opennodelist = new List<Node>();
        closenodelist = new HashSet<Node>();
        startnode = new Node(start);
        targetnode = new Node(target);
        if (FindShortPath())
        {
            UpdateMovementStep();
            return movementSteps;
        }
        return null;
    }

    //Astar
    private bool FindShortPath()
    {
        opennodelist.Add(startnode);
        while (opennodelist.Count > 0)
        {
            opennodelist.Sort();
            Node closenode = opennodelist[0];
            opennodelist.RemoveAt(0);
            closenodelist.Add(closenode);
            if (closenode.position == targetnode.position)
            {
                targetnode = closenode;
                FoundPath = true;
                return true;
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Mathf.Abs(i + j) == 1)
                    {
                        Node NeigNode = GetValidNode(closenode.position.x + i, closenode.position.y + j);
                        if (NeigNode != null)
                        {
                            NeigNode.parentnode = closenode;
                            NeigNode.gCost = 1 + closenode.gCost;
                            NeigNode.hCost = GetDistance(targetnode, NeigNode);
                            opennodelist.Add(NeigNode);
                        }
                    }
                }
            }
            
        }
        return false;
    }

    //获得两坐标间距离
    private int GetDistance(Node n1, Node n2)
    {
        return Mathf.Abs(n1.position.x - n2.position.x) + Mathf.Abs(n1.position.y - n2.position.y);
    }

    //判断astar算法中node是否可行
    public Node GetValidNode(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridNodes.width || y >= gridNodes.hight)
            return null;
        Node node = gridNodes.nodes[x, y];
        if (!node.isBeing || node.isObstacle || closenodelist.Contains(node) || opennodelist.Contains(node))
            return null;
        return node;
    }

    //生成指定场景的GridNode,在加载新的场景时调用
    public bool CreateGridNode(string SceneName,out Vector2Int min,out Vector2Int max)
    {
        int width;
        int hight;
        Vector2Int origin;
        min = Vector2Int.zero;
        max = Vector2Int.zero;
        if (MapManager.Instance.GetGridMap(SceneName, out width, out hight, out origin))
        {
            min = origin;
            max = new Vector2Int(origin.x + width - 1, origin.y + hight - 1);
            gridNodes = new GridNodes(width, hight);
            gridNodes.Origin = origin;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < hight; y++)
                {
                    string key = "x" + (x + origin.x) + "y" + (y + origin.y) + SceneName;
                    Tile tile = MapManager.Instance.GetTile(key);
                    if (tile != null)
                    {
                        gridNodes.nodes[x, y] = new Node(new Vector2Int(x, y));
                        gridNodes.nodes[x, y].isBeing = true;
                        gridNodes.nodes[x, y].isObstacle = tile.isObstacle;
                    }
                }
            }
            /* debug
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < hight; y++)
                {
                    Debug.Log(gridNodes.nodes[x, y].isBeing);
                    Debug.Log(gridNodes.nodes[x, y].isObstacle);
                }
            }
            */
            

                    return true;
        }
        return false;
    }

    //完成astar后生成路径
    private void UpdateMovementStep()
    {
        Node nextnode = targetnode;
        while (nextnode != null)
        {
            MovementStep newstep = new MovementStep();
            newstep.GridPosition = new Vector2Int(nextnode.position.x + gridNodes.Origin.x, nextnode.position.y + gridNodes.Origin.y);
            movementSteps.Push(newstep);
            nextnode = nextnode.parentnode;
        }
    }

    //将node设为不可移动
    public void SetNodeObstacle(Vector2Int node, bool obstacle)
    {
        if (gridNodes == null)
            return;
        node -= gridNodes.Origin;
        if (node.x >= 0 && node.x < gridNodes.width && node.y >= 0 && node.y < gridNodes.hight)
            gridNodes.nodes[node.x, node.y].isObstacle = obstacle;
    }

    //TEST
    /*
    public void Test()
    {
        if (gridNodes == null)
            return;
        for (int x = 0; x < gridNodes.width; x++)
        {
            for (int y = 0; y < gridNodes.hight; y++)
            {
                string s = x.ToString() + y.ToString() +"OBS:"+gridNodes.nodes[x, y].isObstacle;
                Debug.Log(s);
                
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Test();
    }
    */



}
