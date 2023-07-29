using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hamburger : MonoBehaviour
{
    private Rigidbody rigid;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Terrain"))
        {
            rigid.constraints = RigidbodyConstraints.FreezePosition;
        }
    }
}
