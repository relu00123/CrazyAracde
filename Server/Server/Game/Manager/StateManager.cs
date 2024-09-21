using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;


public static class StateManager
{
    // 상태 매핑을 위한 static 자료구조
    private static Dictionary<Type, CreatureState> _stateMapping = new Dictionary<Type, CreatureState>
    {
        { typeof(Player_IdleState), CreatureState.Idle },
        { typeof(Player_MovingState), CreatureState.Moving },
        { typeof(Player_Bubble_IdleState), CreatureState.BubbleIdle },
        { typeof(Player_Bubble_MovingState), CreatureState.BubbleMoving },
        { typeof(Player_DeadState), CreatureState.Dead }
    };

    // IObjectState에서 CreatureState로의 변환 함수
    public static CreatureState GetCreatureStateFromObjectState(IObjectState objectState)
    {
        Type stateType = objectState.GetType();
        return _stateMapping.ContainsKey(stateType) ? _stateMapping[stateType] : CreatureState.Idle;
    }
}