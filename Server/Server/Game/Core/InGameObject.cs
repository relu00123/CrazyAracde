using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class InGameObject : Entity
    {
        public int _layeridx { get; private set; }
        public Collider _collider {  get; private set; }
        public Transform _transform { get; private set; }
        

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

        public void OnBeginOverlap(Collider other)
        {
            // 충돌 시작 시 동작
        }

        public void OnOverlap(Collider other)
        {
            // 충돌 중 동작
        }

        public void OnEndOverlap(Collider other)
        {
            // 충돌 종료시 동작 
        }
    }
}
