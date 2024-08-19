using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class InGame
    {
        private GameRoom _gameRoom;

        private int _nextObjectId = 1;

        private ObjectLayerManager _objectLayerManager;


        public InGame(GameRoom gameRoom)  // 나중에 필요한 정보들 추가해서 구조체로 바꿀 수도 있음. 
        {
            
            _gameRoom = gameRoom;
            _objectLayerManager = new ObjectLayerManager();
        }

        public InGameObject CreateObject(int layerIndex, string objectName)
        {
            int objectId = _nextObjectId++;
            InGameObject newObject = new InGameObject(objectId, objectName, layerIndex);
            _objectLayerManager.AddObjectToLayer(layerIndex, newObject);
            return newObject;
        }

        public List<InGameObject> GetObjectsInLayer(int layerIndex)
        {
            return _objectLayerManager.GetObjectsInLayer(layerIndex);
        }


    }
}
