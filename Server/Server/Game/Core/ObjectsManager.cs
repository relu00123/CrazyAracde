using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    // 객체의 생성, 초기화, 등의 작업을 담당
    // ObjectLayerManager와 협력하여 객체를 레이어에 추가하고 관리하는 역할을 한다. 
    public class ObjectsManager
    {
        private int _nextObjectId = 1;
        private ObjectLayerManager _objectLayerManager;

        public ObjectsManager(ObjectLayerManager objectLayerManager)
        {
            _objectLayerManager = objectLayerManager;
        }

        public InGameObject CreateObject(int layerIndex, string objectName, Vector2? colliderSize = null)
        {
            InGameObject newObject = InitializeObject(layerIndex, objectName, Vector2.Zero, Vector2.One, colliderSize);
            _objectLayerManager.AddObjectToLayer(layerIndex, newObject);
            return newObject;
        }

        public InGameObject CreateObject(int layerIndex, string objectName, PositionType posType, Vector2 posValue, Vector2? scale = null, Vector2? colliderSize = null)
        {
            Vector2 position = DeterminePosition(posType, posValue);
            Vector2 objectScale = scale ?? Vector2.One;

            InGameObject newObject = InitializeObject(layerIndex, objectName, position, objectScale, colliderSize);
            _objectLayerManager.AddObjectToLayer(layerIndex, newObject);
            return newObject;
        }

        private InGameObject InitializeObject(int layerIndex, string objectName, Vector2 position, Vector2 scale, Vector2? colliderSize)
        {
            int objectId = _nextObjectId++;
            InGameObject newObject = new InGameObject(objectId, objectName, layerIndex);

            newObject._transform.Position = position;
            newObject._transform.Scale = scale;

            if (colliderSize.HasValue)
            {
                newObject.InitializeCollider(colliderSize.Value);
                // _collisionManager.AddCollider(newObject._collider);  // 필요한 경우에 추가
            }

            return newObject;
        }

        private Vector2 DeterminePosition(PositionType posType, Vector2 posValue)
        {
            switch (posType)
            {
                case PositionType.TileCenterPos:
                    return CalculateTileCenter((int)posValue.X, (int)posValue.Y);
                case PositionType.TileUnderPos:
                    return CalculateTileUnder((int)posValue.X, (int)posValue.Y);
                case PositionType.AbsolutePos:
                    return posValue;
                default:
                    return Vector2.Zero;
            }
        }

        private Vector2 CalculateTileCenter(int tileX, int tileY, float tileSize = 40)
        {
            return new Vector2(tileX * tileSize + tileSize / 2, tileY * tileSize + tileSize / 2);
        }

        private Vector2 CalculateTileUnder(int tileX, int tileY, float tileSize = 40)
        {
            return new Vector2(tileX * tileSize + tileSize / 2, tileY * tileSize);
        }
    }
}
