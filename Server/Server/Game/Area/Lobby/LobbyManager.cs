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

        List<PlayerInfo> _players =  new List<PlayerInfo>();

        public List<PlayerInfo> Players
        {
            get { return _players; }
        }

        


        public void Add(PlayerInfo player)
        {
            lock (_lock)
            {
                // 자기 자신에게 기존에 있던 타인의 정보들을 보내야 한다.
                S_OtherPlayerEnterLobby OtherEnterPacket = new S_OtherPlayerEnterLobby();
                foreach (PlayerInfo p in _players)
                {
                    OtherEnterPacket.Otherplayers.Add(p.lobbyPlayerInfo);
                }
                player.Session.Send(OtherEnterPacket);

                // 자기 자신에 대한 정보를 자신에게 보내야 한다.
                S_MyPlayerEnterLobby MeEnterPacket = new S_MyPlayerEnterLobby();
                MeEnterPacket.Player = player.lobbyPlayerInfo;
                player.Session.Send(MeEnterPacket);

                // 타인에게 자기 자신에 대한 정보를 보내야 한다.
                S_OtherPlayerEnterLobby MeBroadcastPacket = new S_OtherPlayerEnterLobby();
                MeBroadcastPacket.Otherplayers.Add(player.lobbyPlayerInfo);
                foreach (PlayerInfo p in _players)
                {
                    p.Session.Send(MeBroadcastPacket);
                }

                Console.WriteLine("Clinet의 Lobby List UI에 추가해야함");
                 
                _players.Add(player);
             }    
        }

        public bool Erase(PlayerInfo player)
        {
            lock (_lock)
            {
                Console.WriteLine("Clinet의 Lobby List UI에 제거해야함");
                // 여기서 Client의 UI에서 제거되도록 Packet을 날려줘야함
                foreach (PlayerInfo cur in _players)
                {
                    if (cur.PlayerDbId == player.PlayerDbId)
                    {
                        _players.Remove(cur);
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
