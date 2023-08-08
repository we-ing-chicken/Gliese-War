using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum DropLevel
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh
}

public class Drop : MonoBehaviour
{
    [SerializeField] private Inventory _inven;
    private int itemCount;
    [SerializeField] private DropLevel dropLevel;
    public bool isFirst = false;
    [SerializeField] private bool onTree = false;

    public AudioClip eatSound;

    private void Start()
    {
        _inven = FarmingManager.Instance.inventory;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     DropItem();
        // }
    }

    public void DropBasicItem()
    {
        Item[] items = new Item[5];
        for (int i = 0; i < items.Length; ++i)
            items[i] = new Item();
        items[0].itemCategory = Item.ItemCategory.Helmet;
        items[1].itemCategory = Item.ItemCategory.Armor;
        items[2].itemCategory = Item.ItemCategory.Shoes;
        items[3].itemCategory = Item.ItemCategory.Weapon;
        items[4].itemCategory = Item.ItemCategory.Weapon;

        for (int i = 0; i < 5; ++i)
        {
            GameObject temp;

            switch (items[i].itemCategory)
            {
                case Item.ItemCategory.Helmet:
                    temp = Instantiate(_inven.helmet[0].itemPrefab);
                    break;

                case Item.ItemCategory.Armor:
                    temp = Instantiate(_inven.armor[0].itemPrefab);
                    break;

                case Item.ItemCategory.Shoes:
                    temp = Instantiate(_inven.shoes[0].itemPrefab);
                    break;

                case Item.ItemCategory.Weapon:
                {
                    items[i].weaponType = (Item.WeaponType)Random.Range(0, 3);
                    switch (items[i].weaponType)
                    {
                        case Item.WeaponType.Hammer:
                            temp = Instantiate(_inven.hammer[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Sword:
                            temp = Instantiate(_inven.sword[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Spear:
                            temp = Instantiate(_inven.spear[0].itemPrefab);
                            break;

                        default:
                            temp = Instantiate(_inven.sword[0].itemPrefab);
                            break;                        
                    }
                    
                }
                    break;
                
                default:
                    temp = Instantiate(_inven.sword[0].itemPrefab);
                    break;
            }

            temp.transform.position = transform.position;
            if(items[i].itemCategory == Item.ItemCategory.Weapon)
                temp.transform.localScale = new Vector3(1f, 1f, 1f);
            else
                temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            
            BoxCollider col = temp.GetComponent<BoxCollider>();
            col.enabled = true;
            
            Magnet mag = temp.AddComponent<Magnet>();
            mag.itemCategory = items[i].itemCategory;
            mag.isFirst = true;
            
            if (items[i].itemCategory == Item.ItemCategory.Weapon)
            {
                mag.weaponType = items[i].weaponType;
            }
            else
            {
                mag.weaponType = Item.WeaponType.Nothing;
            }
            mag._inven = _inven;
                
            Rigidbody comp = temp.AddComponent<Rigidbody>();
            comp.AddExplosionForce(200f, transform.position, 200f, 10f);
        }
    }

    public void DropItem()
    {
        int itemCount = GetItemCount();

        for (int i = 0; i < itemCount; ++i)
        {
            Item.ItemCategory itemCategory = (Item.ItemCategory)Random.Range(0, 4);
            Item.WeaponType weaponType = Item.WeaponType.Nothing;

            if (isFirst)
            {
                itemCategory = Item.ItemCategory.Weapon;
                weaponType = Item.WeaponType.Sword;
            }
            
            GameObject temp;

            switch (itemCategory)
            {
                case Item.ItemCategory.Helmet:
                    temp = Instantiate(_inven.helmet[0].itemPrefab);
                    break;

                case Item.ItemCategory.Armor:
                    temp = Instantiate(_inven.armor[0].itemPrefab);
                    break;

                case Item.ItemCategory.Shoes:
                    temp = Instantiate(_inven.shoes[0].itemPrefab);
                    break;

                case Item.ItemCategory.Weapon:
                {
                    weaponType = (Item.WeaponType)Random.Range(0, 3);
                    switch (weaponType)
                    {
                        case Item.WeaponType.Hammer:
                            temp = Instantiate(_inven.hammer[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Sword:
                            temp = Instantiate(_inven.sword[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Spear:
                            temp = Instantiate(_inven.spear[0].itemPrefab);
                            break;

                        default:
                            temp = Instantiate(_inven.sword[0].itemPrefab);
                            break;                        
                    }
                    
                }
                    break;
                
                default:
                    temp = Instantiate(_inven.sword[0].itemPrefab);
                    break;
            }

            temp.transform.position = transform.position;
            if(itemCategory == Item.ItemCategory.Weapon)
                temp.transform.localScale = new Vector3(1f, 1f, 1f);
            else
                temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
                

            BoxCollider col = temp.GetComponent<BoxCollider>();
            col.enabled = true;
            
            Magnet mag = temp.AddComponent<Magnet>();
            mag.itemCategory = itemCategory;

            if (onTree)
                mag.onTree = true;
            else
                mag.onTree = false;
            
            
            if (itemCategory == Item.ItemCategory.Weapon)
            {
                mag.weaponType = weaponType;
            }
            else
            {
                mag.weaponType = Item.WeaponType.Nothing;
            }
            mag._inven = _inven;
                
            Rigidbody comp = temp.AddComponent<Rigidbody>();
            comp.AddExplosionForce(200f, transform.position, 200f, 10f);
        }
    }

    private int GetItemCount()
    {
        switch (dropLevel)
        {
            case DropLevel.VeryLow:
                return 3;
            
            case DropLevel.Low:
                return 4;
            
            case DropLevel.Medium:
                return Random.Range(5, 7);
            
            case DropLevel.High:
                return Random.Range(7, 9);
            
            case DropLevel.VeryHigh:
                return Random.Range(9, 11);
        }

        return 1;
    }
}