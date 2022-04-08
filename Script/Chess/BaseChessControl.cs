using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseChessControl : MonoBehaviour
{
    public CharacterDate_SO Datetemp;
    [HideInInspector]
    public CharacterDate_SO characterDate;
    [HideInInspector]
    public Vector2Int currentPos;
    [HideInInspector]
    public Vector2Int targetPos;
    [HideInInspector]
    public int skillindex;
    public GameObject HealthBarPre;
    public float PathDuration;
    public Transform HealthBarPos;

    private List<Vector3> path;
    private bool moving;
    private bool isskill;
    private Transform attacktarget;
    private List<BaseChessControl> skilltargets;
    private Animator Anim;
    private Transform healthbar;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        if (characterDate == null)
            characterDate = Instantiate(Datetemp);
    }

    //生成时，给位置赋值，生成血量ui，将所在位置的格子设为不可移动
    protected virtual void OnEnable()
    {
        currentPos = new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.z - 0.5f));
        Canvas[] canvas = FindObjectsOfType<Canvas>();
        foreach (var can in canvas)
        {
            if (can.CompareTag("HealthBar"))
            {
                healthbar = Instantiate(HealthBarPre, can.transform).transform;
                healthbar.GetComponent<HealthBar>().target = HealthBarPos;
                UpdateHealthBar();
            }
        }
        ChessManager.Instance.SetNodeObstacle(currentPos, true);
    }
    private void UpdateHealthBar()
    {
        if (healthbar == null)
            return;
        healthbar.GetComponent<HealthBar>().SetHealthBar(characterDate.Currenthealth, characterDate.MaxHealth);
    }



    protected virtual void Update()
    {
        //若在移动，判断是否到达移动位置
        if (moving)
        {
            if (Vector3.Distance(transform.position, new Vector3((float)targetPos.x + 0.5f, 1.05f, (float)targetPos.y + 0.5f)) < 0.1f)
            {
                moving = false;
                currentPos = targetPos;
                TurnManager.Instance.ActionEnd();
            }
        }
        Anim.SetBool("moving", moving);
        if (!moving && !isskill && transform.CompareTag("Enemy"))
            ForwardToTarget();
        
    }
    public bool Move()
    {
        ChessManager.Instance.ChessMove(currentPos, targetPos,out path);
        if (path != null)
        {
            Vector3[] movementPath = new Vector3[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                movementPath[i] = path[i];
            }
            transform.DOPath(movementPath, PathDuration).SetLookAt(0);
            moving = true;
        }
        else
            return false;

        ChessManager.Instance.SetNodeObstacle(currentPos, targetPos);
        return true;
    }
    public void Attack(Transform target)
    {
        transform.forward = target.position - transform.position;
        ChessManager.Instance.ClearScopeTile();
        attacktarget = target;
        Anim.SetTrigger("Attack");
    }
    public void Hit()
    {
        attacktarget.GetComponent<BaseChessControl>().GetHit(characterDate.Attack,this);
    }
    public void GetHit(int damage,BaseChessControl attacker)
    {
        Anim.SetTrigger("GetHit");
        characterDate.Currenthealth = Mathf.Max(characterDate.Currenthealth - damage, 0);
        UpdateHealthBar();
        DeadCheck();
        if (transform.CompareTag("Enemy"))
            GetComponent<EnemyAI>().GetHit(damage, attacker);

    }
    public void AttackSkillEnd()
    {
        isskill = false;
        TurnManager.Instance.ActionEnd();
    }

    public void Skill(List<BaseChessControl> targets,Vector3 forward)
    {
        isskill = true;
        transform.forward = forward;
        skilltargets = targets;
        //动画
        Anim.SetInteger("Skill", skillindex);
        Anim.SetTrigger("isskill");
        characterDate.skill[skillindex].currentcd = 0;

    }
    public void SkillHit()
    {
        foreach (var enemy in skilltargets)
            enemy.GetHit(characterDate.skill[skillindex].Damage,this);
    }

    public void Heal(int point)
    {
        characterDate.Currenthealth = Mathf.Min(characterDate.Currenthealth + point, characterDate.MaxHealth);
        UpdateHealthBar();
    }
    protected virtual void Dead()
    {
        Anim.SetTrigger("dead");
        //设置网格为无阻碍
        ChessManager.Instance.SetNodeObstacle(currentPos, false);
        Destroy(gameObject, 2f);
        Destroy(healthbar.gameObject, 2f);
    }

    private void DeadCheck()
    {
        if (characterDate.Currenthealth <= 0)
            Dead();
    }

    public void ForwardToTarget()
    {
        if (GetComponent<EnemyAI>().currentTarget != null)
            transform.forward = GetComponent<EnemyAI>().currentTarget.transform.position - transform.position;
    }

}
