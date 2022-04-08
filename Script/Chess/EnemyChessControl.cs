using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChessControl : BaseChessControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
        TurnManager.Instance.EnemyRegister(this);
    }

    protected override void Dead()
    {
        base.Dead();
        TurnManager.Instance.EnemyDis(this);
    }
}
