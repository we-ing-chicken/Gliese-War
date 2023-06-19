using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private MeshCollider col;

    public GameObject handR;
    public GameObject back;
    public GameObject attackEffectPos;
    public GameObject shoesEffectPos;

    public void AttackStart()
    {
        if (CPlayer.instance.weaponNow == 1)
        {
            switch (CPlayer.instance.weapon1.item.weaponType)
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
        else if (CPlayer.instance.weaponNow == 2)
        {

            switch (CPlayer.instance.weapon2.item.weaponType)
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

        CPlayer.instance.isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        CPlayer.instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        CPlayer.instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = CPlayer.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        CPlayer.instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        CPlayer.instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = CPlayer.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandKnife()
    {
        CPlayer.instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        CPlayer.instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = CPlayer.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = CPlayer.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        CPlayer.instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        CPlayer.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = CPlayer.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        CPlayer.instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        CPlayer.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = CPlayer.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        CPlayer.instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        CPlayer.instance.isAttack = false;
    }
}
