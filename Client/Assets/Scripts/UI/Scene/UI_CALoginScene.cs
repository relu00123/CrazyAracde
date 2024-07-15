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
        Debug.Log("Create Button ����");

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


            // ID ������ ������ ���
            if (res.CreateOk == false)
            {
                // �̹� �����ϴ� ID�Դϴ�. ��� ����ϴ� UI����� 
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" �̹� �����ϴ� ���̵� �Դϴ�.");
                

                // Panel ��Ȱ��ȭ
                DisableLoginPanel(true);
            }

            // ID ������ ������ ���
            else if (res.CreateOk == true)
            {
                // ID�� �����߽��ϴ�. ��� ����ϴ� UI �����
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" ID�� �����߽��ϴ�.");
            }

        });
    }

    public void OnClickLoginButton(PointerEventData evt)
    {
        Debug.Log("Login Button ����");

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
                Debug.Log("DataBase�� �ִ� ID / Pwd �Է�");
                // TODO 
                // ���� �κ� ���������� �Ѵ�. 
                Managers.Network.AccountId = res.AccountId;
                Managers.Network.Token = res.Token;
                Managers.Network.ConnectToGame(res.ServerList[0]);
                Managers.Scene.LoadScene(Define.Scene.CAMainLobby);
            }

            else
            {
                // ��ϵ��� ���� ID�̰ų� ��й�ȣ�� �߸� �Է��߽��ϴ�. ��� ����ϴ� UI �����
                PopupUI.gameObject.SetActive(true);
                PopupUI.GetCancelButton().gameObject.BindEvent(OnCancelButtonClicked);
                PopupUI.SetMessageText(" ��ϵ��� ���� ID�̰ų� ��й�ȣ�� �߸� �Է��߽��ϴ�.");
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
