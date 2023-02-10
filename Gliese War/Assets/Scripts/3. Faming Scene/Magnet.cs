using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rigid;
    private float magnetStrength = 10f;
    private float distanceStretch = 50f;
    private bool isEat = false;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance <= 5 || isEat )
        {
            rigid.constraints = RigidbodyConstraints.None;
            
            Vector3 direction = player.transform.position - transform.position;
            float magnetDistanceStr = (distanceStretch / distance) * magnetStrength;
            rigid.AddForce(magnetDistanceStr*direction,ForceMode.Force );
            isEat = true;
        }
        else if (transform.position.y <= 48)
        {
            rigid.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            Destroy(gameObject);
    }
}
