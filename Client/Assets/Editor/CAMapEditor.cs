using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CAMapEditor : MonoBehaviour
{
    [MenuItem("Tools/CreateMap %#c")]
    public static void CreateMap()
    {
        SaveMapByPath("Assets/Resources/Map_CA");
        SaveMapByPath("../Common/MapData_CA");
    }

    private static void SaveMapByPath(string pathPrefix)
    {
        // `Prefabs/CAMap` ���� ���� ��� ������ �ε�
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map_CA");

        foreach (GameObject go in gameObjects)
        {
            // ������ ���� ��� Tilemap ������Ʈ ��������
            Tilemap[] tilemaps = go.GetComponentsInChildren<Tilemap>(true);

            // ������ ������ ����Ʈ ����
            List<TileData> tileDataList = new List<TileData>();

            foreach (Tilemap tilemap in tilemaps)
            {
                foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                {
                    TileBase tile = tilemap.GetTile(position);
                    if (tile != null)
                    {
                        if (tile is CustomTile customTile)
                        {
                            tileDataList.Add(new TileData
                            {
                                position = position,
                                tileName = tile.name,
                                tilemapName = tilemap.name,
                                isSlippery = customTile.isSlippery,
                                isSpine = customTile.isSpine,
                                isProvidingStealth = customTile.isProvidingStealth,
                                isMoveable = customTile.isMoveable,
                                childTileName = customTile.childTileSprite != null ? customTile.childTileSprite.name : null
                            }); 
                        }
                        else
                        {
                            tileDataList.Add(new TileData
                            {
                                position = position,
                                tileName = tile.name,
                                tilemapName = tilemap.name
                            });
                        }
                    }
                }
            }

            // ������ ��� ����
            string filePath = $"{pathPrefix}/{go.name}.json";

            // ���丮 ���� Ȯ�� �� ����
            if (!Directory.Exists(pathPrefix))
            {
                Directory.CreateDirectory(pathPrefix);
            }

            // JSON ���Ϸ� ����
            string json = JsonUtility.ToJson(new TileDataList(tileDataList), true);
            File.WriteAllText(filePath, json);

            Debug.Log($"Map {go.name} has been created and saved at {filePath}!");
        }
    }

    [System.Serializable]
    public class TileData
    {
        public Vector3Int position;
        public string tileName;
        public string tilemapName;
        public bool isSlippery = false;
        public bool isSpine = false;
        public bool isProvidingStealth = false;
        public bool isMoveable = false;
        public string childTileName;
    }

    [System.Serializable]
    public class TileDataList
    {
        public List<TileData> tiles;

        public TileDataList(List<TileData> tiles)
        {
            this.tiles = tiles;
        }
    }
}