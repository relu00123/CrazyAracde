using Google.Protobuf.Protocol;
using Server.Game.Core;
using System;
using System.Collections.Generic;
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

        public InGameObject CreateAndBroadcastObject(int layerIndex, string objectName, PositionType posType, ObjectType objecttype,
            Vector2 posValue, Vector2? scale = null, Vector2?colliderSize = null)
        {
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

            _gameRoom.BroadcastPacket(spawnObjectPacket);

            return newObject;
        }
    }
}
