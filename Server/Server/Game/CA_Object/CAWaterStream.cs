﻿using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

public class CAWaterStream : InGameObject
{
    private float existenceTime = 1.0f; // 물줄기가 존재할 시간 (초 단위)
    private float cur_Time = 0f;
    private DateTime lastUpdateTime;
    private IJob _job;

    public CAWaterStream(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {
        //this.position = position;

        // Collider 생성 (폭발로 인해 물줄기가 플레이어와 충돌할 때 처리)
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));

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
            _job = _possessGame._gameRoom.PushAfter(100, WaterStreamUpdate);
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
}