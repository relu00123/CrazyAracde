using System;
using System.Collections.Generic;
using System.Text;

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

       

    }
}
