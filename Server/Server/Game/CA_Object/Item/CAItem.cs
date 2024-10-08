using Server.Game.CA_Object;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Google.Protobuf.Protocol;
using Server;



public class CAItem : InGameObject
{
    public CAItemType itemType { get; set; }
    public Vector2Int position { get; set; } = new Vector2Int();
    public CAItem(int id, string name, Transform transform, int layer)
       : base(id, name, transform, layer)
    {
        // Collider 부분은 나중에 작업 
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
        //lastUpdateTime = DateTime.Now;
    }

    public override void OnBeginOverlap(InGameObject other)
    {
        if (other is CAPlayer player && other._currentState is IPlayerEatItem eatableState)
        {
            // 0. 충분히 가까울때만 Item을 먹을 수 있도록 하기
            Vector2 pos1 = _collider.GetColliderCenterPos();
            Vector2 pos2 = other._collider.GetColliderCenterPos();

            float distance = Vector2.Distance(pos1, pos2);

            if (distance > 0.65f) return;

            eatableState.EatItem(other, itemType);

            // 먹었을때 사운드 재생 (다만 해당 플레이어만 Sound재생 하고 싶다.)  - 공용함수 만들어야 할듯? 
            ClientSession clientSession = player._possessGame.FindOwnerClient(player);
            if (clientSession != null)
            {
                S_PlaySoundEffect soundEffectPacket = new S_PlaySoundEffect
                {
                    SoundEffectType = SoundEffectType.EatItemSoundEffect
                };

                clientSession.Send(soundEffectPacket);
            }


            // 1. 클라이언트에서 아이템을 삭제해준다. 
            S_DestroyObject destroyObject = new S_DestroyObject
            {
                ObjectId = this.Id
            };

            _possessGame._gameRoom.BroadcastPacket(destroyObject);

            // 2. 서버에서 아이템을 삭제해준다. 
            TileInfo InstalledTileData = _possessGame._caMapManager._tileMapData[position.x, position.y];
            if (InstalledTileData.inGameObject is CAItem)
                InstalledTileData.inGameObject = null;

            _possessGame._objectsManager.DestroyObjectbyId(this.Id);
        }
    }
}