using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Server.Game;
using Newtonsoft.Json;

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

        public void LoadMap(string MapName, string pathPrefix = "../../../../../Common/MapData")
        {
            string filePath = $"{pathPrefix}/MapName";

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
