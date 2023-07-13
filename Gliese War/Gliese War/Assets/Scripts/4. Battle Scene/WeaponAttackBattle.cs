using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Photon.Pun;
using UnityEngine;

public class WeaponAttackBattle : MonoBehaviour
{
    private MeshCollider col;
    
    public GameObject handR;
    public GameObject back;
    public GameObject attackEffectPos;
    public GameObject shoesEffectPos;

    private BattlePlayer mine;
    private BattlePlayer[] players;

    private void Start()
    {
        players = FindObjectsOfType<BattlePlayer>();

        foreach (BattlePlayer p in players)
        {
            if (p.photonView.IsMine)
            {
                mine = p;
                break;
            }
        }
    }

    public void AttackStart()
    {
        //네트워크 매니저가 배틀매니저에 플레이어 인덱스의 자기 pnum을 토스 
        if (mine.weaponNow == 1)
        {
            switch (mine.weapon1.item.weaponType)
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
        else if (mine.weaponNow == 2)
        {

            switch (mine.weapon2.item.weaponType)
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

        mine.isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        mine.handR.transform.GetChild(0).gameObject.SetActive(true);
        mine.back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = mine.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        mine.handR.transform.GetChild(1).gameObject.SetActive(true);
        mine.back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = mine.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSword()
    {
        mine.handR.transform.GetChild(2).gameObject.SetActive(true);
        mine.back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = mine.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.5f);
        
        col = mine.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        mine.handR.transform.GetChild(0).gameObject.SetActive(false);
        mine.back.transform.GetChild(0).gameObject.SetActive(true);
        
        mine.isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = mine.handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        mine.handR.transform.GetChild(1).gameObject.SetActive(false);
        mine.back.transform.GetChild(1).gameObject.SetActive(true);
        
        mine.isAttack = false;
    }
    
    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);
        
        col = mine.handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        mine.handR.transform.GetChild(2).gameObject.SetActive(false);
        mine.back.transform.GetChild(2).gameObject.SetActive(true);
        
        mine.isAttack = false;
    }
}
