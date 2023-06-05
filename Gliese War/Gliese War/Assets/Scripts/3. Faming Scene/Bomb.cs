using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        ps.trigger.SetCollider(1, Player.instance);
        
        // particles
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var outData);

        if (numEnter == 0) return;
        if (outData.GetColliderCount(0) == 0) return;

        // iterate
        GameObject obj = outData.GetCollider(0, 0).gameObject;

        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<Player>().GetDamage(5);
            return;
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}