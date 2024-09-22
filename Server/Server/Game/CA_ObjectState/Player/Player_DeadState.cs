using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

 
public class Player_DeadState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        // 죽은 상태에서는 움직일 수 없다. 
        return;
    }

    public override void EnterState(InGameObject obj, IObjectState previousState)
    {
        // Animation 변경을 위해서 추가한 코드 
        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation
        {
            ObjectId = obj.Id,
            PlayerAnim = PlayerAnimState.PlayerAnimDead,
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
}
