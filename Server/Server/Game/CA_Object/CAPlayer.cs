using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;


public class CAPlayer : InGameObject
{
    public CAPlayer(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {
        // Player에 필요한 Collider 생성
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));

        _currentState = new Player_IdleState();
    }

    IJob _job;

    public double _bubbledTime { get; set;}
    public bool   _bubbleEmergencyAnimChanged { get; set; }

    public CharacterType _characterType { get; set; }
    

    public override void Update()
    {
        //Console.WriteLine($"Update Function Called! (Object ID : {Id}) (CurPosition : {_transform.Position})");

        if (_possessGame != null)
        {
            if (_currentState != null)
            {
                _currentState.UpdateState(this);
            }


            else
            {
                switch (_state)
                {
                    case CreatureState.Idle:
                        UpdateIdle();
                        break;
                    case CreatureState.Moving:
                        UpdateMoving();
                        break;
                    case CreatureState.BubbleMoving:
                        UpdateMoving();
                        break;
                    case CreatureState.BubbleIdle:
                        UpdateIdle();
                        break;
                }
            }

             _job =  _possessGame._gameRoom.PushAfter(10, Update);
        }
    }

    public override void UpdateIdle()
    {

    }

    public override void UpdateMoving()
    {
        base.UpdateMoving();
    }

    public override void OnBeginOverlap(InGameObject other)
    {
        //Console.WriteLine("ON BEGIN OVERLAP FUNCTION CALLED FROM CHARACTER!!");

        if (other is CAPlayer)
        {
            //Console.WriteLine("캐릭터와 출돌 발생");

            if (_currentState is Player_Bubble_IdleState || _currentState is Player_Bubble_MovingState )
            {
                Vector2 pos1 = _collider.GetColliderCenterPos();
                Vector2 pos2 = other._collider.GetColliderCenterPos();

                float distance = Vector2.Distance(pos1, pos2);

                if (distance < 0.45f)
                {
                    CAPlayer ohterPlayer = other as CAPlayer;

                    if (ohterPlayer._characterType == _characterType)
                    {
                        // Console.WriteLine("다른 캐릭터가 자신을 살리는 경우");
                        // 살아날때 에니메이션도 재생시켜주도록 하자. 
                        ChangeState(new Player_IdleState());

                        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation()
                        {
                            ObjectId = Id,
                            PlayerAnim = PlayerAnimState.PlayerAnimBubbleEscape,
                        };

                        _possessGame._gameRoom.BroadcastPacket(changeAnimPkt);
                    }

                    else
                    {
                        // Console.WriteLine("다른 캐릭터가 자신을 죽이는 경우");
                        ChangeState(new Player_DeadState());
                    }
                }
            }
        }
    }
}

