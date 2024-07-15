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

        // ���� �������� ������ �ִ��� Ȯ��
        int startIndex = (curPage - 1) * usersPerPage;
        if (startIndex >= _userList.Count)
        {
            // ���� �������� ������ ������ �������� ����
            curPage = Mathf.Max(1, curPage - 1);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {

        onUpdateUI?.Invoke(GetUsersByPage(curPage));


        // ���� �������� ���� ����� UI�� ǥ�� 
        List<LobbyPlayerInfo> usersOnCurrentPage = GetUsersByPage(curPage);

        // TO DO 
        // ���� �ִ�  User����� ����ؼ� UI ������Ʈ
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
