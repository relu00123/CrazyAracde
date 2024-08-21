using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo
{
    public bool isBlocktPermanently = false;
    public bool isBlocktTemporary = false;
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


[System.Serializable]
public class TileData   // Enum값도 집어넣을 수 있다는 것 확인하였음. 
{
    public Vector3Int position;
    public string tileName;
    public string tilemapName;
    public bool isSlippery = false;
    public bool isSpine = false;
    public bool isProvidingStealth = false;
    public bool isMoveable = false;
    public string childTileName;
    //public CharacterType charTypeTest;   // Enum저장되는지 테스트 용도. 사용되는 값은 아님.
}


public class CAMapManager : MonoBehaviour
{
    private TileInfo[,] _tileMapData;

    private Tilemap[] _tileMaps;

    public Grid _currentGrid { get; private set; }

    public void Init()
    {
        int width = 15; // 가로 타일의 개수 
        int height = 14; // 세로 타일의 개수
        _tileMapData = new TileInfo[width, height];

        // 타일맵 초기화 ( 세로의 가장 마지막줄은 갈수 없는 곳임
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                _tileMapData[x, y] = new TileInfo { isBlocktPermanently = false, isBlocktTemporary = false };
            }
        }
    }
 

    public void LoadMap(MapType maptype)
    {
        string prefabName = maptype.ToString();
        string jsonFileName = maptype.ToString();

        // 타일맵 프리팹 로드
        GameObject tilemapInstance = Managers.Resource.Instantiate($"Map_CA/{prefabName}");
        tilemapInstance.name = "Map";

        _currentGrid = tilemapInstance.GetComponent<Grid>();

        // 타일맵 프리팹의 정보로 부터 진행되는 작업
        _tileMaps = FindObjectsOfType<Tilemap>();

        Tilemap WallsCollider = FindTileMap("WallsCollider");
        Tilemap Boxes = FindTileMap("Boxes");

        // 투명하게 만들 타일들을 투명하게 만들어준다.
        ExchangeToTransparentTile(WallsCollider);
        ExchangeToTransparentTile(Boxes);

        // JsonFile Load
        TextAsset jsonFile = Resources.Load<TextAsset>($"Map_CA/{jsonFileName}");

        if (jsonFile == null)
        {
            Debug.Log($"{jsonFileName} could not be loaded.");
            return;
        }

        string json = jsonFile.text;
        TileDataList tileDataList = JsonUtility.FromJson<TileDataList>(json);

        // Json File의 Data로부터 진행되는 작업 
        foreach (TileData tileData in tileDataList.tiles)
        {
            // WallsCollider는 영구적으로 이동할 수 없는 곳이다. 
            if (tileData.tilemapName == "WallsCollider")
            {
                _tileMapData[tileData.position.x, tileData.position.y].isBlocktPermanently = true;
            }

            // TODO 
            // Box타일들은 MapManager에서 관리하는 것이 아닐 Object를 생성하도록 ObjectManager에 전달해주어야 한다. 
        }
    }

    public Tilemap FindTileMap(string name)
    {
        foreach (Tilemap tilemap in _tileMaps)
        {
            if (tilemap.name == name)
                return tilemap;
        }

        return null;
    }

    public void ExchangeToTransparentTile(Tilemap TargetTilemap)
    {
        Tile TransparentTile = Resources.Load<Tile>("Tiles/TransparentTile");

        if (TargetTilemap != null)
        {
            // 모든 타일을 순회하며 새로운 타일로 교체합니다.
            foreach (Vector3Int pos in TargetTilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = TargetTilemap.GetTile(pos);
                if (tile != null)
                {
                    // 해당 위치의 타일을 새로운 타일로 교체
                    TargetTilemap.SetTile(pos, TransparentTile);
                }
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");  // 조금더 좋은 성능으로 변환 필요 
        if (map != null)
        {
            GameObject.Destroy(map);
            _currentGrid = null;
        }
    }


    public void SetTileState(int x, int y, bool isblockedTemp, bool isblockedPer)
    {
        _tileMapData[x, y].isBlocktTemporary = isblockedTemp;
        _tileMapData[x, y].isBlocktPermanently = isblockedPer;
    }

    public bool isTileBlocked(int x, int y)
    {
        return _tileMapData[x, y].isBlocktTemporary || _tileMapData[x, y].isBlocktPermanently;
    }




}
