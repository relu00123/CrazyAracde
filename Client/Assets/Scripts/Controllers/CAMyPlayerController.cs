using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAMyPlayerController : CAPlayerController
{
    float move_packet_coolTime = 0.1f;
    float last_sent_move_packet = 0f;

    private MoveDir currentInputDir = MoveDir.MoveNone;
    private MoveDir previousInputDir = MoveDir.MoveNone;

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

    protected override void UpdateController()
    {
        if (Time.time - last_sent_move_packet >= move_packet_coolTime)
        {
            // 기존 로직 (09.04까지)
            // 이동 패킷을 보낼 수 있다.
            // 이동키를 누르고 있는지 계산한다.
            //MoveDir CurInputDir = GetDirInput();

            //if (CurInputDir != MoveDir.MoveNone)
            //{
            //    // 이동 패킷을 보낸다. 
            //    SendMovePacket(CurInputDir, CreatureState.Idle);
            //}



            // 새로운 로직 (09.05부터) 
            currentInputDir = GetDirInput();

            if (currentInputDir == MoveDir.MoveNone && previousInputDir != MoveDir.MoveNone)
            {
                SendMovePacket(MoveDir.MoveNone, CreatureState.Idle);
            }

            else if (currentInputDir != MoveDir.MoveNone)
            {
                SendMovePacket(currentInputDir, CreatureState.Idle);
            }

            // 이전 입력 상태를 현재 상태로 업데이트
            previousInputDir = currentInputDir;


        }
        base.UpdateController();
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
       // Debug.Log($"{InGameObj.Name}");
       // Debug.Log($"Move packet sent : {dir} , CurObject Pos : {transform.position.x} {transform.position.y}");

        Vector3 targetpos = CalculateNextPosByInput(dir, transform.position);

        PositionInfo posinfo = new PositionInfo
        {
            
            MoveDir = dir,
            PosX = targetpos.x,
            PosY = targetpos.y,
            //PosX = Player.CurrentPos.Value.x,
            //PosY = Player.CurrentPos.Value.y,
            State = state,
        };

        C_Move movepkt = new C_Move();
        movepkt.PosInfo = posinfo;

        Managers.Network.Send(movepkt);
        last_sent_move_packet = Time.time;
    }

    private Vector3 CalculateNextPosByInput(MoveDir dir, Vector3 curpos)
    {
        Vector3 nextpos = new Vector3();

        nextpos = curpos;

        switch (dir)
        {
            case MoveDir.Left:
                nextpos.x = curpos.x - 0.3f;
                nextpos.y = curpos.y;
                break;
            case MoveDir.Right:
                nextpos.x = curpos.x + 0.3f;
                nextpos.y = curpos.y;
                break;
            case MoveDir.Up:
                nextpos.x = curpos.x;
                nextpos.y = curpos.y + 0.3f;
                break;
            case MoveDir.Down:
                nextpos.x = curpos.x;
                nextpos.y = curpos.y - 0.3f;
                break;
        }

        return nextpos;

    }


}
 