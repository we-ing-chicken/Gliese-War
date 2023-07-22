using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject[] HitEffects;
    public GameObject HealEffect;
    
    [SerializeField] private Sprite[] weaponImage;

    [SerializeField] private Image weaponNum1Image;
    [SerializeField] private Image weaponNum2Image;
    
    [SerializeField] private Image weapon1Image;
    [SerializeField] private Image weapon2Image;

    public GameObject characterUIParent;
    public GameObject[] characterUIs;
    public GameObject characterCam;

    public GameObject helmetEquip;
    public GameObject armorEquip;
    public GameObject shoeEquip;
    public GameObject weapon1Equip;
    public GameObject weapon1Magic;
    public GameObject weapon2Equip;
    public GameObject weapon2Magic;

    public Sprite[] magics;

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
                SwitchGameObjectActive(characterCam);
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
    
    public void SetEquipWeaponImage()
    {
        if (!BattlePlayer.instance.photonView.IsMine) return;
        
        if (BattlePlayer.instance.weapon1 == null)
        {
            Color color = weapon1Image.color;
            color.a = 0f;
            weapon1Image.color = color;
            
            color = weaponNum1Image.color;
            color.a = 0.5f;
            weaponNum1Image.color = color;
        }
        else
        {
            switch (BattlePlayer.instance.weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon1Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon1Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Sword:
                    weapon1Image.sprite = weaponImage[2];
                    break;
            }
            
            if (BattlePlayer.instance.weaponNow == 1)
            {
                Color color = weapon1Image.color;
                color.a = 1f;
                weapon1Image.color = color;

                color = weaponNum1Image.color;
                color.a = 1f;
                weaponNum1Image.color = color;
            }
            else
            {
                Color color = weapon1Image.color;
                color.a = 0.5f;
                weapon1Image.color = color;

                color = weaponNum1Image.color;
                color.a = 0.5f;
                weaponNum1Image.color = color;
            }
        }

        if (BattlePlayer.instance.weapon2 == null)
        {
            Color color = weapon2Image.color;
            color.a = 0f;
            weapon2Image.color = color;
            
            color = weaponNum2Image.color;
            color.a = 0.5f;
            weaponNum2Image.color = color;
        }
        else
        {
            switch (BattlePlayer.instance.weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon2Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon2Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Sword:
                    weapon2Image.sprite = weaponImage[2];
                    break;
            }
            
            if (BattlePlayer.instance.weaponNow == 2)
            {
                Color color = weapon2Image.color;
                color.a = 1f;
                weapon2Image.color = color;
            
                color = weaponNum2Image.color;
                color.a = 1f;
                weaponNum2Image.color = color;
            }
            else
            {
                Color color = weapon2Image.color;
                color.a = 0.5f;
                weapon2Image.color = color;
            
                color = weaponNum2Image.color;
                color.a = 0.5f;
                weaponNum2Image.color = color;
            }
        }
        
    }

    public void MakeUICharacter()
    {
        GameObject temp;
        
        if (GameManager.Instance == null)
        {
            temp = Instantiate(characterUIs[2]);
            temp.transform.position = characterUIParent.transform.position;
        }
        else
        {
            temp = Instantiate(GameManager.Instance.characters[GameManager.Instance.charNum]);
            temp.transform.position = characterUIParent.transform.position;
        }
        
        temp.transform.SetParent(characterUIParent.transform);
    }
}
