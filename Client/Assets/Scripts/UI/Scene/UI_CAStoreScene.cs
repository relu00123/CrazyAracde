using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CAStoreScene : UI_Scene
{
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private RectTransform ItemPanelViewport;
    [SerializeField] private GridLayoutGroup ItemPanelGridLayout;
    [SerializeField] private Transform ItemPanelContent;

    [SerializeField]
    public GameObject mainCategoryPanel; // MainCategory 패널

    //public List<GameObject> subCategoryPanels = new List<GameObject>();

    public Dictionary<string, GameObject> subCategoryPanels = new Dictionary<string, GameObject>();



    [SerializeField]
    public GameObject itemListPanel;

    //public GameObject[] subCategoryPanels; // 각 대분류 패널 (예: 아이템, 캐릭터 등)


    public override void Init()
    {
        base.Init();

        //mainCategoryPanel = Instantiate(mainCategoryPanel, transform);

        InitializeSubCategory();
        SetupMainCategoryHoverEvents();

        AdjustGridCellSize();
        PopulateItems();
    }

    void InitializeSubCategory()
    {
        foreach (Transform child in transform.Find("SubCategoryPanels"))
        {
            GameObject obj = child.gameObject;
            subCategoryPanels.Add(obj.name, obj);
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


            Debug.Log($"Item {i} Position: {newItem.GetComponent<RectTransform>().anchoredPosition}");
            Debug.Log($"Item {i} Size: {newItem.GetComponent<RectTransform>().sizeDelta}");
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

}
