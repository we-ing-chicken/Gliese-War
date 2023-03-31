using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Equip : MonoBehaviour
{
    [SerializeField] private Image itemImage;

    private void Start()
    {
        itemImage = gameObject.GetComponent<Image>();
    }

    public void SetImage(Item item)
    {
        itemImage.sprite = item.itemImage;
    }
}
