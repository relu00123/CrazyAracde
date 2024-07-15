using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game 
{
    public class PlayerInfo
    {
        public LobbyPlayerInfo lobbyPlayerInfo { get; set; }


        public int PlayerDbId { get; set; }

        public ClientSession Session { get; set; }

        public string name { get; set; }


        //public StatInfo Stat { get; private set; } = new StatInfo();
        public LevelInfo Level { get; private set; } = new LevelInfo();
        // TODO 
        // Inventory 


    }
}
