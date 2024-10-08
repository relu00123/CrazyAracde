using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using Server.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;


public class CAPlayer : InGameObject
{
    public CAPlayerStats Stats { get; private set; }

    public CAPlayer(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {
        // Player에 필요한 Collider 생성
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));

        _currentState = new Player_IdleState();

        Stats = new CAPlayerStats();
    }

    IJob _job;

    public double _bubbledTime { get; set;}
    public bool   _bubbleEmergencyAnimChanged { get; set; }

    public CharacterType _characterType { get; set; }

    
    public override void Update()
    {
        if (_possessGame._isGameFinished) 
            return;
        
        // 수정된 코드 
        if (isRemoveResreved) return;

        //Console.WriteLine($"Update Function Called! (Object ID : {Id}) (CurPosition : {_transform.Position})");

        if (_possessGame != null)
        {
            if (_currentState != null)
            {
                _currentState.UpdateState(this);
            }

            else
            {
                switch (_state)
                {
                    case CreatureState.Idle:
                        UpdateIdle();
                        break;
                    case CreatureState.Moving:
                        UpdateMoving();
                        break;
                    case CreatureState.BubbleMoving:
                        UpdateMoving();
                        break;
                    case CreatureState.BubbleIdle:
                        UpdateIdle();
                        break;
                }
            }

             _job =  _possessGame._gameRoom.PushAfter(5, Update);
        }
    }

    public override void UpdateMoving()
    {
        //Console.WriteLine("Update Moving Function Called!");
        Console.WriteLine($"현재 위치 : ({_transform.Position.X} , {_transform.Position.Y})");

        var (CurColliderLeftX, CurColliderRightX, CurColliderUpY, CurColliderDownY) = _collider.CalculateTempBounds(_transform.Position);
        var exemptTiles = _possessGame._collisionManager.GetCollisionExemptTiles(CurColliderLeftX, CurColliderRightX, CurColliderUpY, CurColliderDownY, _possessGame._caMapManager._tileMapData);


        Vector2 direction = Vector2.Normalize(_targetPos - _transform.Position);

        if (direction.Length() > 0.001f)  // 벡터의 길이가 0이 아닌지 확인
        {
            direction = Vector2.Normalize(direction);
        }
        else
        {
            direction = new Vector2(0, 0);  // 방향 벡터가 0일 경우, 기본 값 설정
        }

        Vector2 nextPosition = _transform.Position + direction * _moveSpeed  * 10 * (float)_possessGame._gameRoom._deltaTime;

        if (_collider != null)
        {
            var (tempLeftX, tempRightX, tempUpY, tempDownY) = _collider.CalculateTempBounds(nextPosition);


            var (collisionInfos, isOutofBoundsCollision) = _possessGame._collisionManager.GetCollidedTiles(Direction, tempLeftX, tempRightX, tempUpY, tempDownY,
                 _possessGame._caMapManager._tileMapData, exemptTiles);

            // 바깥으로 나갔을때 예외처리 한번 해줘야함.
            if (isOutofBoundsCollision)
            {
                Console.WriteLine("Out of Bounds!");
                return;
            }

            if (collisionInfos.Count > 0)
            {
                // 충돌이 발생한 경우, 위치를 조정 

                nextPosition = _possessGame._collisionManager.GetCorrectedPosForObj(_transform.Position, collisionInfos, Direction);
                _targetPos = nextPosition;
                Console.WriteLine($"충돌 발생후 target Pos : <{_targetPos.X},{_targetPos.Y}>");
            }
             
        }

        // _targetPos에 거의 도달했을 때 위치를 정확하게 맞춤
        if (Vector2.Distance(nextPosition, _targetPos) < 0.01f)
        {
            Console.WriteLine("최종 목적지 근처시에 targetPos로 좌표 강제 이동");
            nextPosition = _targetPos;  // 최종 목적지에 도달하면 위치를 정확히 맞춤
        }

        Console.WriteLine($"위치 설정 :  <{nextPosition.X} , {nextPosition.Y}>");
        _transform.Position = nextPosition;

        //Console.WriteLine($"Next Pose ({nextPosition.X} , {nextPosition.Y})");

        S_Move movePkt = new S_Move();
        movePkt.ObjectId = Id;
        movePkt.PosInfo = new PositionInfo();
        movePkt.PosInfo.PosX = nextPosition.X;
        movePkt.PosInfo.PosY = nextPosition.Y;
        movePkt.PosInfo.MoveDir = Direction;
        movePkt.PosInfo.State = _state;
        _possessGame._gameRoom.BroadcastPacket(movePkt);
    }


    public override Vector2 CalculateTargetPositon(MoveDir dir)
    {
        Vector2 targetPosition = _transform.Position;
        float moveDistance = _moveSpeed * Stats.SpeedWeight;

        switch (dir)
        {
            case MoveDir.Up:
                targetPosition += new Vector2(0, moveDistance);
                break;
            case MoveDir.Down:
                targetPosition += new Vector2(0, -moveDistance);
                break;
            case MoveDir.Left:
                targetPosition += new Vector2(-moveDistance, 0);
                break;
            case MoveDir.Right:
                targetPosition += new Vector2(moveDistance, 0);
                break;
        }

        return targetPosition;
    }

    public override void UpdateIdle()
    {

    }

    public override void OnBeginOverlap(InGameObject other)
    {
        //Console.WriteLine("ON BEGIN OVERLAP FUNCTION CALLED FROM CHARACTER!!");

        if (other is CAPlayer)
        {
            //Console.WriteLine("캐릭터와 출돌 발생");

            if (_currentState is Player_Bubble_IdleState || _currentState is Player_Bubble_MovingState )
            {
                Vector2 pos1 = _collider.GetColliderCenterPos();
                Vector2 pos2 = other._collider.GetColliderCenterPos();

                float distance = Vector2.Distance(pos1, pos2);

                if (distance < 0.45f)
                {
                    CAPlayer ohterPlayer = other as CAPlayer;

                    if (ohterPlayer._characterType == _characterType)
                    {
                        // Console.WriteLine("다른 캐릭터가 자신을 살리는 경우");
                        // 살아날때 에니메이션도 재생시켜주도록 하자. 
                        ChangeState(new Player_IdleState());

                        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation()
                        {
                            ObjectId = Id,
                            PlayerAnim = PlayerAnimState.PlayerAnimBubbleEscape,
                        };

                        _possessGame._gameRoom.BroadcastPacket(changeAnimPkt);

                        // 살리는 사운드 재생
                        S_PlaySoundEffect soundEffectPacket = new S_PlaySoundEffect
                        {
                            SoundEffectType = SoundEffectType.PlayerReviveSoundEffect
                        };

                        _possessGame._gameRoom.BroadcastPacket(soundEffectPacket);
                    }

                    else
                    {
                        // Console.WriteLine("다른 캐릭터가 자신을 죽이는 경우");
                        ChangeState(new Player_DeadState());
                    }
                }
            }
        }
    }
}

