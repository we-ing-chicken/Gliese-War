using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    private int master;
    private ParticleSystem ps;
    
    void Start()
    {
        StartCoroutine(Timer());
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

    IEnumerator Timer()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= 5f)
            {
                Destroy(transform.parent.gameObject);
                Debug.Log("끝");
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

        for (int i = 0; i < BattleManager.Instance.players.Length; ++i)
        {
            if (BattleManager.Instance.players[i] == null) continue;
            
            //ps.trigger.SetCollider(i, BattleManager.Instance.players[i].transform);
        }

        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var outData);

        if (numEnter == 0) return;
        if (outData.GetColliderCount(0) == 0) return;

        // iterate
        GameObject obj = outData.GetCollider(0, 0).gameObject;

        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<BattlePlayer>().GetDamage(1);
            return;
        }


        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}