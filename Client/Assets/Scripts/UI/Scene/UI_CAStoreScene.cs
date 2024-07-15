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
    public GameObject mainCategoryPanel; // MainCategory 패널

    [SerializeField]
    public GameObject itemListPanel;

    //public GameObject[] subCategoryPanels; // 각 대분류 패널 (예: 아이템, 캐릭터 등)


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

        //// 아이템의 세로 길이를 ViewPort 세로 길이의 절반으로 설정
        //float cellHeight = viewportHeight / 4;
        //float cellWidth = ItemPanelViewport.rect.width / 2;

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


        // 내코드
        //var AnchorDiff = itemListPanel.GetComponent<RectTransform>().anchorMax - itemListPanel.GetComponent<RectTransform>().anchorMin;
        //float width = GetComponent<RectTransform>().rect.width;
        //float height = GetComponent<RectTransform>().rect.height;

        //width = width * AnchorDiff.x;
        //height = height * AnchorDiff.y;

        //float cellHeight = height / 4;
        //float cellWidth = width / 2;

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);


        // GPT가 작성해준 코드
        //RectTransform rectTransform = GetComponent<RectTransform>();
        //RectTransform itemListPanelRectTransform = itemListPanel.GetComponent<RectTransform>();

        //// Anchor 차이를 계산
        //Vector2 anchorDiff = itemListPanelRectTransform.anchorMax - itemListPanelRectTransform.anchorMin;

        //// 부모의 크기를 기준으로 실제 크기 계산
        //float parentWidth = rectTransform.rect.width;
        //float parentHeight = rectTransform.rect.height;

        //float width = parentWidth * anchorDiff.x;
        //float height = parentHeight * anchorDiff.y;

        //// Spacing 값을 고려
        //float spacingX = ItemPanelGridLayout.spacing.x;
        //float spacingY = ItemPanelGridLayout.spacing.y;

        //// Scrollbar 크기 고려
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

        //// 아이템의 세로 길이를 ViewPort 세로 길이의 1/4로 설정
        //float cellHeight = (height - spacingY * 3 - horizontalScrollbarHeight) / 4; // 4행
        //float cellWidth = (width - spacingX * 1 - verticalScrollbarWidth) / 2; // 2열

        //ItemPanelGridLayout.cellSize = new Vector2(cellWidth, cellHeight);
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
