using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public Inventory _inven;
    private GameObject player;
    private Rigidbody rigid;
    private float magnetStrength = 10f;
    private float distanceStretch = 50f;
    private bool isEat = false;
    public Item.ItemCategory itemCategory;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance <= 5 || isEat )
        {
            rigid.constraints = RigidbodyConstraints.None;
            
            Vector3 direction = player.transform.position - transform.position;
            float magnetDistanceStr = (distanceStretch / distance) * magnetStrength;
            rigid.AddForce(magnetDistanceStr*direction,ForceMode.Force );
            isEat = true;
        }
        else if (transform.position.y <= 49)
        {
            rigid.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Item.ItemRank rank = RandomManager.RandomBox(100000000);
            switch(itemCategory)
            {
                case Item.ItemCategory.Hammer:
                    _inven.AcquireItem(_inven.hammer[(int)rank]);
                    break;
                
                case Item.ItemCategory.Knife:
                    _inven.AcquireItem(_inven.knife[(int)rank]);
                    break;
                
                case Item.ItemCategory.Spear:
                    _inven.AcquireItem(_inven.spear[(int)rank]);
                    break;
                
                default:
                    _inven.AcquireItem(_inven.hammer[(int)rank]);
                    break;
            }
         
            Destroy(gameObject);
        }
    }
}
