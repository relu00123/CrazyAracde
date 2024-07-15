using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChattingPannel : UI_Base
{
    // �ڽ� ��ü���� ������ ��� ���� ����
    public GameObject scrollText;
    public GameObject inputText;
    public GameObject textTemplate;
    private RectTransform contentRectTransform;

    // �ʿ��� ������Ʈ ������ ���� ���� ����
    private ScrollRect scrollRect;
    private TMP_InputField inputField;
    private Scrollbar verticalScrollbar;

    public override void Init()
    {
        // �ڽ� ��ü �ʱ�ȭ
        scrollText = transform.Find("ScrollText").gameObject;
        inputText = transform.Find("InputText").gameObject;
        textTemplate = scrollText.transform.Find("Viewport/Content/TextTemplate").gameObject;


        // ������Ʈ ���� �ʱ�ȭ
        scrollRect = scrollText.GetComponent<ScrollRect>();
        inputField = inputText.GetComponentInChildren<TMP_InputField>();
        verticalScrollbar = scrollText.transform.Find("VerticalScrollbar").GetComponent<Scrollbar>();
        contentRectTransform = scrollText.transform.Find("Viewport/Content").GetComponent<RectTransform>();

        inputField.onEndEdit.AddListener(OnEndEdit);
        verticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }


    private void OnEndEdit(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            UnityEngine.Debug.Log("�Էµ� �ؽ�Ʈ: " + text);
            AddTextToScrollViewRequest(text);
            inputField.text = "";

            // �Է� �ʵ忡 ��Ŀ���� �ٽ� �����Ͽ� Ŀ���� ��� �����̵��� ��
            inputField.ActivateInputField();
        }
    }

    private void AddTextToScrollViewRequest(string text)
    {
        if (textTemplate != null)
        {
            // �Ʒ��� ������ Network���� Packet�� �޾����� �ؾ��ϴ� �������
            // ���⿡���� Server�� Packet�� �������Ѵ�. 
            C_Chatting chattingPacket = new C_Chatting()
            {
               Name = Managers.UserInfo.myLobbyPlayerInfo.Name,
               Chat = text,
            };

            Managers.Network.Send(chattingPacket);
        }
    }

    public void AddToScrollView(S_Chatting chatpacket)
    {
        string text = "";
        text += chatpacket.Name;
        text += " : ";
        text += chatpacket.Chat;


        if(textTemplate != null)
        {
            GameObject newTextObj = Instantiate(textTemplate, contentRectTransform);
            TMP_Text newText = newTextObj.GetComponent<TMP_Text>();
            newText.text = text;
            newTextObj.SetActive(true);

            // ��ũ�� �並 �� �Ʒ��� �̵�
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }


    public void OnScrollbarValueChanged(float vlaue)
    {
        //verticalScrollbar.size = 1;
    }

}
