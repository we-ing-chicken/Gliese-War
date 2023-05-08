using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;

public class Warp : MonoBehaviour
{
    public GameObject warpTo;
    private bool isLockOn;

    private void Start()
    {
        isLockOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            Debug.Log("Enter");
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
            FarmingManager.Instance.ActiveG(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
            FarmingManager.Instance.ActiveG(gameObject);
            return;
        }
        
        if (other.CompareTag("Player") && isLockOn && !FarmingManager.Instance._isFading)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("Move");
                other.GetComponent<Player>().isNear = false;
                isLockOn = false;
                FarmingManager.Instance.UnActiveG();
                
                FarmingManager.Instance.StartFadeOut();
                StartCoroutine(Move());

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>().isNear && isLockOn)
        {
            Debug.Log("Exit");
            other.GetComponent<Player>().isNear = false;
            isLockOn = false;
            FarmingManager.Instance.UnActiveG();
        }
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(2f);
        Player.instance.GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        Player.instance.transform.position = warpTo.transform.position;
        yield return new WaitForSeconds(0.1f);
        Player.instance.GetComponent<CharacterController>().enabled = true;
        isLockOn= false;
        Player.instance.isNear = false;
        FarmingManager.Instance.UnActiveG();
        yield return null;
    }
}