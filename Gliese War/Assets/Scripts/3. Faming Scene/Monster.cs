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
    private Animator animator;

    private int HP;
    private int damage;
    private bool attackCoolTime;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        drop = GetComponent<Drop>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        HP = 100;
        damage = 10;
        attackCoolTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && !attackCoolTime)
        {
            attackCoolTime = true;
            agent.enabled = true;
            collision.gameObject.GetComponent<Player>().GetDamage(damage);
            FarmingManager.Instance.HitScreen();
            StartCoroutine(AttackWait());
        }
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(2f);
        attackCoolTime = false;
        agent.velocity = Vector3.zero;
    }

    public void KnockBack()
    {
        Debug.Log("넛백");
        Vector3 dir = transform.position - Player.instance.transform.position;
        dir = dir.normalized;
        dir += Vector3.up;
        agent.enabled = false;
        rigid.AddForce(dir * 2,ForceMode.VelocityChange);
        StartCoroutine(KnockBackWait());
    }

    IEnumerator KnockBackWait()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        agent.enabled = true;
        agent.isStopped = false;
    }

    public void EndAttack()
    {
        animator.SetBool("isAttack", false);
        Debug.Log("전 " + agent.isStopped);
        agent.isStopped = false;
        Debug.Log("후 " + agent.isStopped);
        Debug.Log("공격 종료");
    }
}
