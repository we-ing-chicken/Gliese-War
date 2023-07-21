using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPunCallbacks, IDamageable
{
    public int startingHealth;
    public int health { get; protected set; }
    public bool dead { get; protected set; }

    private const float minTimeDamaged = 0.1f;
    private float lastDamagedTime;

    public event Action OnDeath;

    protected bool IsInvulnerable            //���� ����. ������ �ǰݽý��۰� ����� ó��.
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeDamaged) return false;

            return true;
        }
    }

    new protected virtual void OnEnable()
    {
        dead = false;
        
        if(GameManager.Instance != null)
            startingHealth = GameManager.Instance.stat.health;
        else
            startingHealth = 100;

        health = startingHealth;
    }

    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        if (IsInvulnerable || damageMessage.damager == GetComponent<LivingEntity>().photonView.ViewID || dead) return false;

        
        lastDamagedTime = Time.time;
        health -= damageMessage.damage;

        if (health <= 0) Die();

        return true;
    }

    public virtual void RestoreHealth(int newHealth)
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
