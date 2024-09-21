using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

 
public class Player_Bubble_IdleState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        if (dir == MoveDir.MoveNone)
        {
            Console.Write("Stop Walking (Move Key detached)");

            // 아무 행동도 안해도 될 것 같다. 어처피 Idle상태였으니까
            // gameObject.ChangeState(CreatureState.Idle);
            return;
        }

        base.ApplyMove(gameObject, dir);
        gameObject.ChangeState(new Player_Bubble_MovingState());

    }

    public override void EnterState(InGameObject obj)
    {
        // 물방울에 갇히면 기존속도의 반의 속도로 움직여야 한다.
        obj._moveSpeed = (obj._moveSpeed / 2f);
    }

    public override void ExitState(InGameObject obj)
    {
        // 원래 속도로 돌려준다.
        obj._moveSpeed = (obj._moveSpeed * 2f);
    }
}
