using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeteoFarming : MonoBehaviour
{
    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        
        StartCoroutine(Timer());
        StartCoroutine(SoundTimer());
    }

    IEnumerator SoundTimer()
    {
        yield return new WaitForSeconds(2.5f);
        audio.Play();
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
        
        // particles
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        ps = GetComponent<ParticleSystem>();
        
        ps.trigger.SetCollider(0, CPlayer.Instance.transform);

            // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var outData);

        if (numEnter == 0) return;
        if (outData.GetColliderCount(0) == 0) return;

        // iterate
        GameObject obj = outData.GetCollider(0, 0).gameObject;

        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<CPlayer>().GetDamage(5);
            return;
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}