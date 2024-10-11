using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAMyPlayerController : CAPlayerController
{
    private float move_packet_coolTime = 0.1f;
    private float last_sent_move_packet = 0f;

    private float bomb_packet_coolTime = 0.1f;
    private float last_sent_bomb_packet = 0f; 


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
        switch (ObjState)
        {
            case CreatureState.Idle:
                HandleMovementInput();
                HandleBombInput();
                break;
            case CreatureState.Moving:
                HandleMovementInput();
                HandleBombInput();
                break;
            case CreatureState.BubbleIdle:
                HandleMovementInput();
                break;
            case CreatureState.BubbleMoving:
                HandleMovementInput();
                break;
        }

        base.UpdateController();
    }

    public void HandleMovementInput()
    {
        if (Time.time - last_sent_move_packet >= move_packet_coolTime)
        {
            currentInputDir = GetDirInput();

            // 이동방향키에서 손을 뗀 순간
            if (currentInputDir == MoveDir.MoveNone && previousInputDir != MoveDir.MoveNone)
            {
                SendMovePacket(MoveDir.MoveNone);
            }

            // 이동방향키중 하나를 누르고 있는 경우
            else if (currentInputDir != MoveDir.MoveNone)   
            {
                SendMovePacket(currentInputDir);
            }

            // 이전 입력 상태를 현재 상태로 업데이트
            previousInputDir = currentInputDir;
        }
    }

    private void HandleBombInput()
    {
        // 서버 부하 방지용 쿨타임. 
         if (Time.time - last_sent_bomb_packet >= bomb_packet_coolTime)
        {
            // 폭탄 설치 키를 입력했는지 확인
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 폭탄을 설치해달라고 하는 패킷을 보낸다. 
                C_InstallBomb installBombPkt = new C_InstallBomb()
                {
                    Power = _ballon.power,
                    BombType = _ballon.bombSkinType,
                    CharPosX = InGameObj.UnityObject.transform.position.x,
                    CharPosY = InGameObj.UnityObject.transform.position.y,
                };

                Managers.Network.Send(installBombPkt);
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

    void SendMovePacket(MoveDir dir)
    {
        C_CaMove movepkt = new C_CaMove();

        movepkt.Dir = dir;
        movepkt.Clientposx = InGameObj.UnityObject.transform.position.x;
        movepkt.Clientposy = InGameObj.UnityObject.transform.position.y;

        //var pos = InGameObj.UnityObject.GetComponent<Transform>().position;
        //float temp = pos.x;


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
 