using Google.Protobuf.Protocol;
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


    // ���� Background �� ForeGround�� �ش��ϴ� Tile�� Json���� ������ �ʿ�� ���� �� ����.
    // ���߿� ���� ����ó�� �ϴ��� �ؾ߰ڴ�. 
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
                        // �������� �ڵ�
                        UnityEngine.Tilemaps.TileData tileData = new UnityEngine.Tilemaps.TileData();
                        tile.GetTileData(position, null, ref tileData);

                        Sprite tileSprite = tileData.sprite;
                        string atlasName = "";
                        if (tileSprite != null)
                        {
                            // ��������Ʈ���� ���� �ؽ���(Atlas)�̸��� ���
                            Texture2D atlasTexture = tileSprite.texture;
                            Debug.Log($"Ÿ�� ��ġ: {position}, Atlas �̸�: {atlasTexture.name}");
                            atlasName = atlasTexture.name;
                        }


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
                                childTileName = customTile.childTileSprite != null ? customTile.childTileSprite.name : null,
                                spawnType = customTile.spawnType,
                                AtlasName = atlasName
                                //charTypeTest = customTile.charactertype,
                            }) ; 
                        }
                        else
                        {
                            tileDataList.Add(new TileData
                            {
                                position = position,
                                tileName = tile.name,
                                tilemapName = tilemap.name,
                                AtlasName = atlasName
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

    //[System.Serializable]
    //public class TileData   // Enum���� ������� �� �ִٴ� �� Ȯ���Ͽ���. 
    //{
    //    public Vector3Int position;
    //    public string tileName;
    //    public string tilemapName;
    //    public bool isSlippery = false;
    //    public bool isSpine = false;
    //    public bool isProvidingStealth = false;
    //    public bool isMoveable = false;
    //    public string childTileName;
    //    //public CharacterType charTypeTest;   // Enum����Ǵ��� �׽�Ʈ �뵵. ���Ǵ� ���� �ƴ�.
    //}

    //[System.Serializable]
    //public class TileDataList
    //{
    //    public List<TileData> tiles;

    //    public TileDataList(List<TileData> tiles)
    //    {
    //        this.tiles = tiles;
    //    }
    //}
}