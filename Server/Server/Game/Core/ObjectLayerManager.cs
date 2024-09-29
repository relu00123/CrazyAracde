using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
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
                string layerName = Enum.GetName(typeof(LayerType), layerIndex);
                Console.WriteLine($"Adding Object in {layerName} (index: {layerIndex})");
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

        public void RemoveObjectFromLayer(InGameObject obj, LayerType layerType = LayerType.DefaultLayer)
        {
            //int layerIndex = -1;

            if (layerType == LayerType.DefaultLayer)
            {
                for (int i = 0; i < LayerCount; ++i)
                {
                    if (_layerObjects[i].Remove(obj))
                    {
                        string layerName = Enum.GetName(typeof(LayerType), obj._layeridx);
                        Console.WriteLine($"Deleting Object in {layerName} (index: {obj._layeridx})");
                        break;
                    }
                }
            }


            else
            {
                int layerIndex = (int)layerType;

                if (layerIndex >= 0 && layerIndex < LayerCount)
                {
                    if ( _layerObjects[layerIndex].Remove(obj))
                    {
                        string layerName = Enum.GetName(typeof(LayerType), obj._layeridx);
                        Console.WriteLine($"Deleting Object in {layerName} (index: {obj._layeridx})");
                    }
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

        public void RemoveReserveObjects()
        {
            foreach(var layer in _layerObjects)
            {
                layer.RemoveAll(obj => obj.isRemoveResreved);
            }
        }
    }
}
