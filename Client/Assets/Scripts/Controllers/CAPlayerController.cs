using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAPlayerController : CABaseController
{
    public Animator _animator { get; set; }
    public SpriteRenderer _spriteRenderer { get; set; }

    public CharacterType _charType { get; set; }


   // bool _moveKeyPressed = false;

    //CACreatureState characterstate;

    public virtual void Test()
    {
        Debug.Log("TestFunction Called from PlayerController");
        //if (_animator == null)
        //{
        //    Debug.Log("Animator Not Found!");
        //}

        //else
        //{
        //    Debug.Log("Animator Found!");
        //}
    }

    protected override void Init()
    {
        base.Init();

    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();

        string animation_name = _charType.ToString();

        switch (State)
        {
            case CreatureState.Idle:
                animation_name += "_Idle";
                break;
            case CreatureState.Moving:
                animation_name += "_Walk";
                break;
        }

        switch (Dir)
        {
            case MoveDir.Up:
                animation_name += "_Back";
                _spriteRenderer.flipX = false;
                break;
            case MoveDir.Down:
                animation_name += "_Front";
                _spriteRenderer.flipX = false;
                break;
            case MoveDir.Left:
                animation_name += "_Side";
                _spriteRenderer.flipX = true;
                break;
            case MoveDir.Right:
                animation_name += "_Side";
                _spriteRenderer.flipX = false;
                break;
            case MoveDir.MoveNone:
               animation_name += "_Back";
                break;
        }

        animation_name += "_InGame";

        Debug.Log($"cur animation name : {animation_name}");


        // 수정된 코드
        AnimatorStateInfo currentAnimationState = _animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름 비교
        if (!currentAnimationState.IsName(animation_name))
        {
            Debug.Log($"Playing new animation: {animation_name}");
            _animator.Play(animation_name);
        }

        // 기존 코드 
        //_animator.Play(animation_name);

        // Animation 이름을 잘 확인해야 한다. 


    }



    //void Update()
    //{
    //    UpdateController();
    //}

    //public void UpdateController()
    //{
    //    switch (characterstate)
    //    {
    //        case CACreatureState.CaIdle:
    //            break;
    //        case CACreatureState.CaMoving:
    //            break;
    //        case CACreatureState.CaDead:
    //            break;
    //    }
    //}

    //void GetDirInput()
    //{
    //    _moveKeyPressed = true;


    //}
}
 
