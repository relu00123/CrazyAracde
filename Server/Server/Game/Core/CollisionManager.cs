using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Google.Protobuf.Protocol;
using Server.Game.CA_Object;

namespace Server.Game.Core
{
    public class CollisionManager
    {
        private ObjectLayerManager _objectLayerManager;
        private Dictionary<ulong, bool> _collisionMap;

        public CollisionManager(ObjectLayerManager objectLayerManager)
        {
            _objectLayerManager = objectLayerManager;
            _collisionMap = new Dictionary<ulong, bool>();
        }

        public void Tick()
        {
            for (int layerIndex = 0; layerIndex <  30 ; layerIndex++)
            {
                var objectsInLayer = _objectLayerManager.GetObjectsInLayer(layerIndex);

                for (int i = 0; i < objectsInLayer.Count; i++)
                {
                    for (int j = i + 1; j < objectsInLayer.Count; j++)
                    {
                        var leftObj = objectsInLayer[i];
                        var rightObj = objectsInLayer[j];

                        var leftCollider = leftObj._collider;
                        var rightCollider = rightObj._collider;

                        if (leftCollider != null && rightCollider != null)
                        {
                            CollisionBtwObjects(leftObj, rightObj);
                        }
                    }
                }
            }
        }

        private void CollisionBtwObjects(InGameObject leftObj, InGameObject rightObj)
        {
            var leftCollider = leftObj._collider;
            var rightCollider = rightObj._collider;

            if (leftCollider == null || rightCollider == null) return;

            ulong collisionID = GenerateCollisionID(leftCollider, rightCollider);

            bool wasColliding = _collisionMap.ContainsKey(collisionID) && _collisionMap[collisionID];
            bool isColliding = leftCollider.IsCollidingWith(rightCollider);

            if (isColliding)
            {
                if (wasColliding)
                {
                    leftCollider.OnOverlap(rightCollider);
                    rightCollider.OnOverlap(leftCollider);
                }
                else
                {
                    leftCollider.BeginOverlap(rightCollider);
                    rightCollider.BeginOverlap(leftCollider);
                    _collisionMap[collisionID] = true;
                }
            }
            else if (wasColliding)
            {
                leftCollider.EndOverlap(rightCollider);
                rightCollider.EndOverlap(leftCollider);
                _collisionMap[collisionID] = false;
            }
        }

        private ulong GenerateCollisionID(Collider left, Collider right)
        {
            return ((ulong)left.GetHashCode() << 32) | (uint)right.GetHashCode();
        }



        // TileMap에 의한 충돌
        public  bool IsCollidedWithMap(float leftX, float RightX, float upY, float downY , TileInfo[,] tileMapData)
        {
            int startX = (int)Math.Floor(leftX);     // 좌측 X 경계
            int endX = (int)Math.Floor(RightX);   // 우측 X 경계

            int startY = (int)Math.Floor(downY); // 하단 Y 경계
            int endY = (int)Math.Floor(upY);       // 상단 Y 경계 

            // 충돌 검사 : 이동하려는 위치에 벽이 있는지 확인
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY;  y <= endY; y++)
                {
                    // 타일맵의 경계를 넘어가면 충돌로 간주 
                    if (x < 0 || y < 0 || x >= tileMapData.GetLength(0) || y >= tileMapData.GetLength(1))
                        return true;

                    // 벽과 충돌이 발생했는지 확인 
                    if (tileMapData[x, y].isBlocktPermanently || tileMapData[x, y].isBlocktTemporary)
                        return true; 
                }
            }

            return false;
        }

        public Vector2 GetCorrectedPosition(Vector2 currentPosition, MoveDir direction)
        {
            // 좌표를 타일 크기에 맞춰 보정
            float correctedX;
            float correctedY;

            switch (direction)
            {
                case MoveDir.Left:
                    // 왼쪽으로 이동할 때는 타일의 좌측 경계에 위치시킴
                    correctedX = (float)Math.Floor(currentPosition.X) + 0.5f;
                    correctedY = currentPosition.Y;  // Y는 그대로 타일 중심에 위치
                    break;

                case MoveDir.Right:
                    // 오른쪽으로 이동할 때는 타일의 우측 경계에 위치시킴
                    correctedX = (float)Math.Floor(currentPosition.X) + 0.5f;
                    correctedY = currentPosition.Y; // Y는 그대로 타일 중심에 위치
                    break;

                case MoveDir.Up:
                    // 위쪽으로 이동할 때는 타일의 위쪽 경계에 위치시킴
                    correctedX = currentPosition.X;  // X는 그대로 타일 중심에 위치
                    correctedY = (float)Math.Floor(currentPosition.Y) + 0.5f;
                    break;

                case MoveDir.Down:
                    // 아래로 이동할 때는 타일의 아래쪽 경계에 위치시킴
                    correctedX = currentPosition.X;  // X는 그대로 타일 중심에 위치
                    correctedY = (float)Math.Floor(currentPosition.Y) + 0.5f;
                    break;

                default:
                    // 기본적으로 타일 중심으로 맞추기
                    correctedX = (float)Math.Floor(currentPosition.X) + 0.5f;
                    correctedY = (float)Math.Floor(currentPosition.Y) + 0.5f;
                    break;
            }

            return new Vector2(correctedX, correctedY);
        }



    }
}
