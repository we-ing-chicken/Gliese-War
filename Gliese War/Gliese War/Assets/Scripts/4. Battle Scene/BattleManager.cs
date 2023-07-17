using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public GameObject[] players;
    public List<int> player_indexes = new List<int>();
    
    [Header("Canvas")] public GameObject invenCanvas;
    public Canvas pauseCanvas;
    public Canvas fadeCanvas;
    public Canvas hitCanvas;

    public Item[] sword;
    public Item[] spear;
    public Item[] hammer;
    
    public bool _isFading = true;
    public bool _isInven = false;

    public static BattleManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(BattleManager)) as BattleManager;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        players = new GameObject[4];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!pauseCanvas.gameObject.activeSelf)
            {
                SwitchCanvasActive(invenCanvas);
                //SwitchGameObjectActive(characterCam);
            }

            if (_isInven)
                _isInven = false;
            else
                _isInven = true;
        }
    }

    public void BM_RemoveList(int removeNum)
    {
        for (int i = player_indexes.Count - 1; i >= 0; i--)
        {
            if (player_indexes[i] == removeNum)
            {
                Debug.Log("removelist" + player_indexes[i]);
                player_indexes.Remove(player_indexes[i]);
            }
                
        }

    }
    
    private void SwitchCanvasActive(Canvas temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }
    
    private void SwitchCanvasActive(GameObject temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }
    private void SwitchGameObjectActive(GameObject temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }
    
    public void HitScreen()
    {
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine()
    {
        for (int i = 0; i < 6; ++i)
        {
            SwitchCanvasActive(hitCanvas);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
}
