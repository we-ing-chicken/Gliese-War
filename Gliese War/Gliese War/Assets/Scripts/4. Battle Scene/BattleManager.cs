using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    
    enum Scene
    {
        LoginScene = 0,
        LobbyScene = 1,
        FarmingScene = 2,
        BattleScene = 3,
        END
    }
    
    [Header("Players")]
    public GameObject[] players;
    public List<int> player_indexes = new List<int>();
    public int alivePlayer;
    
    [Header("Canvas")] public GameObject invenCanvas;
    public Canvas pauseCanvas;
    public Canvas fadeCanvas;
    public Canvas hitCanvas;
    public Canvas playCanvas;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    public GameObject HPText;

    [Header("Item")]
    public Item[] sword;
    public Item[] spear;
    public Item[] hammer;
    
    [Header("Flag")]
    public bool _isFading = true;
    public bool _isInven = false;
    public bool gameWait = false;

    public GameObject[] HitEffects;
    public GameObject HealEffect;
    
    [SerializeField] private Sprite[] weaponImage;

    [SerializeField] private Image weaponNum1Image;
    [SerializeField] private Image weaponNum2Image;
    
    [SerializeField] private Image weapon1Image;
    [SerializeField] private Image weapon2Image;
    public GameObject MagneticHitImage;

    [Header("Character UI")]
    public GameObject characterUIParent;
    public GameObject[] characterUIs;
    public GameObject characterCam;
    
    [Header("Equip")]
    public GameObject helmetEquip;
    public GameObject armorEquip;
    public GameObject shoeEquip;
    public GameObject weapon1Equip;
    public GameObject weapon1Magic;
    public GameObject weapon2Equip;
    public GameObject weapon2Magic;

    [Header("Magic")]
    public Sprite[] magics;
    public GameObject magicSlider;
    public GameObject magicCoolImage;

    [Header("Wait")]
    public GameObject mainCamera;
    public GameObject mainVCam;
    
    public GameObject loadingCamera;
    public GameObject loadingVCam;
    public GameObject dollyTrack;
    public GameObject dollyCart;

    public GameObject water;
    public GameObject waitPos;

    public GameObject testPlayer;
    public GameObject temp;
    

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

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            temp = Instantiate(GameManager.Instance.battleCharacters[GameManager.Instance.charNum]);
            temp.GetComponent<BattlePlayer>().isWait = true;
            temp.transform.position = waitPos.transform.position;
            
        }
        else
        { 
            temp = Instantiate(testPlayer);
            temp.GetComponent<BattlePlayer>().isWait = true;
            temp.transform.position = waitPos.transform.position;
        }

        Cursor.visible = false;
    }

    private void Update()
    {
        if (gameWait)
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
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (invenCanvas.gameObject.activeSelf)
                {
                    SwitchCanvasActive(invenCanvas);
                    SwitchGameObjectActive(characterCam);
                }
                else
                {

                    SwitchCanvasActive(pauseCanvas);
                    
                    if(pauseCanvas.gameObject.activeSelf)
                        Cursor.visible = true;
                    else
                        Cursor.visible = false;
                }
            }
        }
    }

    public void GameStart()
    {
        dollyCart.GetComponent<CinemachineDollyCart>().m_Speed = 15;
        StartCoroutine(StartCart());
    }

    IEnumerator StartCart()
    {
        while (true)
        {
            if (dollyTrack.GetComponent<CinemachineSmoothPath>().m_Waypoints[3].position +
                dollyTrack.transform.position == loadingCamera.transform.position)
            {
                Destroy(temp.gameObject);

                loadingCamera.GetComponent<AudioListener>().enabled = false;
                
                loadingVCam.SetActive(false);
                loadingCamera.SetActive(false);

                mainVCam.SetActive(true);
                mainCamera.SetActive(true);

                playCanvas.gameObject.SetActive(true);

                for (int i = 0; i < players.Length; ++i)
                {
                    if (players[i] == null) continue;

                    if (players[i].GetComponent<BattlePlayer>().photonView.IsMine)
                    {
                        players[i].GetComponent<BattlePlayer>().virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

                        players[i].GetComponent<AudioListener>().enabled = true;

                        CServercamTest sct = Camera.main.GetComponent<CServercamTest>();

                        sct.bp = GetComponent<BattlePlayer>();
                        players[i].GetComponent<BattlePlayer>().virtualCamera.Follow = players[i].GetComponent<BattlePlayer>().transform;
                        players[i].GetComponent<BattlePlayer>().virtualCamera.LookAt = players[i].GetComponent<BattlePlayer>().transform;

                        MakeUICharacter();

                        players[i].GetComponent<BattlePlayer>().isStart = true;
                    }
                }


                water.SetActive(false);
                
                Magnetic.instance.magneticStart();
                

                // if (BattlePlayer.instance.photonView.IsMine)
                // {
                //     BattlePlayer.instance.transform.position = NetworkManager.Instance.spawnpoints[BattlePlayer.instance.myindex].transform.position;
                // }
                yield break;
            }

            yield return null;
        }
    }
    
    public void Resume()
    {
        Cursor.visible = false;
        SwitchCanvasActive(pauseCanvas);
    }

    public void Exit()
    {
        Cursor.visible = true;
        NetworkManager.Instance.connect = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene((int)(Scene.LobbyScene));
    }
    
    public void openWinCanvas()
    {
        Cursor.visible = true;
        SwitchCanvasActive(winCanvas);
    }
    
    public void openLoseCanvas()
    {
        Cursor.visible = true;
        SwitchCanvasActive(loseCanvas);
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
        if (BattlePlayer.instance.photonView.IsMine && !BattlePlayer.instance.isalive) return;
        
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
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i] == null) continue;

            if (players[i].GetComponent<BattlePlayer>().photonView.IsMine)
            {
                GameObject temp;

                if (GameManager.Instance == null)
                {
                    temp = Instantiate(characterUIs[2]);
                }
                else
                {
                    temp = Instantiate(GameManager.Instance.battleCharacters[GameManager.Instance.charNum]);
                }

                temp.GetComponent<BattlePlayer>().isUI = true;
                temp.transform.SetParent(characterUIParent.transform);
                temp.transform.position = characterUIParent.transform.position;
                temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}
