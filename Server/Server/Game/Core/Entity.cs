using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Entity
    {
        public int Id { get; private set; }
        public string Name { get; set; }

        public Entity(int id)
        {
            Id = id;
        }

        public Entity(int id, string name) : this(id)
        {
            Name = name;
        }
    }


}
