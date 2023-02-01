using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item;
    public int itemCount;
    public Image itemImage;

    [SerializeField]
    private Text textName;
    //[SerializeField] private Text textCount;

    private void Start()
    {
        
    }

    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;
        
        textName.text = item.itemName;
        textName.color = SetNameRankColor(item.itemRank);
        //textCount.text = itemCount.ToString();
    }

    private Color SetNameRankColor(Item.ItemRank rank)
    {
        switch (rank)
        {
            case Item.ItemRank.Normal:
                return Color.white;

            case Item.ItemRank.Rare:
                return Color.blue;
            
            case Item.ItemRank.Unique:
                return Color.magenta;
            
            case Item.ItemRank.Legendary:
                return Color.yellow;
            
            case Item.ItemRank.Epic:
                return Color.cyan;
            
            default:
                return Color.black;
        }
    }
    
    // 해당 슬롯의 아이탬 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        //textCount.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    public void ClearSlot()
    {
        // item = null;
        // itemCount = 0;
        // itemImage.sprite = null;
        // //SetColor(0);
        //
        // textName.text = "";
        // textCount.text = "0";
        
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragSlot.instance.dragSlot = this;
        DragSlot.instance.DragSetData(itemImage, textName.text);
        //DragSlot.instance.DragSetData(itemImage, textName.text, textCount.text);
        DragSlot.instance.transform.position = eventData.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.DragSetData(itemImage, "");
        //DragSlot.instance.DragSetData(itemImage, "", "");
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
