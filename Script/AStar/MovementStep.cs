using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStep
{
    //grid中坐标
    public Vector2Int GridPosition;
    //实际坐标
    public Vector3 ChessPosition => new Vector3((float)GridPosition.x + 0.5f, 1.05f, (float)GridPosition.y + 0.5f);
}
