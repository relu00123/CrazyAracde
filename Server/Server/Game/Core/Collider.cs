using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Server.Game.CA_Object;

namespace Server.Game
{
    public class Collider
    {
        public Vector2 OffsetPos { get; private set; }  // 충돌 박스의 위치 오프셋
        public Vector2 OffsetScale { get; private set; } // 타일 좌표계를 기반으로 하는 충돌 박스의 크기 

        private InGameObject _owner;

        // 충돌 영역 경계값 (AABB)
        public float UpYValue { get; private set; }
        public float DownYValue { get; private set; }
        public float LeftXValue { get; private set; }
        public float RightXValue { get; private set; }


        public Collider(InGameObject owner, Vector2 offsetPos, Vector2 offsetScale)
        {
            _owner = owner;
            OffsetPos = offsetPos;
            OffsetScale = offsetScale;

            UpdateColliderWorldMat();
        }

        // Collider의 위치와 크기에 따른 경계 좌표(AABB)를 업에디트
        public void UpdateColliderWorldMat()
        {
            

            // 캐릭터의 중심 좌표 (타일 좌표에서 중앙으로 이동)
            var position = _owner._transform.Position  + OffsetPos;

            // 타일 좌표계에서 충돌 박스 크기 반영 (OffsetScale에 비례)
            var scale = OffsetScale;

            UpdateBounds(position, scale);
        }

        private void UpdateBounds(Vector2 position, Vector2 scale)  // AABB 경계값 업데이트 (position은 캐릭터의 중앙 좌표)
        {
            // Collider의 경계 좌표 계산 (position은 중앙 좌표 기준)
            var topLeft = position - scale / 2;
            var bottomRight = position + scale / 2;

            LeftXValue = topLeft.X;
            RightXValue = bottomRight.X;
            UpYValue = bottomRight.Y;   // 사실 leftBottom
            DownYValue = topLeft.Y; // 사실 RightTop

            Console.WriteLine($"OnwerName : {_owner.Name} , ColliderPos : LB({LeftXValue},{DownYValue}), RT({RightXValue},{UpYValue})");
        }

        public (float leftX, float rightX, float upY, float downY) CalculateTempBounds(Vector2 targetPosition)
        {
            // 목표 위치에서의 좌표를 기반으로 임시 경계값을 계산
            var Position = targetPosition + OffsetPos;
            var scale = OffsetScale;

            var topLeft = Position - scale / 2;
            var bottomRight = Position + scale / 2;

            float tempLeftX = topLeft.X;
            float tempRightX = bottomRight.X;
            float tempUpY = bottomRight.Y;
            float tempDownY = topLeft.Y;

            return (tempLeftX, tempRightX, tempUpY, tempDownY);
        }


        public bool IsCollidingWith(Collider other)
        {
            if (RightXValue < other.LeftXValue || LeftXValue > other.RightXValue) return false;
            if (DownYValue > other.UpYValue || UpYValue < other.DownYValue) return false;

            return true;


            //return !(RightXValue < other.LeftXValue ||    // 오른쪽 경계가 상대의 왼쪽 경계보다 왼쪽에 있으면 충돌 X
            //         LeftXValue > other.RightXValue ||    // 왼쪽 경계가 상대의 오른쪽 경계보다 오른쪽에 있으면 충돌 X
            //         UpYValue > other.DownYValue ||       // 아래쪽 경계가 상대의 위쪽 경계보다 위에 있으면 충돌 X (Y축 반전)
            //         DownYValue < other.UpYValue);        // 위쪽 경계가 상대의 아래 경계보다 아래에 있으면 충돌 X (Y축 반전)
        }

        public void BeginOverlap(Collider other)
        {
            //_owner.OnBeginOverlap(other);
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
