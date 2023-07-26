using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    private AudioSource audio;
    private int master;
    private ParticleSystem ps;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        
        StartCoroutine(Timer());
        StartCoroutine(SoundTimer());
    }
    
    public void SetMaster(int m)
    {
        master = m;
        
        ps = GetComponent<ParticleSystem>();

        for(int i = 0 ; i < BattleManager.Instance.players.Length ; ++i)
        {
            if (BattleManager.Instance.players[i] == null) continue;
            if (BattleManager.Instance.players[i].GetComponent<BattlePlayer>().myindex == m) continue;
            
            ps.trigger.SetCollider(i, BattleManager.Instance.players[i].transform);
        }
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
        // particles
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        ps = GetComponent<ParticleSystem>();
        
        for(int i = 0 ; i < BattleManager.Instance.players.Length ; ++i)
        {
            if (BattleManager.Instance.players == null) continue;
            
            //ps.trigger.SetCollider(i, BattleManager.Instance.players[i].transform);
        }
        
        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var outData);

        if (numEnter == 0) return;
        if (outData.GetColliderCount(0) == 0) return;

        // iterate
        GameObject obj = outData.GetCollider(0, 0).gameObject;

        if (obj.CompareTag("Player")  && obj.GetComponent<BattlePlayer>().photonView.IsMine)
        {
            DamageMessage dm;
            dm.damager = master;
            dm.damage = 30;
            obj.GetComponent<BattlePlayer>().ApplyDamage(dm);
            //obj.GetComponent<BattlePlayer>().GetDamage(1);
            
            return;
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}