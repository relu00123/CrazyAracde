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
    public virtual void ApplyMove(InGameObject gameObject, MoveDir moveDir)
    {
        Vector2 originalPosition = gameObject._transform.Position;
        Console.WriteLine($"Cur Obj Pos : ({gameObject._transform.Position.X},{gameObject._transform.Position.Y})");

        // 방향키 입력에 따른 캐릭터의 속도를 적용해서 TargetPosition을 구한다. 
        Vector2 targetPosition = gameObject.CalculateTargetPositon(moveDir);


        gameObject._targetPos = targetPosition;
        gameObject.Direction = moveDir;
        //gameObject.ChangeState(CreatureState.Moving);
    }

     


    public abstract void EnterState(InGameObject gameObject, IObjectState previousState);
    public abstract void ExitState(InGameObject gameObject);
    public abstract void UpdateState(InGameObject gameObject);
}
