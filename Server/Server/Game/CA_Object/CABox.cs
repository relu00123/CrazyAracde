﻿using Server.Game.CA_Object;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Google.Protobuf.Protocol;


public class CABox : InGameObject
{
    public Vector2Int position { get; set; } = new Vector2Int();
    public CABox(int id, string name, Transform transform, int layer)
       : base(id, name, transform, layer)
    {
        // Bomb에 필요한 Collider 생성 ?  아직 필요한지 정확하게 파악안됨.
        // 폭탄이 터질때 Collision System을 어떻게 만들지에 따라서 갈릴듯. 
        //_collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
        //lastUpdateTime = DateTime.Now;
    }
 
    public void DestroyBox()
    {
        //Console.WriteLine($"Destroy Box Function Called. Pos : <{_transform.Position.X},{_transform.Position.Y}>");

        // 1. 클라이언트에서 이 클래스의 ObjectID를 가지고 있는 객체를 찾아서 destroy
        S_DestroyBox destroyBoxPkt = new S_DestroyBox()
        {
            ObjectId = this.Id
        };

        _possessGame._gameRoom.BroadcastPacket(destroyBoxPkt);

        // 2. Server에서 TileMap의 공간을 비워주고 InGameObject도 null로 설정
        TileInfo installedTileData = _possessGame._caMapManager._tileMapData[position.x, position.y];
        installedTileData.isBlocktPermanently = false;
        installedTileData.inGameObject = null;

        // 3. ObjectLayerManager에서도 이 오브젝트를 삭제
        _possessGame._objectsManager.DestroyObjectbyId(this.Id);
    }
}
