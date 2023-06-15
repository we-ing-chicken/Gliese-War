using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WeaponAttackBattle : MonoBehaviour
{
    private MeshCollider col;
    
    public void AttackStart()
    {
        if (BattlePlayer.instance.weaponNow == 1)
        {
            switch (BattlePlayer.instance.weapon1.item.weaponType)
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
        else if (BattlePlayer.instance.weaponNow == 2)
        {

            switch (BattlePlayer.instance.weapon2.item.weaponType)
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

        BattlePlayer.instance.isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        BattlePlayer.instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        BattlePlayer.instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = BattlePlayer.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        BattlePlayer.instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        BattlePlayer.instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = BattlePlayer.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandKnife()
    {
        BattlePlayer.instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        BattlePlayer.instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = BattlePlayer.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = BattlePlayer.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattlePlayer.instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        BattlePlayer.instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        BattlePlayer.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = BattlePlayer.instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattlePlayer.instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        BattlePlayer.instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        BattlePlayer.instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = BattlePlayer.instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattlePlayer.instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        BattlePlayer.instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        BattlePlayer.instance.isAttack = false;
    }
}
