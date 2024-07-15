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
    public GameObject mainCategoryPanel; // MainCategory �г�

    //public List<GameObject> subCategoryPanels = new List<GameObject>();

    public Dictionary<string, GameObject> subCategoryPanels = new Dictionary<string, GameObject>();



    [SerializeField]
    public GameObject itemListPanel;

    //public GameObject[] subCategoryPanels; // �� ��з� �г� (��: ������, ĳ���� ��)


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

        // �θ��� ���� ũ�� ���
        float parentWidth = itemListPanelRectTransform.rect.width;
        float parentHeight = itemListPanelRectTransform.rect.height;

        // Spacing ���� ���
        float spacingX = ItemPanelGridLayout.spacing.x;
        float spacingY = ItemPanelGridLayout.spacing.y;

        // GridLayoutGroup�� Padding ���� ���
        float paddingLeft = ItemPanelGridLayout.padding.left;
        float paddingRight = ItemPanelGridLayout.padding.right;
        float paddingTop = ItemPanelGridLayout.padding.top;
        float paddingBottom = ItemPanelGridLayout.padding.bottom;

        // Scrollbar ũ�� ���
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

        // �������� ���� ���̸� ViewPort ���� ������ 1/4�� ����
        float cellHeight = (parentHeight - paddingTop - paddingBottom - spacingY * 3 - horizontalScrollbarHeight) / 4; // 4��
        float cellWidth = (parentWidth - paddingLeft - paddingRight - spacingX * 1 - verticalScrollbarWidth) / 2; // 2��

        ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


    }

    // �ӽ÷� ����� �Լ�
    private void PopulateItems()
    {
        Debug.Log(ItemPanelContent.name);
        Debug.Log(ItemPrefab);


        for (int i = 0; i < 20; i++)
        {
            GameObject newItem = Instantiate(ItemPrefab, ItemPanelContent);
            newItem.SetActive(true);


            RectTransform rectTransform = newItem.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;  // Scale�� 1�� ����
            rectTransform.anchoredPosition3D = Vector3.zero;  // �ʱ� ��ġ�� (0,0,0)���� ����


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
        // ��� SubCategoryPanels ��Ȱ��ȭ
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
        // ��� �Һз� �г� ��Ȱ��ȭ
        foreach (Transform category in mainCategoryPanel.transform)
        {
            Transform subCategoryPanel = category.Find("SubCategoryPanel");
            if (subCategoryPanel != null)
            {
                subCategoryPanel.gameObject.SetActive(false);
            }
        }

        // �ش� ��з��� �Һз� �г� Ȱ��ȭ
        Transform activeSubCategoryPanel = mainCategory.Find("SubCategoryPanel");
        if (activeSubCategoryPanel != null)
        {
            activeSubCategoryPanel.gameObject.SetActive(true);
        }
    }

    private void OnMainCategoryExit(Transform mainCategory)
    {
        // ��з����� ���콺�� ���� �� �Һз� �г� ��Ȱ��ȭ
        Transform subCategoryPanel = mainCategory.Find("SubCategoryPanel");
        if (subCategoryPanel != null)
        {
            subCategoryPanel.gameObject.SetActive(false);
        }
    }

}
