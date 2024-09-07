﻿using Google.Protobuf.Collections;
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
            // 게임의 로직을 업데이트 하는 부분
            // 충돌 검사 실행
            //_collisionManager.Tick();
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
            InGameObject newObject = _objectsManager.CreateObject(LayerType.DefaultLayer, objectName, posType, posValue, scale, colliderSize);

            S_SpawnObject spawnObjectPacket = new S_SpawnObject
            {
                Objectid = newObject.Id,
                Objecttype = objecttype,
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
            T newObject = _objectsManager.CreateObject<T>(LayerType.DefaultLayer, objectName, posType, posValue, scale, colliderSize);

            S_SpawnObject spawnObjectPacket = new S_SpawnObject
            {
                Objectid = newObject.Id,
                Objecttype = objecttype,
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
            gameObject._inGame = this;
            gameObject.Update();
        }

        public void ApplyMoveTemp(InGameObject gameObject, MoveDir dir)
        {
            Console.Write("ApplyMoveTemp Called");

            Vector2 originalPosition = gameObject._transform.Position;

            Console.WriteLine($"Obj Pos : ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");

            if (dir == MoveDir.MoveNone)
            {
                Console.Write("Stop Walking (Move Key detached)");
                gameObject.ChangeState(CreatureState.Idle);
                return;
            }

            // TODO
            // 방향키 입력에 따른 캐릭터의 속도를 적용해서 TargetPosition을 구한다. 
            
            Vector2 targetPosition = gameObject.CalculateTargetPositon(dir);



            // TODO.. 
            gameObject._targetPos = targetPosition;
            gameObject.Direction = dir;
            gameObject.ChangeState(CreatureState.Moving);
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


        public void TestFunction()
        {
            Console.WriteLine("Test Function Called From InGame !!!");
        }
    }
}
