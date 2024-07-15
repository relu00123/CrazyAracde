using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    public GameObject itemListPanel;

    //public GameObject[] subCategoryPanels; // �� ��з� �г� (��: ������, ĳ���� ��)


    public override void Init()
    {
        base.Init();

        //mainCategoryPanel = Instantiate(mainCategoryPanel, transform);

        SetupMainCategoryHoverEvents();

        AdjustGridCellSize();
        PopulateItems();
    }

    private void AdjustGridCellSize()
    {
        //float viewportHeight = ItemPanelViewport.rect.height;

        //// �������� ���� ���̸� ViewPort ���� ������ �������� ����
        //float cellHeight = viewportHeight / 4;
        //float cellWidth = ItemPanelViewport.rect.width / 2;

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


        // ���ڵ�
        //var AnchorDiff = itemListPanel.GetComponent<RectTransform>().anchorMax - itemListPanel.GetComponent<RectTransform>().anchorMin;
        //float width = GetComponent<RectTransform>().rect.width;
        //float height = GetComponent<RectTransform>().rect.height;

        //width = width * AnchorDiff.x;
        //height = height * AnchorDiff.y;

        //float cellHeight = height / 4;
        //float cellWidth = width / 2;

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


        // GPT�� �ۼ����� �ڵ�
        //RectTransform rectTransform = GetComponent<RectTransform>();
        //RectTransform itemListPanelRectTransform = itemListPanel.GetComponent<RectTransform>();

        //// Anchor ���̸� ���
        //Vector2 anchorDiff = itemListPanelRectTransform.anchorMax - itemListPanelRectTransform.anchorMin;

        //// �θ��� ũ�⸦ �������� ���� ũ�� ���
        //float parentWidth = rectTransform.rect.width;
        //float parentHeight = rectTransform.rect.height;

        //float width = parentWidth * anchorDiff.x;
        //float height = parentHeight * anchorDiff.y;

        //// Spacing ���� ���
        //float spacingX = ItemPanelGridLayout.spacing.x;
        //float spacingY = ItemPanelGridLayout.spacing.y;

        //// Scrollbar ũ�� ���
        //float verticalScrollbarWidth = 0;
        //float horizontalScrollbarHeight = 0;

        //Scrollbar verticalScrollbar = itemListPanel.GetComponentInChildren<Scrollbar>();
        //Scrollbar horizontalScrollbar = itemListPanel.GetComponentInChildren<Scrollbar>();

        //if (verticalScrollbar != null && verticalScrollbar.direction == Scrollbar.Direction.BottomToTop)
        //{
        //    verticalScrollbarWidth = verticalScrollbar.GetComponent<RectTransform>().rect.width;
        //}
        //if (horizontalScrollbar != null && horizontalScrollbar.direction == Scrollbar.Direction.LeftToRight)
        //{
        //    horizontalScrollbarHeight = horizontalScrollbar.GetComponent<RectTransform>().rect.height;
        //}

        //// �������� ���� ���̸� ViewPort ���� ������ 1/4�� ����
        //float cellHeight = (height - spacingY * 3 - horizontalScrollbarHeight) / 4; // 4��
        //float cellWidth = (width - spacingX * 1 - verticalScrollbarWidth) / 2; // 2��

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);
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
        foreach (Transform mainCategory in mainCategoryPanel.transform)
        {
            EventTrigger trigger = mainCategory.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => OnMainCategoryHover(mainCategory));

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => OnMainCategoryExit(mainCategory));

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
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
