using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Server.Game;
using Newtonsoft.Json;
using Google.Protobuf.Protocol;
using System.Diagnostics;
using System.Numerics;

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

[Serializable]
public class Vector3Int
{
    public int x;
    public int y;
    public int z;

    public Vector3Int() { }

    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
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

namespace Server.Game
{
    public class CAMapManager
    { 
        private TileInfo[,] _tileMapData;

        public InGame _currentGame { get; private set; }

        public  MapType _mapType { get; private set; }

        public CAMapManager(MapType mapType, InGame currentGame)
        {
            _mapType = mapType;
            _currentGame = currentGame;
            Init();
             
        }

        private void Init()
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

            LoadMap(_mapType);
        }

        private void LoadMap(MapType mapType, string pathPrefix = "../../../../../Common/MapData_CA")
        {
            string MapName = mapType.ToString();
            string filePath = $"{pathPrefix}/{MapName}.json";

            Console.WriteLine($"Loading Map : {MapName}");

            // JsonFile 읽기 
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Map file {filePath} does not exist.");
                return;
            }

            string json = File.ReadAllText(filePath);
            TileDataList tileDataList = JsonConvert.DeserializeObject<TileDataList>(json);

            foreach (var tileData in tileDataList.tiles)
            {
                // TODO;  일단 테스트용
                if (tileData.tilemapName == "WallsCollider")
                {
                    Console.WriteLine("WallsCollider Tile Detected!");
                    _tileMapData[tileData.position.x, tileData.position.y].isBlocktPermanently = true;

                    // Client에 해당 위치에 벽을 생성하라고 해볼 것이다. 
                    // 서버에서도 이 Object들을 관리하고 있어야 한다.  
                    // 지금은 임시로 Wall 에 대해서 LayerIndex 1번을 사용하도록 한다.
                    // 나중에는 layer의 이름으로 index로 찾을 수 있게 하던가 enum Type으로 관리해야할 것임. 
                    // 이부분 코드가나중에도 재사용성이 매우 높기 때문에 함수로 만들어 버릴 것임. 

                    _currentGame.CreateAndBroadcastObject(
                        0, 
                        "wall", 
                        PositionType.TileCenterPos,
                        ObjectType.ObjectBox, 
                        new Vector2(tileData.position.x, tileData.position.y)
                    );
                }
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

    
}
