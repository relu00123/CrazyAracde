using System;
using System.Collections.Generic;
using System.Text;
using Server.Game.CA_Object;

namespace Server.Game
{
    public class ObjectLayerManager
    {
        public const int LayerCount = 30;
        private List<InGameObject>[] _layerObjects = new List<InGameObject>[LayerCount];

        public ObjectLayerManager()
        {
            for (int i = 0; i < LayerCount; i++)
            {
                _layerObjects[i] = new List<InGameObject>();
            }
        }

        public void AddObjectToLayer(int layerIndex, InGameObject obj)
        {
            if (layerIndex >= 0 && layerIndex < LayerCount)
            {
                _layerObjects[layerIndex].Add(obj);
            }
            else
            {
                throw new IndexOutOfRangeException("Invalid layer index.");
            }
        }

        public bool RemoveObjectFromLayer(int objectid)
        {
            InGameObject obj = FindObjectById(objectid);

            if (obj == null)
                return false;

            RemoveObjectFromLayer(obj);

            return true;
        }

        public void RemoveObjectFromLayer(InGameObject obj, int layerIndex = -1)
        {
            if (layerIndex == -1)
            {
                for (int i = 0; i < LayerCount; ++i)
                {
                    if (_layerObjects[i].Remove(obj))
                        break;
                }
            }


            else
            {
                if (layerIndex >= 0 && layerIndex < LayerCount)
                {
                    _layerObjects[layerIndex].Remove(obj);
                }
            }

             
        }

        public List<InGameObject> GetObjectsInLayer(int layerIndex)
        {
            if (layerIndex >= 0 && layerIndex < LayerCount)
            {
                return _layerObjects[layerIndex];
            }
            else
            {
                throw new IndexOutOfRangeException("Invalid layer index.");
            }
        }

        public InGameObject FindObjectById(int objectId)
        {
            for (int i = 0; i < LayerCount; ++i)
            {
                for (int j = 0; j < _layerObjects[i].Count; ++j)
                {
                    if (_layerObjects[i][j].Id == objectId) 
                        return _layerObjects[i][j];
                }
            }

            return null;
        }
    }
}
