using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas invenCanvas;
    private GraphicRaycaster raycaster;
    public Item item;
    public int itemCount;
    public Image itemImage;

    [SerializeField]
    private Text textName;
    //[SerializeField] private Text textCount;

    private void Start()
    {
        invenCanvas = GameObject.Find("InvenCanvas").GetComponent<Canvas>();
        raycaster = invenCanvas.GetComponent<GraphicRaycaster>();
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
            
            case Item.ItemRank.Epic:
                return Color.magenta;
            
            case Item.ItemRank.Unique:
                return Color.yellow;
            
            case Item.ItemRank.Legendary:
                return Color.cyan;
            
            
            default:
                return Color.black;
        }
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
        DragSlot.instance.dragedSlot = this;
        DragSlot.instance.DragSetData(itemImage, textName.text, item);
        DragSlot.instance.transform.position = eventData.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData,results);
        GameObject hitObject = results[2].gameObject;

        if(DragSlot.instance.item.itemCategory.ToString() == hitObject.tag.ToString())
        {
            Equip temp = hitObject.GetComponent<Equip>();
            temp.SetImage(DragSlot.instance.item);
            
            Inventory.instance.DeleteItem(DragSlot.instance.dragedSlot);
            
            switch (DragSlot.instance.item.itemCategory)
            {
                case Item.ItemCategory.Helmet:
                {
                    if(Player.instance.helmet != null)
                        Inventory.instance.AcquireItem(Player.instance.helmet);
                    Player.instance.helmet = DragSlot.instance.item;
                }
                    break;

                case Item.ItemCategory.Armor:
                {
                    if(Player.instance.armor != null)
                        Inventory.instance.AcquireItem(Player.instance.armor);
                    Player.instance.armor = DragSlot.instance.item;
                }
                    break;

                case Item.ItemCategory.Shoes:
                {
                    if(Player.instance.shoe != null)
                        Inventory.instance.AcquireItem(Player.instance.shoe);
                    Player.instance.shoe = DragSlot.instance.item;
                }
                    break;

                case Item.ItemCategory.Weapon:
                {
                    if (hitObject.name == "Weapon1")
                    {
                        if (Player.instance.weapon1 != null)
                        {
                            Inventory.instance.AcquireItem(Player.instance.weapon1);
                        }
                        Player.instance.weapon1 = DragSlot.instance.item;
                    }
                    else if (hitObject.name == "Weapon2")
                    {
                        if (Player.instance.weapon2 != null)
                        {
                            Inventory.instance.AcquireItem(Player.instance.weapon2);
                        }
                        Player.instance.weapon2 = DragSlot.instance.item;
                    }
                    
                    
                }
                    break;
            }
            
        }
        
        DragSlot.instance.DragSetData(itemImage, "", null);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragedSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
