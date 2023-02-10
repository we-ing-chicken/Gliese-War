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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        int itemCount = Random.Range(1, 5);
        for (int i = 0; i < itemCount; ++i)
        {
            Item.ItemCategory itemCategory = (Item.ItemCategory)Random.Range(0, 4);
            GameObject temp;

            switch (itemCategory)
            {
                case Item.ItemCategory.Hammer:
                    temp = Instantiate(_inven.hammer[0].itemPrefab);
                    break;

                case Item.ItemCategory.Knife:
                    temp = Instantiate(_inven.knife[0].itemPrefab);
                    break;

                case Item.ItemCategory.Spear:
                    temp = Instantiate(_inven.spear[0].itemPrefab);
                    break;
                
                default:
                    temp = Instantiate(_inven.knife[0].itemPrefab);
                    break;
            }

            temp.transform.position = transform.position;
            temp.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            
            Magnet mag = temp.AddComponent<Magnet>();
            mag.itemCategory = itemCategory;
            mag._inven = _inven;
                
            Rigidbody comp = temp.AddComponent<Rigidbody>();
            comp.AddExplosionForce(200f, transform.position, 200f, 10f);
        }
    }
}