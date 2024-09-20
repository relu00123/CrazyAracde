using Google.Protobuf.Protocol;
using Server.Game.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Numerics;
using System.Text;

namespace Server.Game.CA_Object
{
    public class InGameObject : Entity
    {
        public int _layeridx { get; private set; }
        public Collider _collider { get; protected set; }
        public Transform _transform { get; set; }

        public Vector2 _targetPos { get; set; }

        private MoveDir _direction;

        public MoveDir Direction
        {
            get { return _direction; }
            set { if (value == MoveDir.MoveNone) return; _direction = value; }

        }


        public float _moveSpeed { get; set; } = 0.3f;  //= 0.3f;

        public InGame _possessGame { get; set; }

        public CreatureState _state { get; private set; } = CreatureState.Idle;

        private const float Tolerance = 1e-5f;

        public InGameObject(int id, string name, int layer)
            : base(id, name)
        {
            _layeridx = layer;
            _transform = new Transform(); // 기본 Transform으로 설정 
        }

        public InGameObject(int id, string name, Transform transform, int layer)
           : base(id, name)
        {
            _layeridx = layer;
            _transform = transform;
        }

        public void InitializeCollider(Vector2 colliderSize)
        {
            _collider = new Collider(this, Vector2.Zero, colliderSize);
        }

        // 충돌 관련 
        public virtual void OnBeginOverlap(InGameObject other)
        {
            // 충돌 시작 시 동작
            Console.WriteLine("ON BEGIN OVERLAP FUNCTION CALLED!!");
        }

        public virtual void OnOverlap(Collider other)
        {
            // 충돌 중 동작
        }

        public virtual void OnEndOverlap(Collider other)
        {
            // 충돌 종료시 동작 
        }

        public virtual void Update()
        {

        }

        // FSM 관련
        public void ChangeState(CreatureState newState)
        {
            if (IsValidStateTransition(newState))
            {
                _state = newState;
                Console.WriteLine($"State changed to: {_state}");

                S_Move movePkt = new S_Move();
                movePkt.ObjectId = Id;
                movePkt.PosInfo = new PositionInfo();
                movePkt.PosInfo.PosX = _transform.Position.X;
                movePkt.PosInfo.PosY = _transform.Position.Y;
                movePkt.PosInfo.State = _state;
                movePkt.PosInfo.MoveDir = Direction;
                _possessGame._gameRoom.BroadcastPacket(movePkt);
            }

            else
            {
                Console.WriteLine($"Invalid state transition: {_state} to {newState}");
            }
        }

        protected virtual bool IsValidStateTransition(CreatureState newState)
        {
            // 기본 상태 전환 규칙 (필요에 따라 자식에서 재정의 할 것임..)
            switch (_state)
            {
                case CreatureState.Idle:
                    return newState == CreatureState.Moving || newState == CreatureState.Dead || newState == CreatureState.Idle
                        || newState == CreatureState.Bubble;

                case CreatureState.Moving:
                    return newState == CreatureState.Idle || newState == CreatureState.Moving
                        || newState == CreatureState.Bubble; 

                case CreatureState.Bubble: 

                case CreatureState.Dead:
                    return false; // Death 상태에서 다른 상태로 전환 불가


                // 추가 상태 전환 규칙을 여기에 정의
                default:
                    return false;
            }
        }


        // 상태별 Update
        public virtual void UpdateIdle()
        {

        }

        public Vector2 CalculateTargetPositon(MoveDir dir)
        {
            Vector2 targetPosition = _transform.Position;
            float moveDistance = _moveSpeed;

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

        public virtual void UpdateMoving()
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

            Vector2 nextPosition = _transform.Position + direction * _moveSpeed * 10 * (float)_possessGame._gameRoom._deltaTime;

            if (_collider != null)
            {
                var (tempLeftX, tempRightX, tempUpY, tempDownY) = _collider.CalculateTempBounds(nextPosition);


                var (collisionInfos, isOutofBoundsCollision) = _possessGame._collisionManager.GetCollidedTiles(Direction, tempLeftX, tempRightX, tempUpY, tempDownY,
                     _possessGame._caMapManager._tileMapData, exemptTiles);

                // Debuggin 해보자.
                /* {
                    if (isOutofBoundsCollision)
                    {
                        Console.WriteLine("Out of Bounds!");
                        return;
                    }

                    for (int i = 0; i < collisionInfos.Count; ++i)
                    {
                        Console.WriteLine($"======== 충돌 정보 {i}==============");
                        Console.WriteLine($"충돌 좌표 <{collisionInfos[i].CollidedTile.X} , {collisionInfos[i].CollidedTile.Y}> ");
                        Console.WriteLine($"왼쪽   충돌 여부 {collisionInfos[i].IsLeftCollision}");
                        Console.WriteLine($"오른쪽 충돌 여부 {collisionInfos[i].IsRightCollision}");
                        Console.WriteLine($"위     충돌 여부 {collisionInfos[i].IsTopCollision}");
                        Console.WriteLine($"아래   충돌 여부 {collisionInfos[i].IsBottomCollision}");
                        Console.WriteLine($"겹침   정도      {collisionInfos[i].OverlapPercentage}");
                        Console.WriteLine($"==================================");
                    }
                } */
                 
                // 바깥으로 나갔을때 예외처리 한번 해줘야함.
                if (isOutofBoundsCollision)
                {
                    Console.WriteLine("Out of Bounds!");
                    return;
                }

                if (collisionInfos.Count > 0 )
                {
                    // 충돌이 발생한 경우, 위치를 조정 

                    nextPosition =  _possessGame._collisionManager.GetCorrectedPosForObj(_transform.Position, collisionInfos, Direction);
                    _targetPos = nextPosition;
                    Console.WriteLine($"충돌 발생후 target Pos : <{_targetPos.X},{_targetPos.Y}>");
                }


    
                // 이함수부터 아래의 로직을 싹 고쳐야함. 일단 GetCollidedTiles 라는 이름으로 IsCollidedWithMapTest() 교체하도록 함수작성은 해놨음.
                //CollisionInfo collisionInfo = _inGame._collisionManager.IsCollidedWithMapTest(Direction, tempLeftX, tempRightX, tempUpY, tempDownY, _inGame._caMapManager._tileMapData);

                //if (collisionInfo.IsCollided)
                //{
                //    // 충돌이 발생한 경우, 위치를 보정
                //    Console.WriteLine("Collision detected during movement");
                //    nextPosition = _inGame._collisionManager.GetCorrectedPositionForCharacter(_transform.Position, Direction, collisionInfo, _inGame._caMapManager._tileMapData);

                //    _targetPos = nextPosition;
                //}

                // 위의 주석 부터 여기까지 수정을 해야함!
            }


            //if (direction.Length() > 0.0001f)  // 매우 작은 값을 비교하여 0이 아닌지 확인
            //{
            //    direction = Vector2.Normalize(direction);  
            //}
            //else
            //{
            //    Console.WriteLine("방향이 0 일때는 움직이지 않도록 설정");
            //    direction = new Vector2(0, 0); // 방향이 0일 때는 움직이지 않도록 설정
            //}

            
            //_transform.Position = _targetPos;

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
            //movePkt.PosInfo.PosX = _transform.Position.X;
            //movePkt.PosInfo.PosY = _transform.Position.Y;
            movePkt.PosInfo.PosX = nextPosition.X;
            movePkt.PosInfo.PosY = nextPosition.Y;
            movePkt.PosInfo.MoveDir = Direction;
            movePkt.PosInfo.State = CreatureState.Moving;
            _possessGame._gameRoom.BroadcastPacket(movePkt);



            // 기존 코드
            /*
            //bool isidle = false;

            if (Vector2.Distance(_transform.Position, _targetPos) < Tolerance)
            {
                //isidle = true;
                //ChangeState(CreatureState.Idle);
                _transform.Position = _targetPos; // 위치를 정확히 목표 위치로 맞춤
            }

            else
            {
                // 목표 위치로 이동
                Vector2 direction = Vector2.Normalize(_targetPos - _transform.Position);
                Vector2 nextPosition = _transform.Position + direction * _moveSpeed * (float)_inGame._gameRoom._deltaTime;


                // 충돌 및 경계 체크 함수 필요 ToDo.. 다음 주석 친 코드 비슷한 느낌으로 짤듯? 아니면 충돌시 State를 변경해줄수도
                //if (CanMoveTo(nextPosition))
                //{
                //    _transform.Position = nextPosition;
                //}
                //else
                //{
                //    // 충돌이 발생했거나 경계를 넘어설 경우
                //    _state = CreatureState.Idle;
                //    Console.WriteLine($"Movement stopped due to collision or boundary at {_transform.Position}");
                //}

                // 목표 위치를 초과하지 않도록 위치를 조정
                if (Vector2.Distance(nextPosition, _targetPos) < Vector2.Distance(_transform.Position, _targetPos))
                {
                    _transform.Position = nextPosition;
                }
                else
                {
                    _transform.Position = _targetPos;
                }
            }


            // 일단 임시로..! 이코드 쓰면 안되고 나중에 고쳐야함
            S_Move movePkt = new S_Move();
            movePkt.ObjectId = Id;
            movePkt.PosInfo = new PositionInfo();
            movePkt.PosInfo.PosX = _transform.Position.X;
            movePkt.PosInfo.PosY = _transform.Position.Y;
            movePkt.PosInfo.MoveDir = Direction;

            //if (isidle) 
            //   movePkt.PosInfo.State = CreatureState.Idle;
            //else
            //    movePkt.PosInfo.State = CreatureState.Moving;

            movePkt.PosInfo.State = CreatureState.Moving;

           // Console.WriteLine($"Sending S_Move pkt, (Pos : {movePkt.PosInfo.PosX} , {movePkt.PosInfo.PosY})");
            _inGame._gameRoom.BroadcastPacket(movePkt);
            */
        }
    }
}
