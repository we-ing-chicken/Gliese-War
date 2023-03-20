using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    [SerializeField] private Inventory _inven;
    private int itemCount;

    private void Start()
    {
        _inven = FarmingManager.Instance.inventory;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DropItem();
        }
    }

    public void DropItem()
    {
        int itemCount = Random.Range(1, 5);
        for (int i = 0; i < itemCount; ++i)
        {
            Item.ItemCategory itemCategory = (Item.ItemCategory)Random.Range(0, 4);
            Item.WeaponType weaponType = Item.WeaponType.Nothing;
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
                        case Item.WeaponType.Knife:
                            temp = Instantiate(_inven.knife[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Spear:
                            temp = Instantiate(_inven.spear[0].itemPrefab);
                            break;
                        
                        case Item.WeaponType.Hammer:
                            temp = Instantiate(_inven.hammer[0].itemPrefab);
                            break;
                        
                        default:
                            temp = Instantiate(_inven.knife[0].itemPrefab);
                            break;                        
                    }
                    
                }
                    break;
                
                default:
                    temp = Instantiate(_inven.knife[0].itemPrefab);
                    break;
            }

            temp.transform.position = transform.position;
            temp.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Magnet mag = temp.AddComponent<Magnet>();
            mag.itemCategory = itemCategory;
            
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
}