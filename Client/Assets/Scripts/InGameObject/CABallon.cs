using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CABallon : MonoBehaviour
{
    // Inventory����� ����ٸ� ���⿡�� ������ ���������� ����. 
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
