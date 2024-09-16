using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CABallon : MonoBehaviour
{
    // Inventory기능을 만든다면 여기에서 정보를 가져오도록 하자. 
    public BombType bombSkinType { get; set; } = BombType.Black;  

    public int power { get;  set; } = 1;

    public Animator animator { get; private set; }

    
    void Start()
    {
        animator = GetComponent<Animator>();

        
    }

     
    void Update()
    {
        
    }
}
