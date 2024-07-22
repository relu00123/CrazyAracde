using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;


 

public class UI_CAStoreScene : UI_Scene
{
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private RectTransform ItemPanelViewport;
    [SerializeField] private GridLayoutGroup ItemPanelGridLayout;
    [SerializeField] private Transform ItemPanelContent;
    [SerializeField] private GameObject FreeMoneyButton;
    [SerializeField] private GameObject ToLobbyButton;

    [SerializeField]
    public GameObject mainCategoryPanel; // MainCategory 패널

    //public List<GameObject> subCategoryPanels = new List<GameObject>();

    public Dictionary<string, GameObject> subCategoryPanels = new Dictionary<string, GameObject>();


    public GameObject FocusedSubCategoryPanel;

    public GameObject FocusedSubCategoryButton;

    [SerializeField] private int MoneyAmount = 50000;



    [SerializeField]
    public GameObject itemListPanel;

    //public GameObject[] subCategoryPanels; // 각 대분류 패널 (예: 아이템, 캐릭터 등)


    Color NormalSubCatButton = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
    Color HoverSubCatButton = new Color(3f / 255f, 12f / 255f, 77f / 255f, 92f / 255f);
    Color ClickedSubCatButton = new Color(226f / 255f, 23f / 255f, 255f / 255f, 119f / 255f);


    public override void Init()
    {
        base.Init();

        //mainCategoryPanel = Instantiate(mainCategoryPanel, transform);

        InitializeSubCategory();
        SetupMainCategoryHoverEvents();
        SetupMainCategoryExitEvents();
        SetupSubCategoryButtonEvents();
        SetupFreeMoneyEvents();
        SetupOnToLobbyButton();

        AdjustGridCellSize();
        //PopulateItems();
    }

    void InitializeSubCategory()
    {
        foreach (Transform child in transform.Find("SubCategoryPanels"))
        {
            GameObject obj = child.gameObject;
            subCategoryPanels.Add(obj.name, obj);

            if (child.gameObject.name == "MainSubPanel")
                FocusedSubCategoryPanel = child.gameObject;
        }
    }


    private void AdjustGridCellSize()
    {
        // GPT Traial2 
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform itemListPanelRectTransform = itemListPanel.GetComponent<RectTransform>();

        // 부모의 절대 크기 계산
        float parentWidth = itemListPanelRectTransform.rect.width;
        float parentHeight = itemListPanelRectTransform.rect.height;

        // Spacing 값을 고려
        float spacingX = ItemPanelGridLayout.spacing.x;
        float spacingY = ItemPanelGridLayout.spacing.y;

        // GridLayoutGroup의 Padding 값을 고려
        float paddingLeft = ItemPanelGridLayout.padding.left;
        float paddingRight = ItemPanelGridLayout.padding.right;
        float paddingTop = ItemPanelGridLayout.padding.top;
        float paddingBottom = ItemPanelGridLayout.padding.bottom;

        // Scrollbar 크기 고려
        float verticalScrollbarWidth = 0;
        float horizontalScrollbarHeight = 0;

        Scrollbar verticalScrollbar = itemListPanel.GetComponentsInChildren<Scrollbar>()
                                          .FirstOrDefault(sb => sb.name == "Scrollbar Vertical");
        Scrollbar horizontalScrollbar = itemListPanel.GetComponentsInChildren<Scrollbar>()
                                                     .FirstOrDefault(sb => sb.name == "Scrollbar Horizontal");

        if (verticalScrollbar != null && verticalScrollbar.direction == Scrollbar.Direction.BottomToTop)
        {
            verticalScrollbarWidth = verticalScrollbar.GetComponent<RectTransform>().rect.width;
        }
        if (horizontalScrollbar != null && horizontalScrollbar.direction == Scrollbar.Direction.LeftToRight)
        {
            horizontalScrollbarHeight = horizontalScrollbar.GetComponent<RectTransform>().rect.height;
        }

        // 아이템의 세로 길이를 ViewPort 세로 길이의 1/4로 설정
        float cellHeight = (parentHeight - paddingTop - paddingBottom - spacingY * 3 - horizontalScrollbarHeight) / 4; // 4행
        float cellWidth = (parentWidth - paddingLeft - paddingRight - spacingX * 1 - verticalScrollbarWidth) / 2; // 2열

        ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


    }

    // 임시로 사용할 함수
    private void PopulateItems()
    {
        Debug.Log(ItemPanelContent.name);
        Debug.Log(ItemPrefab);


        for (int i = 0; i < 20; i++)
        {
            GameObject newItem = Instantiate(ItemPrefab, ItemPanelContent);
            newItem.SetActive(true);


            RectTransform rectTransform = newItem.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;  // Scale을 1로 설정
            rectTransform.anchoredPosition3D = Vector3.zero;  // 초기 위치를 (0,0,0)으로 설정


            //Debug.Log($"Item {i} Position: {newItem.GetComponent<RectTransform>().anchoredPosition}");
            //Debug.Log($"Item {i} Size: {newItem.GetComponent<RectTransform>().sizeDelta}");
        }
    }


    private void SetupMainCategoryHoverEvents()
    {
        foreach (Transform button in transform.Find("MainCategoryPanel"))
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnMainCatHover((button.gameObject)); });

            trigger.triggers.Add(entryEnter);


            EventTrigger.Entry ClickTrigger = new EventTrigger.Entry();
            ClickTrigger.eventID = EventTriggerType.PointerClick;
            ClickTrigger.callback.AddListener((data) => {  OnClickMainCat(button.gameObject); });

            trigger.triggers.Add(ClickTrigger);
        }
    }

    private void SetupMainCategoryExitEvents()
    {
        GameObject MainPanel = transform.Find("MainCategoryPanel").gameObject;
        EventTrigger trigger = MainPanel.GetComponent<EventTrigger>();

        EventTrigger.Entry LeaveTrigger = new EventTrigger.Entry();
        LeaveTrigger.eventID = EventTriggerType.PointerExit;
        LeaveTrigger.callback.AddListener((eventData) => 
        {
            if (FocusedSubCategoryPanel != null)
            {
                foreach (KeyValuePair<string, GameObject> kvp in subCategoryPanels)
                {
                    kvp.Value.SetActive(false);
                }

                FocusedSubCategoryPanel.SetActive(true);
            }
        });

        trigger.triggers.Add(LeaveTrigger);
    }

    private void SetupSubCategoryButtonEvents()
    {
        // 모든 소분류 패널들의 Button들에 대해서 Event Trigger를 달아 줄 것임. 
        foreach (KeyValuePair<string, GameObject> kvp in subCategoryPanels)
        {
            foreach(Transform item in kvp.Value.transform)
            {
                EventTrigger trigger = item.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry Hover = new EventTrigger.Entry();
                Hover.eventID = EventTriggerType.PointerEnter; 
                Hover.callback.AddListener((eventData) => {
                    GameObject HoveredObject = ((PointerEventData)eventData).pointerEnter;
                    if (HoveredObject != FocusedSubCategoryButton)
                        item.gameObject.GetComponent<UnityEngine.UI.Image>().color = HoverSubCatButton;
                });

                EventTrigger.Entry Click = new EventTrigger.Entry();
                Click.eventID = EventTriggerType.PointerClick;
                Click.callback.AddListener((eventData) => {
                    GameObject ClickedObject = ((PointerEventData)eventData).pointerPress;
                    if (FocusedSubCategoryButton != null)
                        FocusedSubCategoryButton.GetComponent<UnityEngine.UI.Image>().color = NormalSubCatButton;
                    ClickedObject.GetComponent<UnityEngine.UI.Image>().color = ClickedSubCatButton;
                    FocusedSubCategoryButton = ClickedObject;

                    // Click 한 소분류에 해당하는 아이템들을 로드해와야 한다. 
                    string mainname = FocusedSubCategoryPanel.name;
                    mainname = mainname.Replace("SubPanel", "");
                    string subname = FocusedSubCategoryButton.gameObject.name;

                    Debug.Log($"main name : {mainname}");
                    Debug.Log($"Sub  name : {subname}");

                    DeleteShownItems();
                    ShowCertainCategoryItems(mainname, subname);

                
                });

                EventTrigger.Entry Leave = new EventTrigger.Entry();
                Leave.eventID = EventTriggerType.PointerExit;
                Leave.callback.AddListener((eventData) => {
                    GameObject exitedObject = ((PointerEventData)eventData).pointerEnter;
                    if (exitedObject != FocusedSubCategoryButton)
                        exitedObject.GetComponent<UnityEngine.UI.Image>().color = NormalSubCatButton;
                });

                trigger.triggers.Add(Hover);
                trigger.triggers.Add(Click);
                trigger.triggers.Add(Leave);
            }
        }
    }

    public void SetupFreeMoneyEvents()
    {
        EventTrigger trigger = FreeMoneyButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry Click = new EventTrigger.Entry();
        Click.eventID = EventTriggerType.PointerClick;
        Click.callback.AddListener((eventData) => {
            // 돈을 충전해 달라는 Packet을 Server에 보내야한다.
            Debug.Log("Money Fill Required from Clinet!!");

            C_AddMoney AddMoneyPacket = new C_AddMoney();
            AddMoneyPacket.Moneyamount = MoneyAmount;

            Managers.Network.Send(AddMoneyPacket);
        });

        trigger.triggers.Add(Click);
    }


    public void SetupOnToLobbyButton()
    {
        EventTrigger trigger = ToLobbyButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry Click = new EventTrigger.Entry();
        Click.eventID = EventTriggerType.PointerClick;
        Click.callback.AddListener((eventData) => {
            // 메인 로비로 이동
            Managers.Scene.LoadScene(Define.Scene.CAMainLobby);

            C_EnterLobby enterLobbyPacket = new C_EnterLobby();
            enterLobbyPacket.Player = Managers.UserInfo.myLobbyPlayerInfo;

            Managers.Network.Send(enterLobbyPacket);
           
        });

        trigger.triggers.Add(Click);
    }
     

    public void DeleteShownItems()
    {
        foreach (Transform child in ItemPanelContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowCertainCategoryItems(string mainPanelName, string subPanelName)
    {
        if (Managers.Data.CAItemCategoryDict.TryGetValue(mainPanelName, out Dictionary<string, List<int>> subCategoryDict))
        {
            if (subCategoryDict.TryGetValue(subPanelName, out List<int> itemList))
            {
                foreach (int itemId in itemList)
                {
                    if (Managers.Data.CAItemDict.TryGetValue(itemId, out CAItemData itemData))
                    {

                        GameObject newItem = Instantiate(ItemPrefab, ItemPanelContent);

                        UI_StoreItem ItemScript = newItem.GetComponent<UI_StoreItem>();
                        
                        if (ItemScript != null)
                        {
                            ItemScript.SetInfo(itemData);
                        }
                       
                        newItem.SetActive(true);
                        RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                        rectTransform.localScale = Vector3.one;  // Scale을 1로 설정
                        rectTransform.anchoredPosition3D = Vector3.zero;  // 초기 위치를 (0,0,0)으로 설정


                        Debug.Log($"item id : {itemData.id} , item Name : {itemData.itemName}");

                        // 작동이 잘 안되면 Populate 함수보고 작업하기 
                    }
                }
            }
        }
    }

    private void OnClickMainCat(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} Clicked!");

        foreach (Transform button in transform.Find("MainCategoryPanel"))
        {
            RectTransform buttonRect = button.gameObject.GetComponent<RectTransform>();
            Vector2 anchorMax = buttonRect.anchorMax;
            buttonRect.anchorMax = new Vector2(anchorMax.x, 0.8f);
        }

        RectTransform clickedButtonRect = gameObject.GetComponent<RectTransform>();
        Vector2 clickedAnchorMax = clickedButtonRect.anchorMax;
        clickedButtonRect.anchorMax = new Vector2(clickedAnchorMax.x, 1);

        // 클릭시 지금 Focused된 SubCategory를 설정
        string PanelName = gameObject.name + "SubPanel";
        if (subCategoryPanels.TryGetValue(PanelName, out GameObject panel))
        {
            FocusedSubCategoryPanel = panel;
        }
    }

    private void OnMainCatHover(GameObject button)
    {
        // 모든 SubCategoryPanels 비활성화
        foreach (KeyValuePair<string, GameObject> kvp in subCategoryPanels)
        {
            kvp.Value.SetActive(false);
        }

        string PanelName = button.name + "SubPanel";

        if (subCategoryPanels.TryGetValue(PanelName, out GameObject panel))
        {
            panel.SetActive(true);
        }

        else
        {
            Debug.Log($"{PanelName} Panel Does not exist");
        }
    }

    private void OnMainCategoryHover(Transform mainCategory)
    {
        // 모든 소분류 패널 비활성화
        foreach (Transform category in mainCategoryPanel.transform)
        {
            Transform subCategoryPanel = category.Find("SubCategoryPanel");
            if (subCategoryPanel != null)
            {
                subCategoryPanel.gameObject.SetActive(false);
            }
        }

        // 해당 대분류의 소분류 패널 활성화
        Transform activeSubCategoryPanel = mainCategory.Find("SubCategoryPanel");
        if (activeSubCategoryPanel != null)
        {
            activeSubCategoryPanel.gameObject.SetActive(true);
        }
    }

    private void OnMainCategoryExit(Transform mainCategory)
    {
        // 대분류에서 마우스를 뗐을 때 소분류 패널 비활성화
        Transform subCategoryPanel = mainCategory.Find("SubCategoryPanel");
        if (subCategoryPanel != null)
        {
            subCategoryPanel.gameObject.SetActive(false);
        }
    }


    public void UpdateMoneyUI()
    {
        // TODO 
        // Player의 Inventory에서 돈을 가져와서 가지고 있는 돈을 UI에 뿌려줘야 한다.
        // 7-Segement의 UpdateMoney() 를 호출해야할듯
        // 그런데 지금 인벤토리가 없다!! 초비상!!!! 
        Debug.Log("UpdateMoneyUI 호출");
    }

}
