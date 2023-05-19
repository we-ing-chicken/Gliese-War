using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    private NavMeshAgent agent;
    private Drop drop;
    private Rigidbody rigid;
    private Animator animator;

    private int HP;
    private int damage;
    private bool attackCoolTime;

    private Material mat;
    private Color before;

    private bool isDead;
    
    //[SerializeField] private Transform pfBoxBroken;
    private Transform broken;

    [SerializeField] GameObject attackPart; 
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        drop = GetComponent<Drop>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        mat = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
        before = mat.color;
        
        HP = 100;
        damage = 10;
        attackCoolTime = false;

        isDead = false;

        StartCoroutine(AnimationStop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    IEnumerator AnimationStop()
    {
        animator.StartPlayback();
        
        float sec = Random.Range(0, 5f);
        yield return new WaitForSeconds(sec);
        
        animator.StopPlayback();
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            GetDamage();
            KnockBack();
        }
        
        if (other.transform.CompareTag("Player") && !attackCoolTime)
        {
            attackCoolTime = true;
            StartCoroutine(AttackWait());
        }
    }

    private void GetDamage()
    {
        HP -= Player.instance.GetAttackPower();
        StartCoroutine(HitColor());
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            drop.DropItem();
            DestructObject();
        }
    }

    IEnumerator HitColor()
    {
        bool flag = true;
        
        for (int i = 0; i < 6; ++i)
        {
            if (flag)
            {
                mat.color = Color.red;
                flag = false;
            }
            else
            {
                mat.color = before;
                flag = true;
            }
            yield return new WaitForSeconds(0.1f);
            mat.color = before;
        }
        mat.color = before;
        
        yield return null;
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(2f);
        attackCoolTime = false;
        agent.velocity = Vector3.zero;
    }

    public void KnockBack()
    {
        if (gameObject == null)
            return;
        
        Debug.Log("넛백");
        if(agent != null)
            agent.isStopped = true;
        Vector3 dir = transform.position - Player.instance.transform.position;
        dir = dir.normalized;
        rigid.AddForce(dir * 20,ForceMode.Impulse);
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
        Debug.Log("몬스터 공격 시작");
        agent.isStopped = true;
        rigid.constraints = RigidbodyConstraints.FreezePosition;
        attackPart.GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        if(transform.CompareTag("Golam"))
            yield return new WaitForSeconds(0.55f);
        else if( transform.CompareTag("Bee"))
            yield return new WaitForSeconds(0.5f);
        
        attackPart.GetComponent<BoxCollider>().enabled = false;
        agent.isStopped = false;
        rigid.constraints = RigidbodyConstraints.None;
        animator.SetBool("isAttack", false);
        Debug.Log("몬스터 공격 종료");
        
        yield return null;
    }

    public void DestructObject()
    {
        //broken = Instantiate(pfBoxBroken, transform.position + new Vector3(0,4f,0), transform.rotation);
        broken = transform.GetChild(3);
        broken.gameObject.SetActive(true);
        
        Destroy(transform.GetComponent<BehaviourTreeRunner>());
        //Destroy(agent);
        agent.enabled = false;
        
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        
        foreach (Transform child in broken)
        {
            child.AddComponent<Rigidbody>();
            
            Material mat = child.GetComponent<MeshRenderer>().material;
            mat.color = before;
            
            Rigidbody comp = child.GetComponent<Rigidbody>();
            comp.AddExplosionForce(50000f, Vector3.up, 120f);
        }

        //Destroy(gameObject);
    }
}
