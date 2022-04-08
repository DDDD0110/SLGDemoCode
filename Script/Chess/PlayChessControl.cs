using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayChessControl : BaseChessControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
        TurnManager.Instance.PlayerRegister(this);
    }
    protected override void Dead()
    {
        base.Dead();
        TurnManager.Instance.PlayerDis(this);
    }
}
