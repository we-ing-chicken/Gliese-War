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
    [SerializeField] private bool isFirst = false;
    [SerializeField] private bool onTree = false;

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
            
                

            MeshCollider col = temp.GetComponent<MeshCollider>();
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
                return 1;
            
            case DropLevel.Low:
                return 2;
            
            case DropLevel.Medium:
                return Random.Range(2, 4);
            
            case DropLevel.High:
                return Random.Range(4, 6);
            
            case DropLevel.VeryHigh:
                return Random.Range(6, 8);
        }

        return 1;
    }
}