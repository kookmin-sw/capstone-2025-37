using DG.Tweening;
using DominoGames.Util;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UI_DynamicScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    public int itemCount = 0;
    [AssetSelector(Paths = "Assets/Prefabs/ListItems")] public GameObject itemPrefab;

    [Header("생성할 리스트 아이템 갯수")]
    public int instantiateCount = 5;
    [Header("한 줄에 존재하는 리스트 아이템 갯수")]
    public int rowCount = 3;

    public float speedThreshold = 0.5f;

    [SerializeField] private List<UI_DynamicScrollViewItem> listItems = new();
    private int displayedFirstRowIndex = 0;
    private Vector2 paddingSpacing = new();

    [EnumToggleButtons]
    public enum EScrollType
    {
        TopToBottom, // 위에서 아래로 리스트 아이템 생성
        BottomToTop, // 아래에서 위로
        LeftToRight, // 왼쪽에서 오른쪽으로
        RightToLeft, // 오른쪽에서 왼쪽으로
    }

    public EScrollType scrollType = EScrollType.TopToBottom;


    private bool doNotShowInspector = true;
    [DisableIf("doNotShowInspector")] public RectOffset originPadding = new();

    [Button]
    private void InitSettings()
    {
        scrollRect = GetComponent<ScrollRect>();
        originPadding = GetComponent<ScrollRect>().content.GetComponent<GridLayoutGroup>().padding;
        ClearListItems();
        InstantiateListItems();
    }


    private GridLayoutGroup contentGrid;
    private ScrollRect scrollRect;

    bool isInit = false;
    [Button]
    public virtual void Init(int itemCount = 0)
    {
        isInit = true;

        this.itemCount = itemCount;

        scrollRect = GetComponent<ScrollRect>();
        contentGrid = scrollRect.content.GetComponent<GridLayoutGroup>();

        displayedFirstRowIndex = 0;
        InitItems();

        RectOffset newPadding = new(originPadding.left, originPadding.right, originPadding.top, originPadding.bottom);
        contentGrid.padding = newPadding;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentGrid.GetComponent<RectTransform>());

        paddingSpacing = contentGrid.cellSize + contentGrid.spacing;
    }

    public void SetPositionToOrigin()
    {
        contentGrid.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }



    private void ClearListItems()
    {
        int childCount = GetComponent<ScrollRect>().content.transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            DestroyImmediate(GetComponent<ScrollRect>().content.GetChild(0).gameObject);
        }

        listItems.Clear();
    }
    private void InstantiateListItems()
    {
        for(int i = 0; i < instantiateCount; i++)
        {
            var obj = Instantiate(itemPrefab, scrollRect.content.transform);
            var listItem = obj.GetComponent<UI_DynamicScrollViewItem>();
            listItem.scrollView = this;
            listItem.gameObject.SetActive(false);
            listItems.Add(listItem);

            if(i >= 0 && i < itemCount)
            {
                listItem.gameObject.SetActive(true);
                listItem.InitItem(i);
            }
        }
    }

    private void InitItems()
    {
        for(int i = 0; i < listItems.Count; i++)
        {
            if(i >= 0 && i < itemCount)
            {
                listItems[i].gameObject.SetActive(true);
                listItems[i].InitItem(i);
                listItems[i].itemId = i;
            }
        }
    }
    



    private void UpdateContentPading()
    {
        var clampIndex = Mathf.Clamp(displayedFirstRowIndex, 0, itemCount / rowCount);
        if (scrollType == EScrollType.TopToBottom)
        {
            contentGrid.padding.top = originPadding.top + (int)paddingSpacing.y * clampIndex;
        }
        else if(scrollType == EScrollType.BottomToTop)
        {
            contentGrid.padding.bottom = originPadding.bottom + (int)paddingSpacing.y * clampIndex;
        }
        else if(scrollType == EScrollType.LeftToRight)
        {
            contentGrid.padding.left = originPadding.left + (int)paddingSpacing.x * clampIndex;
        }
        else if(scrollType == EScrollType.RightToLeft)
        {
            contentGrid.padding.right = originPadding.right + (int)paddingSpacing.x * clampIndex;
        }
    }


    private void AddListItemTopIndex()
    {
        for (int i = 0; i < rowCount; i++)
        {
            int itemIndex = (displayedFirstRowIndex * rowCount + i) % listItems.Count;
            int itemId = displayedFirstRowIndex * rowCount + instantiateCount + i;

            if (itemIndex < 0)
            {
                itemIndex = instantiateCount + itemIndex;
            }

            listItems[itemIndex].gameObject.SetActive(false);
            listItems[itemIndex].transform.SetAsLastSibling();
            listItems[itemIndex].itemId = itemId;

            if (itemId >= 0 && itemId < itemCount)
            {
                listItems[itemIndex].gameObject.SetActive(true);
                listItems[itemIndex].InitItem(itemId);
            }
        }

        displayedFirstRowIndex++;
    }
    private void MinusListItemTopIndex()
    {
        for (int i = 0; i < rowCount; i++)
        {
            var itemIndex = (displayedFirstRowIndex * rowCount + instantiateCount - i - 1) % listItems.Count;
            var itemId = displayedFirstRowIndex * rowCount - i - 1;

            if(itemIndex < 0)
            {
                itemIndex = instantiateCount + itemIndex;
            }

            listItems[itemIndex].gameObject.SetActive(false);
            listItems[itemIndex].transform.SetAsFirstSibling();
            listItems[itemIndex].itemId = itemId;

            if (itemId >= 0 && itemId < itemCount)
            {
                listItems[itemIndex].gameObject.SetActive(true);
                listItems[itemIndex].InitItem(itemId);
            }
        }

        displayedFirstRowIndex--;
    }




    float sensitiveGap = 50f;
    private void Update()
    {
        if(scrollRect == null)
        {
            return;
        }

        if(scrollType == EScrollType.LeftToRight)
        {
            while(scrollRect.content.transform.localPosition.x < -paddingSpacing.x * (displayedFirstRowIndex + 2) - paddingSpacing.x)
            {
                AddListItemTopIndex(); // 오른쪽에 아이템 추가
            }

            while (scrollRect.content.transform.localPosition.x > -paddingSpacing.x * (displayedFirstRowIndex + 2) + paddingSpacing.x)
            {
                MinusListItemTopIndex(); // 왼쪽에 아이템 추가
            }
        }
        else if(scrollType == EScrollType.TopToBottom)
        {
            while (scrollRect.content.transform.localPosition.y - paddingSpacing.y > paddingSpacing.y * (displayedFirstRowIndex + 2))
            {
                AddListItemTopIndex(); // 아래에 아이템 추가
            }

            while (scrollRect.content.transform.localPosition.y + paddingSpacing.y < paddingSpacing.y * (displayedFirstRowIndex + 2))
            {
                // 위에 아이템 추가
                MinusListItemTopIndex();
            }
        }

        UpdateContentPading();

        if (usePositionConstraint)
        {
            SetPositionConstraint();
        }
    }


    [SerializeField] bool usePositionConstraint = false;
    bool canSetScrollPos = false;
    bool isDragging = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.content.GetComponent<RectTransform>().DOKill();
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canSetScrollPos = true;
        isDragging = false;
    }


    public void SetContentCenterPosition(int itemId)
    {
        if (scrollType == EScrollType.LeftToRight)
        {
            scrollRect.velocity = Vector3.zero;
            var pos = scrollRect.content.GetComponent<RectTransform>().anchoredPosition;
            pos.x = -(itemId / rowCount) * paddingSpacing.x;
            scrollRect.content.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    private Action<int> onPositionConstraintAction;
    public void SetOnPositionConstraintAction(Action<int> action)
    {
        this.onPositionConstraintAction = action;
    }
    public void SetPositionConstraint()
    {
        if (canSetScrollPos && !isDragging && scrollRect.velocity.magnitude < speedThreshold)
        {
            if(scrollType == EScrollType.LeftToRight)
            {
                int centerIndex = Mathf.RoundToInt(Mathf.Clamp(-scrollRect.content.GetComponent<RectTransform>().anchoredPosition.x / paddingSpacing.x, 0, (itemCount / rowCount) - 1));

                scrollRect.content.GetComponent<RectTransform>().DOAnchorPosX(-paddingSpacing.x * centerIndex, 0.5f).SetEase(Ease.OutQuad);
                onPositionConstraintAction?.Invoke(centerIndex);
            }

            scrollRect.velocity = Vector2.zero;
            canSetScrollPos = false;
        }
    }
}