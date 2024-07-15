using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChattingPannel : UI_Base
{
    // 자식 객체들을 참조할 멤버 변수 선언
    public GameObject scrollText;
    public GameObject inputText;
    public GameObject textTemplate;
    private RectTransform contentRectTransform;

    // 필요한 컴포넌트 참조를 위한 변수 선언
    private ScrollRect scrollRect;
    private TMP_InputField inputField;
    private Scrollbar verticalScrollbar;

    public override void Init()
    {
        // 자식 객체 초기화
        scrollText = transform.Find("ScrollText").gameObject;
        inputText = transform.Find("InputText").gameObject;
        textTemplate = scrollText.transform.Find("Viewport/Content/TextTemplate").gameObject;


        // 컴포넌트 참조 초기화
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
            UnityEngine.Debug.Log("입력된 텍스트: " + text);
            AddTextToScrollViewRequest(text);
            inputField.text = "";

            // 입력 필드에 포커스를 다시 설정하여 커서가 계속 깜박이도록 함
            inputField.ActivateInputField();
        }
    }

    private void AddTextToScrollViewRequest(string text)
    {
        if (textTemplate != null)
        {
            // 아래의 내용이 Network에서 Packet을 받았을때 해야하는 내용들임
            // 여기에서는 Server에 Packet을 보내야한다. 
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

            // 스크롤 뷰를 맨 아래로 이동
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }


    public void OnScrollbarValueChanged(float vlaue)
    {
        //verticalScrollbar.size = 1;
    }

}
