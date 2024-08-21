using Google.Protobuf.Protocol;
using Server.Game.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class InGame
    {
        private GameRoom _gameRoom;

        private int _nextObjectId = 1;

        private ObjectLayerManager _objectLayerManager;

        private CAMapManager _caMapManager;

        private CollisionManager _collisionManager;


        public InGame(GameRoom gameRoom, MapType mapType)  // 나중에 필요한 정보들 추가해서 구조체로 바꿀 수도 있음. 
        {
            
            _gameRoom = gameRoom;
            _objectLayerManager = new ObjectLayerManager();
            _caMapManager = new CAMapManager(mapType);
            _collisionManager = new CollisionManager(_objectLayerManager);
        }

        public InGameObject CreateObject(int layerIndex, string objectName, Vector2? colliderSize = null)
        {
            int objectId = _nextObjectId++;
            InGameObject newObject = new InGameObject(objectId, objectName, layerIndex);
            

            if (colliderSize.HasValue)
            {
                newObject.InitializeCollider(colliderSize.Value);
                //_collisionManager.AddCollider(newObject._collider);
            }

            _objectLayerManager.AddObjectToLayer(layerIndex, newObject);

            return newObject;
        }

        public List<InGameObject> GetObjectsInLayer(int layerIndex)
        {
            return _objectLayerManager.GetObjectsInLayer(layerIndex);
        }

        public void Update()
        {
            // 게임의 로직을 업데이트 하는 부분
            // 충돌 검사 실행
            //_collisionManager.Tick();
        }


    }
}
