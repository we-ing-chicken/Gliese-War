using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public enum ItemRank
    {
        Normal,
        Rare,
        Epic,
        Unique,
        Legendary
    }

    public enum ItemCategory
    {
        Helmet,
        Armor,
        Shoes,
        Weapon
    }

    public enum WeaponType
    {
        Sword,
        Spear,
        Hammer,
        Nothing
    }

    public string itemName;
    public ItemRank itemRank;
    public ItemCategory itemCategory;
    public WeaponType weaponType;
    public Sprite itemImage;
    public GameObject itemPrefab;
}
