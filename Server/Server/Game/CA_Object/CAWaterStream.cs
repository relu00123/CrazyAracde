using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

public class CAWaterStream : InGameObject
{
    private float existenceTime = 0.3f; // 물줄기가 존재할 시간 (초 단위)
    private float cur_Time = 0f;
    private DateTime lastUpdateTime;
    private IJob _job;

    public CAWaterStream(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {
        //this.position = position;

        // Collider 생성 (폭발로 인해 물줄기가 플레이어와 충돌할 때 처리)
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.8f, 0.8f));

        // 현재 시간 저장
        lastUpdateTime = DateTime.Now;
        
    }

    // 물줄기 업데이트
    public void WaterStreamUpdate()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsedTime = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        cur_Time += (float)elapsedTime.TotalSeconds;

        if (cur_Time > existenceTime)
        {
            DestroyWaterStream();
        }
        else
        {
            // 200ms 후 다시 업데이트 (일정 간격으로 업데이트)
            _job = _possessGame._gameRoom.PushAfter(50, WaterStreamUpdate);
        }
    }

    // 물줄기 객체가 플레이어와 충돌할 때 처리
    public void OnPlayerCollision(Player player)
    {
        // 플레이어를 물방울에 가두는 로직
        Console.WriteLine("Player trapped in water bubble!");

        // 플레이어를 갇히는 상태로 전환
        //player.TrapInWaterBubble();
    }

    // 물줄기 객체 파괴
    public void DestroyWaterStream()
    {
        // 수정된코드
        if (isRemoveResreved) return; 

        if (_job != null)
        {
            _job.Cancel = true;
            _job = null;
        }

        Console.WriteLine("Destroying water stream");

        // 충돌 처리 제거
        _collider = null;

        // 물줄기 제거 (클라이언트)
        S_DestroyObject destroyObject = new S_DestroyObject
        {
            ObjectId = this.Id
        };
        _possessGame._gameRoom.BroadcastPacket(destroyObject);


        // 물줄기 제거 (서버)
        _possessGame._objectsManager.DestroyObjectbyId(this.Id);
    }

    public override void OnBeginOverlap(InGameObject other)
    {
        //Console.WriteLine("ON BEGIN OVERLAP FUNCTION CALLED FROM WATERSTREAM!!");

        if (other is CAPlayer player && !(other._currentState is Player_DeadState))
        {
            //Console.WriteLine("물줄기랑 캐릭터와의 충돌 발생!!");

            // 이곳에서 캐릭터를 물방울에 갇힘 상태로 만들어 줘야 한다. 
            if (other._currentState is Player_MovingState)
                other.ChangeState(new Player_Bubble_MovingState());
            else
                other.ChangeState(new Player_Bubble_IdleState());


            // Player가 물방울에 갇히는 Sound를 재생하도록 해야한다. 
            S_PlaySoundEffect soundEffectPacket = new S_PlaySoundEffect
            {
                SoundEffectType = SoundEffectType.PlayerBubbleSoundEffect
            };

            _possessGame._gameRoom.BroadcastPacket(soundEffectPacket);
        }
    }
}
