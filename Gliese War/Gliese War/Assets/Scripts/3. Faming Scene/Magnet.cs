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
    public Item.WeaponType weaponType;
    public bool isFirst = false;

    public bool onTree = false;

    // Start is called before the first frame update
    void Start()
    {
        player = CPlayer.Instance.gameObject;
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance <= 10 || isEat )
        {
            rigid.constraints = RigidbodyConstraints.None;
            
            Vector3 direction = player.transform.position - transform.position;
            float magnetDistanceStr = (distanceStretch / distance) * magnetStrength;
            rigid.AddForce(magnetDistanceStr*direction,ForceMode.Force );
            isEat = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            CPlayer.Instance.audio.PlayOneShot(CPlayer.Instance.eatSound, 1f);
            Item.ItemRank rank;
            
            if (isFirst)
                rank = Item.ItemRank.Normal;
            else
                rank = RandomManager.RandomBox(100000000);
            
            switch(itemCategory)
            {
                case Item.ItemCategory.Helmet:
                    _inven.AcquireItem(_inven.helmet[(int)rank]);
                    break;
                
                case Item.ItemCategory.Armor:
                    _inven.AcquireItem(_inven.armor[(int)rank]);
                    break;
                
                case Item.ItemCategory.Shoes:
                    _inven.AcquireItem(_inven.shoes[(int)rank]);
                    break;

                case Item.ItemCategory.Weapon:
                {
                    switch (weaponType)
                    {
                        case Item.WeaponType.Sword: 
                            _inven.AcquireItem(_inven.sword[(int)rank]);
                            break;
                        
                        case Item.WeaponType.Spear:
                            _inven.AcquireItem(_inven.spear[(int)rank]);
                            break;
                        
                        case Item.WeaponType.Hammer:
                            _inven.AcquireItem(_inven.hammer[(int)rank]);
                            break;
                    }
                } 
                    break;
                
                default:
                    _inven.AcquireItem(_inven.hammer[(int)rank]);
                    break;
            }

            Destroy(gameObject);
        }
        
        if(!isEat)
            rigid.constraints = RigidbodyConstraints.FreezePosition;
    }
}
