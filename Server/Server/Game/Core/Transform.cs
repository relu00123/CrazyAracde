using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }

        public Transform()
        {
            Position = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
        }

        public Transform(Vector3 position, Vector3 scale)
        {
            Position = position;
            Scale = scale;
        }
    }
}
