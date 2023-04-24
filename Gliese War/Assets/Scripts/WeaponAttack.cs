using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public void AttackStart()
    { 
        MeshCollider col= Player.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;

        Player.instance.isAttack = true;
        Debug.Log("공격 시작");
    }

    public void AttackEnd()
    {
        MeshCollider col= Player.instance.handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        Player.instance.isAttack = false;
        Debug.Log("공격 끝");
    }
}
