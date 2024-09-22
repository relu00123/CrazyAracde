using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

 

public class Player_MovingState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        if (dir == MoveDir.MoveNone)
        {
            Console.Write("Stop Walking (Move Key detached)");

            // Idle상태로 바꿔준다. 
            gameObject.ChangeState(new Player_IdleState());
            return;
        }

        base.ApplyMove(gameObject, dir);
        gameObject.ChangeState(new Player_MovingState());
    }

    public override void EnterState(InGameObject obj, IObjectState previousState)
    {
        // Animation 변경을 위해서 추가한 코드 
        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation
        {
            ObjectId = obj.Id,
            PlayerAnim = PlayerAnimState.PlayerAnimMoving,
        };

        obj._possessGame._gameRoom.BroadcastPacket(changeAnimPkt);
        // Animation 변경을 위해서 추가한 코드 끝
    }

    public override void UpdateState(InGameObject gameObject)
    {
        gameObject.UpdateMoving();
    }

    public override void ExitState(InGameObject obj)
    {

    }
}
