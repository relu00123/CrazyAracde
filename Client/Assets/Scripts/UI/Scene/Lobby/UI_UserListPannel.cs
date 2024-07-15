using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserListPannel : UI_Base
{
    private VerticalLayoutGroup UserGrid { get; set; }

    [SerializeField]
    private UI_UserInfoItem userinfo_UI;

    private List<UI_UserInfoItem> userInfoItems = new List<UI_UserInfoItem>();


    public override void Init()
    {
        UserGrid = GetComponent<VerticalLayoutGroup>();

        if (UserGrid == null)
        {
            Debug.Log("U_UserListPanel.cs - UserGrid 가져오기 실패");
        }

        else
        {
            Debug.Log("U_UserListPanel.cs - UserGrid 성공적으로 가져옴");
        }

        Managers.UserList.onUpdateUI += UpdateUserListUI;

        for (int i = 0; i < Managers.UserList.usersPerPage; i++)
        {
            UI_UserInfoItem item = Instantiate(userinfo_UI, UserGrid.transform);
            item.gameObject.SetActive(false);
            userInfoItems.Add(item);
        }

        UpdateChildrenSize();
    }

    public void UpdateUserListUI(List<LobbyPlayerInfo> userList)
    {
        // 표시할 사용자 목록 크기
        int userCount = userList.Count;

        for (int i = 0; i < userInfoItems.Count; i++)
        {

            if (i < userCount)
            {
                // 정보변경
                userInfoItems[i].SetLobbyPlayerInfo(userList[i]);
                userInfoItems[i].gameObject.SetActive(true);
            }

            else
            {
                userInfoItems[i].gameObject.SetActive(false);
            }
        }

        UpdateChildrenSize();
    }

    private void UpdateChildrenSize()
    {
        RectTransform parentRectTransform = UserGrid.GetComponent<RectTransform>();
        float parentHeight = parentRectTransform.rect.height;
        float itemHeight = parentHeight / 10;

        foreach (UI_UserInfoItem item in userInfoItems)
        {
            RectTransform itemRectTransform = item.GetComponent<RectTransform>();

            if (itemRectTransform != null)
            {
                itemRectTransform.sizeDelta = new Vector2(itemRectTransform.sizeDelta.x, itemHeight);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
    }
}
