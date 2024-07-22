using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server.Game.Area.Lobby
{
    public class LobbyManager
    {
        public static LobbyManager Instance { get; } = new LobbyManager();

        object _lock = new object();

         
    }
}
