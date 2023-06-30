using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    private static TestManager _instance;
    
    [Header("Canvas")] public GameObject invenCanvas;
    public Canvas pauseCanvas;
    public Canvas fadeCanvas;
    public Canvas hitCanvas;

    public Item[] knife;
    public Item[] spear;
    public Item[] hammer;
    
    public static TestManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(TestManager)) as TestManager;
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
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
