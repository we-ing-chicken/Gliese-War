using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    public GameObject damager;      // 공격자
    public float damage;            // 공격량

    public Vector3 hitPoint;        // 공격 위치
    public Vector3 hitNormal;       // 공격 방향
}
