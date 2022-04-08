using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessManager : Singleton<ChessManager>
{
    private AStar aStar;
    private Stack<MovementStep> movementSteps = new Stack<MovementStep>();
    private Vector2Int min;
    private Vector2Int max;
    private Vector2Int skillmin;
    private Vector2Int skillmax;
    private Forward skillforward = Forward.UP;
    private Dictionary<Forward, List<Vector2Int>> SkillScopeMap = new Dictionary<Forward, List<Vector2Int>>();
    private Vector2Int currentChessPos;
    private bool skillscope = false;

    public List<GameObject> PlayerChess = new List<GameObject>();
    public List<GameObject> EnemyChess = new List<GameObject>();

    public float chessspeed;
    public int moveradius;
    public Tilemap StepMap;
    public TileBase steptile;
    public TileBase attacktile;

    protected override void Awake()
    {
        base.Awake();
        aStar = GetComponent<AStar>();
    }

    private void Start()
    {
        foreach (var chess in PlayerChess)
            chess.SetActive(true);
        foreach (var chess in EnemyChess)
            chess.SetActive(true);
        InitalSkillMap();
    }
    private void Update()
    {
        if (skillscope)
            UpdateSkillScope();
        if (skillscope && Input.GetMouseButtonDown(0))
        {
            skillscope = false;
            ClearScopeTile();
            Vector3 f = new Vector3(0, 0, 1f);
            switch (skillforward)
            {
                case Forward.UP:
                    f = new Vector3(0, 0, 1f);
                    break;
                case Forward.DOWN:
                    f = new Vector3(0, 0, -1f);
                    break;
                case Forward.RIGHT:
                    f = new Vector3(1f, 0, 0);
                    break;
                case Forward.LEFT:
                    f = new Vector3(-1f, 0, 0);
                    break;
            }
            TurnManager.Instance.ActivateSkill(SkillScopeMap[skillforward][0], SkillScopeMap[skillforward][1], f);
        }

    }

    //ASTAR����·��
    public void ChessMove(Vector2Int startpos, Vector2Int targetpos,out List<Vector3> paths)
    {
        ClearScopeTile();
        //����·��
        movementSteps = aStar.BuildPath(startpos, targetpos);
        if (movementSteps == null)
        {
            Debug.Log("����·��ʧ��");
            paths = null;
            TurnManager.Instance.ChessMoveFail();
            return ;
        }
        List<Vector3> steps = new List<Vector3>();
        foreach (MovementStep step in movementSteps)
        {
            steps.Add(step.ChessPosition);
        }
        paths = steps;
    }
    //����Atar�õ�Nodes,�����곡����͵���
    public void GreateAStarGrid(string sceneName)
    {
        if (!aStar.CreateGridNode(sceneName, out min, out max))
            Debug.Log("����GridNodeʧ��");
    }

    //�޸�Nodes��node������
    public void SetNodeObstacle(Vector2Int startnode,Vector2Int targetnode)
    {
            aStar.SetNodeObstacle(startnode, false);
            aStar.SetNodeObstacle(targetnode, true);
    }
    public void SetNodeObstacle(Vector2Int pos, bool obstacle)
    {
        aStar.SetNodeObstacle(pos, obstacle);
    }

    //��ʾ���ƶ��ĸ���
    public void ShowScopeTile(Vector2Int center)
    {
        currentChessPos = center;
        for (int x = -moveradius; x <= moveradius; x++)
        {
            for (int y = -moveradius; y <= moveradius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= moveradius)
                {
                    Vector2Int pos = new Vector2Int(center.x + x, center.y + y);
                    if (ValidPos(pos))
                    {
                        StepMap.SetTile(new Vector3Int(pos.x,pos.y, 0), steptile);
                    }
                }

            }
        }
    }
    //��ʾ�ɹ����ĸ���
    public void ShowAttackScope(Vector2Int scopemin, Vector2Int scopemax)
    {
        for (int x = scopemin.x; x <= scopemax.x; x++)
        {
            for (int y = scopemin.y; y <= scopemax.y; y++)
            {
                if (x >= min.x && y >= min.y && x <= max.x && y <= max.y)
                    StepMap.SetTile(new Vector3Int(x, y, 0), attacktile);
            }
        }
    }
    //��ʾSkill��Χ
    public void ShowSkillScope(Vector2Int minscope,Vector2Int maxscope,Vector2Int chesspos)
    {
        skillscope = true;
        currentChessPos = chesspos;
        skillmin = minscope;
        skillmax = maxscope;
        UpdateSkillMap();
    }
    //����skill��Χ
    private void UpdateSkillScope()
    {
        var temp = MouseManager.Instance.ChooseSkillForward(currentChessPos);
        if (temp == skillforward)
            return;
        skillforward = temp;
        ClearScopeTile();
        ShowAttackScope(SkillScopeMap[skillforward][0], SkillScopeMap[skillforward][1]);
    }
    //�����ʾ�ĸ���
    public void ClearScopeTile()
    {
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                StepMap.SetTile(new Vector3Int(x,y, 0), null);
            }
        }
    }

    //�ж�pos�ĸ����Ƿ�����Ϊ�ƶ�Ŀ��
    public bool ValidPos(Vector2Int pos)
    {
        if (pos.x >= min.x && pos.y >= min.y && pos.x <= max.x && pos.y <= max.y)
        {
            if (aStar.gridNodes.nodes[pos.x - min.x, pos.y - min.y].isBeing &&
                !aStar.gridNodes.nodes[pos.x - min.x, pos.y - min.y].isObstacle)
                return true;
        }
        return false;
    }
    //��¼skill�ɹ����ķ�Χ
    private void InitalSkillMap()
    {
        SkillScopeMap.Add(Forward.UP, new List<Vector2Int>());
        SkillScopeMap.Add(Forward.DOWN, new List<Vector2Int>());
        SkillScopeMap.Add(Forward.RIGHT, new List<Vector2Int>());
        SkillScopeMap.Add(Forward.LEFT, new List<Vector2Int>());
    }
    private void UpdateSkillMap()
    {
        foreach (var value in SkillScopeMap.Values)
            value.Clear();
        Vector2Int tmin = Vector2Int.zero;
        Vector2Int tmax = Vector2Int.zero;

        //UP
        tmin = currentChessPos + skillmin;
        tmax = currentChessPos + skillmax;
        SkillScopeMap[Forward.UP].Add(tmin);
        SkillScopeMap[Forward.UP].Add(tmax);
        //DOWN
        tmin = currentChessPos - skillmax;
        tmax = currentChessPos - skillmin;
        SkillScopeMap[Forward.DOWN].Add(tmin);
        SkillScopeMap[Forward.DOWN].Add(tmax);
        //RIGHT
        tmin = currentChessPos + new Vector2Int(skillmin.y, skillmin.x);
        tmax = currentChessPos + new Vector2Int(skillmax.y, skillmax.x);
        SkillScopeMap[Forward.RIGHT].Add(tmin);
        SkillScopeMap[Forward.RIGHT].Add(tmax);
        //LEFT
        tmin = currentChessPos - new Vector2Int(skillmax.y, skillmax.x);
        tmax = currentChessPos - new Vector2Int(skillmin.y, skillmin.x);
        SkillScopeMap[Forward.LEFT].Add(tmin);
        SkillScopeMap[Forward.LEFT].Add(tmax);

    }



    //�жϸ÷����Ƿ�ɴ�
    public bool Isreachable(Vector2Int target)
    {
        if (Mathf.Abs(target.x - currentChessPos.x) + Mathf.Abs(target.y - currentChessPos.y) <= moveradius)
            if (ValidPos(target))
            {
                var step = aStar.BuildPath(currentChessPos, target);
                if (step.Count != 0 && step.Count <= 2 * moveradius)
                    return true;
            }
        return false;
    }
    public void EndAction()
    {
        skillscope = false;
    }


}

public enum Forward { UP,DOWN,LEFT,RIGHT};
