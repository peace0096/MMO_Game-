using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{
#if UNITY_EDITOR

    // 메뉴키 추가 + 단축키
    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        // Map 오브젝트 내에 있는 모든 게임오브젝트 로드
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {
            // TmBase가 가장 크다.
            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

            using (var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMin; y >= tmBase.cellBounds.yMax; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }

            }
        }
        
    }

#endif
}
