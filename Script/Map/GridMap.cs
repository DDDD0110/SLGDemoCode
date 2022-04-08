using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapDate_SO mapdate;
    public bool IsObstacle;
    private Tilemap tilemap;


    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            if (mapdate != null)
                mapdate.tiles.Clear();
        }

    }
    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            CreateMapDate();
#if UNITY_EDITOR
            if (mapdate != null)
            {
                EditorUtility.SetDirty(mapdate);
            }
#endif
                
                
        }
    }
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    //根据绘制的地图生成mapdate
    private void CreateMapDate()
    {
        Vector3Int startpos = tilemap.cellBounds.min;
        Vector3Int endpos = tilemap.cellBounds.max;
        //Debug.Log(tilemap.cellBounds.size);
        //Debug.Log(startpos);
        //Debug.Log(endpos);
        for (int x=startpos.x; x < endpos.x; x++)
        {
            for (int y=startpos.y; y < endpos.y; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    Tile t = new Tile();
                    t.position = new Vector2Int(x, y);
                    t.isObstacle = IsObstacle;
                    mapdate.tiles.Add(t);
                }

            }
        }

    }
}
