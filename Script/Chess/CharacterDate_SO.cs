using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new Character",menuName ="SO/Character")]
public class CharacterDate_SO : ScriptableObject
{
    public int MaxHealth;
    public int Currenthealth;
    public int Attack;
    public List<Skill> skill = new List<Skill>();
}

[System.Serializable]
public class Skill
{
    public Vector2Int ScopeMin;
    public Vector2Int ScopeMax;
    public int Damage;
    public int CD;
    public int currentcd;
}
