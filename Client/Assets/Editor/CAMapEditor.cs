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


    // 굳이 Background 와 ForeGround에 해당하는 Tile을 Json으로 저장할 필요는 없을 것 같다.
    // 나중에 봐서 예외처리 하던지 해야겠다. 
    private static void SaveMapByPath(string pathPrefix)
    {
        // `Prefabs/CAMap` 폴더 내의 모든 프리팹 로드
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map_CA");

        foreach (GameObject go in gameObjects)
        {
            // 프리팹 내의 모든 Tilemap 컴포넌트 가져오기
            Tilemap[] tilemaps = go.GetComponentsInChildren<Tilemap>(true);

            // 저장할 데이터 리스트 생성
            List<TileData> tileDataList = new List<TileData>();

            foreach (Tilemap tilemap in tilemaps)
            {
                foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                {
                    TileBase tile = tilemap.GetTile(position);

                    if (tile != null)
                    {
                        // 수정중인 코드
                        UnityEngine.Tilemaps.TileData tileData = new UnityEngine.Tilemaps.TileData();
                        tile.GetTileData(position, null, ref tileData);

                        Sprite tileSprite = tileData.sprite;
                        string atlasName = "";
                        if (tileSprite != null)
                        {
                            // 스프라이트에서 원래 텍스쳐(Atlas)이름을 출력
                            Texture2D atlasTexture = tileSprite.texture;
                            Debug.Log($"타일 위치: {position}, Atlas 이름: {atlasTexture.name}");
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

            // 저장할 경로 설정
            string filePath = $"{pathPrefix}/{go.name}.json";

            // 디렉토리 존재 확인 및 생성
            if (!Directory.Exists(pathPrefix))
            {
                Directory.CreateDirectory(pathPrefix);
            }

            // JSON 파일로 저장
            string json = JsonUtility.ToJson(new TileDataList(tileDataList), true);
            File.WriteAllText(filePath, json);

            Debug.Log($"Map {go.name} has been created and saved at {filePath}!");
        }
    }

    //[System.Serializable]
    //public class TileData   // Enum값도 집어넣을 수 있다는 것 확인하였음. 
    //{
    //    public Vector3Int position;
    //    public string tileName;
    //    public string tilemapName;
    //    public bool isSlippery = false;
    //    public bool isSpine = false;
    //    public bool isProvidingStealth = false;
    //    public bool isMoveable = false;
    //    public string childTileName;
    //    //public CharacterType charTypeTest;   // Enum저장되는지 테스트 용도. 사용되는 값은 아님.
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