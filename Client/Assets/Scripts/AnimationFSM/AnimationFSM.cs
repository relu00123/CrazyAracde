using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AnimationFSM<TState, TOwnerController> : MonoBehaviour
    where TState : Enum
    where TOwnerController : CABaseController
{
    public  Animator _animator { get; set; }
    public TState _currentAnimState { get; set; }
    public TOwnerController _ownerController { get; set; }

    public AnimationFSM(TOwnerController ownerController)
    {
        _currentAnimState = default(TState);
        _ownerController = ownerController;
    }

    public virtual void SetState(TState newState)
    {
        if (_currentAnimState.Equals(newState))
            return;

        _currentAnimState = newState;
        UpdateAnimator();
    }

    public abstract void UpdateAnimator();  // 추상 메서드로 정의
}
