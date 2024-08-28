using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


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

            if (obj.UnityObject != null)
            {
                GameObject.Destroy(obj.UnityObject);
            }
        }
        else
        {
            throw new IndexOutOfRangeException("Invalid layer index.");
        }
    }

    public void HandleSpawnObject(S_SpawnObject spawnObjectPacket)
    {
        // Position 정보를 가져옵니다.
        Vector2 position = DeterminePosition(spawnObjectPacket.Positioninfo);

        // 객체를 생성하고 Unity에 Instantiate합니다.
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/InGameObject/{spawnObjectPacket.Objecttype}");


        GameObject unityObject = GameObject.Instantiate(prefab, position, Quaternion.identity);

        // Player인 경우
        if (spawnObjectPacket.Objecttype == ObjectType.ObjectPlayer)
        {
            if (unityObject.GetComponentInChildren<Animator>() != null)
                Debug.Log("Player Animator Found!");

            foreach (var kvp in spawnObjectPacket.AdditionalData)
            {
                if (kvp.Key == ObjectSpawnKeyType.Character)
                {
                    var playerInfoValue = kvp.PlayerInfoVlaue;
                    CharacterType chartype = playerInfoValue.CharactertypeValue;

                    string chartype_string = chartype.ToString();
                    string additional_info_temp = "_Walk_Front_InGame";
                    chartype_string += additional_info_temp;
                    Debug.Log(chartype_string);

                    unityObject.GetComponentInChildren<Animator>().Play(chartype_string, -1, 0f);
                }
            }

             
        }

        InGameObject newObject = new InGameObject(spawnObjectPacket.Objectid, spawnObjectPacket.Objecttype.ToString());
        newObject.AttachUnityObject(unityObject);

        // 레이어에 추가하여 관리합니다.
        AddObjectToLayer(0, newObject); // 예시로 0번 레이어에 추가

        // 추가 로직: 필요시 스크립트에 접근하거나 초기화 작업 수행
    }

    private Vector2 DeterminePosition(PositionInfo positionInfo)
    {
        switch (positionInfo.Type)
        {
            case PositionType.TileCenterPos:
                return CalculateTileCenter((int)positionInfo.PosX, (int)positionInfo.PosY);
            case PositionType.TileUnderPos:
                return CalculateTileUnder((int)positionInfo.PosX, (int)positionInfo.PosY);
            case PositionType.AbsolutePos:
                return new Vector2(positionInfo.PosX, positionInfo.PosY);
            default:
                return Vector2.zero;
        }
    }

    private Vector2 CalculateTileCenter(int tileX, int tileY)
    {
        return new Vector2(tileX + 0.5f, tileY + 0.5f);
        //return new Vector2(tileX * 40 + 20, tileY * 40 + 20);
    }

    private Vector2 CalculateTileUnder(int tileX, int tileY)
    {
        return new Vector2(tileX * 40 + 20, tileY * 40);
    }

    public InGameObject FindObjectbyId(int id, LayerType? layerType = null)
    {
        if (layerType == null)
        {
            // 모든 Layer에서 id에 해당하는 object가 있는지 찾는다.
            for (int i = 0; i < _layerObjects.Length; ++i)
            {
                foreach(InGameObject obj in _layerObjects[i])
                {
                    if (obj.Id == id)
                        return obj;
                }
            }
        }

        else
        {
            // 특정 Layer에서 해당하는 object가 있는지 찾는다.
            int layeridx = (int)layerType;

            foreach (InGameObject obj in _layerObjects[layeridx])
            {
                if (obj.Id == id)
                    return obj;
            }
        }

        return null;
    }
}
