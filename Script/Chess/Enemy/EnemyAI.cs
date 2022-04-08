using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    public BaseChessControl currentTarget;
    public int MaxSteps;
    public int ChangeTargetThreshold;

    private BaseChessControl self;
    private CharacterDate_SO selfDate;
    private Vector2Int MovePosTarget;
    private List<Vector3> moveSteps;
    private Vector3 currentMoveTarget;
    private List<Vector2Int[,]> skillsScope = new List<Vector2Int[,]>();
    private int MaxChooseTime;


    private void Start()
    {
        self = transform.GetComponent<EnemyChessControl>();
        selfDate = self.characterDate;
        updateSkillsScope();
    }

    private void updateSkillsScope()
    {
        if (selfDate == null)
            return;
        skillsScope.Clear();
        for (int i = 0; i < selfDate.skill.Count; i++)
        {
            Vector2Int[,] scope = new Vector2Int[4,2];
            //Up
            scope[0, 0] = self.currentPos + selfDate.skill[i].ScopeMin;
            scope[0, 1] = self.currentPos + selfDate.skill[i].ScopeMax;
            //Down
            scope[1, 0] = self.currentPos - selfDate.skill[i].ScopeMax;
            scope[1, 1] = self.currentPos - selfDate.skill[i].ScopeMin;
            //Right
            scope[2, 0] = self.currentPos + new Vector2Int(selfDate.skill[i].ScopeMin.y, selfDate.skill[i].ScopeMin.x);
            scope[2, 1] = self.currentPos + new Vector2Int(selfDate.skill[i].ScopeMax.y, selfDate.skill[i].ScopeMax.x);
            //Left
            scope[3, 0] = self.currentPos - new Vector2Int(selfDate.skill[i].ScopeMax.y, selfDate.skill[i].ScopeMax.x);
            scope[3, 1] = self.currentPos - new Vector2Int(selfDate.skill[i].ScopeMin.y, selfDate.skill[i].ScopeMin.x);
            skillsScope.Add(scope);
        }
    }
    //判断target是否在Attack范围
    public bool TargetInAttackScope()
    {
        if (currentTarget == null)
            return false;
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1.8f)
            return true;
        else
            return false;
    }

    //判断target是否在某一skill范围，若有返回SKILL的INDEX和方位
    public int TargetInSkillScope(out Forward forward)
    {
        int skillindex = -1;
        forward = Forward.UP;
        if (currentTarget != null)
        {
            for (int i = 0; i < skillsScope.Count; i++)
            {
                if (selfDate.skill[i].currentcd < selfDate.skill[i].CD)
                    continue;
                for (int j = 0; j < 4; j++)
                {
                    if (currentTarget.currentPos.x >= skillsScope[i][j, 0].x && currentTarget.currentPos.y >= skillsScope[i][j, 0].y
                        && currentTarget.currentPos.x <= skillsScope[i][j, 1].x && currentTarget.currentPos.y <= skillsScope[i][j, 1].y)
                    {
                        skillindex = i;
                        switch (j)
                        {
                            case 0:
                                forward = Forward.UP;
                                break;
                            case 1:
                                forward = Forward.DOWN;
                                break;
                            case 2:
                                forward = Forward.RIGHT;
                                break;
                            case 3:
                                forward = Forward.LEFT;
                                break;
                        }
                    }
                    if (skillindex != -1)
                        break;
                }
                if (skillindex != -1)
                    break;
            }
        }
        return skillindex;
    }

    private bool ChooseMovePos()
    {
        if (currentTarget == null)
            return false;
        Vector2Int offset = self.currentPos - currentTarget.currentPos;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            MaxChooseTime = (Mathf.Abs(offset.x));
        else
            MaxChooseTime = (Mathf.Abs(offset.y));
        if (offset.x < 0)
            offset.x = -1;
        else
            offset.x = 1;
        if (offset.y < 0)
            offset.y = -1;
        else
            offset.y = 1;
        int x = 0;
        int y = 0;
        int k = 1;
        bool found = false;
        Vector2Int pos = Vector2Int.zero;
        while (k < MaxChooseTime)
        {
            x = k;
            y = 0;
            while (y <= k)
            {
                pos = new Vector2Int(x * offset.x, y * offset.y) + currentTarget.currentPos;
                if (ChessManager.Instance.ValidPos(pos))
                {
                    found = true;
                    break;
                }
                pos = new Vector2Int(y * offset.y,x * offset.x) + currentTarget.currentPos;
                if (ChessManager.Instance.ValidPos(pos))
                {
                    found = true;
                    break;
                }
                y++;
            }
            if (found)
            {
                MovePosTarget = pos;
                break;               
            }
            else
                k++;
        }
        return found;

        
    }

    //向目标点移动
    private void MoveToTarget()
    {

        if (!ChooseMovePos())
        {
            TurnManager.Instance.NextEnemy();
            return;
        }
        ChessManager.Instance.ChessMove(self.currentPos, MovePosTarget, out moveSteps);
        if (moveSteps != null)
        {
            if (moveSteps.Count > MaxSteps)
                currentMoveTarget = moveSteps[MaxSteps - 1];
            else
                currentMoveTarget = moveSteps[moveSteps.Count - 1];
            self.targetPos = new Vector2Int((int)(currentMoveTarget.x - 0.5f), (int)(currentMoveTarget.z - 0.5f));
            TurnManager.Instance.ChessMove(self.targetPos, true);
        }
        else
            TurnManager.Instance.NextEnemy();
    }

    //是否要改变Target
    public void GetHit(int damage,BaseChessControl attacker)
    {
        if (attacker == currentTarget)
            return;
        //Debug.Log("受到" + attacker.transform.name + "的" + damage + "点伤害");
        //伤害>=阈值，改变Target
        float temp = Random.Range(0, ChangeTargetThreshold + 1f);
        if (damage >= ChangeTargetThreshold)
        {
            if (TurnManager.Instance.EnemyChangeTarget(GetComponent<BaseChessControl>(), attacker))
                currentTarget = attacker;
        }
        //伤害<阈值，有概率改变Target，概率大小根据伤害决定，伤害越高概率越大
        else if (temp < damage)
            if (TurnManager.Instance.EnemyChangeTarget(GetComponent<BaseChessControl>(), attacker))
                currentTarget = attacker;
    }

    public void CheckTarget()
    {
        if (currentTarget == null)
            TurnManager.Instance.FindEnemyTarget(GetComponent<BaseChessControl>());
    }

    public void EnemyTurn()
    {
        if (currentTarget == null)
        {
            TurnManager.Instance.NextEnemy();
            return;
        }
        for (int i = 0; i < selfDate.skill.Count; i++)
        {
            selfDate.skill[i].currentcd++;
        }
        updateSkillsScope();
        bool canattack = TargetInAttackScope();
        Forward fw;
        int canskill = TargetInSkillScope(out fw);
        Vector2Int Smin = Vector2Int.zero;
        Vector2Int Smax = Vector2Int.zero;
        Vector3 f = Vector3.zero;
        if (canskill != -1)
        {
            self.skillindex = canskill;

            switch (fw)
            {
                case Forward.UP:
                    f = new Vector3(0, 0, 1f);
                    Smin = skillsScope[canskill][0, 0];
                    Smax = skillsScope[canskill][0, 1];
                    break;
                case Forward.DOWN:
                    Smin = skillsScope[canskill][1, 0];
                    Smax = skillsScope[canskill][1, 1];
                    f = new Vector3(0, 0, -1f);
                    break;
                case Forward.RIGHT:
                    Smin = skillsScope[canskill][2, 0];
                    Smax = skillsScope[canskill][2, 1];
                    f = new Vector3(1f, 0, 0);
                    break;
                case Forward.LEFT:
                    Smin = skillsScope[canskill][3, 0];
                    Smax = skillsScope[canskill][3, 1];
                    f = new Vector3(-1f, 0, 0);
                    break;
            }
        }
        if (canattack && canskill == -1)
        {
            //Attack
            TurnManager.Instance.ChessAttack(currentTarget.transform, true);
        }
        else if (canattack && canskill != -1)
        {
            int p = Random.Range(0,10);
            if (p <= 5)
            {
                TurnManager.Instance.ChessAttack(currentTarget.transform, true);
            }
            else
            {
                TurnManager.Instance.ActivateSkill(Smin, Smax, f, true);
            }
            //Attack or Skill
        }
        else if (!canattack && canskill != -1)
        {
            int p = Random.Range(1, 10);
            if (p <= 8)
            {
                TurnManager.Instance.ActivateSkill(Smin, Smax, f, true);
            }
            else
                MoveToTarget();
            //skill or move
        }
        else
        {
            //move
            MoveToTarget();
        }
    }
    private void Update()
    {
        CheckTarget();
    }


}
