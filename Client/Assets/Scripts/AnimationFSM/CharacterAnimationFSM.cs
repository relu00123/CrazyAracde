using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterAnimationFSM : AnimationFSM<CreatureState, CAPlayerController>
{
     public CharacterAnimationFSM( CAPlayerController ownerController)
        : base(ownerController) { }


    public override void UpdateAnimator()
    {
        Debug.Log("Update Animatior Called!");

        CharacterType charType = _ownerController._charType;
        MoveDir dir = _ownerController.Dir;
        SpriteRenderer spriteRenderer = _ownerController._spriteRenderer;

        string animation_name = charType.ToString();

        switch (_currentAnimState)
        {
            case CreatureState.Idle:
                animation_name += "_Idle";
                break;
            case CreatureState.Moving:
                animation_name += "_Walk";
                break;
        }

        switch (dir)
        {
            case MoveDir.Up:
                animation_name += "_Back";
                spriteRenderer.flipX = false;
                break;
            case MoveDir.Down:
                animation_name += "_Front";
                spriteRenderer.flipX = false;
                break;
            case MoveDir.Left:
                animation_name += "_Side";
                spriteRenderer.flipX = true;
                break;
            case MoveDir.Right:
                animation_name += "_Side";
                spriteRenderer.flipX = false;
                break;
            case MoveDir.MoveNone:
                animation_name += "_Back";
                break;
        }

        animation_name += "_InGame";

        // 수정된 코드
        AnimatorStateInfo currentAnimationState = _animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름 비교
        if (!currentAnimationState.IsName(animation_name))
        {
            _animator.Play(animation_name);
        }
    }
}
