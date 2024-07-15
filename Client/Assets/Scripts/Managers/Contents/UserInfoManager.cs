using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoManager
{
    Dictionary<int, LobbyPlayerInfo> _players = new Dictionary<int, LobbyPlayerInfo>();


    public int myPlayerDBId { get; private set; }

    public LobbyPlayerInfo myLobbyPlayerInfo { get; private set; }
   
    public void Add(LobbyPlayerInfo player, bool isMyPlayer = false)
    {
        _players.Add(player.PlayerDbId, player);
        if (isMyPlayer)
        {
            myPlayerDBId = player.PlayerDbId;
            myLobbyPlayerInfo = player;
        }

        Managers.UserList.AddUser(player);

        Debug.Log($"User Added. Current player : {_players.Count}");
    }

    public void Remove(LobbyPlayerInfo player)
    {
        if (player.PlayerDbId == myPlayerDBId)
        {
            myPlayerDBId = -1;
        }

        _players.Remove(player.PlayerDbId);

        Managers.UserList.RemoveUser(player);

        Debug.Log($"User Levaed. Current player : {_players.Count}");

    }


    public LobbyPlayerInfo  Find(int playerDbId)
    {
        return _players[playerDbId]; 
    }

    public void ChangePlayerInfo(LobbyPlayerInfo player)
    {
        _players[player.PlayerDbId] = player; 
    }
}
