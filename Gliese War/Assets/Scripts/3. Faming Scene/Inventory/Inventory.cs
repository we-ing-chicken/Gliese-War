using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //public static bool inventoryActivated = false;

    [SerializeField] private GameObject inventoryBase;
    [SerializeField] private GameObject slotsParent;
    [SerializeField] private Slot slotPrefab;
    
    private List<Slot> slots;
    public Dictionary<Item, int> itemList;
    
    public Item[] knife;
    public Item[] spear;
    public Item[] hammer;
    
    void Start()
    {
        //slots = slotsParent.GetComponentsInChildren<Slot>();
        slots = new List<Slot>();
        itemList = new Dictionary<Item, int>();
        
        for (int i = 0; i < knife.Length ; ++i)
        {
            itemList.Add(knife[i], 0);
        }
        
        for (int i = 0; i < spear.Length ; ++i)
        {
            itemList.Add(spear[i], 0);
        }
        
        for (int i = 0; i < hammer.Length ; ++i)
        {
            itemList.Add(hammer[i], 0);
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     inventoryActivated = !inventoryActivated;
        //
        //     if (inventoryActivated)
        //         OpenInventory();
        //     else
        //         CloseInventory();
        // }
    }

    // private void OpenInventory()
    // {
    //     inventoryBase.SetActive(true);
    // }
    //
    // private void CloseInventory()
    // {
    //     inventoryBase.SetActive(false);
    // }

    public void AcquireItem(Item _item, int _count = 1)
    {
        Slot temp = Instantiate(slotPrefab,slotsParent.transform);
        slots.Add(temp);
        temp.AddItem(_item,_count);
        
        RectTransform slotsParentComponent = slotsParent.GetComponent<RectTransform>();
        if(slots.Count >= 4)
            slotsParentComponent.sizeDelta = new Vector2(slotsParentComponent.sizeDelta.x, 200 * slots.Count);
        else
            slotsParentComponent.sizeDelta = new Vector2(slotsParentComponent.sizeDelta.x, 800);
        
        slotsParent.transform.position = new Vector3(slotsParent.transform.position.x, 500 - 100 * slots.Count, 0);
        
    }

    public void MixButton()
    {
        for (int i = 0; i < slots.Count; ++i)
        {
            if (slots[i].item != null)
            {
                if (itemList.ContainsKey(slots[i].item))
                    itemList[slots[i].item]++;
            }
        }

        foreach (KeyValuePair<Item, int> d in itemList)
        {
            if (d.Value >= 3 && d.Key.itemRank != Item.ItemRank.Legendary)
                DeleteItem(d.Key, d.Value);
        }

        ResetItemList();
    }

    private void DeleteItem(Item item, int count)
    {
        for (int i = 0; i < 3 * (count/3); ++i)
        {
            foreach (Slot s in slots)
            {
                if (s.item == item)
                {
                    //s.ClearSlot();
                    slots.Remove(s);
                    Destroy(s.gameObject);
                    break;
                }
            }
        }

        for (int i = 0; i < count / 3; ++i)
        {
            AddUpgradeItem(item);
        }
    }
    
    private void ResetItemList()
    {
        foreach (KeyValuePair<Item, int> d in itemList.ToList())
            itemList[d.Key] = 0;
    }

    private void AddUpgradeItem(Item item)
    {
        switch (item.itemCategory)
        {
            case Item.ItemCategory.Hammer:
                AcquireItem(hammer[(int)item.itemRank + 1]);
                break;
            
            case Item.ItemCategory.Knife:
                AcquireItem(knife[(int)item.itemRank + 1]);
                break;
            
            case Item.ItemCategory.Spear:
                AcquireItem(spear[(int)item.itemRank + 1]);
                break;
        }
    }
    
    public void SortButton()
    {
        ResetItemList();

        for (int i = 0; i < slots.Count; ++i)
        {
            if (slots[i].item != null)
            {
                if (!itemList.ContainsKey(slots[i].item))
                    itemList.Add(slots[i].item, 1);
                else
                    itemList[slots[i].item]++;
            }
        }

        foreach (Slot s in slots)
        {
            Destroy(s.gameObject);
        }
        
        slots = new List<Slot>();
        
        foreach (KeyValuePair<Item, int> d in itemList)
        {
            for (int i = 0; i < d.Value; ++i)
            {
                switch (d.Key.itemCategory)
                {
                    case Item.ItemCategory.Knife:
                        AcquireItem(knife[GetItemNum(d.Key)]);
                        break;

                    case Item.ItemCategory.Hammer:
                        AcquireItem(hammer[GetItemNum(d.Key)]);
                        break;

                    case Item.ItemCategory.Spear:
                        AcquireItem(spear[GetItemNum(d.Key)]);
                        break;

                    default:
                        break;
                }
            }
        }

        ResetItemList();
    }

    private int GetItemNum(Item item)
    {
        return (int)item.itemRank;
    }
    
    
}