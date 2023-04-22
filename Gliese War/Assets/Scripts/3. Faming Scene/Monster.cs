using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            GetDamage();
            Debug.Log("맞음");
        }
        
        if (other.transform.CompareTag("Player") && !attackCoolTime)
        {
            attackCoolTime = true;
            other.gameObject.GetComponent<Player>().GetDamage(damage);
            FarmingManager.Instance.HitScreen();
            StartCoroutine(AttackWait());
        }
    }

    private void GetDamage()
    {
        HP -= Player.instance.GetAttackPower();

        if (HP <= 0)
        {
            Destroy(gameObject);
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
        rigid.AddForce(dir * 2,ForceMode.VelocityChange);
        StartCoroutine(KnockBackWait());
    }

    IEnumerator KnockBackWait()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        agent.isStopped = false;
    }

    public void StartAttack()
    {
        Debug.Log("공격 시작");
        agent.isStopped = true;
        rigid.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void EndAttack()
    {
        agent.isStopped = false;
        rigid.constraints = RigidbodyConstraints.None;
        animator.SetBool("isAttack", false);
        Debug.Log("공격 종료");
    }
}
