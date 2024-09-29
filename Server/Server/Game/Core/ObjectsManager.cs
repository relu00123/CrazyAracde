using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
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

        public InGameObject CreateObject(LayerType layerType, string objectName, PositionType posType, Vector2 posValue, Vector2? scale = null, Vector2? colliderSize = null)
        {
            int LayerIndex = (int)layerType;

            Vector2 position = DeterminePosition(posType, posValue);
            Vector2 objectScale = scale ?? Vector2.One;

            InGameObject newObject = InitializeObject(LayerIndex, objectName, position, objectScale, colliderSize);
            _objectLayerManager.AddObjectToLayer(LayerIndex, newObject);
            return newObject;
        }

        public bool DestroyObjectbyId(int objid)
        {
            //수정 코드
            InGameObject obj = _objectLayerManager.FindObjectById(objid);
            if (obj!= null)
            {
                obj.isRemoveResreved = true;
                return true;
            }

            return false;


           // 기존 코드
           //if ( _objectLayerManager.RemoveObjectFromLayer(objid))
           //     return true;

           // return false; 
        }

        public T CreateObject<T>(LayerType layerType, string objectName, PositionType posType, Vector2 posValue, Vector2? scale = null, Vector2? colliderSize = null)
            where T : InGameObject
        {
            int LayerIndex = (int)layerType;

            Vector2 position = DeterminePosition(posType, posValue);
            Vector2 objectScale = scale ?? Vector2.One;

            T newObject = InitializeObject<T>(LayerIndex, objectName, position, objectScale, colliderSize);
            _objectLayerManager.AddObjectToLayer(LayerIndex, newObject);
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

        private T InitializeObject<T>(int layerIndex, string objectName, Vector2 position, Vector2 scale, Vector2? colliderSize)
             where T : InGameObject
        {
            int objectId = _nextObjectId++;

            Transform transform = new Transform();
            transform.Position = position;
            transform.Scale = scale;

            IInGameObjectFactory<T> factory = new InGameObjectFactory<T>();
            T newObject = factory.Create(objectId, objectName, transform, layerIndex);

            if (colliderSize.HasValue)
                newObject.InitializeCollider(colliderSize.Value);

            return newObject;
        }
       
        private Vector2 DeterminePosition(PositionType posType, Vector2 posValue)
        {
            // 0.5f를 붙여주는 이유는 Json에 저장할때는 Tile의 가로행, 세로행으로 저장되었지만, Object를 Spawn할때는
            // 정중앙에서 생성하려 하므로. 
            return new Vector2(posValue.X + 0.5f, posValue.Y + 0.5f);

            //switch (posType)
            //{
            //    case PositionType.TileCenterPos:
            //        return CalculateTileCenter((int)posValue.X, (int)posValue.Y);
            //    case PositionType.TileUnderPos:
            //        return CalculateTileUnder((int)posValue.X, (int)posValue.Y);
            //    case PositionType.AbsolutePos:
            //        return posValue;
            //    default:
            //        return Vector2.Zero;
            //}
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
