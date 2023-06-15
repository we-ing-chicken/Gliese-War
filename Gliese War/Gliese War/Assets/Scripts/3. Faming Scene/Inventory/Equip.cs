using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Equip : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Image magicImage;

    private void Start()
    {
        itemImage = gameObject.GetComponent<Image>();

        if (CompareTag("Weapon"))
            magicImage = gameObject.transform.GetChild(0).GetComponent<Image>();
    }

    public void SetAlpha(float a)
    {
        Color color = itemImage.color;
        color.a = a;
        itemImage.color = color;
    }

    public void SetImage(RealItem realitem)
    {
        itemImage.sprite = realitem.item.itemImage;
        
        if (CompareTag("Weapon"))
        {
            switch (realitem.magic)
            {
                case Magic.Water:
                    magicImage.sprite = Inventory.instance.magicImages[0];
                    break;
                
                case Magic.Fire:
                    magicImage.sprite = Inventory.instance.magicImages[1];
                    break;
                
                case Magic.Light:
                    magicImage.sprite = Inventory.instance.magicImages[2];
                    break;
                
                case Magic.Nothing:
                    magicImage.sprite = null;
                    break;
            }
            
        }

        if (magicImage == null)
            return;
        
        if (realitem.magic == Magic.Nothing)
        {
            Color color = magicImage.color;
            color.a = 0f;
            magicImage.color = color;
        }
        else
        {            
            Color color = magicImage.color;
            color.a = 1f;
            magicImage.color = color;
        }
    }
}
