using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    public GameObject damager;      // ������
    public float damage;            // ���ݷ�

    public Vector3 hitPoint;        // ���� ��ġ
    public Vector3 hitNormal;       // ���� ����
}
