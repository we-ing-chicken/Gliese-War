using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WeaponAttackPS : MonoBehaviour
{
    private MeshCollider col;
    
    public GameObject handR;
    public GameObject back;
    public GameObject attackEffectPos;
    public GameObject shoesEffectPos;
    
    public void AttackStart()
    {
        if (playerScript.instance.weaponNow == 1)
        {
            switch (playerScript.instance.weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Sword:
                    TurnOnHandSword();
                    StartCoroutine(TurnOffHandKnife());
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }
        else if (playerScript.instance.weaponNow == 2)
        {

            switch (playerScript.instance.weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Sword:
                    TurnOnHandSword();
                    StartCoroutine(TurnOffHandKnife());
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }

        playerScript.instance.isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        playerScript.instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        playerScript.instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = playerScript.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        playerScript.instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        playerScript.instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = playerScript.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSword()
    {
        playerScript.instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        playerScript.instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = playerScript.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = playerScript.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        playerScript.instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        playerScript.instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        playerScript.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = playerScript.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        playerScript.instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        playerScript.instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        playerScript.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = playerScript.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        playerScript.instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        playerScript.instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        playerScript.instance.isAttack = false;
    }
}
