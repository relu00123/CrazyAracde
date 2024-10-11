using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;



public interface IObjectState
{
    void ApplyMove(InGameObject gameObject, C_CaMove movepkt,  MoveDir dir);
    //void ApplyMove(InGameObject gameObject,  MoveDir dir);
    void EnterState(InGameObject gameObject, IObjectState previousState);
    void ExitState(InGameObject gameObject);
    void UpdateState(InGameObject gameObject);
}
