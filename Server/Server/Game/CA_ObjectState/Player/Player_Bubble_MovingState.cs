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

    public override void EnterState(InGameObject obj, IObjectState previousState)
    {
        obj._moveSpeed = (obj._moveSpeed / 2f);

        // 수정중인 코드
        CAPlayer player = obj as CAPlayer;

        if (!(previousState is Player_Bubble_MovingState || previousState is Player_Bubble_IdleState))
        {
            // bubble에 갇힌 시간 초기화
            player._bubbledTime = 0f;
            player._bubbleEmergencyAnimChanged = false;
        }

        // 수정중인 코드 끝 


        // Animation 변경을 위해서 추가한 코드 
        if (!(previousState is Player_Bubble_MovingState || previousState is Player_Bubble_IdleState))
        {

            S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation
            {
                ObjectId = obj.Id,
                PlayerAnim = PlayerAnimState.PlayerAnimBubble,
            };

            obj._possessGame._gameRoom.BroadcastPacket(changeAnimPkt);
        }
         
        
        // Animation 변경을 위해서 추가한 코드 끝

    }
    public override void UpdateState(InGameObject gameObject)
    {
        gameObject.UpdateMoving();

        CAPlayer player = gameObject as CAPlayer;

        player._bubbledTime += player._possessGame._gameRoom._deltaTime;

        if (player._bubbledTime > 3f && player._bubbleEmergencyAnimChanged == false)
        {
            player._bubbleEmergencyAnimChanged = true;
            Console.WriteLine("ToEmergencyAnim!");

            S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation()
            {
                ObjectId = gameObject.Id,
                PlayerAnim = PlayerAnimState.PlayerAnimBubbleEmergency,
            };

            gameObject._possessGame._gameRoom.BroadcastPacket(changeAnimPkt);
        }


        if (player._bubbledTime > 4f)
        {
            Console.WriteLine("물방울이 터져야합니다!!!");
            gameObject.ChangeState(new Player_DeadState());
        }
    }

    public override void ExitState(InGameObject obj)
    {
        obj._moveSpeed = (obj._moveSpeed * 2f);
    }
    

    
}
