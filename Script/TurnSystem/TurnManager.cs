using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    public TurnState CurrentState;
    public GameObject PlayerTurnTile;
    public GameObject EnemyTurnTile;
    public Animator ActionPanel;
    public GameObject ChoosePanel;
    public GameObject SkillPanel;
    public TurnArrow arrow;
    public GameObject Log;
    public WinLostPanel winlostPanel;
    private List<BaseChessControl> PlayerList = new List<BaseChessControl>();
    private List<BaseChessControl> EnemyList = new List<BaseChessControl>();
    public Dictionary<int, List<BaseChessControl>> PlayerDistribution;
    private int playerindex;
    private int enemyindex;
    private bool skillusing;
    public bool action = false;
    public int healpoint;



    private void Start()
    {
        CurrentState = TurnState.Start;
        StartTurn();
    }

    void StartTurn()
    {
        playerindex = 0;
        enemyindex = 0;
        PlayerDistribution = new Dictionary<int, List<BaseChessControl>>();
   
    }

    IEnumerator TurnToPlayer()
    {
        PlayerTurnTile.SetActive(true);
        yield return new WaitForSeconds(3f);
        PlayerTurnTile.SetActive(false);
        ActionPanel.SetBool("isopen", true);
        foreach (var player in PlayerList)
        {
            for (int i = 0; i < player.characterDate.skill.Count; i++)
                player.characterDate.skill[i].currentcd++;
        }
        action = false;
        CurrentState = TurnState.PlayerTurn;
        UpdateArrow();
        
    }

    IEnumerator TurnToEnemy()
    {
        ActionPanel.SetBool("isopen", false);
        EnemyTurnTile.SetActive(true);
        yield return new WaitForSeconds(3f);
        EnemyTurnTile.SetActive(false);
        action = false;
        CurrentState = TurnState.EnemyTurn;
        UpdateArrow();
        

    }
    private void Win()
    {
        CurrentState = TurnState.Win;
        winlostPanel.iswin = true;
        winlostPanel.gameObject.SetActive(true);
        GameManager.Instance.UnlockNextLevel();
        Debug.Log("Win");
    }

    private void Lost()
    {
        CurrentState = TurnState.Lost;
        winlostPanel.iswin = false;
        winlostPanel.gameObject.SetActive(true);
        Debug.Log("Lost");
    }

    //ע�����
    public void PlayerRegister(BaseChessControl player)
    {
        PlayerList.Add(player);
    }
    public void PlayerDis(BaseChessControl player)
    {
        PlayerList.Remove(player);
        DistributeTargetForEnemy();
        WinLostCheck();
    }
    //ע�����
    public void EnemyRegister(BaseChessControl enemy)
    {
        EnemyList.Add(enemy);
    }
    public void EnemyDis(BaseChessControl enemy)
    {
        EnemyList.Remove(enemy);
        foreach (var v in PlayerDistribution.Values)
            if (v.Contains(enemy))
                v.Remove(enemy);
        WinLostCheck();
    }
    //���move��ťʱ����
    public void PlayerMove()
    {
        AudioManager.Instance.ButtonPlay();
        if (CurrentState != TurnState.PlayerTurn || action)
            return;
        action = true;
        ChessManager.Instance.ShowScopeTile(PlayerList[playerindex].currentPos);
        MouseManager.Instance.StartChooseTarget();
    }
    //�����ƶ���Ŀ��λ�ã��Ƿ��ǵ�������
    public void ChessMove(Vector2Int target,bool isenemy = false)
    {
        if (isenemy)
        {
            if (EnemyList.Count == 0 || enemyindex >= EnemyList.Count)
            {
                Debug.Log("EnemyList����");
                return;
            }
            EnemyList[enemyindex].targetPos = target;
            if (!EnemyList[enemyindex].Move())
                ActionEnd();
            enemyindex++;
        }
        else
        {
            if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
            {
                Debug.Log("PlayerList����");
                return;
            }
            PlayerList[playerindex].targetPos = target;
            if (PlayerList[playerindex].Move())
                playerindex++;
            else
            {
                ActionEnd();
            }
        }
    }
    //��ǰ�����ж�����
    public void ActionEnd()
    {
        action = false;
        skillusing = false;
        if (CurrentState == TurnState.PlayerTurn)
            ChoosePanel.SetActive(true);
        CloseSkillPanel();
        UpdateArrow();
    }
    private void ReturnActionPanel()
    {
        if (CurrentState != TurnState.PlayerTurn || !action)
            return;
        action = false;
        skillusing = false;
        CloseSkillPanel();
        ChoosePanel.SetActive(true);
        MouseManager.Instance.EndChoose();
        ChessManager.Instance.ClearScopeTile();
        ChessManager.Instance.EndAction();
    }
    public void UpdateArrow()
    {
        switch (CurrentState)
        {
            case TurnState.PlayerTurn:
                if (playerindex < PlayerList.Count)
                    arrow.target = PlayerList[playerindex].transform;
                else
                    arrow.target = null;
                break;
            case TurnState.EnemyTurn:
                if (enemyindex < EnemyList.Count)
                    arrow.target = EnemyList[enemyindex].transform;
                else
                    arrow.target = null;
                break;
            default:
                arrow.target = null;
                break;

        }
    }
    //���attack��ťʱ����
    public void PlayerAttack()
    {
        AudioManager.Instance.ButtonPlay();
        if (CurrentState != TurnState.PlayerTurn || action)
            return;
        action = true;
        MouseManager.Instance.StartChooseAttack();
        if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
        {
            Debug.Log("PlayerList����");
            return;
        }
        Vector2Int min = PlayerList[playerindex].currentPos - Vector2Int.one;
        Vector2Int max = PlayerList[playerindex].currentPos + Vector2Int.one;
        ChessManager.Instance.ShowAttackScope(min, max);
    }
    public void ChessAttack(Transform target, bool isenemy = false)
    {
        if (isenemy)
        {
            if (EnemyList.Count == 0 || enemyindex >= EnemyList.Count)
            {
                Debug.Log("EnemyList����");
                return;
            }
            EnemyList[enemyindex].Attack(target);
            enemyindex++;
        }
        else
        {
            if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
            {
                Debug.Log("PlayerList����");
                return;
            }
            PlayerList[playerindex].Attack(target);
            playerindex++;
        }
    }
    //ѡ��ʹ��skill����ã���ʾSKILLPANEL
    public void PlayerSkill()
    {
        AudioManager.Instance.ButtonPlay();
        if (CurrentState != TurnState.PlayerTurn || action)
            return;
        action = true;
        if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
        {
            Debug.Log("PlayerList����");
            return;
        }
        ChoosePanel.SetActive(false);
        SkillPanel.SetActive(true);
        for (int i = 0; i < PlayerList[playerindex].characterDate.skill.Count; i++)
        {
            SkillPanel.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    //��SKILLPANELѡ��ָ��SKILL�����
    public void UseSkill(int skillindex)
    {
        if (playerindex >= PlayerList.Count || skillindex > PlayerList[playerindex].characterDate.skill.Count)
            return;
        if (skillusing)
            return;
        AudioManager.Instance.ButtonPlay();
        skillusing = true;
        var skill = PlayerList[playerindex].characterDate.skill[skillindex];
        if(skill.currentcd<skill.CD)
        {
            Log.GetComponent<Log>().SetLog("������ȴ��,ʣ��"+(skill.CD-skill.currentcd).ToString()+"�غ�");
            Log.SetActive(true);
            skillusing = false;
            return;
        }
        PlayerList[playerindex].skillindex = skillindex;
        ChessManager.Instance.ShowSkillScope(skill.ScopeMin, skill.ScopeMax, PlayerList[playerindex].currentPos);
    }

    //����SKILL
    public void ActivateSkill(Vector2Int ScopeMin, Vector2Int ScopeMax,Vector3 forward,bool isenemy = false)
    {
        if (isenemy)
        {
            List<BaseChessControl> targets = new List<BaseChessControl>();
            foreach (var player in PlayerList)
            {
                if (ChessInScope(player.currentPos, ScopeMin, ScopeMax))
                    targets.Add(player);
            }
            EnemyList[enemyindex].Skill(targets, forward);
            enemyindex++;
        }
        else
        {
            //��ù�����Χ�ڵĵ�enemy
            List<BaseChessControl> targets = new List<BaseChessControl>();
            foreach (var enemy in EnemyList)
            {
                if (ChessInScope(enemy.currentPos, ScopeMin, ScopeMax))
                    targets.Add(enemy);
            }
            PlayerList[playerindex].Skill(targets,forward);
            playerindex++;
        }
    }

    //Heal��ť����
    public void PlayerHeal()
    {
        AudioManager.Instance.ButtonPlay();
        if (CurrentState != TurnState.PlayerTurn || action)
            return;
        if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
        {
            Debug.Log("PlayerList����");
            return;
        }
        action = true;
        PlayerList[playerindex].Heal(healpoint);
        playerindex++;
        ActionEnd();
    }

    private void ClearChessList()
    {
        PlayerList.Clear();
        EnemyList.Clear();
    }

    //�ж����ѡȡ��Ŀ���Ƿ��ڵ�ǰPlayer���Թ������ķ�Χ
    public bool canAttack(Transform target)
    {
        if (PlayerList.Count == 0 || playerindex >= PlayerList.Count)
            return false;
        else if (Vector3.Distance(PlayerList[playerindex].transform.position, target.position) < 1.8f)
            return true;
        else
            return false;

    }

    public bool ChessInScope(Vector2Int pos, Vector2Int min, Vector2Int max)
    {
        if (pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y)
            return true;
        return false;
    }

    private void CloseSkillPanel()
    {
        for (int i = 0; i < SkillPanel.transform.childCount; i++)
            SkillPanel.transform.GetChild(i).gameObject.SetActive(false);
        SkillPanel.SetActive(false);
    }

    private void EnemyTurn()
    {
        if (action)
            return;
        action = true;
        EnemyList[enemyindex].transform.GetComponent<EnemyAI>().EnemyTurn();
    }

    //��enemy����target
    private void DistributeTargetForEnemy()
    {
        if (PlayerList.Count == 0)
            return;
        PlayerDistribution.Clear();
        for(int i=0;i<PlayerList.Count;i++)
        {
            PlayerDistribution.Add(i, new List<BaseChessControl>());
        }
        foreach (var enemy in EnemyList)
        {
            int index = -1;
            int min = int.MaxValue;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerDistribution[i].Count >= 2)
                    continue;
                int dis = (int)Vector2Int.Distance(enemy.currentPos, PlayerList[i].currentPos);
                if (dis < min)
                {
                    min = dis;
                    index = i;
                }
            }
            if (index == -1)
                index = (int)Random.Range(0, PlayerList.Count);
            enemy.GetComponent<EnemyAI>().currentTarget = PlayerList[index];
            PlayerDistribution[index].Add(enemy);

        }
    }
    //��ָ��player��Ϊenemy����target
    public bool EnemyChangeTarget(BaseChessControl enemy, BaseChessControl player)
    {
        int pindex = -1;
        for (int i = 0; i < PlayerList.Count; i++)
            if (PlayerList[i] == player)
            {
                pindex = i;
                break;
            }
        if (pindex == -1 || pindex >= PlayerList.Count)
        {
            Debug.Log("ChangeEnemyTarget error");
            return false;
        }
        foreach (var v in PlayerDistribution.Values)
            if (v.Contains(enemy))
                v.Remove(enemy);
        //���л���Target��enemy����2�����þ�����Զ��enemy����Ѱ��Ŀ��
        BaseChessControl tempenemy = null;
        if (PlayerDistribution[pindex].Count >= 2)
        {
            int e;
            if (Vector2Int.Distance(PlayerList[pindex].currentPos, PlayerDistribution[pindex][0].currentPos)
                > Vector2Int.Distance(PlayerList[pindex].currentPos, PlayerDistribution[pindex][1].currentPos))
                e = 0;
            else
                e = 1;
            tempenemy = PlayerDistribution[pindex][e];
            PlayerDistribution[pindex].RemoveAt(e);
        }

        PlayerDistribution[pindex].Add(enemy);
        if (tempenemy != null)
            FindEnemyTarget(tempenemy);
        return true;

    }
    //��enemy���·���target
    public void FindEnemyTarget(BaseChessControl enemy)
    {
        if (PlayerList.Count == 0)
            return;
        int index = -1;
        int min = int.MaxValue;
        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerDistribution[i].Count >= 2)
                continue;
            int dis = (int)Vector2Int.Distance(enemy.currentPos, PlayerList[i].currentPos);
            if (dis < min)
            {
                min = dis;
                index = i;
            }
        }
        if (index == -1)
        {
            index = (int)Random.Range(0, PlayerList.Count);
        }

        enemy.GetComponent<EnemyAI>().currentTarget = PlayerList[index];
        PlayerDistribution[index].Add(enemy);
    }
    //�޷��ƶ���ָ��λ��
    public void ChessMoveFail()
    {
        if (CurrentState == TurnState.PlayerTurn)
        {
            Log.GetComponent<Log>().SetLog("�޷��ƶ����ô�");
            Log.SetActive(true);
        }
    }

    private void WinLostCheck()
    {
        if (EnemyList.Count == 0)
        {
            Win();
            return;
        }
        if (PlayerList.Count == 0)
        {
            Lost();
            return;
        }
    }

    public void CannotMove()
    {
        AudioManager.Instance.Button03Play();
        Log.GetComponent<Log>().SetLog("�޷��ƶ����ô�");
        Log.SetActive(true);
    }


    private void Update()
    {
        if (PlayerList.Count == 0 || EnemyList.Count == 0)
            return;
        if (Input.GetKeyDown(KeyCode.R))
            ReturnActionPanel();
        //�ж��Ƿ�Ҫת���غ�
        switch (CurrentState)
        {
            case TurnState.Start:
                DistributeTargetForEnemy();
                StartCoroutine(TurnToPlayer());
                break;
            case TurnState.PlayerTurn:
                if (!action && playerindex >= PlayerList.Count)
                {
                    playerindex = 0;
                    action = true;
                    StartCoroutine(TurnToEnemy());
                }
                break;
            case TurnState.EnemyTurn:
                if (!action && enemyindex >= EnemyList.Count)
                {
                    enemyindex = 0;
                    action = true;
                    StartCoroutine(TurnToPlayer());
                }
                else if (enemyindex < EnemyList.Count && !action)
                    EnemyTurn();
                break;
            default:
                break;
        }

    }
    public void ReturnMenu()
    {
        GameManager.Instance.TransitionToLevel("Menu");
    }

    public void NextEnemy()
    {
        if (CurrentState != TurnState.EnemyTurn)
            return;
        enemyindex++;
        action = false;
    }

    /*
    public void BuildDebug(int i)
    {
        Log.GetComponent<Log>().SetLog(i.ToString());
        Log.SetActive(true);
    }
    */
}

public enum TurnState {Start,PlayerTurn,EnemyTurn,Win,Lost }
