using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D defaultcursor;
    public Texture2D GroundCursor;
    public Texture2D AttackCursor;
    public Texture2D SkillCursor;

    private bool isWaitTarget = false;
    private bool isChooseHit = false;
    private bool isChooseSkill = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Update()
    {
        SetCursorTexture();
        //move选择目标
        if (isWaitTarget && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            ChooseMove();
        //attack选择目标
        if (isChooseHit && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            ChooseAttack();
    }
    private void SetCursorTexture()
    {
        if (isWaitTarget)
            Cursor.SetCursor(GroundCursor, new Vector2(10f, 2f), CursorMode.Auto);
        else if (isChooseHit)
            Cursor.SetCursor(AttackCursor, new Vector2(10f, 2f), CursorMode.Auto);
        else if(isChooseSkill)
            Cursor.SetCursor(SkillCursor, new Vector2(10f, 2f), CursorMode.Auto);
        else
            Cursor.SetCursor(defaultcursor, new Vector2(10f, 2f), CursorMode.Auto);
    }
    private void ChooseMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point);
            Transform gameObj = hitInfo.collider.transform;
            if (gameObj.CompareTag("Ground"))
            {
                Vector2Int target = new Vector2Int((int)(gameObj.position.x - 0.5f), (int)(gameObj.position.z - 0.5f));
                if (ChessManager.Instance.Isreachable(target))
                {
                    //Debug.Log(target);
                    isWaitTarget = false;
                    AudioManager.Instance.Button02Play();
                    TurnManager.Instance.ChessMove(target);
                }
                else
                {
                    TurnManager.Instance.CannotMove();
                }
            }
        }
    }

    private void ChooseAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point);
            Transform gameObj = hitInfo.collider.transform;
            if (gameObj.CompareTag("Enemy"))
            {
                if (TurnManager.Instance.canAttack(gameObj))
                {
                    isChooseHit = false;
                    TurnManager.Instance.ChessAttack(gameObj);
                }
                else
                {
                    Debug.Log("无法攻击该目标");
                }
            }
        }
    }

    public void StartChooseTarget()
    {
        isWaitTarget = true;
    }
    public void StartChooseAttack()
    {
        isChooseHit = true;
    }

    //判断鼠标在哪个center的哪个方位
    public Forward ChooseSkillForward(Vector2Int center )
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo))
        {
            Transform gameObj = hitinfo.collider.transform;
            if (gameObj.CompareTag("Ground"))
            {
                Vector2Int target = new Vector2Int((int)(gameObj.position.x - 0.5f), (int)(gameObj.position.z - 0.5f));
                Vector2Int f = target - center;
                if (f.y >= f.x && f.y >= -f.x)
                    return Forward.UP;
                else if (f.y < f.x && f.y > -f.x)
                    return Forward.RIGHT;
                else if (f.y <= f.x && f.y <= -f.x)
                    return Forward.DOWN;
                else if (f.y > f.x && f.y < -f.x)
                    return Forward.LEFT;

            }
        }
        return Forward.UP;
    }
    public void EndChoose()
    {
        isWaitTarget = false;
        isChooseHit = false;
        isChooseSkill = false;
    }
}
