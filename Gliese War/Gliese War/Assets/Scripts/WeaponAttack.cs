using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private MeshCollider col;
    
    public void AttackStart()
    {
        if (Player.instance.weaponNow == 1)
        {
            switch (Player.instance.weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Knife:
                    TurnOnHandKnife();
                    StartCoroutine(TurnOffHandKnife());
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }
        else if (Player.instance.weaponNow == 2)
        {

            switch (Player.instance.weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Knife:
                    TurnOnHandKnife();
                    StartCoroutine(TurnOffHandKnife());
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }

        Player.instance.isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        Player.instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        Player.instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = Player.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        Player.instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        Player.instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = Player.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandKnife()
    {
        Player.instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        Player.instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = Player.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = Player.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        Player.instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        Player.instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        Player.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = Player.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        Player.instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        Player.instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        Player.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = Player.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        Player.instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        Player.instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        Player.instance.isAttack = false;
    }
}
