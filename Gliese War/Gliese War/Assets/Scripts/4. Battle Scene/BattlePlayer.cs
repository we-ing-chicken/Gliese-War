using Cinemachine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class BattlePlayer : LivingEntity, IPunObservable
{
    static public BattlePlayer instance;
    public int myindex;
    private float Gravity = 9.8f;
    public int life;
    public float MouseX;
    public float mouseSpeed;

    private string moveFBAxisName = "Vertical"; //  յ                 Է     ̸ 
    private string moveLRAxisName = "Horizontal"; //  ¿                 Է     ̸ 
    private string meleeAttackButtonName = "Fire1"; //  ߻縦       Է    ư  ̸ 
    private string magicAttackButtonName = "Fire2"; //  ߻縦       Է    ư  ̸ 
    private string JumpButtonName = "Jump";

    public int offensivePower;
    public int defensivePower;
    public int moveSpeed; //  յ            ӵ 
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playertransform;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private Transform fowardTarget;
    [SerializeField] private Transform backwardTarget;
    [SerializeField] private Transform lbTarget;
    [SerializeField] private Transform lfTarget;
    [SerializeField] private Transform rbTarget;
    [SerializeField] private Transform rfTarget;
    [SerializeField] private bool ignoreGravity = false;

    public List<Item> items;

    public RealItem helmet;
    public RealItem armor;
    public RealItem shoe;
    public RealItem weapon1;
    public RealItem weapon2;
    public int weaponNow = 1;
    bool ismove = false;
    bool isjump = false;
    public bool isalive;

    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject footL;
    [SerializeField] private GameObject footR;
    public GameObject handR;
    public GameObject back;
    private MeshCollider col;

    [SerializeField] private GameObject attackEffectPos;
    [SerializeField] private GameObject[] attackEffect;

    [SerializeField] private GameObject shoesEffectPos;
    [SerializeField] private GameObject whatMagicPos;

    public bool isAttack = false;

    private CharacterController charactercontroller;
    
    private bool isMagic;   // while right mouse down
    private bool isCool;    // after use magic
    private Coroutine magicCor; // set magic area position coroutine 
    [SerializeField] private GameObject[] magicEffect;
    private Vector3 magicPosition;
    public int myMagicNum = 0; // my magic num
    
    private int magicMaster;    // who's magic
    private float magicCooltime;    // how long wait for use magic again
    private int magicNum = 0;   // other magic num
    private bool otherMagic; // check other use magic

    public float moveFB { get; private set; } //             ̵   Է° 
    public float moveLR { get; private set; } //         ¿  ̵   Է° 
    public float rot { get; private set; } //        ȸ    Է° 
    public bool LeftMouseButtonDown { get; private set; } //         ߻ 1  Է° 
    public bool Mgattack { get; private set; } //         ߻ 2  Է° 
    public bool p_Jump { get; private set; } //              Է° 

    public float JumpPower;

    private Vector3 moveDir;

    public bool isNear;

    private Vector3 remotePos;
    private Quaternion remoteRot;

    private float lag;
    new private Rigidbody rigidbody;
    private float remotetime;

    private bool isSafe = false;
    
    private float statusTime;
    private bool isOn = false;

    public bool isUI = false;
    public bool isWait = false;
    public bool isStart = false;

    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        if (isUI)
        {
            GetComponent<AudioListener>().enabled = false;
            return;
        }
        
        Debug.Log(photonView.IsMine);

        instance = this;
        if (photonView.IsMine)
        {
            applyItems();
        }
        else
        {
            GetComponent<AudioListener>().enabled = false;
        }
        if (!isalive) isalive = true;

        //charactercontroller = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
        moveDir = Vector3.zero;
        rot = 1.0f;
        isNear = false;
        life = 10;

        if (GameManager.Instance != null)
        {
            offensivePower = GameManager.Instance.stat.attackPower;
            defensivePower = GameManager.Instance.stat.defensePower;
            moveSpeed = GameManager.Instance.stat.moveSpeed;
        }
        else
        {
            offensivePower = 10;
            defensivePower = 10;
            moveSpeed = 10;
        }

        SetBattleItemEquip();
        SetEquipItemImage();

        RefreshStat();
        SetMagicImage();

        magicCooltime = 5f;
        isCool = false;

        EquipWeapon();
        if (photonView.IsMine)
        {
            WhatMagicEffect(myindex, (int)GetMagic());
        }
    }

    private void Update()
    {
        if (isUI) return;
        
        if(isWait)
        {
            if (!BattleManager.Instance.gameWait && NetworkManager.Instance.connect)
            {
                BattleManager.Instance.gameWait = true;
                NetworkManager.Instance.JoinRoom();
            }
        }
        

        if (NetworkManager.Instance.sendOK && photonView.ViewID != 0 && PhotonNetwork.CurrentRoom.Players.Count == 1)
        {
            NetworkManager.Instance.sendOK = false;
            photonView.RPC("SendIndex", RpcTarget.All, photonView.ViewID, myindex);
            photonView.RPC("StartGame", RpcTarget.All);
            photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex, (int)GetWeaponNum(), (int)weapon1.magic);
            WhatMagicEffect(myindex, (int)GetMagic());
        }

        if (isWait || !isStart) return;
        
        
        //if (charactercontroller == null) return;

        if (photonView.IsMine && isalive)
        {
            moveFB = Input.GetAxis(moveFBAxisName);
            moveLR = Input.GetAxis(moveLRAxisName);

            ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));
            Mgattack = Input.GetButton(magicAttackButtonName);
            p_Jump = Input.GetButton(JumpButtonName);

        }

        if(photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weapon1 == null) return;
                if (weaponNow == 1) return;
                weaponNow = 1;

                switch (weapon1.magic)
                {
                    case Magic.Fire:
                        myMagicNum = 0;
                        break;

                    case Magic.Water:
                        myMagicNum = 1;
                        break;

                    case Magic.Light:
                        myMagicNum = 2;
                        break;

                    case Magic.Nothing:
                        break;
                }

                if (weapon1 != null)
                    EquipWeapon();

                RefreshStat();
                WhatMagicEffect(myindex, (int)GetMagic());
                SetMagicImage();
                photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex, (int)weapon1.item.weaponType, (int)weapon1.magic);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weapon2 == null) return;
                if (weaponNow == 2) return;
                
                weaponNow = 2;

                switch (weapon2.magic)
                {
                    case Magic.Fire:
                        myMagicNum = 0;
                        break;

                    case Magic.Water:
                        myMagicNum = 1;
                        break;

                    case Magic.Light:
                        myMagicNum = 2;
                        break;

                    case Magic.Nothing:
                        break;
                }

                if (weapon2 != null)
                    EquipWeapon();

                RefreshStat();
                WhatMagicEffect(myindex, (int)GetMagic());
                SetMagicImage();
                photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex,(int)weapon2.item.weaponType, (int)weapon2.magic);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                weapon1.item = BattleManager.Instance.sword[1];
                EquipWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                weapon1.item = BattleManager.Instance.spear[1];
                EquipWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                weapon1.item = BattleManager.Instance.hammer[1];
                EquipWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                myMagicNum = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                myMagicNum = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                myMagicNum = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Debug.Log(base.health);
                GetDamage(5);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(Burns());
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine(Toxic());
            }
        }

        if (photonView.IsMine)
        {
            LeftMouseButtonDown = Input.GetMouseButtonDown(0);
        }

        if (LeftMouseButtonDown && !isAttack)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("SendMouseButtonDown", RpcTarget.Others, LeftMouseButtonDown);
                if (isMagic)
                {
                    animator.SetTrigger("magicAttack");
                    MakeMagic(myMagicNum, MagicArea.Instance.transform.position, myindex);
                    isMagic = false;
                    StopCoroutine(magicCor);
                    
                    photonView.RPC("SendMagic", RpcTarget.Others, MagicArea.Instance.transform.position, myindex, myMagicNum);
                    
                    isCool = true;
                    MagicArea.Instance.transform.position = new Vector3(0,0,0);
                    StartCoroutine(CheckCoolTime());

                    return;
                }
                isAttack = true;
                AttackStart();
                AttackAnimation();
                StartCoroutine(AttackEffect());
            }
            else
            {
                if (otherMagic)
                {
                    animator.SetTrigger("magicAttack");
                    otherMagic = false;
                }
                else
                {
                    AttackStart();
                    StartCoroutine(AttackEffect());
                }
                LeftMouseButtonDown = false;
                photonView.RPC("SendMouseButtonDown", RpcTarget.All, LeftMouseButtonDown);

            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (GetMagic() == Magic.Nothing) return;
            
            if (isCool)
            {
                Debug.Log("쿨타임");
                return;
            }

            isMagic = true;
            magicCor = StartCoroutine(SetMagicArea());
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (GetMagic() == Magic.Nothing) return;
            
            isMagic = false;
            if (magicCor != null) StopCoroutine(magicCor);
            MagicArea.Instance.transform.position = new Vector3(0,0,0);
        }


        //if (charactercontroller == null) return;

        //Debug.Log("IsMine : " + photonView.IsMine + ", remotePos : " + remotePos);

        //if (!charactercontroller.isGrounded)
        //{
        //    if (!ignoreGravity)
        //        Fall();
        //}
        //else
        //{


        if (p_Jump && !isjump)
        {
            isAttack = true;
            isjump = true;
            animator.SetTrigger("doJump");
            rigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);

            //Jump();

        }
        
        if (photonView.IsMine)
        {
            if (BattleManager.Instance.mainCamera.gameObject.activeSelf)
            {
                
            
            //remoteDir = new Vector3(moveLR, 0, moveFB).normalized;
            //player_lookTarget();


            if (CheckHitWall(new Vector3(moveFB,0,moveLR)) || CheckHitWall(new Vector3(-moveFB,0,-moveLR)))
            {
            }
            else
            {
                Vector3 lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
                Vector3 lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;
                moveDir = lookForward * moveFB + lookRight * moveLR;
                //Move();
                transform.position += moveDir * moveSpeed * Time.deltaTime;
                playertransform.LookAt(playertransform.position + moveDir);
            
            }   
                //Look();
                MouseX = MouseX + (Input.GetAxis("Mouse X") * mouseSpeed);
                remoteRot = Quaternion.Euler(0, MouseX, 0);
                transform.rotation = remoteRot;
            
            }
        }
        else
        {
            //ismove = (remoteDir.x > 0 || remoteDir.z > 0);
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            remoteRot = Quaternion.Euler(0, MouseX, 0);
            transform.rotation = remoteRot;
        }
        //}

        //charactercontroller.Move(moveDir * Time.deltaTime * moveSpeed);
        animate_Run(isalive);
    }
    
    bool CheckHitWall(Vector3 movement)
    {
        // 움직임에 대한 로컬 벡터를 월드 벡터로 변환해준다.
        movement = transform.TransformDirection(movement);
        // scope로 ray 충돌을 확인할 범위를 지정할 수 있다.
        float scope = 1f;

        // 플레이어의 머리, 가슴, 발 총 3군데에서 ray를 쏜다.
        List<Vector3> rayPositions = new List<Vector3>();
        rayPositions.Add(transform.position + Vector3.up * 0.1f);
        rayPositions.Add(transform.position + Vector3.up * GetComponent<CapsuleCollider>().height * 0.5f);
        rayPositions.Add(transform.position + Vector3.up * GetComponent<CapsuleCollider>().height);

        // 디버깅을 위해 ray를 화면에 그린다.
        foreach (Vector3 pos in rayPositions)
        {
            Debug.DrawRay(pos, movement * scope, Color.red);
        }

        // ray와 벽의 충돌을 확인한다.
        foreach (Vector3 pos in rayPositions)
        {
            if (Physics.Raycast(pos, movement, out RaycastHit hit, scope))
            {
                if (hit.collider.CompareTag("Wall"))
                    return true;
            }
        }
        return false;
    }

    IEnumerator SetMagicArea()
    {
        Debug.Log("On");

        Transform playerTransform = null;
        
        for (int i = 0; i < BattleManager.Instance.players.Length; ++i)
        {
            if (BattleManager.Instance.players[i] == null) continue;

            if (BattleManager.Instance.players[i].GetComponent<BattlePlayer>().photonView.IsMine)
            {
                playerTransform = BattleManager.Instance.players[i].GetComponent<BattlePlayer>().transform;
            }
        }
        
        float distance = Vector3.Distance(Camera.main.transform.position, playerTransform.position);
        RaycastHit[] hit;

        while (true)
        {
            Vector3 direction = ((playerTransform.position + new Vector3(0f, 1f, 0f)) - Camera.main.transform.position).normalized;
            hit = (Physics.RaycastAll(Camera.main.transform.position, direction, distance + 10f));

            for (int i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform.CompareTag("Terrain"))
                {
                    MagicArea.Instance.transform.position = hit[i].point;
                    break;
                }
            }

            yield return null;
        }
    }

    void SetMagicImage()
    {
        if (GetMagic() == Magic.Nothing)
        {
            Color c = BattleManager.Instance.magicCoolImage.GetComponent<Image>().color;
            c.a = 0f;
            BattleManager.Instance.magicCoolImage.GetComponent<Image>().color = c;
            BattleManager.Instance.magicSlider.GetComponent<Slider>().value = 0f;
        }
        else
        {
            Color c = BattleManager.Instance.magicCoolImage.GetComponent<Image>().color;
            c.a = 100f;
            BattleManager.Instance.magicCoolImage.GetComponent<Image>().color = c;
                    
            BattleManager.Instance.magicCoolImage.GetComponent<Image>().sprite = BattleManager.Instance.magics[(int)GetMagic()];
            
            if (isCool) return;
            
            isCool = true;
            StartCoroutine(CheckCoolTime(2.5f));
        }
    }

    void MakeMagic(int num, Vector3 pos, int who)
    {
        GameObject magic = Instantiate(magicEffect[num]); //1 Tornado , 2 Thunder  0 Fire

        switch (num)
        {
            case (int)Magic.Fire:
                magic.transform.GetChild(0).GetChild(5).GetChild(3).GetComponent<Meteo>().SetMaster(who);
                break;

            case (int)Magic.Water:
                magic.transform.GetChild(1).GetComponent<Tornado>().SetMaster(who);
                break;
            
            case (int)Magic.Light:
                magic.transform.GetChild(2).GetChild(1).GetComponent<Thunder>().SetMaster(who);
                break;
        }
        
        magic.transform.position = pos;
    }

    IEnumerator CheckCoolTime(float time = 0f)
    {

        while (true)
        {
            time += Time.deltaTime;
            BattleManager.Instance.magicSlider.GetComponent<Slider>().value = time / magicCooltime;

            if (time >= magicCooltime)
            {
                isCool = false;
                BattleManager.Instance.magicSlider.GetComponent<Slider>().value = 1f;
                break;
            }

            yield return null;
        }
    }

    private void Move()
    {
        //player_lookTarget();

        //transform.position += remoteDir * moveSpeed * Time.deltaTime;

        //moveDir = charactercontroller.transform.TransformDirection(remoteDir);
    }

    private void Jump()
    {

        //remoteDir.y = JumpPower;
    }
    private void Fall()
    {
        //remoteDir.y -= Gravity * Time.deltaTime;
    }
    private void Look()
    {
        //if(photonView.IsMine)
        //{
        //    MouseX = MouseX + (Input.GetAxis("Mouse X") * mouseSpeed);
        //}

        //remoteRot = Quaternion.Euler(0, MouseX, 0);
    }
    private void player_lookTarget()
    {
        //if (charactercontroller == null) return;

        if (moveLR < 0 && moveFB < 0)    // left + back
        {
            player_Rotate(lbTarget.position);
        }
        else if (moveLR < 0 && moveFB > 0)    // left + forward
        {
            player_Rotate(lfTarget.position);

        }
        else if (moveLR > 0 && moveFB < 0)    // right + back
        {
            player_Rotate(rbTarget.position);

        }
        else if (moveLR > 0 && moveFB > 0)    // right + forward
        {
            player_Rotate(rfTarget.position);

        }
        else if (moveLR < 0)
        {
            player_Rotate(leftTarget.position);
        }
        else if (moveLR > 0)
        {
            player_Rotate(rightTarget.position);

        }
        else if (moveFB < 0)
        {
            player_Rotate(backwardTarget.position);

        }
        else if (moveFB > 0)
        {
            player_Rotate(fowardTarget.position);
        }
    }
    private void player_Rotate(Vector3 movePoint)
    {
        Vector3 relativePosition = movePoint - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        playertransform.rotation = Quaternion.Lerp(playertransform.rotation, rotation, Time.deltaTime * 10);
    }

    private void animate_Run(bool ia)
    {
            if(ia) animator.SetBool("isRun", ismove);
    }

    private void AttackAnimation()
    {
        //Debug.Log(weaponNow);
        if (weaponNow == 1)
        {
            if (weapon1 == null)
                return;

            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    animator.SetTrigger("attackHammer");
                    break;

                case Item.WeaponType.Sword:
                    animator.SetTrigger("attackSword");
                    break;

                case Item.WeaponType.Spear:
                    animator.SetTrigger("attackSpear");
                    break;
            }
        }
        else if (weaponNow == 2)
        {
            if (weapon2 == null)
                return;

            switch (weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    animator.SetTrigger("attackHammer");
                    break;

                case Item.WeaponType.Sword:
                    animator.SetTrigger("attackSword");
                    break;

                case Item.WeaponType.Spear:
                    animator.SetTrigger("attackSpear");
                    break;
            }
        }
    }

    IEnumerator AttackEffect()
    {
        if (weaponNow == 1)
        {
            if (weapon1 == null)
                yield return null;

            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    yield return new WaitForSeconds(0.7f);
                    attackEffectPos.transform.GetChild(2).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(2));
                    break;

                case Item.WeaponType.Sword:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(0));

                    break;

                case Item.WeaponType.Spear:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(1).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(1));
                    break;
            }
        }
        else if (weaponNow == 2)
        {
            if (weapon2 == null)
                yield return null;

            switch (weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    yield return new WaitForSeconds(0.7f);
                    attackEffectPos.transform.GetChild(2).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(2));
                    break;

                case Item.WeaponType.Sword:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(0));

                    break;

                case Item.WeaponType.Spear:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(1).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(1));
                    break;
            }
        }
        yield return null;
    }

    IEnumerator QuitAttackEffect(int pos)
    {
        yield return new WaitForSeconds(0.5f);
        attackEffectPos.transform.GetChild(pos).gameObject.SetActive(false);
        yield return null;
    }

    public void UnEquip(RealItem realItem)
    {
        //offensivePower -= realItem.stat.attackPower;
        defensivePower -= realItem.stat.defensePower;
        moveSpeed -= realItem.stat.moveSpeed;
        RefreshStat();
    }

    public void Equip(RealItem realItem)
    {
        //offensivePower += realItem.stat.attackPower;
        defensivePower += realItem.stat.defensePower;
        moveSpeed += realItem.stat.moveSpeed;
        RefreshStat();
        SetShoesEffect();
    }

    private void RefreshStat()
    {
        Inventory.instance.statParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Health : " + health + " / " + startingHealth;
        Inventory.instance.statParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Attack : " + (offensivePower + GetWeaponStat());
        Inventory.instance.statParent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Defense : " + defensivePower;
        Inventory.instance.statParent.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Speed : " + moveSpeed;
    }

    private int GetWeaponStat()
    {
        if (weapon1 == null && weapon2 == null) return 0;

        int power = 0;

        if (weaponNow == 1)
        {
            if (weapon1 != null)
                power = weapon1.stat.attackPower;
        }
        else if (weaponNow == 2)
        {
            if (weapon2 != null)
                power = weapon2.stat.attackPower;
        }

        return power;
    }

    public void WearHelmet()
    {
        if (head.transform.childCount == 1)
            Destroy(head.transform.GetChild(0).gameObject);

        GameObject temp = Instantiate(Inventory.instance.helmet[0].itemPrefab, head.transform);
        //temp.transform.position = new Vector3(-0.15f, 0.65f, 0f);
        temp.transform.localEulerAngles = new Vector3(0f, -90f, 180f);
        temp.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
    }

    public void WearArmor()
    {
        if (body.transform.childCount == 1)
            Destroy(body.transform.GetChild(0).gameObject);

        GameObject temp = Instantiate(Inventory.instance.armor[0].itemPrefab, body.transform);
        //temp.transform.position = new Vector3(0f, 0.15f, 0f);
        temp.transform.localEulerAngles = new Vector3(0, -90f, 180f);
        temp.transform.localScale = new Vector3(0.15f, 0.18f, 0.17f);
    }

    public void WearShoe()
    {
        if (footL.transform.childCount == 1)
        {
            Destroy(footL.transform.GetChild(0).gameObject);
            Destroy(footL.transform.GetChild(0).gameObject);
        }

        GameObject temp = Instantiate(Inventory.instance.shoes[0].itemPrefab, footL.transform);
        //temp.transform.position += new Vector3(0f, 0.2f, -0.05f);
        temp.transform.localEulerAngles = new Vector3(0, 90f, 180f);
        temp.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        GameObject temp2 = Instantiate(Inventory.instance.shoes[0].itemPrefab, footR.transform);
        //temp2.transform.position += new Vector3(0.05f, 0f, 0.05f);
        temp2.transform.localEulerAngles = new Vector3(0, 90f, 180f);
        temp2.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    public void EquipWeapon()
    {

        if (handR.transform.childCount == 1)
            Destroy(handR.transform.GetChild(0).gameObject);

        if (weaponNow == 1)
        {
            if (weapon1 == null)
                return;

            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    EquipHammer();
                    break;

                case Item.WeaponType.Sword:
                    EquipSword();
                    break;

                case Item.WeaponType.Spear:
                    EquipSpear();
                    break;
            }
        }
        else if (weaponNow == 2)
        {
            if (weapon2 == null)
                return;

            switch (weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    EquipHammer();
                    break;

                case Item.WeaponType.Sword:
                    EquipSword();
                    break;

                case Item.WeaponType.Spear:
                    EquipSpear();
                    break;
            }
        }
    
        BattleManager.Instance.SetEquipWeaponImage();
    }

    private void EquipHammer()
    {
        back.transform.GetChild(0).gameObject.SetActive(true);
        back.transform.GetChild(1).gameObject.SetActive(false);
        back.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void EquipSpear()
    {
        back.transform.GetChild(1).gameObject.SetActive(true);
        back.transform.GetChild(0).gameObject.SetActive(false);
        back.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void EquipSword()
    {
        back.transform.GetChild(2).gameObject.SetActive(true);
        back.transform.GetChild(0).gameObject.SetActive(false);
        back.transform.GetChild(1).gameObject.SetActive(false);
    }

    public int GetAttackPower()
    {
        return (offensivePower + GetWeaponStat());
    }

    public void SetShoesEffect()
    {
        if (shoe == null)
            return;

        for (int i = 0; i < 5; ++i)
            shoesEffectPos.transform.GetChild(i).gameObject.SetActive(false);

        switch (shoe.item.itemRank)
        {
            case Item.ItemRank.Normal:
                shoesEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                break;

            case Item.ItemRank.Rare:
                shoesEffectPos.transform.GetChild(1).gameObject.SetActive(true);
                break;

            case Item.ItemRank.Epic:
                shoesEffectPos.transform.GetChild(2).gameObject.SetActive(true);
                break;

            case Item.ItemRank.Unique:
                shoesEffectPos.transform.GetChild(3).gameObject.SetActive(true);
                break;

            case Item.ItemRank.Legendary:
                shoesEffectPos.transform.GetChild(4).gameObject.SetActive(true);
                break;

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(MouseX);
            //stream.SendNext(Time.deltaTime);
        }
        else
        {
            moveDir = (Vector3)stream.ReceiveNext() - transform.position;
            MouseX = (float)stream.ReceiveNext();
            //remotetime = (float)stream.ReceiveNext();

            //lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            //remoteDir.y = -remoteDir.y;
            //remoteDir = -remoteDir;
            //Debug.Log("PhotonNetwork.Time : " + PhotonNetwork.Time + ", info.SentServerTime : " + info.SentServerTime + ", lag : " + lag);
            //Debug.Log("1remoteDir : " + remoteDir);
            //remoteDir = remoteDir + remoteDir * lag;
            //if (remoteDir.x <= 0)
            //    remoteDir.x = remoteDir.x - remoteDir.x * lag;
            //else
            //    remoteDir.x = remoteDir.x + remoteDir.x * lag;

            ////remoteDir.y = remoteDir.y + remoteDir.y * lag;
            //if (remoteDir.z <= 0)
            //    remoteDir.z = remoteDir.z - remoteDir.z * lag;
            //else
            //    remoteDir.z = remoteDir.z + remoteDir.z * lag;

            //Debug.Log("2remoteDir : " + remoteDir);
            //MouseX = MouseX * lag;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.collider.tag == "Weapon")
        //{
        //    Debug.Log($"OnControllerColliderHit - {hit.collider.name}");
            
        //}
        // TO DO : 무기 스크립트 생성, 충돌 대상 태그가 플레이어라면 대상의 ApplyDamage 호출
           
    }
    void applyItems()
    {
        if (GameManager.Instance == null) return;
        helmet = GameManager.Instance.helmet;
        armor = GameManager.Instance.armor;
        shoe = GameManager.Instance.shoe;
        weapon1 = GameManager.Instance.weapon1;
        weapon2 = GameManager.Instance.weapon2;
    }

    void SetBattleItemEquip()
    {
        if (GameManager.Instance == null)
        {
            //Debug.Log(weapon1.item + ", " + weapon1.magic + ", " + weapon1.stat.attackPower);
            if(weapon1 == null) 
            {
                weapon1 = new RealItem();
                weapon1.item = BattleManager.Instance.sword[1];
                weapon1.magic = Magic.Fire;
            }

            if(weapon2 == null)
            {
                weapon2 = new RealItem();
                weapon2.item = BattleManager.Instance.spear[1];
                weapon2.magic = Magic.Nothing;
            }
        }
        else
        {
            if (photonView.IsMine)
            {
                helmet = GameManager.Instance.helmet;
                armor = GameManager.Instance.armor;
                shoe = GameManager.Instance.shoe;
                weapon1 = GameManager.Instance.weapon1;
                weapon2 = GameManager.Instance.weapon2;
            }
        }
    }

    void SetEquipItemImage()
    {
        if (GameManager.Instance == null) return;
        if (isWait || isUI ) return;
        if (!photonView.IsMine) return;
        
        if(BattleManager.Instance.helmetEquip != null) BattleManager.Instance.helmetEquip.GetComponent<Image>().sprite = helmet.item.itemImage;
        if(BattleManager.Instance.armorEquip != null) BattleManager.Instance.armorEquip.GetComponent<Image>().sprite = armor.item.itemImage;
        if(BattleManager.Instance.shoeEquip != null) BattleManager.Instance.shoeEquip.GetComponent<Image>().sprite = shoe.item.itemImage;

        if (BattleManager.Instance.weapon1Equip != null)
        {
            BattleManager.Instance.weapon1Equip.GetComponent<Image>().sprite = weapon1.item.itemImage;
            BattleManager.Instance.weapon1Magic.GetComponent<Image>().sprite = GetMagicImage(weapon1.magic);
            if (weapon1.magic == Magic.Nothing)
            {
                Color color = BattleManager.Instance.weapon1Magic.GetComponent<Image>().color = new Color();
                color.a = 0;
                BattleManager.Instance.weapon1Magic.GetComponent<Image>().color = color;
            }
        }

        if (BattleManager.Instance.weapon2Equip != null)
        {
            BattleManager.Instance.weapon2Equip.GetComponent<Image>().sprite = weapon2.item.itemImage;
            BattleManager.Instance.weapon2Magic.GetComponent<Image>().sprite = GetMagicImage(weapon2.magic);
            if (weapon2.magic == Magic.Nothing)
            {
                Color color = BattleManager.Instance.weapon2Magic.GetComponent<Image>().color = new Color();
                color.a = 0;
                BattleManager.Instance.weapon2Magic.GetComponent<Image>().color = color;
            }
        }
    }

    Sprite GetMagicImage(Magic magic)
    {
        switch (magic)
        {
            case Magic.Fire:
                return BattleManager.Instance.magics[0];
            
            case Magic.Water:
                return BattleManager.Instance.magics[1];
            
            case Magic.Light:
                return BattleManager.Instance.magics[2];
            
            case Magic.Nothing:
                return null;
            
        }

        return null;
    }

    Item.WeaponType GetWeaponNum()
    {
        if (weaponNow == 1)
        {
            return weapon1.item.weaponType;
        }
        else if (weaponNow == 2)
        {
            return weapon2.item.weaponType;
        }

        return Item.WeaponType.Nothing;
    }

    Magic GetMagic()
    {
        if (weaponNow == 1)
        {
            return weapon1.magic;
        }
        else if (weaponNow == 2)
        {
            return weapon2.magic;
        }

        return Magic.Nothing;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void RestoreHealth(int newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;
        
        
        if (damageMessage.damage > 0)
        {
            BattleManager.Instance.HitScreen();
        }
        else
        {
            if (health - damageMessage.damage > startingHealth)
            {
                health = startingHealth;
            }
        }
        
        MyHPBar.Instance.SetHPBar(startingHealth, health);
        
        return true;
    }

    public override void Die()
    {
        base.Die();

        SetParameter();

        StartCoroutine("die");

        //BattleManager.Instance.BM_RemoveList(myindex);
        
    }
    public void SetParameter()
    {
        isalive = false;
        animator.SetBool("isRun", false);
        moveDir = Vector3.zero;
        moveSpeed = 0;
        isAttack = true;
        ismove = false;
    }
    IEnumerator die()
    {
        animator.SetTrigger("dying");
        yield return new WaitForSeconds(2.0f);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            isAttack = false;
            animator.SetBool("doJump", false);
            isjump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inside") && photonView.IsMine)
        {
            isSafe = true;
        }
        
        if (other.CompareTag("Weapon") && !other.transform.GetComponentInParent<BattlePlayer>().photonView.IsMine)
        {
            health -= 10;
            DamageMessage dm;
            dm.damager = other.transform.GetComponentInParent<BattlePlayer>().myindex;
            dm.damage = 10;
            ApplyDamage(dm);

            ShowHitEffect(myindex, other.transform.GetComponentInParent<BattlePlayer>().myMagicNum);
            
            photonView.RPC("SendHit", RpcTarget.Others, myindex, other.transform.GetComponentInParent<BattlePlayer>().myMagicNum);
        }

        GameObject healEffect;
        if (other.CompareTag("Item") && photonView.IsMine)
        {
            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = -20;
            ApplyDamage(dm);
        
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = other.transform.position;

            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Item"))
        {
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = new Vector3(other.transform.position.x,other.transform.position.y - 0.5f,other.transform.position.z);

            Destroy(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inside") && photonView.IsMine)
        {
            isSafe = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isSafe)
        {
            if (other.CompareTag("Outside") && photonView.IsMine)
            {
                Debug.Log("밖");
                DamageMessage dm;
                dm.damager = myindex;
                dm.damage = 1;
                ApplyDamage(dm);
                BattleManager.Instance.HitScreen();
                //GetDamage(1);
            }
        }
        
        if (other.CompareTag("Heal") && photonView.IsMine)
        {
            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = -1;
            ApplyDamage(dm);
        }
    }
    
    public void GetDamage(int val)
    {
        health -= val;
        MyHPBar.Instance.SetHPBar(startingHealth, health);
        BattleManager.Instance.HitScreen();
    }

    public void ShowHitEffect(int who, int magicNum)
    {
        GameObject hitEffect;
        hitEffect = Instantiate(BattleManager.Instance.HitEffects[magicNum]);
        hitEffect.transform.position = BattleManager.Instance.players[who].transform.position;
    }
    
    public void AttackStart()
    {
        if (weaponNow == 1)
        {
            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Sword:
                    StartCoroutine(TurnOffHandSword());
                    TurnOnHandSword();
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }
        else if (weaponNow == 2)
        {

            switch (weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer();
                    StartCoroutine(TurnOffHandHammer());
                    break;
                
                case Item.WeaponType.Sword:
                    TurnOnHandSword();
                    StartCoroutine(TurnOffHandSword());
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear();
                    StartCoroutine(TurnOffHandSpear());
                    break;
            }
        }

        isAttack = true;
    }

    public void TurnOnHandHammer()
    {
        handR.transform.GetChild(0).gameObject.SetActive(true);
        back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear()
    {
        handR.transform.GetChild(1).gameObject.SetActive(true);
        back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSword()
    {
        handR.transform.GetChild(2).gameObject.SetActive(true);
        back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        handR.transform.GetChild(0).gameObject.SetActive(false);
        back.transform.GetChild(0).gameObject.SetActive(true);
        
        isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear()
    {
        yield return new WaitForSeconds(0.6f);
        
        col = handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        handR.transform.GetChild(1).gameObject.SetActive(false);
        back.transform.GetChild(1).gameObject.SetActive(true);
        
        isAttack = false;
    }
    
    IEnumerator TurnOffHandSword()
    {
        yield return new WaitForSeconds(1f);
        
        col = handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        handR.transform.GetChild(2).gameObject.SetActive(false);
        back.transform.GetChild(2).gameObject.SetActive(true);
        
        isAttack = false;
    }

    [PunRPC]
    void SendMouseButtonDown(bool LBD)
    {
        LeftMouseButtonDown = LBD;
    }

    [PunRPC]
    void SendMagic(Vector3 position, int who, int which)
    {
        magicPosition = position;
        magicMaster = who;
        magicNum = which;
        otherMagic = true;
        
        MakeMagic(magicNum, magicPosition, magicMaster);
    }

    IEnumerator Burns()
    {
        if (isOn) yield break;
        
        float total = 7f;
        statusTime = 0f;
        isOn = true;
        
        StartCoroutine(Timecheck());
        
        while (true)
        {
            if (statusTime >= total)
            {
                isOn = false;
                break;
            }

            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = 2;
            ApplyDamage(dm);
            
            BattleManager.Instance.HitScreen();
            
            yield return new WaitForSeconds(2f);
        }
        
        yield return null;
    }

    IEnumerator Toxic()
    {
        if (isOn) yield break;
        
        float total = 7f;
        statusTime = 0f;
        isOn = true;
        
        StartCoroutine(Timecheck());
        
        while (true)
        {
            if (statusTime >= total)
            {
                isOn = false;
                break;
            }

            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = 2;
            ApplyDamage(dm);
            
            BattleManager.Instance.HitScreen();
            
            yield return new WaitForSeconds(2f);
        }
        
        yield return null;
    }

    IEnumerator Timecheck()
    {
        while (true)
        {
            statusTime += Time.deltaTime;
            
            if (!isOn)
                break;

            yield return null;
        }
    }

    void WhatWeapon(int who, int weaopnNum)
    {
        if (BattleManager.Instance.players[who] == null) return;

        Item.WeaponType weapon;
        
        switch (weaopnNum)
        {
            case (int)Item.WeaponType.Hammer:
                weapon = Item.WeaponType.Hammer;
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;

            case (int)Item.WeaponType.Spear:
                weapon = Item.WeaponType.Spear;
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;

            case (int)Item.WeaponType.Sword:
                weapon = Item.WeaponType.Sword;
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2)
                    .gameObject.SetActive(true);
                break;

            default:
                weapon = Item.WeaponType.Nothing;
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;
        }

        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon1.item.weaponType = weapon;
    }

    void WhatMagicEffect(int who, int magicNum)
    {
        if (BattleManager.Instance.players[who] == null) return;
        
        switch (magicNum)
        {
            case (int)Magic.Fire:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;

            case (int)Magic.Water:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;

            case (int)Magic.Light:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(true);
                break;

            default:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(false);
                break;
        }

    }

    [PunRPC]
    void SendHit(int who, int magicNum)
    {
        ShowHitEffect(who, magicNum);
    }

    [PunRPC]
    void SendIndex(int viewID, int index)
    {
        GameObject[] pl = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < pl.Length; ++i)
        {
            if (pl[i].GetComponent<BattlePlayer>().photonView.IsMine)
            {
                BattleManager.Instance.players[pl[i].GetComponent<BattlePlayer>().myindex] = pl[i].gameObject;
                continue;
            }

            if (pl[i].GetComponent<BattlePlayer>().photonView.ViewID == viewID)
            {
                pl[i].GetComponent<BattlePlayer>().myindex = index;
                BattleManager.Instance.players[index] = pl[i].gameObject;
            }
        }
    }

    [PunRPC]
    void StartGame()
    {
        isStart = true;
        BattleManager.Instance.GameStart();
    }

    [PunRPC]
    void ChangeWeapon(int who, int weaponNum, int magicNum)
    {
        WhatWeapon(who, weaponNum);
        WhatMagicEffect(who, magicNum);
    }
}