﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

public   class CABaseController : MonoBehaviour
{
    PositionInfo _positionInfo = new PositionInfo();

    public InGameObject InGameObj { get; set; }

    



    // 여기 코드때문에 그런듯? 
    Vector3 _destination = new Vector3(0f, 0f, 0f);

    [SerializeField] public float MoveSpeed = 3f; // 케릭터 이동 속도

    protected bool _updated = false;

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            // 기존에 여기
            ObjState = value.State;
            Dir = value.MoveDir;

            // 바뀌고 여기로
            _positionInfo = value;
            _destination = new Vector3(value.PosX, value.PosY, transform.position.z);
        }
    }

    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            PosInfo.MoveDir = value;
            UpdateAnimation();
            _updated = true;
        }
    }

    

    public virtual CreatureState ObjState
    {
        get { return PosInfo.State; }
        set
        {
            PosInfo.State = value;
            UpdateAnimation();
            _updated = true;
        }
    }

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _destination.x = InGameObj.UnityObject.transform.position.x;
        _destination.y = InGameObj.UnityObject.transform.position.y;
    }

    void Update()
    {
        UpdateController();
    }


    protected virtual void UpdateController()
    {
        switch (ObjState)
        {
            case CreatureState.Idle:
                SmoothMove();
                UpdateIdle();
                break;
            case CreatureState.Moving:
                SmoothMove();
                UpdateMoving();
                break;
            case CreatureState.BubbleIdle:
                SmoothMove();
                UpdateIdle();
                break;
            case CreatureState.BubbleMoving:
                SmoothMove();
                UpdateMoving();
                break;
        }
    }

    public virtual void ChangeAnimation(Enum animState)
    {

    }

    protected virtual void UpdateAnimation()
    {
        Console.Write("Update Animation Function Called!");
    }

    protected virtual void UpdateIdle()
    {
        Console.WriteLine("Update Idle Function Called!");
    }

    protected virtual void UpdateMoving()
    {
        Console.WriteLine("Update Moving Function Called!");
    }

    protected virtual void SmoothMove()
    {
        // 현재 위치에서 목적지로 이동
        Vector3 direction = _destination - transform.position;
        float distance = direction.magnitude;


        if (distance > 0.05f)
        {
            direction.Normalize();
            transform.position += direction * MoveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = _destination;
        }
    }
}

