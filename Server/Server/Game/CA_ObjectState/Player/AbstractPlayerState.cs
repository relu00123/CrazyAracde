using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using Server.Game.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

 

public abstract class AbstractPlayerState : IObjectState
{
    public virtual void ApplyMove(InGameObject gameObject, C_CaMove movePkt, MoveDir moveDir)
    {
        Vector2 originalPosition = gameObject._transform.Position;
        Console.WriteLine($"Cur Obj Pos : ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");


        // 10.11
        // Client에서 손을 땐 경우에 발생한다. 
        // 이때는 Client에서 보낸 위치와 , 서버에서 받은 시간의 차이만큼을 targetPosition으로 잡아야 한다.
        // 물론 이 과정에서 충돌이 발생해서 이동할 수 없다면 이동을 시켜주면 안된다. 
        // 우선은 간단하게 계산안하고 바로 멈추는 것으로 시작해보자. 
        if (moveDir == MoveDir.MoveNone)
        {
            Vector2 ClientInitialPos = new Vector2(movePkt.Clientposx, movePkt.Clientposy);
            gameObject._targetPos = ClientInitialPos;
        }

        else
        {
            // 방향키 입력에 따른 캐릭터의 속도를 적용해서 TargetPosition을 구한다. 
            Vector2 targetPosition = gameObject.CalculateTargetPositon(moveDir, movePkt.Clientposx, movePkt.Clientposy);


            gameObject._targetPos = targetPosition;
            gameObject.Direction = moveDir;
            //gameObject.ChangeState(CreatureState.Moving);
        }
    }

    public abstract void EnterState(InGameObject gameObject, IObjectState previousState);
    public abstract void ExitState(InGameObject gameObject);
    public abstract void UpdateState(InGameObject gameObject);
}
