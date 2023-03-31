using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public Slot dragedSlot;

    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemName;
    
    void Start()
    {
        instance = this;
    }
    
    public void DragSetData(RealItem _realItem)
    {
        if (_realItem != null)
        {
            dragedSlot.realItem.item = _realItem.item;
            dragedSlot.realItem.stat = _realItem.stat;

            itemImage.sprite = _realItem.item.itemImage;
            itemName.text = _realItem.item.itemName;
        }
        else
        {
            itemImage.sprite = null;
            itemName.text = "";
        }
        
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
}
