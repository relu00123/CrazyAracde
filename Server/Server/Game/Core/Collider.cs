using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class Collider
    {
        public Vector2 OffsetPos { get; private set; }
        public Vector2 OffsetScale { get; private set; }

        public bool IsAbsolute { get; private set; }

        private Matrix3x2 _colliderWorldMat;
        private InGameObject _owner;

        // 충돌 영역 경계값
        public float UpYValue { get; private set; }
        public float DownYValue { get; private set; }
        public float LeftXValue { get; private set; }
        public float RightXValue { get; private set; }


        public Collider(InGameObject owner, Vector2 offsetPos, Vector2 offsetScale, bool isAbsolute = false)
        {
            _owner = owner;
            OffsetPos = offsetPos;
            OffsetScale = offsetScale;
            IsAbsolute = isAbsolute;
        }

        public void UpdateColliderWorldMat()
        {
            var position = _owner._transform.Position + OffsetPos;
            var scale = _owner._transform.Scale * OffsetScale;
            _colliderWorldMat = Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(position);

            UpdateBounds();
        }

        private void UpdateBounds()
        {
            var topLeft = Vector2.Transform(new Vector2(-0.5f, 0.5f), _colliderWorldMat);
            var bottomRight = Vector2.Transform(new Vector2(0.5f, -0.5f), _colliderWorldMat);

            LeftXValue = topLeft.X;
            RightXValue = bottomRight.X;
            UpYValue = topLeft.Y;
            DownYValue = bottomRight.Y;
        }

        public bool IsCollidingWith(Collider other)
        {
            return !(RightXValue < other.LeftXValue ||
                     LeftXValue > other.RightXValue ||
                     UpYValue < other.DownYValue ||
                     DownYValue > other.UpYValue);
        }

        public void BeginOverlap(Collider other)
        {
            _owner.OnBeginOverlap(other);
        }


        public void OnOverlap(Collider other)
        {
            // 충돌 중 이벤트 처리
            _owner.OnOverlap(other);
        }

        public void EndOverlap(Collider other)
        {
            // 충돌 종료 이벤트 처리
            _owner.OnEndOverlap(other);
        }
    }
}
