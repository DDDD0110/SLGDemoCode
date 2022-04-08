using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Map",menuName ="SO/TileMap/map")]
public class MapDate_SO : ScriptableObject
{
    public string SceneName;
    public Vector2Int Origin;
    public int width;
    public int hight;
    public bool available;
    public List<Tile> tiles = new List<Tile>();
    
}
