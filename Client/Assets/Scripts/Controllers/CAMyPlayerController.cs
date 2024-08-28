using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAMyPlayerController : CAPlayerController
{
    float move_packet_coolTime = 2f;
    float last_sent_move_packet = 0f;

    public override void Test()
    {
        Debug.Log("TestFunction Called from MyPlayerController");

    }

    enum MoveDirType
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }


    private void Update()
    {
        if (Time.time - last_sent_move_packet >= move_packet_coolTime)
        {
            // 이동 패킷을 보낼 수 있다.
            // 이동키를 누르고 있는지 계산한다.
            MoveDir CurInputDir = GetDirInput();

            if (CurInputDir != MoveDir.MoveNone )
            {
                // 이동 패킷을 보낸다. 
                SendMovePacket(CurInputDir, CreatureState.Idle);
            }    

        }
    }

     MoveDir GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
            return MoveDir.Up;
        if (Input.GetKey(KeyCode.A))
            return MoveDir.Left;
        if (Input.GetKey(KeyCode.D))
            return MoveDir.Right;
        if (Input.GetKey(KeyCode.S))
            return MoveDir.Down;
        else
            return MoveDir.MoveNone;
    }

    void SendMovePacket(MoveDir dir, CreatureState state)
    {
        Debug.Log($"{Player.Name}");

        if (Player.CurrentPos.HasValue)
        {
            Debug.Log($"Move packet sent : {dir} , CurObject Pos : {Player.CurrentPos.Value.x} {Player.CurrentPos.Value.y}");

            PositionInfo posinfo = new PositionInfo
            {
                MoveDir = dir,
                PosX = Player.CurrentPos.Value.x,
                PosY = Player.CurrentPos.Value.y,
                State = state,
            };

            C_Move movepkt = new C_Move();
            movepkt.PosInfo = posinfo;

            Managers.Network.Send(movepkt);
            last_sent_move_packet = Time.time;
        }
        else
        {
            Debug.Log("CurrentPos is null");
        }
    }


}
 