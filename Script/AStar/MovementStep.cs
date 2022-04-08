using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStep
{
    //grid������
    public Vector2Int GridPosition;
    //ʵ������
    public Vector3 ChessPosition => new Vector3((float)GridPosition.x + 0.5f, 1.05f, (float)GridPosition.y + 0.5f);
}
