using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 

public class UI_GameRoomCreatePopup : UI_Popup
{
    [SerializeField] private Sprite CheckedImage;

    enum Buttons
    {
        CreateBtn,
        CancelBtn,
        MannerModeBtn,
        FreeModeBtn,
        RandomModeBtn,
        SecretRoomBtn,
    }

    enum GameObjects
    {
        Password,
    }

    Dictionary<TeamModeType, Buttons> TeamModeAssistDic = new Dictionary<TeamModeType, Buttons>
    {
        { TeamModeType.MannerMode, Buttons.MannerModeBtn },
        { TeamModeType.FreeMode, Buttons.FreeModeBtn },
        { TeamModeType.RandomMode, Buttons.RandomModeBtn }
    };

    // ���߿� CreateGameRoom Info ��Ŷ�� �������� ��Ƽ� ��������. 
    TeamModeType SelectedTeamMode = TeamModeType.MannerMode;
    bool         IsPasswordUsed = false;
   
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
         
        GetButton((int)Buttons.CreateBtn).gameObject.BindEvent(OnCreateBtnClicked);
        GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnCancelBtnClicked);
        GetButton((int)Buttons.MannerModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.FreeModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.RandomModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.SecretRoomBtn).gameObject.BindEvent(OnSecretRoomBtnClicked);

        GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = false;
    }

    public void OnTeamModeSelectBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnTeamModeSelectBtnClicked!");

        // ������ ���õǾ� �ִ��� �ʱ�ȭ (Image�� ������ �����ϰ� �ٲ۴�
        if (TeamModeAssistDic.TryGetValue(SelectedTeamMode, out Buttons MappingButton))
        {
            CheckUIApply(GetButton((int)MappingButton).gameObject, false);
        }

        // Ŭ���� ��ư�� �̸��� ������� TeamModeType ��ȯ
        string clickedButtonName = evt.pointerPress.name.Replace("Btn", "").ToUpper();
        if (Enum.TryParse(clickedButtonName, true, out TeamModeType teamMode))
        {
            SelectedTeamMode = teamMode;
            CheckUIApply(evt.pointerPress.gameObject, true);
        }
    }

    public void OnSecretRoomBtnClicked(PointerEventData evt)
    {
        Debug.Log("SecretRoom Button Clicked!");

        // ��й�ȣ �������� ����
        if (IsPasswordUsed)
        {
            // ��й�ȣ�� ���̻� ������� �ʱ� ������ üũ ǥ�ø� ������Ѵ�. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, false);


            // Password  ������ �ϴû����� ������ �Ѵ�.
            // Input Field�� ���̻� Ŭ���� �� ������ ������ �Ѵ�. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = false;
        }

        // ��й�ȣ �������� ����
        else
        {
            // ��й�ȣ�� ����Ѵٴ� üũ ǥ�ø� �޾���� �Ѵ�. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, true);

            // Password Panel�� ������ �Ͼ������ ������ �Ѵ�.
            // Input Field�� Ŭ���� �� �ֵ��� ������ �Ѵ�. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = true;
        }

        IsPasswordUsed = !IsPasswordUsed;
        GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().text = "";
    }

    public void OnCreateBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnClickBtnClicked!");
    }

    public void OnCancelBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnCancelBtnClicked!");

        // Ŭ���� ��ư ��ü ��������
        GameObject clickedButton = evt.pointerPress;
        if (clickedButton != null)
        {
            Debug.Log($"Clicked Button: {clickedButton.name}");

            // �θ� ��ü ��������
            Transform parentTransform = clickedButton.transform.parent;

            // �θ��� �θ� ��ü ��������
            Transform grandParentTransform = parentTransform.parent;

            grandParentTransform.gameObject.SetActive(false);

        }
        Managers.UI.ClosePopupUI();
    }

    private void CheckUIApply(GameObject targetObject,  bool isAssigning = true)
    {
        if (isAssigning == true)  // �̹����� �����ϴ� ���
        {
            targetObject.GetComponent<Image>().sprite = CheckedImage;
            targetObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }

        else // �̹����� ���� ���
        {
            targetObject.GetComponent<Image>().sprite = null;
            targetObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
