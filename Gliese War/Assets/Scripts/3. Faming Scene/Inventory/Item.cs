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
        Knife,
        Spear,
        Hammer
    }

    public string itemName;
    public ItemRank itemRank;
    public ItemCategory itemCategory;
    public Sprite itemImage;
    public GameObject itemPrefab;
}
