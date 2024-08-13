using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CACharacter : MonoBehaviour
{
    //enum CharacterType
    //{
    //    Dao,
    //    Marid,
    //    Bazzi,
    //    Cappie
    //}
    public CharacterType characterType {  set; get; }

    public Animator animator { get; private set; }

    void Start()
    {
        animator = GetComponent<Animator>();
        string animationName = characterType.ToString() + "_Idle";
        animator.Play(animationName);
    }

    void Update()
    {
        
    }

    

}
