using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Google.Protobuf.Protocol;
using Server.Game.CA_Object;

namespace Server.Game.Core
{
    // 09.06 작성
    public struct CollisionInfo
    {
        public bool IsCollided;       // 충돌 발생 여부
        public bool IsLeftCollision;  // 왼쪽 충돌 여부
        public bool IsRightCollision; // 오른쪽 충돌 여부
        public bool IsTopCollision;   // 위쪽 충돌 여부
        public bool IsBottomCollision; // 아래쪽 충돌 여부
        public float OverlapPercentage; // 충돌 겹침 정도 (0.0 ~ 1.0)
    }

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

        public CollisionInfo IsCollidedWithMapTest(MoveDir dir, float leftX, float rightX, float upY, float downY, TileInfo[,] tileMapData)
        {
            CollisionInfo collisionInfo = new CollisionInfo();

            int startX = (int)Math.Floor(leftX);  // 좌측 X 경계
            int endX = (int)Math.Floor(rightX);   // 우측 X 경계

            int startY = (int)Math.Floor(downY);  // 하단 Y 경계
            int endY = (int)Math.Floor(upY);      // 상단 Y 경계

            // 충돌 검사: 이동하려는 위치에 벽이 있는지 확인
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    // 타일맵의 경계를 넘어가면 충돌로 간주
                    if (x < 0 || y < 0 || x >= tileMapData.GetLength(0) || y >= tileMapData.GetLength(1))
                    {
                        collisionInfo.IsCollided = true;
                        return collisionInfo;
                    }

                    // 벽과 충돌이 발생했는지 확인
                    if (tileMapData[x, y].isBlocktPermanently || tileMapData[x, y].isBlocktTemporary)
                    {
                        // 충돌 정보 업데이트
                        collisionInfo.IsCollided = true;

                        // 왼쪽 충돌 (캐릭터의 오른쪽 경계가 타일의 왼쪽 경계에 닿은 경우)
                        if (leftX < x + 1 && rightX >= x + 1)
                        {
                            collisionInfo.IsLeftCollision = true;
                        }

                        // 오른쪽 충돌 (캐릭터의 왼쪽 경계가 타일의 오른쪽 경계에 닿은 경우)
                        if (rightX > x && leftX <= x)
                        {
                            collisionInfo.IsRightCollision = true;
                        }

                        // 위쪽 충돌 (캐릭터의 아래쪽 경계가 타일의 위쪽 경계에 닿은 경우)
                        if (upY > y && downY <= y)
                        {
                            collisionInfo.IsTopCollision = true;
                        }

                        // 아래쪽 충돌 (캐릭터의 위쪽 경계가 타일의 아래쪽 경계에 닿은 경우)
                        if (downY < y + 1 && upY >= y + 1)
                        {
                            collisionInfo.IsBottomCollision = true;
                        }

                        if (dir == MoveDir.Left || dir == MoveDir.Right) // y축 겹침 정도를 계산해야함
                        {
                            Console.WriteLine("y축 겹침 정도를 계산해야함");
                            if (collisionInfo.IsTopCollision == true)
                            {
                                Console.WriteLine("케릭터의 위에서 충돌 발생");
                                Console.WriteLine($"y : {y} , downy : {downY} , upy : {upY}");
                                Console.WriteLine($"y overlap 정도 :  {upY - y}");
                                collisionInfo.OverlapPercentage = upY - y;
                            }

                            else if (collisionInfo.IsBottomCollision == true)
                            {
                                Console.WriteLine("케릭터의 아래에서 충돌 발생");
                                Console.WriteLine($"y : {y} , downy : {downY} , upy : {upY}");
                                Console.WriteLine($"y overlap 정도 : {y + 1 - downY}");
                                collisionInfo.OverlapPercentage = y + 1 - downY;
                            }
                        }

                        else if (dir == MoveDir.Up || dir == MoveDir.Down)  // x축 겹침 정도를 계산해야함 
                        {
                            Console.WriteLine("x축 겹침 정도를 계산해야함");
                            if (collisionInfo.IsLeftCollision == true)
                            {
                                Console.WriteLine("케릭터의 왼쪽에서 충돌 발생");
                                Console.WriteLine($"x : {x} , leftx : {leftX} , rightx : {rightX}");
                                Console.WriteLine($"x overlap 정도 :  {x + 1 - leftX}");
                                collisionInfo.OverlapPercentage = x + 1 - leftX;
                            }

                            else if (collisionInfo.IsRightCollision)
                            {
                                Console.WriteLine("케릭터의 오른쪽에서 충돌 발생");
                                Console.WriteLine($"x : {x} , leftx : {leftX} , rightx : {rightX}");
                                Console.WriteLine($"x overlap 정도 :  {rightX - x}");
                                collisionInfo.OverlapPercentage = rightX - x;
                            }
                        }


                        // X축 겹침 계산
                        //float xOverlap = Math.Max(0, Math.Min(rightX, x + 1) - Math.Max(leftX, x));
                        //float xTotal = rightX - leftX;  // 캐릭터의 X축 전체 길이
                        //float xOverlapPercentage = xOverlap / xTotal;

                        // Y축 겹침 계산
                        //float yOverlap = Math.Max(0, Math.Min(upY, y + 1) - Math.Max(downY, y));
                        //float yTotal = upY - downY;  // 캐릭터의 Y축 전체 길이
                        //float yOverlapPercentage = yOverlap / yTotal;

                        //Console.WriteLine($"x_axis_overlap_Percentage : {xOverlapPercentage}");
                        //Console.WriteLine($"y_axis_overlap_Percentage : {yOverlapPercentage}");

                        // X축과 Y축 겹침 정도 중 더 작은 값을 사용
                        //collisionInfo.OverlapPercentage = Math.Min(xOverlapPercentage, yOverlapPercentage);
                        return collisionInfo;
                    }
                }
            }

            return collisionInfo;
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

        public Vector2 GetCorrectedPositionForCharacter(Vector2 currentPosition, MoveDir direction, CollisionInfo collisionInfo, TileInfo[,] tilemapData)
        {
            float correctedX = currentPosition.X;
            float correctedY = currentPosition.Y;

             
            // 캐릭터가 Y축으로 이동 중일 때
            if (direction == MoveDir.Up || direction == MoveDir.Down)
            {
                // Y축에서 충돌이 발생한 경우
                if (collisionInfo.IsTopCollision || collisionInfo.IsBottomCollision)
                {
                    // Y축은 타일 중앙으로 보정
                    correctedY = (float)Math.Floor(currentPosition.Y) + 0.5f;

                    if (collisionInfo.OverlapPercentage <= 0.3f)
                    {
                        // X축으로 이동 가능 여부 확인 (충돌이 없는 쪽으로 이동)
                        if (collisionInfo.IsRightCollision && CanMoveLeft(correctedX, correctedY, tilemapData))
                        {
                            Console.WriteLine($"Current X : {correctedX}");
                            correctedX -= 0.1f;  // 왼쪽으로 조금씩 이동

                            if (correctedX <= Math.Floor(correctedX) + 0.5f)
                            {
                                correctedX = (float)Math.Floor(correctedX) + 0.5f;  // n.5로 고정
                            }
                            Console.WriteLine($"Corrected X : {correctedX}");
                        }
                        else if (collisionInfo.IsLeftCollision && CanMoveRight(correctedX, correctedY, tilemapData))
                        {
                            Console.WriteLine($"Current X : {correctedX}");
                            correctedX += 0.1f;  // 오른쪽으로 조금씩 이동

                            if (correctedX >= Math.Floor(correctedX) + 0.5f)
                            {
                                correctedX = (float)Math.Floor(correctedX) + 0.5f;  // n.5로 고정
                            }

                            Console.WriteLine($"Corrected X : {correctedX}");
                        }
                    }
                }
            }
            // 캐릭터가 X축으로 이동 중일 때
            else if (direction == MoveDir.Left || direction == MoveDir.Right)
            {
                // X축에서 충돌이 발생한 경우
                if (collisionInfo.IsLeftCollision || collisionInfo.IsRightCollision)
                {
                    // X축은 타일 중앙으로 보정
                    correctedX = (float)Math.Floor(currentPosition.X) + 0.5f;

                    if (collisionInfo.OverlapPercentage <= 0.3f)
                    {
                        // Y축으로 이동 가능 여부 확인 (충돌이 없는 쪽으로 이동)
                        if (collisionInfo.IsBottomCollision && CanMoveUp(correctedX, correctedY, tilemapData))
                        {
                            Console.WriteLine($"Current Y : {correctedY}");
                            correctedY += 0.1f;  // 위로 조금씩 이동

                            // 좌표를 n.5로 고정 (위쪽으로 이동 중일 때)
                            if (correctedY >= Math.Floor(correctedY) + 0.5f)
                            {
                                correctedY = (float)Math.Floor(correctedY) + 0.5f;  // n.5로 고정
                            }

                            Console.WriteLine($"Corrected Y : {correctedY}");
                        }
                        else if (collisionInfo.IsTopCollision && CanMoveDown(correctedX, correctedY, tilemapData))
                        {
                            Console.WriteLine($"Current Y : {correctedY}");
                            correctedY -= 0.1f;  // 아래로 조금씩 이동

                            // 좌표를 n.5로 고정 (아래쪽으로 이동 중일 때)
                            if (correctedY <= Math.Floor(correctedY) + 0.5f)
                            {
                                correctedY = (float)Math.Floor(correctedY) + 0.5f;  // n.5로 고정
                            }

                            Console.WriteLine($"Corrected Y : {correctedY}");
                        }
                    }
                }
                
            }

            return new Vector2(correctedX, correctedY);
        }

        public bool CanMoveLeft(float currentX, float currentY, TileInfo[,] tileMapData)
        {
            // 왼쪽 타일의 좌표
            int leftTileX = (int)Math.Floor(currentX) - 1;
            int currentTileY = (int)Math.Floor(currentY);

            // 왼쪽 위 타일의 좌표
            int leftTopTileY = currentTileY + 1;

            // 타일맵 경계를 벗어나면 false
            if (leftTileX < 0 || currentTileY < 0 || leftTopTileY >= tileMapData.GetLength(1))
                return false;

            // 왼쪽 타일과 왼쪽 위 타일이 모두 비어있는지 확인 (벽이 없으면 true)
            bool isLeftTileEmpty = !(tileMapData[leftTileX, currentTileY].isBlocktPermanently || tileMapData[leftTileX, currentTileY].isBlocktTemporary);
            bool isLeftTopTileEmpty = !(tileMapData[leftTileX, leftTopTileY].isBlocktPermanently || tileMapData[leftTileX, leftTopTileY].isBlocktTemporary);

            // 둘 다 비어있어야 왼쪽 이동 가능
            return isLeftTileEmpty && isLeftTopTileEmpty;
        }

        public bool CanMoveRight(float currentX, float currentY, TileInfo[,] tileMapData)
        {
            // 오른쪽 타일의 좌표
            int rightTileX = (int)Math.Floor(currentX) + 1;
            int currentTileY = (int)Math.Floor(currentY);

            // 오른쪽 위 타일의 좌표
            int rightTopTileY = currentTileY + 1;

            // 타일맵 경계를 벗어나면 false
            if (rightTileX >= tileMapData.GetLength(0) || currentTileY < 0 || rightTopTileY >= tileMapData.GetLength(1))
                return false;

            // 오른쪽 타일과 오른쪽 위 타일이 모두 비어있는지 확인 (벽이 없으면 true)
            bool isRightTileEmpty = !(tileMapData[rightTileX, currentTileY].isBlocktPermanently || tileMapData[rightTileX, currentTileY].isBlocktTemporary);
            bool isRightTopTileEmpty = !(tileMapData[rightTileX, rightTopTileY].isBlocktPermanently || tileMapData[rightTileX, rightTopTileY].isBlocktTemporary);

            // 둘 다 비어있어야 오른쪽 이동 가능
            return isRightTileEmpty && isRightTopTileEmpty;
        }

        public bool CanMoveUp(float currentX, float currentY, TileInfo[,] tileMapData)
        {
            // 위쪽 타일의 좌표
            int currentTileX = (int)Math.Floor(currentX);
            int topTileY = (int)Math.Floor(currentY) + 1;

            // 위쪽 오른쪽 타일의 좌표
            int rightTileX = currentTileX + 1;

            // 타일맵 경계를 벗어나면 false
            if (topTileY >= tileMapData.GetLength(1) || currentTileX < 0 || rightTileX >= tileMapData.GetLength(0))
                return false;

            // 위쪽 타일과 위쪽 오른쪽 타일이 모두 비어있는지 확인 (벽이 없으면 true)
            bool isTopTileEmpty = !(tileMapData[currentTileX, topTileY].isBlocktPermanently || tileMapData[currentTileX, topTileY].isBlocktTemporary);
            bool isRightTileEmpty = !(tileMapData[rightTileX, topTileY].isBlocktPermanently || tileMapData[rightTileX, topTileY].isBlocktTemporary);

            // 둘 다 비어있어야 위로 이동 가능
            return isTopTileEmpty && isRightTileEmpty;
        }

        public bool CanMoveDown(float currentX, float currentY, TileInfo[,] tileMapData)
        {
            // 아래쪽 타일의 좌표
            int currentTileX = (int)Math.Floor(currentX);
            int bottomTileY = (int)Math.Floor(currentY) - 1;

            // 아래쪽 오른쪽 타일의 좌표
            int rightTileX = currentTileX + 1;

            // 타일맵 경계를 벗어나면 false
            if (bottomTileY < 0 || currentTileX < 0 || rightTileX >= tileMapData.GetLength(0))
                return false;

            // 아래쪽 타일과 아래쪽 오른쪽 타일이 모두 비어있는지 확인 (벽이 없으면 true)
            bool isBottomTileEmpty = !(tileMapData[currentTileX, bottomTileY].isBlocktPermanently || tileMapData[currentTileX, bottomTileY].isBlocktTemporary);
            bool isRightTileEmpty = !(tileMapData[rightTileX, bottomTileY].isBlocktPermanently || tileMapData[rightTileX, bottomTileY].isBlocktTemporary);

            // 둘 다 비어있어야 아래로 이동 가능
            return isBottomTileEmpty && isRightTileEmpty;
        }
    }
}
