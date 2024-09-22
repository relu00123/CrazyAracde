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
        Console.WriteLine("ON BEGIN OVERLAP FUNCTION CALLED FROM CHARACTER!!");
    }
}

