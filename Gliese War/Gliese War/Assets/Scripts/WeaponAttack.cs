using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponAttack : MonoBehaviour
{
    private MeshCollider col;

    public GameObject handR;
    public GameObject back;
    public GameObject attackEffectPos;
    public GameObject shoesEffectPos;

    public void AttackStart()
    {
        Debug.Log("A");
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("B");
            if (CPlayer.Instance.weaponNow == 1)
            {
                switch (CPlayer.Instance.weapon1.item.weaponType)
                {
                    case Item.WeaponType.Hammer:
                        TurnOnHandHammer();
                        StartCoroutine(TurnOffHandHammer());
                        break;
                
                    case Item.WeaponType.Sword:
                        TurnOnHandSword();
                        StartCoroutine(TurnOffHandSword());
                        break;
                
                    case Item.WeaponType.Spear:
                        TurnOnHandSpear();
                        StartCoroutine(TurnOffHandSpear());
                        break;
                }
            }
            else if (CPlayer.Instance.weaponNow == 2)
            {

                switch (CPlayer.Instance.weapon2.item.weaponType)
                {
                    case Item.WeaponType.Hammer:
                        TurnOnHandHammer();
                        StartCoroutine(TurnOffHandHammer());
                        break;
                
                    case Item.WeaponType.Sword:
                        TurnOnHandSword();
                        StartCoroutine(TurnOffHandSword());
                        break;
                
                    case Item.WeaponType.Spear:
                        TurnOnHandSpear();
                        StartCoroutine(TurnOffHandSpear());
                        break;
                }
            }

            CPlayer.Instance.isAttack = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Debug.Log("C");
            if (BattlePlayer.instance.weaponNow == 1)
            {
                switch (BattlePlayer.instance.weapon1.item.weaponType)
                {
                    case Item.WeaponType.Hammer:
                        TurnOnHandHammer();
                        StartCoroutine(TurnOffHandHammerB());
                        break;
                
                    case Item.WeaponType.Sword:
                        TurnOnHandSword();
                        StartCoroutine(TurnOffHandSwordB());
                        break;
                
                    case Item.WeaponType.Spear:
                        TurnOnHandSpear();
                        StartCoroutine(TurnOffHandSpearB());
                        break;
                }
            }
            else if (BattlePlayer.instance.weaponNow == 2)
            {

                switch (BattlePlayer.instance.weapon2.item.weaponType)
                {
                    case Item.WeaponType.Hammer:
                        TurnOnHandHammer();
                        StartCoroutine(TurnOffHandHammerB());
                        break;
                
                    case Item.WeaponType.Sword:
                        TurnOnHandSword();
                        StartCoroutine(TurnOffHandSwordB());
                        break;
                
                    case Item.WeaponType.Spear:
                        TurnOnHandSpear();
                        StartCoroutine(TurnOffHandSpearB());
                        break;
                }
            }

            BattlePlayer.instance.isAttack = true;
        }
        
        
    }

    public void TurnOnHandHammer()
    {
        CPlayer.Instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        CPlayer.Instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSword()
    {
        CPlayer.Instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = CPlayer.Instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = CPlayer.Instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSword()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = CPlayer.Instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
    
    public void TurnOnHandHammerB()
    {
        CPlayer.Instance.handR.transform.GetChild(0).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpearB()
    {
        CPlayer.Instance.handR.transform.GetChild(1).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSwordB()
    {
        CPlayer.Instance.handR.transform.GetChild(2).gameObject.SetActive(true);
        CPlayer.Instance.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = CPlayer.Instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammerB()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = CPlayer.Instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(0).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(0).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpearB()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = CPlayer.Instance.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(1).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(1).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
    
    IEnumerator TurnOffHandSwordB()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = CPlayer.Instance.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        CPlayer.Instance.handR.transform.GetChild(2).gameObject.SetActive(false);
        CPlayer.Instance.back.transform.GetChild(2).gameObject.SetActive(true);
        
        CPlayer.Instance.isAttack = false;
    }
}
