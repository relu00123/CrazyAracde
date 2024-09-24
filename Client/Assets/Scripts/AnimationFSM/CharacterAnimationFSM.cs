using Google.Protobuf.Protocol;
using UnityEngine;

public class CharacterAnimationFSM : AnimationFSM<PlayerAnimState, CAPlayerController>
{
     public CharacterAnimationFSM( CAPlayerController ownerController)
        : base(ownerController) { }


    public override void UpdateAnimator()
    {
        Debug.Log("Update Animatior Called!");

        string animation_name = "";

        if (_currentAnimState == PlayerAnimState.PlayerAnimBubble)
            animation_name = GetAnimNameForBubbleState();
        else if (_currentAnimState == PlayerAnimState.PlayerAnimBubbleEmergency)
            animation_name = GetAnimNameForBubbleState(true);
        else if (_currentAnimState == PlayerAnimState.PlayerAnimIdle || _currentAnimState == PlayerAnimState.PlayerAnimMoving)
            animation_name = GetAnimNameForNoneBubbleState();
        else if (_currentAnimState == PlayerAnimState.PlayerAnimDead)
            animation_name = GetAnimNameForDeadState();
        else if (_currentAnimState == PlayerAnimState.PlayerAnimBubbleEscape)
            animation_name = GetAnimNameForBubbleEscape();
        
        // 수정된 코드
        AnimatorStateInfo currentAnimationState = _animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름 비교
        if (!currentAnimationState.IsName(animation_name))
        {
            _animator.Play(animation_name);
        }
    }

    private string GetAnimNameForBubbleEscape()
    {
        CharacterType charType = _ownerController._charType;
        SpriteRenderer spriteRenderer = _ownerController._spriteRenderer;
        string animation_name = charType.ToString();

        animation_name += "_Bubble_Escape";

        return animation_name;
    }

    private string GetAnimNameForBubbleState(bool isEmergency = false)
    {
        CharacterType charType = _ownerController._charType;
        SpriteRenderer spriteRenderer = _ownerController._spriteRenderer;
        string animation_name = charType.ToString();

        animation_name += "_Bubble";

        if (isEmergency)
            animation_name += "_Emergency";

 
        return animation_name;
    }

    private string GetAnimNameForDeadState()
    {
        CharacterType charType = _ownerController._charType;
        MoveDir dir = _ownerController.Dir;
        SpriteRenderer spriteRenderer = _ownerController._spriteRenderer;
        string animation_name = charType.ToString();

        animation_name += "_Dead";

        return animation_name;
    }


    private string GetAnimNameForNoneBubbleState()
    {

        CharacterType charType = _ownerController._charType;
        MoveDir dir = _ownerController.Dir;
        SpriteRenderer spriteRenderer = _ownerController._spriteRenderer;
        string animation_name = charType.ToString();

        switch (_currentAnimState)
        {
            case PlayerAnimState.PlayerAnimIdle:
                animation_name += "_Idle";
                break;
            case PlayerAnimState.PlayerAnimMoving:
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



        return animation_name;
    }
}
