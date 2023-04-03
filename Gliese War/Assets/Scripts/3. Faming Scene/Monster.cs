using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent agent;
    private Drop drop;
    private Rigidbody rigid;

    private int HP;
    private int damage;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        drop = GetComponent<Drop>();
        rigid = GetComponent<Rigidbody>();

        HP = 100;
        damage = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            agent.isStopped = true;
            Debug.Log("어택");
            collision.gameObject.GetComponent<Player>().GetDamage(damage);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    public void KnockBack()
    {
        Debug.Log("넛백");
        Vector3 dir = transform.position - Player.instance.transform.position;
        dir = dir.normalized;
        dir += Vector3.up;
        rigid.AddForce(dir,ForceMode.Impulse);
    }
}
