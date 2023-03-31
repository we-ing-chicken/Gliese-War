using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public struct Stat
{
    public int attackPower;
    public int defensePower;
    public int health;
    public int moveSpeed;
}

public class RealItem
{
    public Item item;
    public Stat stat;
}

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas invenCanvas;
    private GraphicRaycaster raycaster;
    public RealItem realItem;
    public int itemCount;
    public Image itemImage;

    [SerializeField] private Text itemName;
    [SerializeField] private Text stat1Text;
    [SerializeField] private Text stat2Text;
    
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
    public void AddItem(Item _item)
    {
        realItem = new RealItem();
        realItem.item = _item;
        SetStat(realItem);
        
        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        ShowStat(realItem);
    }

    public void ReAddItem(RealItem _realItem)
    {
        realItem = new RealItem();
        realItem.item = _realItem.item;
        realItem.stat = _realItem.stat;
        
        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        ShowStat(realItem);
    }
    
    private void SetStat(RealItem realItem)
    {
        switch (realItem.item.itemCategory)
        {
            case Item.ItemCategory.Helmet:
                realItem.stat.defensePower = GetRankStat(realItem.item);
                realItem.stat.health = GetRankStat(realItem.item);
                break;

            case Item.ItemCategory.Armor:
                realItem.stat.defensePower = GetRankStat(realItem.item);
                realItem.stat.health = GetRankStat(realItem.item);
                break;

            case Item.ItemCategory.Shoes:
                realItem.stat.defensePower = GetRankStat(realItem.item);
                realItem.stat.moveSpeed = GetRankStat(realItem.item);
                break;

            case Item.ItemCategory.Weapon:
                realItem.stat.attackPower = GetRankStat(realItem.item);
                break;
        }
    }
    
    private int GetRankStat(Item item)
    {
        int power = 0;

        switch (item.itemRank)
        {
            case Item.ItemRank.Normal:
                power = Random.Range(1, 5);
                break;

            case Item.ItemRank.Rare:
                power = Random.Range(5, 10);
                break;
            
            case Item.ItemRank.Epic:
                power = Random.Range(10, 15);
                break;
            
            case Item.ItemRank.Unique:
                power = Random.Range(15, 20);
                break;
            
            case Item.ItemRank.Legendary:
                power = Random.Range(20, 25);
                break;
        }

        return power;
    }

    private void ShowStat(RealItem realItem)
    {
        switch (realItem.item.itemCategory)
        {
            case Item.ItemCategory.Helmet:
                stat1Text.text = "Def : " + realItem.stat.defensePower;
                stat2Text.text = "HP : " + realItem.stat.health;
                break;

            case Item.ItemCategory.Armor:
                stat1Text.text = "Def : " + realItem.stat.defensePower;
                stat2Text.text = "HP : " + realItem.stat.health;
                break;

            case Item.ItemCategory.Shoes:
                stat1Text.text = "Def : " + realItem.stat.defensePower;
                stat2Text.text = "Spd : " + realItem.stat.moveSpeed;
                break;

            case Item.ItemCategory.Weapon:
                stat1Text.text = "ATK : " + realItem.stat.attackPower;
                stat2Text.text = "";
                break;
        }
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
        DragSlot.instance.DragSetData(realItem);
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
        GameObject hitObject = results[1].gameObject;
        
        if(DragSlot.instance.dragedSlot.realItem.item.itemCategory.ToString() == hitObject.tag.ToString())
        {
            
            Equip temp = hitObject.GetComponent<Equip>();
            temp.SetImage(DragSlot.instance.dragedSlot.realItem.item);
            Inventory.instance.DeleteItem(DragSlot.instance.dragedSlot);
            
            switch (DragSlot.instance.dragedSlot.realItem.item.itemCategory)
            {
                case Item.ItemCategory.Helmet:
                {
                    if (Player.instance.helmet != null)
                    {
                        Inventory.instance.ReAddItem(Player.instance.helmet);
                        Player.instance.UnEquip(Player.instance.helmet);
                    }
                    Player.instance.helmet = DragSlot.instance.dragedSlot.realItem;
                    Player.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                }
                    break;

                case Item.ItemCategory.Armor:
                {
                    if(Player.instance.armor != null)
                    {
                        Inventory.instance.ReAddItem(Player.instance.armor);
                        Player.instance.UnEquip(Player.instance.armor);
                    }
                    Player.instance.armor = DragSlot.instance.dragedSlot.realItem;
                    Player.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                }
                    break;

                case Item.ItemCategory.Shoes:
                {
                    if(Player.instance.shoe != null)
                    {
                        Inventory.instance.ReAddItem(Player.instance.shoe);
                        Player.instance.UnEquip(Player.instance.shoe);
                    }
                    Player.instance.shoe = DragSlot.instance.dragedSlot.realItem;
                    Player.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                }
                    break;

                case Item.ItemCategory.Weapon:
                {
                    if (hitObject.name == "Weapon1")
                    {
                        if (Player.instance.weapon1 != null)
                        {
                            Inventory.instance.ReAddItem(Player.instance.weapon1);
                            Player.instance.UnEquip(Player.instance.weapon1);
                        }
                        Player.instance.weapon1 = DragSlot.instance.dragedSlot.realItem;
                        Player.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                    }
                    else if (hitObject.name == "Weapon2")
                    {
                        if (Player.instance.weapon2 != null)
                        {
                            Inventory.instance.ReAddItem(Player.instance.weapon2);
                            Player.instance.UnEquip(Player.instance.weapon2);
                        }
                        Player.instance.weapon2 = DragSlot.instance.dragedSlot.realItem;
                        Player.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                    }
                    
                    
                }
                    break;
            }
        }

        DragSlot.instance.DragSetData(null);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragedSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
