using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserListManager 
{
    List<LobbyPlayerInfo> _userList = new List<LobbyPlayerInfo>();
    public int usersPerPage { get; set; } = 2;
    int curPage = 1;

    public event Action<List<LobbyPlayerInfo>> onUpdateUI;


    public void AddUser(LobbyPlayerInfo user)
    {
        _userList.Add(user);
        _userList.Sort((x, y) => x.LevelInfo.Level.CompareTo(y.LevelInfo.Level));
        UpdateUI();
    }

    public void RemoveUser(LobbyPlayerInfo user) {
        _userList.Remove(user);

        // 현재 페이지에 유저가 있는지 확인
        int startIndex = (curPage - 1) * usersPerPage;
        if (startIndex >= _userList.Count)
        {
            // 현재 페이지에 유저가 없으면 페이지를 줄임
            curPage = Mathf.Max(1, curPage - 1);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {

        onUpdateUI?.Invoke(GetUsersByPage(curPage));


        // 현재 페이지의 유저 목록을 UI에 표시 
        List<LobbyPlayerInfo> usersOnCurrentPage = GetUsersByPage(curPage);

        // TO DO 
        // 현재 있는  User목록을 사용해서 UI 업데이트
        foreach (var user in usersOnCurrentPage)
        {
            Debug.Log($"Name : {user.Name}");
        }
    }

    public List<LobbyPlayerInfo> GetUsersByPage(int pageNumber)
    {
        int startIndex = (pageNumber - 1) * usersPerPage;
        int count = Mathf.Min(usersPerPage, _userList.Count - startIndex);
        return _userList.GetRange(startIndex, count);
    }

    public void ChangePlayerInfo(LobbyPlayerInfo playerInfo)
    {
        int index = _userList.FindIndex(p => p.PlayerDbId == playerInfo.PlayerDbId);
        
        if (index != -1)
        {
            _userList[index] = playerInfo;
            _userList.Sort((x, y) => x.LevelInfo.Level.CompareTo(y.LevelInfo.Level));
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("player not found in the list");
        }
    }

    public void NextPage()
    {
        int totalPages = Mathf.CeilToInt((float)_userList.Count / usersPerPage);
        if (curPage < totalPages)
        {
            curPage++;
            UpdateUI();
        }
    }

    public void PreviousPage()
    {
        if (curPage > 1)
        {
            curPage--;
            UpdateUI();
        }
    }
}
