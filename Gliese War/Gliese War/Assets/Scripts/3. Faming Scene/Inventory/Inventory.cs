using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    static public Inventory instance;
    [SerializeField] private GameObject inventoryBase;
    [SerializeField] private GameObject slotsParent;
    [SerializeField] private Slot slotPrefab;
    public GameObject statParent;

    
    private List<Slot> slots;
    
    public Item[] helmet;
    public Item[] armor;
    public Item[] shoes;
    public Item[] knife;
    public Item[] spear;
    public Item[] hammer;

    public Sprite[] magicImages;

    void Start()
    {
        instance = this;
        slots = new List<Slot>();
    }

    public void AcquireItem(Item _item)
    {
        Slot temp = Instantiate(slotPrefab, slotsParent.transform);
        
        temp.AddItem(_item);
        slots.Add(temp);
        
        ResizeSlotParent();
    }

    public void AcquireItemWithMagic(Item _item)
    {
        Slot temp = Instantiate(slotPrefab, slotsParent.transform);
        
        temp.AddItemWithMagic(_item);
        slots.Add(temp);
        
        ResizeSlotParent();
    }
    
    public void ReAddItem(RealItem realItem)
    {
        Slot temp = Instantiate(slotPrefab, slotsParent.transform);
        
        temp.ReAddItem(realItem);
        slots.Add(temp);
        
        ResizeSlotParent();
    }

    public void DeleteItem(Slot temp)
    {
        Destroy(temp.gameObject);
        slots.Remove(temp);
    }

    public void MixButton()
    {
        Dictionary<Item, int> itemCount = new Dictionary<Item, int>();
        
        for (int i = 0; i < slots.Count; ++i)
        {
            if (slots[i].realItem.item != null)
            {
                if (itemCount.ContainsKey(slots[i].realItem.item))
                    itemCount[slots[i].realItem.item]++;
                else
                    itemCount.Add(slots[i].realItem.item,1);
            }
        }
        
        foreach (KeyValuePair<Item, int> d in itemCount)
        {
            if (d.Value >= 3 && d.Key.itemRank != Item.ItemRank.Legendary)
                DeleteItemByUpgrade(d.Key, d.Value);
        }
        
        DeleteAllItemObject();
        MakeAllItemObject();
    }

    private void DeleteItemByUpgrade(Item item, int count)
    {
        for (int i = 0; i < 3 * (count/3); ++i)
        {
            foreach (Slot s in slots)
            {
                if (s.realItem.item == item)
                {
                    slots.Remove(s);
                    break;
                }
            }
        }

        for (int i = 0; i < count / 3; ++i)
        {
            AddUpgradeItem(item);
        }
    }

    private void AddUpgradeItem(Item item)
    {
        switch (item.itemCategory)
        {
            case Item.ItemCategory.Helmet:
                AcquireItemWithMagic(helmet[(int)item.itemRank + 1]);
                break;
            
            case Item.ItemCategory.Armor:
                AcquireItemWithMagic(armor[(int)item.itemRank + 1]);
                break;
            
            case Item.ItemCategory.Shoes:
                AcquireItemWithMagic(shoes[(int)item.itemRank + 1]);
                break;

            case Item.ItemCategory.Weapon:
            {
                switch (item.weaponType)
                {
                    case Item.WeaponType.Knife:
                        AcquireItemWithMagic(knife[(int)item.itemRank + 1]);
                        break;
                    
                    case Item.WeaponType.Spear:
                        AcquireItemWithMagic(spear[(int)item.itemRank + 1]);
                        break;
                    
                    case Item.WeaponType.Hammer:
                        AcquireItemWithMagic(hammer[(int)item.itemRank + 1]);
                        break;
                    
                }
                
            }
                break;
        }
    }
    
    public void SortButton()
    {
        DeleteAllItemObject();

        slots = slots.OrderBy(x => x.realItem.item.itemCategory).ThenBy(x => x.realItem.item.weaponType).ThenByDescending(x => x.realItem.item.itemRank).ToList();

        MakeAllItemObject();
    }

    private void DeleteAllItemObject()
    {
        Transform[] childList = slotsParent.GetComponentsInChildren<Transform>();

        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    private void MakeAllItemObject()
    {
        foreach (Slot s in slots)
        {
            Slot temp = Instantiate(slotPrefab, slotsParent.transform);
            temp.ReAddItem(s.realItem);
        }
        
        ResizeSlotParent();
    }

    private void ResizeSlotParent()
    {
        RectTransform slotsParentComponent = slotsParent.GetComponent<RectTransform>();
        if(slots.Count >= 4)
            slotsParentComponent.sizeDelta = new Vector2(slotsParentComponent.sizeDelta.x, 150 * slots.Count);
        else
            slotsParentComponent.sizeDelta = new Vector2(slotsParentComponent.sizeDelta.x, 800);
        
        slotsParent.transform.position = new Vector3(slotsParent.transform.position.x, 500 - 100 * slots.Count, 0);        
    }
}