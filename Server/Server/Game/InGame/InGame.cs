using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using Server.Game.CA_Object;
using Server.Game.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace Server.Game
{
    public class InGame
    {
        public GameRoom _gameRoom { get; private set; }
        public ObjectsManager _objectsManager { get; private set; }

        public ObjectLayerManager _objectLayerManager { get; private set; }

        public CAMapManager _caMapManager { get; private set; }

        public CollisionManager _collisionManager { get; private set; }


        public InGame(GameRoom gameRoom, MapType mapType)  // 나중에 필요한 정보들 추가해서 구조체로 바꿀 수도 있음. 
        {
            
            _gameRoom = gameRoom;
            _objectLayerManager = new ObjectLayerManager();
            _objectsManager = new ObjectsManager(_objectLayerManager);
            _caMapManager = new CAMapManager(mapType, this);
            _collisionManager = new CollisionManager(_objectLayerManager);
        }

        public void Update()
        {
            _collisionManager.UpdateDynamicCollision();
        }

        public InGameObject CreateAndBroadcastObject(
            LayerType layerType,
            string objectName,
            PositionType posType,
            ObjectType objecttype,
            Vector2 posValue,
            List<KeyValuePairs> additionalData = null,
            Vector2? scale = null, 
            Vector2?colliderSize = null)
        {
            int layerIndex = (int)layerType;
            InGameObject newObject = _objectsManager.CreateObject(layerType, objectName, posType, posValue, scale, colliderSize);

            S_SpawnObject spawnObjectPacket = new S_SpawnObject
            {
                Objectid = newObject.Id,
                Objecttype = objecttype,
                Objectlayer = layerType,
                Positioninfo = new PositionInfo
                {
                    Type = posType,
                    PosX = (int)posValue.X,
                    PosY = (int)posValue.Y,
                }
            };
           
            if (additionalData != null)
            {
                spawnObjectPacket.AdditionalData.AddRange(additionalData);
            }

            _gameRoom.BroadcastPacket(spawnObjectPacket);
   
            return newObject;
        }


        public T CreateAndBroadcastObject<T>(
            LayerType layerType,
            string objectName,
            PositionType posType,
            ObjectType objecttype,
            Vector2 posValue,
            List<KeyValuePairs> additionalData = null,
            Vector2? scale = null,
            Vector2? colliderSize = null)
            where T : InGameObject
        {
            int layerIndex = (int)layerType;
            T newObject = _objectsManager.CreateObject<T>(layerType, objectName, posType, posValue, scale, colliderSize);

            S_SpawnObject spawnObjectPacket = new S_SpawnObject
            {
                Objectid = newObject.Id,
                Objecttype = objecttype,
                Objectlayer = layerType,
                Positioninfo = new PositionInfo
                {
                    Type = posType,
                    PosX = (int)posValue.X,
                    PosY = (int)posValue.Y,
                }
            };

            if (additionalData != null)
            {
                spawnObjectPacket.AdditionalData.AddRange(additionalData);
            }

            _gameRoom.BroadcastPacket(spawnObjectPacket);

            return newObject;
        }

        public void EnterGame(InGameObject gameObject)
        {
            gameObject._possessGame = this;
            gameObject.Update();
        }

        public void ApplyMoveTemp(InGameObject gameObject, MoveDir dir)
        {
            // 새로 도입한 코드
            gameObject._currentState.ApplyMove(gameObject, dir);


            // 기존코드 이분을 새로 도입한 코드에서 전부 처리하도록 할 것이다. 
            //Vector2 originalPosition = gameObject._transform.Position;
            //Console.WriteLine($"Cur Obj Pos : ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");

            //if (dir == MoveDir.MoveNone)
            //{
            //    Console.Write("Stop Walking (Move Key detached)");

            //    // Bubble Idle 이 될수도 있고 Idle이 될 수도 있다.
            //    // 만약에 Dead 상태였으면 해당 Move를 처리하고 싶지 않은데 어떻게 하면 좋을까?
            //    gameObject.ChangeState(CreatureState.Idle);
            //    return;
            //}

            //// 방향키 입력에 따른 캐릭터의 속도를 적용해서 TargetPosition을 구한다. 
            //Vector2 targetPosition = gameObject.CalculateTargetPositon(dir);
 

            //gameObject._targetPos = targetPosition;
            //gameObject.Direction = dir;
            //gameObject.ChangeState(CreatureState.Moving);
            // 기존코드 끝
        }



        public void ApplyMove(InGameObject gameObject, PositionInfo posInfo)
        {
            Vector2 originalPosition = gameObject._transform.Position;
            Vector2 targetPosition = new Vector2(posInfo.PosX, posInfo.PosY);

            // Object의 Position Debugging 코드
            Console.WriteLine($"Obj Pos : ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");

            // 잠시 테스트
            if (posInfo.MoveDir == MoveDir.MoveNone)
            {
                //Console.WriteLine("MoveNone Arrived");
                Console.Write("Stop Walking (Move Key detached)");
                gameObject.ChangeState(CreatureState.Idle);
                return;
            }

            // Object에 충돌체가 존재한다면 TileMap에서 갈수없는 곳에 가려하는지 검사해야한다.
            if (gameObject._collider != null)
            {
                // 목표 위치에서 임시로 경계값을 계산 (Colldier의 실제 값은 변경되지 않는다.)
                var (tempLeftX, tempRightX, tempUpY, tempDownY) = gameObject._collider.CalculateTempBounds(targetPosition);


                // Debugging 용도
                Console.WriteLine($"LeftX   : {tempLeftX}");
                Console.WriteLine($"RightX  : {tempRightX}");
                Console.WriteLine($"TopY    : {tempUpY}");
                Console.WriteLine($"BottomY : {tempDownY}");


                //if (_collisionManager.IsCollidedWithMap(tempLeftX, tempRightX, tempUpY, tempDownY, _caMapManager._tileMapData))
                CollisionInfo collisionInfo = _collisionManager.IsCollidedWithMapTest(posInfo.MoveDir, tempLeftX, tempRightX, tempUpY, tempDownY, _caMapManager._tileMapData);

                if (collisionInfo.IsCollided)
                {
                    Console.WriteLine($"Collision Occured. Original Position ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");
                    //Console.WriteLine($"Collision Info : ");
                    //Console.WriteLine($"isLeftCollision : {collisionInfo.IsLeftCollision}");
                    //Console.WriteLine($"isRightCollision : {collisionInfo.IsRightCollision}");
                    //Console.WriteLine($"isTopCollision : {collisionInfo.IsTopCollision}");
                    //Console.WriteLine($"isBottomCollision : {collisionInfo.IsBottomCollision}");
                    //Console.WriteLine($"CollidePercentage : {collisionInfo.OverlapPercentage}");

                    // 새로운 코드 09.06
                    // 충돌 방향에 따른 보정 처리
                    Vector2 correctedPosition = _collisionManager.GetCorrectedPositionForCharacter(
                        originalPosition, posInfo.MoveDir, collisionInfo, _caMapManager._tileMapData);

                    targetPosition = correctedPosition;

                    Console.WriteLine($"Fixed Position ({targetPosition.X}, {targetPosition.Y})");


                    // 기존 코드 09.05
                    // 추가된 코드 (충돌 발생시 좌표를 보정)

                    //Console.WriteLine($"Collision Detected Original Pos : {gameObject._transform.Position}");
                    //Vector2 correctedPosition = _collisionManager.GetCorrectedPosition(gameObject._transform.Position, posInfo.MoveDir);
                    //targetPosition = correctedPosition;
                    //Console.WriteLine($"Collision Detected Fixed Pos : {correctedPosition}");

                    // 충돌이 발생하면 이동 취소
                    //Console.WriteLine("Collision detected, cannot move to the target position.");
                }
            }

            gameObject._targetPos = targetPosition;
            gameObject.Direction = posInfo.MoveDir;
            gameObject.ChangeState(CreatureState.Moving);
        }

        public void InstallBomb(C_InstallBomb installBombPacket, ClientSession clientSession)
        {
            // 0. 현재 플레이어가 폭탄을 설치할 수 있는 상태인지 확인해야한다. 
            if (!(clientSession.CA_MyPlayer.Stats.IsBombPlaceable()))
                return;

            // 1. 폭탄이 설치 될 좌표 구하기.
            // 들어온 Player의 Position은 발바닦이 아닌 몸 정 중앙의 좌표이다. 
            // 설치해야할 타일 좌표를 구해보자. 
            int install_tile_x = (int)installBombPacket.CharPosX;
            int install_tile_y = (int)installBombPacket.CharPosY;

            // 2. 해당 좌표에 폭탄을 설치할 수 있는지 확인하기.
            TileInfo tileinfo = _caMapManager._tileMapData[install_tile_x, install_tile_y];
            if (tileinfo.isBlocktTemporary || tileinfo.isBlocktPermanently)
            {
                Console.WriteLine($"You can't install Tile on ({install_tile_x},{install_tile_y}). It's Blocked!!");
            }
            else
            {
                clientSession.CA_MyPlayer.Stats.DecreaseCurBombCount();

                // 3. 폭탄 생성 및 모든 클라이언트에게 이 사실을 Broadcast하기 
                BombInfoValue bombInfoValue = new BombInfoValue()
                {
                    BombType = installBombPacket.BombType,
                    BombPosX = install_tile_x,
                    BombPosY = install_tile_y,
                };

                List<KeyValuePairs> BombInfos = new List<KeyValuePairs>
                {
                    new KeyValuePairs { Key = ObjectSpawnKeyType.Bomb  ,  BombInfoValue = bombInfoValue },
                };


                CABomb spawnedBombObj = CreateAndBroadcastObject<CABomb>(
                    LayerType.DefaultLayer,
                    "Bomb",
                    PositionType.TileCenterPos,
                    ObjectType.ObjectBomb,
                    new Vector2(install_tile_x, install_tile_y),
                    BombInfos
                );


                // 4. 서버에 폭탄을 설치 및 관리하기 
                Console.WriteLine($"You can install Tile on ({install_tile_x},{install_tile_y}).");
                _caMapManager._tileMapData[install_tile_x, install_tile_y].isBlocktTemporary = true;


                spawnedBombObj._possessGame = this;
                spawnedBombObj.position = new Vector2Int(install_tile_x, install_tile_y);
                spawnedBombObj.bombOwner = clientSession.CA_MyPlayer; 

                // 5. 폭탄의 세기 설정하기 
                spawnedBombObj.power = clientSession.CA_MyPlayer.Stats.WaterPower;
                _caMapManager._tileMapData[install_tile_x, install_tile_y].inGameObject = spawnedBombObj;
                ((CABomb)(_caMapManager._tileMapData[install_tile_x, install_tile_y].inGameObject)).Bomb_update();
            }
        }


        public void TestFunction()
        {
            Console.WriteLine("Test Function Called From InGame !!!");
        }
    }
}
