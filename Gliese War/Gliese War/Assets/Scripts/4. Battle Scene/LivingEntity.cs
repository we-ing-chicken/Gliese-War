using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPunCallbacks, IDamageable
{
    public float startingHealth = 100f;
    public float health { get; protected set; }
    public bool dead { get; protected set; }

    private const float minTimeBetDamaged = 0.1f;
    private float lastDamagedTime;

    public event Action OnDeath;

    protected bool IsInvulnerabe            //무적 상태. 메이플 피격시스템과 비슷한 처리.
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }

    new protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        if (IsInvulnerabe || damageMessage.damager == gameObject || dead) return false;

        Debug.Log(damageMessage.damager + ", " + damageMessage.damage);
        lastDamagedTime = Time.time;
        health -= damageMessage.damage;

        if (health <= 0) Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;

        health += newHealth;
    }

    public virtual void Die()
    {
        if (OnDeath != null) OnDeath();

        dead = true;
    }
}
