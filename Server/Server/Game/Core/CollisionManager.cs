using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore.Internal;
using Server.Game.CA_Object;

namespace Server.Game.Core
{
    public struct CollisionInfo
    {
        public bool IsCollided;       // 충돌 발생 여부
        public bool IsLeftCollision;  // 왼쪽 충돌 여부
        public bool IsRightCollision; // 오른쪽 충돌 여부
        public bool IsTopCollision;   // 위쪽 충돌 여부
        public bool IsBottomCollision; // 아래쪽 충돌 여부
        public float OverlapPercentage; // 충돌 겹침 정도 (0.0 ~ 1.0)
        public Vector2 CollidedTile; // 충돌한 타일의 좌표 
    }


    public class CollisionManager
    {
        private ObjectLayerManager _objectLayerManager;
        private Dictionary<ulong, bool> _collisionMap;

        public const int LayerCount = 30;

        bool[,] collisionMatrix = new bool[LayerCount, LayerCount];


        public CollisionManager(ObjectLayerManager objectLayerManager)
        {
            _objectLayerManager = objectLayerManager;
            _collisionMap = new Dictionary<ulong, bool>();
            InitializeCollisionMatrix();
        }

        private void InitializeCollisionMatrix()
        {
            // Character
            collisionMatrix[(int)LayerType.CharacterLayer, (int)LayerType.WaterstreamLayer] = true;
            collisionMatrix[(int)LayerType.CharacterLayer, (int)LayerType.ItemLayer] = true;
            collisionMatrix[(int)LayerType.CharacterLayer, (int)LayerType.CharacterLayer] = true;

            // WaterStream
            collisionMatrix[(int)LayerType.WaterstreamLayer, (int)LayerType.CharacterLayer] = true;
            collisionMatrix[(int)LayerType.WaterstreamLayer, (int)LayerType.ItemLayer] = true;


            // ItemLayer 
            collisionMatrix[(int)LayerType.ItemLayer, (int)LayerType.CharacterLayer] = true;
            collisionMatrix[(int)LayerType.ItemLayer, (int)LayerType.WaterstreamLayer] = true;
        }

        public void UpdateDynamicCollision()
        {
            //Console.WriteLine("Update Dynamic Collision Function Called!");

            for (int i = 0; i < LayerCount; i++) // Layer1
            {
                List<InGameObject> ObjectsInLayer1 = _objectLayerManager.GetObjectsInLayer(i);

                for (int j = 0; j <= i; j++) // Layer2
                {
                    if (collisionMatrix[i, j] == false) continue; // 충돌이 일어나지 않은 Layer들간의 비교

                    List<InGameObject> ObjectInLayer2 = _objectLayerManager.GetObjectsInLayer(j);

                    if (i == j) // 같은 레이어 끼리의 비교인 경우
                    {
                        for (int obj1_idx = 0; obj1_idx < ObjectsInLayer1.Count; obj1_idx++)
                        {
                            for (int obj2_idx = obj1_idx + 1; obj2_idx < ObjectInLayer2.Count; obj2_idx++)
                            {
                                CollisionBtwObjects(ObjectsInLayer1[obj1_idx], ObjectInLayer2[obj2_idx]);
                            }
                        }
                    }


                    if (i != j) // 같은 레이어끼리의 비교가 아닌 경우
                    {
                        for (int obj1_idx = 0; obj1_idx < ObjectsInLayer1.Count; obj1_idx++)
                        {
                            for (int obj2_idx = 0; obj2_idx < ObjectInLayer2.Count; obj2_idx++)
                            {
                                // ObjectsInLayer1[obj1_idx] 와
                                // ObjectInLayer2[obj2_idx]  가 충돌하는지 확인. 충돌하면 BeginOverlap 수행. 
                                // 이 함수 가져다 쓰면 될듯?
                                CollisionBtwObjects(ObjectsInLayer1[obj1_idx], ObjectInLayer2[obj2_idx]);
                            }
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

            leftObj._collider.UpdateColliderWorldMat();
            rightObj._collider.UpdateColliderWorldMat();
            
            bool isColliding = leftCollider.IsCollidingWith(rightCollider);

            if (isColliding)
            {
                leftObj.OnBeginOverlap(rightObj);
                rightObj.OnBeginOverlap(leftObj);
            }

            // 이어서 로직작성..

            //ulong collisionID = GenerateCollisionID(leftCollider, rightCollider);

            //bool wasColliding = _collisionMap.ContainsKey(collisionID) && _collisionMap[collisionID];
            //bool isColliding = leftCollider.IsCollidingWith(rightCollider);

            //if (isColliding)
            //{
            //    if (wasColliding)
            //    {
            //        leftCollider.OnOverlap(rightCollider);
            //        rightCollider.OnOverlap(leftCollider);
            //    }
            //    else
            //    {
            //        leftCollider.BeginOverlap(rightCollider);
            //        rightCollider.BeginOverlap(leftCollider);
            //        _collisionMap[collisionID] = true;
            //    }
            //}
            //else if (wasColliding)
            //{
            //    leftCollider.EndOverlap(rightCollider);
            //    rightCollider.EndOverlap(leftCollider);
            //    _collisionMap[collisionID] = false;
            //}
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

         
        private ulong GenerateCollisionID(Collider left, Collider right)
        {
            return ((ulong)left.GetHashCode() << 32) | (uint)right.GetHashCode();
        }


        // 동적 충돌 (매틱마다 호출)



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

        public List<Vector2> GetCollisionExemptTiles(float leftX, float rightX, float upY, float downY, TileInfo[,] tileMapData)
        {
            List<Vector2> exemptTiles = new List<Vector2>();

            // 충돌 범위 내 타일을 순회
            for (int x = (int)Math.Floor(leftX); x <= (int)Math.Floor(rightX); x++)
            {
                for (int y = (int)Math.Floor(downY); y <= (int)Math.Floor(upY); y++)
                {
                    // 타일맵 범위를 벗어나지 않도록 검사
                    if (x < 0 || y < 0 || x >= tileMapData.GetLength(0) || y >= tileMapData.GetLength(1))
                        continue;

                    // 예시: 물폭탄이 있는 타일은 면책 대상
                    if (tileMapData[x, y].isBlocktTemporary)
                    {
                        // 면책 타일로 추가
                        exemptTiles.Add(new Vector2(x, y));
                    }
                }
            }

            Console.Write("Exempt Tiles: ");
            for (int i = 0; i < exemptTiles.Count; ++i)
            {
                Console.Write($"< {exemptTiles[i].X}, {exemptTiles[i].Y} > ");
            }
            Console.WriteLine();

            return exemptTiles;
        }

        public (List<CollisionInfo> collisionInfos, bool isOutOfBoundsCollision) GetCollidedTiles(
            MoveDir direction, float leftX, float rightX, float upY, float downY, 
            TileInfo[,] tileMapData, List<Vector2> exemptTiles)
        {
            List<CollisionInfo> collisionInfos = new List<CollisionInfo>();
            bool isOutOfBoundsCollision = false;

            int startX = (int)Math.Floor(leftX);  // 좌측 X 경계
            int endX = (int)Math.Floor(rightX);   // 우측 X 경계

            int startY = (int)Math.Floor(downY);  // 하단 Y 경계
            int endY = (int)Math.Floor(upY);      // 상단 Y 경계

            // 타일맵 경계 바깥으로 나가는 경우 감지
            if (startX < 0 || endX >= tileMapData.GetLength(0) || startY < 0 || endY >= tileMapData.GetLength(1))
            {
                isOutOfBoundsCollision = true;
                return (collisionInfos, isOutOfBoundsCollision);
            }

            // 충돌 검사 : 이동하려는 위치에 벽이 있는지 확인
            for (int x= startX; x<= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    // 면책타일이라면 충돌검사를 시행하지 않을 것이다. 
                    if (exemptTiles.Contains(new Vector2(x, y)))
                        continue;

                    if (tileMapData[x, y].isBlocktPermanently || tileMapData[x, y].isBlocktTemporary)
                    {
                        CollisionInfo collisionInfo = new CollisionInfo();

                        collisionInfo.CollidedTile = new Vector2((int)x, (int)y);

                        // 왼쪽 충돌 (캐릭터의 오른쪽 경계가 타일의 왼쪽 경계에 닿은 경우)
                        if (leftX < x + 1 && rightX >= x + 1)
                            collisionInfo.IsLeftCollision = true;
                        // 오른쪽 충돌 (캐릭터의 왼쪽 경계가 타일의 오른쪽 경계에 닿은 경우)
                        if (rightX > x && leftX <= x)
                            collisionInfo.IsRightCollision = true;
                        // 위쪽 충돌 (캐릭터의 아래쪽 경계가 타일의 위쪽 경계에 닿은 경우)
                        if (upY > y && downY <= y)
                            collisionInfo.IsTopCollision = true;
                        // 아래쪽 충돌 (캐릭터의 위쪽 경계가 타일의 아래쪽 경계에 닿은 경우)
                        if (downY < y + 1 && upY >= y + 1)
                            collisionInfo.IsBottomCollision = true;

                        float overlapPercentage = CalculateOverlapPercentage(direction, x, y, leftX, rightX, downY, upY, collisionInfo);
                        collisionInfo.OverlapPercentage = overlapPercentage;

                        collisionInfos.Add(collisionInfo);
                    }
                }
            }

            return (collisionInfos, isOutOfBoundsCollision);
        }

        float CalculateOverlapPercentage(MoveDir dir, int x, int y, float leftX, float rightX, float downY, float upY, CollisionInfo collisionInfo)
        {
            float overlapPercentage = 1;

            if (dir == MoveDir.Left || dir == MoveDir.Right) // y축 겹침 정도를 계산해야함
            {
                Console.WriteLine("y축 겹침 정도를 계산해야함");
                if (collisionInfo.IsTopCollision == true)
                {
                    Console.WriteLine("케릭터의 위에서 충돌 발생");
                    Console.WriteLine($"y : {y} , downy : {downY} , upy : {upY}");
                    Console.WriteLine($"y overlap 정도 :  {upY - y}");
                    overlapPercentage = upY - y;
                }

                else if (collisionInfo.IsBottomCollision == true)
                {
                    Console.WriteLine("케릭터의 아래에서 충돌 발생");
                    Console.WriteLine($"y : {y} , downy : {downY} , upy : {upY}");
                    Console.WriteLine($"y overlap 정도 : {y + 1 - downY}");
                    overlapPercentage = y + 1 - downY;
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
                    overlapPercentage = x + 1 - leftX;
                }

                else if (collisionInfo.IsRightCollision)
                {
                    Console.WriteLine("케릭터의 오른쪽에서 충돌 발생");
                    Console.WriteLine($"x : {x} , leftx : {leftX} , rightx : {rightX}");
                    Console.WriteLine($"x overlap 정도 :  {rightX - x}");
                    overlapPercentage = rightX - x;
                }
            }

            return overlapPercentage;
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

        public Vector2 GetCorrectedPosForObj(Vector2 currentPosition, List<CollisionInfo> collisionInfos, MoveDir moveDir)
        {
            float correctedX = currentPosition.X;
            float correctedY = currentPosition.Y;

            for (int i = 0; i < collisionInfos.Count; i++)
            {
                MoveDir slideDir = GetCollisionSlideDir(moveDir, collisionInfos[i]);

                Console.WriteLine($"<{collisionInfos[i].CollidedTile.X},{collisionInfos[i].CollidedTile.Y}> 와의 충돌에서 미끄러질 방향 : {slideDir}");

                if (slideDir == MoveDir.MoveNone)
                    continue;  // 미끄러질 수 없음. Tile과 너무 정면으로 충돌한 상황  
                 
                Vector2 SlideBlockTile = GetSlideBlockTile(moveDir, slideDir, collisionInfos[i].CollidedTile);
                if (SlideBlockTile == new Vector2(float.NaN, float.NaN)) continue;

                if (collisionInfos.Any(info => info.CollidedTile == SlideBlockTile))
                {
                    Console.WriteLine("미끄러짐 불가능!");
                    continue;
                }

                else
                {
                    // 위치 보정 
                    Console.WriteLine("미끄러짐 가능!");
                    Console.WriteLine($"미끄러지기 전 좌표 : <{correctedX} , {correctedY}>");
                    if (collisionInfos[i].OverlapPercentage > 0.8f) continue; // 벽과 너무 많이 겹친 경우 Slide X
                    Vector2 SlidedPos = GetSlidedPos(correctedX, correctedY, slideDir, collisionInfos[i].CollidedTile);
                    correctedX = SlidedPos.X;
                    correctedY = SlidedPos.Y;
                    Console.WriteLine($"미꺼러진   후 좌표 : <{correctedX} , {correctedY}>");
                }
            }

            return new Vector2(correctedX, correctedY);
        }

        MoveDir GetCollisionSlideDir (MoveDir moveDir, CollisionInfo collisionInfo)
        {
            // 타일 한개와의 관계이기 때문에 MoveDir.Up이 면서 leftCollision, RightCollision 경우는 배제. 
            // 이경우는 Slide될 수 없는 경우로 판단

            // 일단 테스트는 이렇게 하고 Up이랑 Down 나중에 묶을 수 있을 것 같다. left랑 right만 확인하면 될듯.
            // 우선은 잘 작동하는지 확인하고 나중에 최적화하자.
            if (moveDir == MoveDir.Up)
            {
                if (collisionInfo.IsTopCollision && collisionInfo.IsLeftCollision && collisionInfo.IsRightCollision) return MoveDir.MoveNone;
                if (collisionInfo.IsTopCollision && collisionInfo.IsLeftCollision) return MoveDir.Right;
                if (collisionInfo.IsTopCollision && collisionInfo.IsRightCollision) return MoveDir.Left;
            }

            else if (moveDir == MoveDir.Down)
            {
                if (collisionInfo.IsBottomCollision && collisionInfo.IsLeftCollision && collisionInfo.IsRightCollision) return MoveDir.MoveNone;
                if (collisionInfo.IsBottomCollision && collisionInfo.IsLeftCollision) return MoveDir.Right;
                if (collisionInfo.IsBottomCollision && collisionInfo.IsRightCollision) return MoveDir.Left;
            }

            else if (moveDir == MoveDir.Left)
            {
                if (collisionInfo.IsLeftCollision && collisionInfo.IsTopCollision && collisionInfo.IsBottomCollision) return MoveDir.MoveNone;
                if (collisionInfo.IsLeftCollision && collisionInfo.IsTopCollision) return MoveDir.Down;
                if (collisionInfo.IsLeftCollision && collisionInfo.IsBottomCollision) return MoveDir.Up;
            }

            else if (moveDir == MoveDir.Right)
            {
                if (collisionInfo.IsRightCollision && collisionInfo.IsTopCollision && collisionInfo.IsBottomCollision) return MoveDir.MoveNone;
                if (collisionInfo.IsRightCollision && collisionInfo.IsTopCollision) return MoveDir.Down;
                if (collisionInfo.IsRightCollision && collisionInfo.IsBottomCollision) return MoveDir.Up;
            }

            return MoveDir.MoveNone;
        }
       
        Vector2 GetSlideBlockTile(MoveDir moveDir, MoveDir slideDir, Vector2 CollidedTile)
        {
            if (moveDir == MoveDir.Up || moveDir == MoveDir.Down)
            {
                if (slideDir == MoveDir.Right) return new Vector2(CollidedTile.X + 1, CollidedTile.Y);
                if (slideDir == MoveDir.Left) return new Vector2(CollidedTile.X - 1, CollidedTile.Y);
            }

            else if (moveDir == MoveDir.Left || moveDir == MoveDir.Right)
            {
                if (slideDir == MoveDir.Up) return new Vector2(CollidedTile.X, CollidedTile.Y + 1);
                if (slideDir == MoveDir.Down) return new Vector2(CollidedTile.X, CollidedTile.Y - 1);
            }

            return new Vector2(float.NaN, float.NaN);
        }

        Vector2 GetSlidedPos(float currentPosX , float currentPoxY , MoveDir slideDir, Vector2 CollidedTilePos)
        {
            // 일단은 Overlap정도와 상관 없이 무조건 Slide되도록 한다.
            // 나중에 Overlap 정도를 계산해서 Slide될 것인지 말 것인지 결정하도록 하겠음.

            float slidedPosX = currentPosX;
            float slidedPosY = currentPoxY; 

            if (slideDir == MoveDir.Left)
            {
                slidedPosX -= 0.15f;  // 왼쪽으로 조금씩 이동

                if (Math.Floor(slidedPosX) != CollidedTilePos.X)
                {
                    if (slidedPosX <= Math.Floor(slidedPosX) + 0.5f)
                        slidedPosX = (float)Math.Floor(slidedPosX) + 0.5f;  // n.5로 고정
                }

                else
                {
                    // 작성할 필요 없을듯? 
                }
            }

            else if (slideDir == MoveDir.Right)
            {
                slidedPosX += 0.15f;  // 오른쪽으로 조금씩 이동

                if (Math.Floor(slidedPosX) != CollidedTilePos.X)
                {
                    if (slidedPosX >= Math.Floor(slidedPosX) + 0.5f)
                        slidedPosX = (float)Math.Floor(slidedPosX) + 0.5f;  // n.5로 고정
                }
            }

            else if (slideDir == MoveDir.Up)
            {
                slidedPosY += 0.15f;  // 위로 조금씩 이동

                if (Math.Floor(slidedPosY) != CollidedTilePos.Y)
                {
                    // 좌표를 n.5로 고정 (위쪽으로 이동 중일 때)
                    if (slidedPosY >= Math.Floor(slidedPosY) + 0.5f)
                        slidedPosY = (float)Math.Floor(slidedPosY) + 0.5f;  // n.5로 고정
                }
            }

            else if (slideDir == MoveDir.Down)
            {
                slidedPosY -= 0.15f;  // 아래로 조금씩 이동

                if (Math.Floor(slidedPosY) != CollidedTilePos.Y)
                {
                    // 좌표를 n.5로 고정 (아래쪽으로 이동 중일 때)
                    if (slidedPosY <= Math.Floor(slidedPosY) + 0.5f)
                        slidedPosY = (float)Math.Floor(slidedPosY) + 0.5f;  // n.5로 고정
                }
            }

            return new Vector2(slidedPosX, slidedPosY);
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

                    if (collisionInfo.OverlapPercentage <= 0.4f)
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
                        else if (collisionInfo.IsLeftCollision && CanMoveRight(correctedX, correctedY, tilemapData, direction))
                        {
                            Console.WriteLine("Moving Right!!!");
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

                    if (collisionInfo.OverlapPercentage <= 0.4f)
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

        public bool CanMoveRight(float currentX, float currentY, TileInfo[,] tileMapData, MoveDir collisionDir)
        {
            // 오른쪽 타일의 X 좌표
            int rightTileX = (int)Math.Floor(currentX) + 1;
            int currentTileY = (int)Math.Floor(currentY);

            int tileYToCheck;

            // 충돌 방향에 따른 타일 검사 (위쪽, 아래쪽에서 충돌했는지 여부)
            switch (collisionDir)
            {
                case MoveDir.Up:
                    // 충돌이 위쪽에서 발생한 경우 -> 위 타일 검사
                    tileYToCheck = currentTileY  +  1;
                    break;

                case MoveDir.Down:
                    // 충돌이 아래쪽에서 발생한 경우 -> 아래 타일 검사
                    tileYToCheck = currentTileY - 1;
                    break;

                default:
                    // 기본적으로 같은 레벨의 타일을 검사
                    tileYToCheck = currentTileY;
                    break;
            }

            // 타일맵 경계를 벗어나면 false 반환
            if (rightTileX >= tileMapData.GetLength(0) || tileYToCheck < 0 || tileYToCheck >= tileMapData.GetLength(1))
                return false;

            // 오른쪽 타일과 충돌한 위치에 해당하는 타일이 모두 비어있는지 확인
            bool isRightTileEmpty = !(tileMapData[rightTileX, currentTileY].isBlocktPermanently || tileMapData[rightTileX, currentTileY].isBlocktTemporary);
            bool isTileToCheckEmpty = !(tileMapData[rightTileX, tileYToCheck].isBlocktPermanently || tileMapData[rightTileX, tileYToCheck].isBlocktTemporary);

            // 둘 다 비어있어야 오른쪽으로 이동 가능
            return isRightTileEmpty && isTileToCheckEmpty;
        }

        //public bool CanMoveRight(float currentX, float currentY, TileInfo[,] tileMapData)
        //{
        //    // 오른쪽 타일의 좌표
        //    int rightTileX = (int)Math.Floor(currentX) + 1;
        //    int currentTileY = (int)Math.Floor(currentY);

        //    // 오른쪽 위 타일의 좌표
        //    int rightTopTileY = currentTileY + 1;

        //    // 타일맵 경계를 벗어나면 false
        //    if (rightTileX >= tileMapData.GetLength(0) || currentTileY < 0 || rightTopTileY >= tileMapData.GetLength(1))
        //        return false;

        //    // 오른쪽 타일과 오른쪽 위 타일이 모두 비어있는지 확인 (벽이 없으면 true)
        //    bool isRightTileEmpty = !(tileMapData[rightTileX, currentTileY].isBlocktPermanently || tileMapData[rightTileX, currentTileY].isBlocktTemporary);
        //    bool isRightTopTileEmpty = !(tileMapData[rightTileX, rightTopTileY].isBlocktPermanently || tileMapData[rightTileX, rightTopTileY].isBlocktTemporary);

        //    // 둘 다 비어있어야 오른쪽 이동 가능
        //    return isRightTileEmpty && isRightTopTileEmpty;
        //}

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
