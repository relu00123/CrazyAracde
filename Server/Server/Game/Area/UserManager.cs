using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore.Internal;
using Server.Game.Area.Lobby;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Server.Game
{
    public class UserManager : JobSerializer
    {
        public static UserManager Instance { get; } = new UserManager();

        object _lock = new object();

        Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();

        public Dictionary<int, PlayerInfo> Players
        {
            get { return _players; }
        }

        public void Add(int _playerID, PlayerInfo _playerInfo)
        {
            lock (_lock)
            {
                // 자기 자신에게 기존에 있던 타인의 정보들을 보내야 한다.
                S_OtherPlayerEnterLobby OtherEnterPacket = new S_OtherPlayerEnterLobby();
                 
                foreach (KeyValuePair<int, PlayerInfo> p in _players)
                {
                    OtherEnterPacket.Otherplayers.Add(p.Value.lobbyPlayerInfo);
                }

                _playerInfo.Session.Send(OtherEnterPacket);

                // 자기 자신에 대한 정보를 자신에게 보내야 한다.
                S_MyPlayerEnterLobby MeEnterPacket = new S_MyPlayerEnterLobby();
                MeEnterPacket.Player = _playerInfo.lobbyPlayerInfo;
                _playerInfo.Session.Send(MeEnterPacket);

                // 타인에게 자기 자신에 대한 정보를 보내야 한다.
                S_OtherPlayerEnterLobby MeBroadcastPacket = new S_OtherPlayerEnterLobby();
                MeBroadcastPacket.Otherplayers.Add(_playerInfo.lobbyPlayerInfo);
                foreach (KeyValuePair<int, PlayerInfo> p in _players)
                {
                    p.Value.Session.Send(MeBroadcastPacket);
                }

                Console.WriteLine("Clinet의 Lobby List UI에 추가해야함");

                _players.Add(_playerID, _playerInfo);
            }
        }

        public void Update()
        {
            Flush();
        }

        public void Remove(int _playerID)
        {
            lock(_lock)
            {
                PlayerInfo Focusedinfo = Find(_playerID);

                if (Focusedinfo != null)
                {
                    Console.WriteLine("Remove Error - UserManager.cs - Remove()");
                }

                // 플레이어가 게임을 나갔다는 패킷을 Broadcast 해야한다. 
                S_PlayerLeaveLobby PlayerLeavePacket = new S_PlayerLeaveLobby();
                PlayerLeavePacket.Player = Focusedinfo.lobbyPlayerInfo;

                foreach (KeyValuePair<int, PlayerInfo> p in _players)
                {
                    p.Value.Session.Send(PlayerLeavePacket);
                }

                _players.Remove(_playerID);
            }
        }

        public PlayerInfo Find(int _playerID)
        {
            lock(_lock)
            {
                PlayerInfo playerinfo = null;
                if (_players.TryGetValue(_playerID, out playerinfo))
                    return playerinfo;
            }
            return null;
        }
    
    
        
    }
}
