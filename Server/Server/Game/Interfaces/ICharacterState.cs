using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

public interface ICharacterState
{
    void ApplyMove(InGameObject gameObject, MoveDir dir);
    void EnterState(InGameObject gameObject);
    void ExitState(InGameObject gameObject);
}
