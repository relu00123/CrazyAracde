using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Google.Protobuf.Protocol;
using System.Diagnostics;
using System.Numerics;
using Server.Game.CA_Object;

public class TileInfo
{
    public bool isBlocktPermanently = false;
    public bool isBlocktTemporary = false;
    public InGameObject inGameObject;

    
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
    public CharacterSpawnType spawnType = CharacterSpawnType.SpawnNothing;
}

namespace Server.Game
{
    public class CAMapManager
    {
        public TileInfo[,] _tileMapData { get; private set; }

        public InGame _currentGame { get; private set; }

        public MapType _mapType { get; private set; }

        private List<Vector2Int> curExplodedPositions = new List<Vector2Int>();

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
                for (int y = 0; y < height; y++)
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

            List<TileData> spawnTiles = new List<TileData>();

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
                        LayerType.DefaultLayer,
                        "Walls",
                        PositionType.TileCenterPos,
                        ObjectType.ObjectBox,
                        new Vector2(tileData.position.x, tileData.position.y)
                    );
                }

                else if (tileData.tilemapName == "Boxes")
                {
                    // 부술수 있는 벽 생성
                    Console.WriteLine($"TileName(Box) : {tileData.tileName}" );

                    string tilename = tileData.tileName;

                    TileInfoValue tileInfoValue = new TileInfoValue()
                    {
                        Tilename = tileData.tileName
                    }; 

                    List<KeyValuePairs> TileInfos = new List<KeyValuePairs>
                    {
                        new KeyValuePairs { Key = ObjectSpawnKeyType.Box, TileInfoValue = tileInfoValue }
                    };

                    _tileMapData[tileData.position.x, tileData.position.y].isBlocktPermanently = true;
                    _currentGame.CreateAndBroadcastObject(
                        LayerType.BoxLayer,
                        tileData.tileName,  // 설마 문제가 되나..?
                        PositionType.TileCenterPos,
                        ObjectType.ObjectBox,
                        new Vector2(tileData.position.x, tileData.position.y),
                        TileInfos
                    );
                }


                else if (tileData.tilemapName == "CharacterSpawn")
                {
                    spawnTiles.Add(tileData);
                }
            }
            SpawnCharacterRandomly(spawnTiles);



            // 맵의 상태를 확인해 보자. 
            for (int y = 0; y < 14; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    if (_tileMapData[x, y].isBlocktTemporary == true || _tileMapData[x, y].isBlocktPermanently == true)
                    {
                        Console.Write("1 ");
                    }
                    else
                    {
                        Console.Write("0 ");
                    }

                }
                Console.WriteLine(0);
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

        private void SpawnCharacterRandomly(List<TileData> spawnTiles)
        {
            Console.WriteLine("SpawnCharacterRandomly Function Called!");

            Random random = new Random();

            for (int i = 0; i < _currentGame._gameRoom.Slots.Length; i++)
            {
                if (spawnTiles.Count == 0) break; // 더 이상 사용할 타일이 없다면 종료.
                int randomidx = random.Next(spawnTiles.Count);

                if (_currentGame._gameRoom.Slots[i].ClientSession == null) continue;

                // 해당 타일을 사용한 후에는 리스트에서 제거하여 중복 스폰 방지
                var selectedTile = spawnTiles[randomidx];
                spawnTiles.RemoveAt(randomidx);

                Console.WriteLine($"Spawning character at tile: {selectedTile.position.x}, {selectedTile.position.y}");


                ObjectSpawnPlayerValue playerInfoValue = new ObjectSpawnPlayerValue
                {
                    CharactertypeValue = _currentGame._gameRoom.Slots[i].CharType
                };

                List<KeyValuePairs> testvalues = new List<KeyValuePairs>
                {
                        new KeyValuePairs { Key = ObjectSpawnKeyType.Character  ,  PlayerInfoVlaue = playerInfoValue  },
                };

                // 해당 ClientSession을 randomidx위치에 Spawn한다 (Broadcast)
                //InGameObject spawnobj =  _currentGame.CreateAndBroadcastObject(
                //        LayerType.DefaultLayer,
                //        "Character",
                //        PositionType.TileCenterPos,
                //        ObjectType.ObjectPlayer,
                //        new Vector2(selectedTile.position.x, selectedTile.position.y),
                //        testvalues
                //);

                CAPlayer spawnobj = _currentGame.CreateAndBroadcastObject<CAPlayer>(
                    LayerType.CharacterLayer,
                    "Character",
                    PositionType.TileCenterPos,
                    ObjectType.ObjectPlayer,
                    new Vector2(selectedTile.position.x, selectedTile.position.y),
                        testvalues
                );

                // 자신의 캐릭터 타입을 지정하는 부분 (09.23수정)
                spawnobj._characterType = _currentGame._gameRoom.Slots[i].CharType;

                // 클라이언트가 자신의 캐릭터를 알 수 있도록 한다.
                _currentGame._gameRoom.Slots[i].ClientSession.CA_MyPlayer = spawnobj;


                for (int idx = 0; idx < _currentGame._gameRoom.Slots.Length; ++idx)
                {
                    if (_currentGame._gameRoom.Slots[idx].ClientSession == null) continue;

                    // 본인이 조작해야하는 플레이어인 경우  경우
                    if (idx == i)
                    {
                        S_OwnPlayerInform ownPlayerInform = new S_OwnPlayerInform
                        {
                            Objid = spawnobj.Id,
                            Layerinfo = (LayerType)spawnobj._layeridx,
                            Chartype = _currentGame._gameRoom.Slots[i].CharType
                        };

                        _currentGame._gameRoom.Slots[idx].ClientSession.Send(ownPlayerInform);

                    }


                    // 다른 플레이어인 경우 
                    else
                    {
                        S_NotOwnPlayerInform NotownPlayerInform = new S_NotOwnPlayerInform
                        {
                            Objid = spawnobj.Id,
                            Layerinfo = (LayerType)spawnobj._layeridx,
                            Chartype = _currentGame._gameRoom.Slots[i].CharType
                        };

                        _currentGame._gameRoom.Slots[idx].ClientSession.Send(NotownPlayerInform);
                    }
                }


                // InGame에 생성된 Object를 알려준다.
                _currentGame.EnterGame(spawnobj);
            }
        }

        public void ExplodeBomb(CABomb bomb)
        {
            curExplodedPositions.Clear();

            ExplodeAtPosition(bomb.position, bomb);

        }

        private void ExplodeAtPosition(Vector2Int position, CABomb bomb)
        {
            bomb.Explode();

            if (curExplodedPositions.Contains(position))
                return; // 이미 방문한 위치는 다시 처리하지 않는다.

            curExplodedPositions.Add(position);

            // 현재 위치에 물줄기 생성
            // Todo.. 테스트중..
            CAWaterStream SpawnedWaterStream = _currentGame.CreateAndBroadcastObject<CAWaterStream>(
                   LayerType.WaterstreamLayer,
                   "WaterStream",
                   PositionType.TileCenterPos,
                   ObjectType.ObjectWaterStream,
                   new Vector2(position.x, position.y)
               );

            SpawnedWaterStream._possessGame = _currentGame;
            SpawnedWaterStream.WaterStreamUpdate();

            // 테스트중..



            SpreadWater(Vector2Int.up, position, bomb.power);
            SpreadWater(Vector2Int.down, position, bomb.power);
            SpreadWater(Vector2Int.left, position, bomb.power);
            SpreadWater(Vector2Int.right, position, bomb.power);

        }

        private void SpreadWater(Vector2Int direction, Vector2Int startPosition, int power)
        {
            for (int i = 1; i <= power; i++)
            {
                Vector2Int nextPosition = new Vector2Int(
                    startPosition.x + direction.x * i,
                    startPosition.y + direction.y * i
                );


                if (!IsWithInMap(nextPosition))
                    break;

                TileInfo tile = _tileMapData[nextPosition.x, nextPosition.y];

                // 물풍선이 있으면 즉시 터뜨린다.
                if (tile.inGameObject is CABomb bomb)
                {
                    ExplodeAtPosition(nextPosition, bomb); // 재귀적으로 해당위치에서 폭발 확산 -> 여기 로직 보완 필요 PushAfter막는 것을 이 함수에서?
                    break;
                }

                // 이미 방문한 좌표라면 생략
                if (curExplodedPositions.Contains(nextPosition))
                    continue;

                var WaterStreamDir = CalculateWaterStreamType(direction, power - i);

                List<KeyValuePairs> StreamInfos = new List<KeyValuePairs>
                {
                    new KeyValuePairs { Key = ObjectSpawnKeyType.Waterstream, StreamInfoValue = WaterStreamDir}
                };


                // 물줄기를 퍼뜨리고, 좌표 리스트에 추가
                curExplodedPositions.Add(nextPosition);
                // 물줄기 생성
                // Todo..  테스트중..
                CAWaterStream SpawnedWaterStream = _currentGame.CreateAndBroadcastObject<CAWaterStream>(
                   LayerType.WaterstreamLayer,
                   "WaterStream",
                   PositionType.TileCenterPos,
                   ObjectType.ObjectWaterStream,
                   new Vector2(nextPosition.x, nextPosition.y),
                   StreamInfos
               );

                SpawnedWaterStream._possessGame = _currentGame;
                SpawnedWaterStream.WaterStreamUpdate();

                // 테스트 중..
            }
        }

        private bool IsWithInMap(Vector2Int position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < 15 && position.y < 14;
        }


        WaterStreamType CalculateWaterStreamType(Vector2Int dir, int power)
        {
            if (dir.Equals(Vector2Int.down))
            {
                if (power == 0)
                    return WaterStreamType.StreamDownEdge;
                return WaterStreamType.StreamDown;
            }

            else if (dir.Equals(Vector2Int.up))
            {
                if (power == 0)
                    return WaterStreamType.StreamUpEdge;
                return WaterStreamType.StreamUp;
            }

            else if (dir.Equals(Vector2Int.right))
            {
                if (power == 0)
                    return WaterStreamType.StreamRightEdge;
                return WaterStreamType.StreamRight;
            }

            else if (dir.Equals(Vector2Int.left))
            {
                if (power == 0)
                    return WaterStreamType.StreamLeftEdge;
                return WaterStreamType.StreamLeft;
            }

            return WaterStreamType.StreamCenter;
        }
    } 
}
