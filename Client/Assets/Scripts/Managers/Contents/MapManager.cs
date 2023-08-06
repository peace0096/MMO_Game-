using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;


public class MapManager
{
    // 그리드 정보 담기
    public Grid CurrentGrid { get; private set; }


    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    // 읽어온 맵 구조를 여기에 표기할 예정
    bool[,] _collision;

    // 갈 수 있는지 판단.
    public bool CanGo(Vector3Int cellPos)
    {
        if(cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !_collision[y, x];
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if(collision != null)
        {
            collision.SetActive(false);
        }

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        StringReader reader = new StringReader(txt.text);

        // Map001 텍스트파일에서 읽어옴. 맵구조 !
        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for(int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for(int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if(map != null)
        {
            GameObject.Destroy(map);
            
        }
    }
}
