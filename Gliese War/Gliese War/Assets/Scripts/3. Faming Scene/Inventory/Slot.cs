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

public enum Magic
{
    Nothing,
    Ice,
    Fire,
    Toxic
}

public class RealItem
{
    public Item item;
    public Stat stat;
    public Magic magic;
}

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas invenCanvas;
    private GraphicRaycaster raycaster;
    public RealItem realItem;

    public Image itemImage;
    [SerializeField] private Text itemName;
    [SerializeField] private Text stat1Text;
    [SerializeField] private Text stat2Text;
    [SerializeField] private Image magicImage;
    
    //[SerializeField] private Text textCount;

    private void Start()
    {
        
        invenCanvas = GameObject.Find("InvenCanvas").GetComponent<Canvas>();
        raycaster = invenCanvas.GetComponent<GraphicRaycaster>();
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item _item)
    {
        realItem = new RealItem();
        realItem.item = _item;
        SetStat(realItem);
        realItem.magic = Magic.Nothing;
        
        Color color = magicImage.color;
        color.a = 0;
        magicImage.color = color;
        
        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        magicImage.sprite = null;
        
        ShowStat(realItem);
    }
    
    public void AddItemWithMagic(Item _item)
    {
        realItem = new RealItem();
        realItem.item = _item;
        SetStat(realItem);
        
        if(_item.itemCategory == Item.ItemCategory.Weapon)
            SetMagic(realItem);

        if (realItem.magic == Magic.Nothing)
        {
            Color color = magicImage.color;
            color.a = 0;
            magicImage.color = color;
        }

        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        magicImage.sprite = SetMagicImage(realItem);

        ShowStat(realItem);
    }

    public Slot ReAddItem(Slot s)
    {
        
        realItem = s.realItem;
        realItem.item = s.realItem.item;
        realItem.stat = s.realItem.stat;
        realItem.magic = s.realItem.magic;
        
        if (realItem.magic == Magic.Nothing)
        {
            Color color = magicImage.color;
            color.a = 0;
            magicImage.color = color;
        }
        
        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        magicImage.sprite = SetMagicImage(realItem);
        ShowStat(realItem);

        return this;
    }
    
    public void ReAddItem(RealItem _realItem)
    {
        realItem = _realItem;
        realItem.item = _realItem.item;
        realItem.stat = _realItem.stat;
        realItem.magic = _realItem.magic;
        
        if (realItem.magic == Magic.Nothing)
        {
            Color color = magicImage.color;
            color.a = 0;
            magicImage.color = color;
        }
        
        itemImage.sprite = realItem.item.itemImage;
        itemName.text = realItem.item.itemName;
        itemName.color = SetNameRankColor(realItem.item.itemRank);
        magicImage.sprite = SetMagicImage(realItem);
        ShowStat(realItem);
    }

    private void SetMagic(RealItem realItem)
    {
        int magic = Random.Range(1, 4);

        switch (magic)
        {
            case 1:
                realItem.magic = Magic.Ice;
                break;
            
            case 2:
                realItem.magic = Magic.Fire;
                break;
            
            case 3:
                realItem.magic = Magic.Toxic;
                break;
        }
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
                power = Random.Range(1, 2);
                break;

            case Item.ItemRank.Rare:
                power = Random.Range(2, 4);
                break;
            
            case Item.ItemRank.Epic:
                power = Random.Range(4, 6);
                break;
            
            case Item.ItemRank.Unique:
                power = Random.Range(6, 8);
                break;
            
            case Item.ItemRank.Legendary:
                power = Random.Range(8, 10);
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

    private Sprite SetMagicImage(RealItem realItem)
    {
        switch (realItem.magic)
        {
            case Magic.Ice:
                return Inventory.instance.magicImages[0];

            case Magic.Fire:
                return Inventory.instance.magicImages[1];
            
            case Magic.Toxic:
                return Inventory.instance.magicImages[2];
            
            case Magic.Nothing:
                return null;
        }

        return null;
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
        DragSlot.instance.transform.position = eventData.position + new Vector2(175f, 0f);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position + new Vector2(175f, 0f);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData,results);
        GameObject hitObject = results[1].gameObject;
        if (hitObject == null)
        {
            DragSlot.instance.DragSetData(null);
            DragSlot.instance.SetColor(0,0);
            DragSlot.instance.dragedSlot = null;
            DragSlot.instance.transform.position = new Vector2(0, 0);
            return;
        }
        
        if(DragSlot.instance.dragedSlot.realItem.item.itemCategory.ToString() == hitObject.tag.ToString())
        {
            Equip temp = hitObject.GetComponent<Equip>();
            temp.SetImage(DragSlot.instance.dragedSlot.realItem);
            temp.SetAlpha(1f);
            
            Inventory.instance.DeleteItem(DragSlot.instance.dragedSlot);
            
            switch (DragSlot.instance.dragedSlot.realItem.item.itemCategory)
            {
                case Item.ItemCategory.Helmet:
                {
                    if (CPlayer.instance.helmet != null)
                    {
                        Inventory.instance.ReAddItem(CPlayer.instance.helmet);
                        CPlayer.instance.UnEquip(CPlayer.instance.helmet);
                    }
                    CPlayer.instance.helmet = DragSlot.instance.dragedSlot.realItem;
                    CPlayer.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                    //Player.instance.WearHelmet();
                }
                    break;

                case Item.ItemCategory.Armor:
                {
                    if(CPlayer.instance.armor != null)
                    {
                        Inventory.instance.ReAddItem(CPlayer.instance.armor);
                        CPlayer.instance.UnEquip(CPlayer.instance.armor);
                    }
                    CPlayer.instance.armor = DragSlot.instance.dragedSlot.realItem;
                    CPlayer.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                    //Player.instance.WearArmor();
                }
                    break;

                case Item.ItemCategory.Shoes:
                {
                    if(CPlayer.instance.shoe != null)
                    {
                        Inventory.instance.ReAddItem(CPlayer.instance.shoe);
                        CPlayer.instance.UnEquip(CPlayer.instance.shoe);
                    }
                    CPlayer.instance.shoe = DragSlot.instance.dragedSlot.realItem;
                    CPlayer.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                    //Player.instance.WearShoe();
                }
                    break;

                case Item.ItemCategory.Weapon:
                {
                    if (hitObject.name == "Weapon1")
                    {
                        if (CPlayer.instance.weapon1 != null)
                        {
                            Inventory.instance.ReAddItem(CPlayer.instance.weapon1);
                            CPlayer.instance.UnEquip(CPlayer.instance.weapon1);
                        }
                        CPlayer.instance.weapon1 = DragSlot.instance.dragedSlot.realItem;
                        CPlayer.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                        CPlayer.instance.EquipWeapon();
                    }
                    else if (hitObject.name == "Weapon2")
                    {
                        if (CPlayer.instance.weapon2 != null)
                        {
                            Inventory.instance.ReAddItem(CPlayer.instance.weapon2);
                            CPlayer.instance.UnEquip(CPlayer.instance.weapon2);
                        }
                        CPlayer.instance.weapon2 = DragSlot.instance.dragedSlot.realItem;
                        CPlayer.instance.Equip(DragSlot.instance.dragedSlot.realItem);
                        CPlayer.instance.EquipWeapon();
                    }
                    
                    
                }
                    break;
            }
        }

        DragSlot.instance.DragSetData(null);
        DragSlot.instance.SetColor(0,0);
        DragSlot.instance.dragedSlot = null;
        DragSlot.instance.transform.position = new Vector2(0, 0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
