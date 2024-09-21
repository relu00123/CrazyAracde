using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

public class Player_Bubble_MovingState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        if (dir == MoveDir.MoveNone)
        {
            Console.Write("Stop Walking (Move Key detached)");

            // Bubble Idle상태로 바꿔준다. 
            gameObject.ChangeState(new Player_Bubble_IdleState());
            return;
        }

        base.ApplyMove(gameObject, dir);
        gameObject.ChangeState(new Player_Bubble_MovingState());
    }

    public override void EnterState(InGameObject obj)
    {

    }

    public override void ExitState(InGameObject obj)
    {

    }
}
