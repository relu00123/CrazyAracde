using Google.Protobuf.Protocol;
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
        public Collider _collider { get; private set; }
        public Transform _transform { get; set; }

        public Vector2 _targetPos { get; set; }

        public float _moveSpeed { get; set; } = 10.0f;

        public InGame _inGame { get; set; }

        public CreatureState _state { get;  private set; } = CreatureState.Idle;

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
        public virtual void OnBeginOverlap(Collider other)
        {
            // 충돌 시작 시 동작
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
            }

            else
            {
                Console.WriteLine($"Invalid state transition: {_state} to {newState}");
            }
        }

        protected virtual bool IsValidStateTransition(CreatureState newState)
        {
            // 기본 상태 전환 규칙 (필요에 따라 자식에서 재정의 할 것임..)
            switch(_state)
            {
                case CreatureState.Idle:
                    return newState == CreatureState.Moving || newState == CreatureState.Dead;

                case CreatureState.Moving:
                    return newState == CreatureState.Idle || newState == CreatureState.Dead;

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

        public virtual void UpdateMoving()
        {
            if (Vector2.Distance(_transform.Position, _targetPos) < Tolerance)
            {
                ChangeState(CreatureState.Idle);
                _transform.Position = _targetPos; // 위치를 정확히 목표 위치로 맞춤
                return;
            }

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
                ChangeState(CreatureState.Idle); // 목표 위치에 도달하면 Idle상태로 전환 
            }


            // 일단 임시로..! 이코드 쓰면 안되고 나중에 고쳐야함
            S_Move movePkt = new S_Move();
            movePkt.ObjectId = Id;
            movePkt.PosInfo = new PositionInfo();
            movePkt.PosInfo.PosX = _transform.Position.X;
            movePkt.PosInfo.PosY = _transform.Position.Y;

            Console.WriteLine($"Sending S_Move pkt, (Pos : {movePkt.PosInfo.PosX} , {movePkt.PosInfo.PosY})");
            _inGame._gameRoom.BroadcastPacket(movePkt);
        }
    }
}
