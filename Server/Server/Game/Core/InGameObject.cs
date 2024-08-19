using System;
using System.Collections.Generic;
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

        public void AddCollider(Collider collider)
        {
            _collider = collider;
        }
    }
}
