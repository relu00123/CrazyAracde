using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

 public class CABomb : InGameObject
{
    private float explode_coolTime = 3.0f;
    private float cur_Time = 0f;
    private DateTime lastUpdateTime;
    public Vector2Int position { get; set; } = new Vector2Int();
    public int power { get; set; }
    public CAPlayer bombOwner { get; set; }

    public IJob _job;

    public CABomb(int id, string name, Transform transform, int layer)
       : base(id, name, transform, layer)
    {
        // Bomb에 필요한 Collider 생성 ?  아직 필요한지 정확하게 파악안됨.
        // 폭탄이 터질때 Collision System을 어떻게 만들지에 따라서 갈릴듯. 
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
        lastUpdateTime = DateTime.Now;
    }


    public void Bomb_update()
    {
        Console.WriteLine("Bomb Update");

        DateTime currentTime = DateTime.Now;
        TimeSpan elapsedTime = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        cur_Time += (float)elapsedTime.TotalSeconds;

        if (cur_Time > explode_coolTime)
        {
            Explode();
        }

        else
        {
            _job = _possessGame._gameRoom.PushAfter(200, Bomb_update);
        }
    }

    public void Explode()
    {
        if (_job == null || _job.Cancel == true) return;

        Console.WriteLine("Explode!");
        
        _job.Cancel = true;
        _job = null;

        if (bombOwner != null)
            bombOwner.Stats.IncreaseCurBombCount();

        // 0. 본인과, 연계된 지역의 WaterStream 객체를 생성 및 Client에 Broadcast 
        // 이 일은 Bomb에서 해주는 것이 아니라 CAMapManager에서 해줘야 한다. 
        _possessGame._caMapManager.ExplodeBomb(this);  // 테스트중

        // 1. 클라이언트에서 이 클래스의 ObjectId를 가지고 있는 객체를 찾아서 Destroty
        S_DestroyObject destroyObject = new S_DestroyObject
        {
            ObjectId = this.Id
        };

        _possessGame._gameRoom.BroadcastPacket(destroyObject);

        // 2. Server에서 TileMap의 공간을 비워주고 inGameObject도 null로 설정.
        TileInfo InstalledTileData = _possessGame._caMapManager._tileMapData[position.x, position.y];
        InstalledTileData.isBlocktTemporary = false;
        InstalledTileData.inGameObject = null;

         
        // 3. ObjectLayerManager에서도 이를 삭제
        _possessGame._objectsManager.DestroyObjectbyId(this.Id);

    }
}