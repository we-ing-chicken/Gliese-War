using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealArea : MonoBehaviour
{
    private ParticleSystem ps;


    void OnParticleTrigger()
    {
        // particles
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        
        ps = GetComponent<ParticleSystem>();

        for (int i = 0; i < BattleManager.Instance.players.Length; ++i)
        {
            if (BattleManager.Instance.players[i] == null) continue;
            
            ps.trigger.SetCollider(i, BattleManager.Instance.players[i].transform);
        }

        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var outData);

        if (numEnter == 0) return;
        if (outData.GetColliderCount(0) == 0) return;

        // iterate
        GameObject obj = outData.GetCollider(0, 0).gameObject;

        if (obj.CompareTag("Player"))
        {
            if (obj.GetComponent<BattlePlayer>().photonView.IsMine)
            {
                
            DamageMessage dm;
            dm.damager = obj.GetComponent<BattlePlayer>().myindex;
            dm.damage = -1;
            obj.GetComponent<BattlePlayer>().ApplyDamage(dm);
            //obj.GetComponent<BattlePlayer>().GetDamage(1);
            return;
            }
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}