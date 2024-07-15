using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 

public class UI_CALoginScene : UI_Scene
{
    public UI_PopUpOneBtn PopupUI { get; private set; }
    public CanvasGroup LoginCanvasGroup { get; private set; }


    enum GameObjects
    {
        AccountName,
        Password
    }

    enum Images
    {
        CreateBtn,
        LoginBtn
    }
    
    enum Canvasgroups
    {
        LoginPanel
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<CanvasGroup>(typeof(Canvasgroups));

        GetImage((int)Images.CreateBtn).gameObject.BindEvent(OnClickCreateButton);
        GetImage((int)Images.LoginBtn).gameObject.BindEvent(OnClickLoginButton);

        PopupUI = GetComponentInChildren<UI_PopUpOneBtn>();
        PopupUI.gameObject.SetActive(false);

    }
 
    public void OnClickCreateButton(PointerEventData evt)
    {
        Debug.Log("Create Button 누름");

        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<TMP_InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<TMP_InputField>().text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            //Debug.Log(res.CreateOk);
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<TMP_InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<TMP_InputField>().text = "";


            // ID 생성에 실패한 경우
            if (res.CreateOk == false)
            {
                // 이미 존재하는 ID입니다. 라고 출력하는 UI만들기 
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" 이미 존재하는 아이디 입니다.");
                

                // Panel 비활성화
                DisableLoginPanel(true);
            }

            // ID 생성에 성공한 경우
            else if (res.CreateOk == true)
            {
                // ID를 생성했습니다. 라고 출력하는 UI 만들기
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" ID를 생성했습니다.");
            }

        });
    }

    public void OnClickLoginButton(PointerEventData evt)
    {
        Debug.Log("Login Button 누름");

        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<TMP_InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<TMP_InputField>().text;


        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {
            Debug.Log(res.LoginOk);
            Managers.Network.ID = Get<GameObject>((int)GameObjects.AccountName).GetComponent<TMP_InputField>().text;
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<TMP_InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<TMP_InputField>().text = "";


            if (res.LoginOk)
            {
                Debug.Log("DataBase에 있는 ID / Pwd 입력");
                // TODO 
                // 게임 로비에 입장시켜줘야 한다. 
                Managers.Network.AccountId = res.AccountId;
                Managers.Network.Token = res.Token;
                Managers.Network.ConnectToGame(res.ServerList[0]);
                Managers.Scene.LoadScene(Define.Scene.CAMainLobby);
            }

            else
            {
                // 등록되지 않은 ID이거나 비밀번호를 잘못 입력했습니다. 라고 출력하는 UI 만들기
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" 등록되지 않은 ID이거나 비밀번호를 잘못 입력했습니다.");
            }


            //if (res.LoginOk)
            //{
            //    Managers.Network.AccountId = res.AccountId;
            //    Managers.Network.Token = res.Token;

            //    UI_SelectServerPopup popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
            //    popup.SetServers(res.ServerList);
            //}
        });
    }
    
    private void OnCancelButtonClicked(PointerEventData evt)
    {
        PopupUI.gameObject.SetActive(false);
        DisableLoginPanel(false);
    }

    public void DisableLoginPanel(bool _state)
    {
        CanvasGroup LoginCanvas = GetCanvasGroup((int)Canvasgroups.LoginPanel);

        if (_state) // Blur + Interactable x
        {
            LoginCanvas.alpha = 0.5f;
            LoginCanvas.blocksRaycasts = false;
            LoginCanvas.interactable = false;
        }

        else // Non Blur + Interactable
        {
            LoginCanvas.alpha = 1;
            LoginCanvas.blocksRaycasts = true;
            LoginCanvas.interactable = true;
        }
    }
}
