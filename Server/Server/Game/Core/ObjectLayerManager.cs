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

        public void RemoveObjectFromLayer(int layerIndex, InGameObject obj)
        {
            if (layerIndex >= 0 && layerIndex < LayerCount)
            {
                _layerObjects[layerIndex].Remove(obj);
            }
            else
            {
                throw new IndexOutOfRangeException("Invalid layer index.");
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
    }
}
