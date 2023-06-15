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
    [SerializeField] private Image magicImage;
    
    void Start()
    {
        instance = this;
    }
    
    public void DragSetData(RealItem _realItem)
    {
        if (_realItem != null)
        {
            dragedSlot.realItem = _realItem;
            dragedSlot.realItem.item = _realItem.item;
            dragedSlot.realItem.stat = _realItem.stat;

            itemImage.sprite = _realItem.item.itemImage;
            itemName.text = _realItem.item.itemName;

            switch (_realItem.magic)
            {
                case Magic.Water:
                    magicImage.sprite = Inventory.instance.magicImages[0];
                    SetColor(1,1);

                    break;
                
                case Magic.Fire:
                    magicImage.sprite = Inventory.instance.magicImages[1];
                    SetColor(1,1);

                    break;
                
                case Magic.Light:
                    magicImage.sprite = Inventory.instance.magicImages[2];
                    SetColor(1,1);
                    break;
                
                case Magic.Nothing:
                    magicImage.sprite = null;
                    SetColor(1,0);
                    break;
            }
        }
        else
        {
            itemImage.sprite = null;
            itemName.text = "";
            magicImage.sprite = null;
            SetColor(1,1);
        }
        
        
    }

    public void SetColor(float _alpha, float _alpha2)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        
        color = magicImage.color;
        color.a = _alpha2;
        magicImage.color = color;
    }
}
