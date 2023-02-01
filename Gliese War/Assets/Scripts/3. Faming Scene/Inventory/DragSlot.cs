using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public Slot dragSlot;

    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemName;
    //[SerializeField] private Text itemCount;
    
    void Start()
    {
        instance = this;
    }
    
    public void DragSetData(Image _itemImage, string _itemName)
    {
        itemImage.sprite = _itemImage.sprite;
        itemName.text = _itemName;

        SetColor(1);
    }

    // public void DragSetData(Image _itemImage, string _itemName, string _itemCount)
    // {
    //     itemImage.sprite = _itemImage.sprite;
    //     itemName.text = _itemName;
    //     itemCount.text = _itemCount;
    //     
    //     SetColor(1);
    // }

    public void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
}
