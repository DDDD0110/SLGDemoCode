using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    public List<MapDate_SO> mapList = new List<MapDate_SO>();
    [SerializeField]
    public Dictionary<string, Tile> TileList = new Dictionary<string, Tile>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        foreach (MapDate_SO map in mapList)
            InitTileList(map);
    }

    //根据map生成数据
    private void InitTileList(MapDate_SO map)
    {
        foreach (Tile tile in map.tiles)
        {
            Tile t = new Tile();
            t.position = tile.position;
            t.isObstacle = tile.isObstacle;
            string key = "x" + t.position.x + "y" + t.position.y + map.SceneName;

            if (TileList.ContainsKey(key))
            {
                if (t.isObstacle)
                    TileList[key].isObstacle = true;
            }
            else
            {
                TileList.Add(key, t);
            }
        }
    }
    //得到tile
    public Tile GetTile(string key)
    {
        if (TileList.ContainsKey(key))
            return TileList[key];
        else
            return null;
    }


    public bool GetGridMap(string SceneName,out int width,out int hight,out Vector2Int Origin)
    {
        width = 0;
        hight = 0;
        Origin = Vector2Int.zero;
        for (int i = 0; i < mapList.Count; i++)
        {
            if (mapList[i].SceneName == SceneName)
            {
                width = mapList[i].width;
                hight = mapList[i].hight;
                Origin = mapList[i].Origin;
                return true;
            }
        }
        return false;
        
    }
}
