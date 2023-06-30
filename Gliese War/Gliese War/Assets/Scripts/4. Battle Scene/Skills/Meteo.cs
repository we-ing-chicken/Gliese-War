using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Meteo : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= 5f)
            {
                Destroy(transform.parent.parent.parent.gameObject);
                break;
            }

            yield return null;
        }
    }

    void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        
        ps.trigger.SetCollider(1, CPlayer.Instance);

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
            obj.GetComponent<CPlayer>().GetDamage(1);
            return;
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}