using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

public class Player_IdleState : AbstractPlayerState, IPlayerEatItem
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
        gameObject.ChangeState(new Player_MovingState());
    }

    public override void EnterState(InGameObject obj, IObjectState previousState)
    {
        // Animation 변경을 위해서 추가한 코드 
        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation
        {
            ObjectId = obj.Id,
            PlayerAnim = PlayerAnimState.PlayerAnimIdle,
        };

        obj._possessGame._gameRoom.BroadcastPacket(changeAnimPkt);
        // Animation 변경을 위해서 추가한 코드 끝
    }

    public override void UpdateState(InGameObject gameObject)
    {

    }
    public override void ExitState(InGameObject obj)
    {

    }

    public void EatItem(InGameObject eatingObj, CAItemType itemType)
    {
        ItemEatHelper.DefaultEatItem(eatingObj, itemType);
    }
}
