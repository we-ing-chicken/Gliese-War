using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BattlePlayer : LivingEntity, IPunObservable
{
    static public BattlePlayer instance;
    public int myindex;
    private float Gravity = 9.8f;
    public int life;
    public float MouseX;
    public float mouseSpeed;
    public string id;
    public string nickName;

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
    public int myMagicNum; // my magic num
    
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
    public bool isDbUpdate = false;

    public AudioSource audio;
    [SerializeField] private AudioClip[] attackSounds;
    public AudioClip electricSound;

    public CinemachineVirtualCamera virtualCamera;
    private Coroutine ignoreAttackCor;
    

    private void Start()
    {
        if (isUI)
        {
            return;
        }
        
        if(isWait)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            SHB();
        }

        instance = this;
        if (photonView.IsMine)
        {
            id = GameManager.Instance.id;
            nickName = GameManager.Instance.nickName;
            
            ApplyItems();
            
            if(weapon1 != null)
                weaponNow = 1;
            else if (weapon1 == null)
                weaponNow = 2;
            else if (weapon2 == null)
                weaponNow = 3;
        }
        else
        {
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

        if(weapon1 != null)
            weaponNow = 1;
        else if (weapon1 == null)
        {
            if(weapon2 != null)
                weaponNow = 2;
        }

        magicCooltime = 5f;
        isCool = false;

        audio = GetComponent<AudioSource>();
        
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
        

        if (NetworkManager.Instance.sendOK && photonView.ViewID != 0 && PhotonNetwork.CurrentRoom.Players.Count == 3)
        {
            NetworkManager.Instance.sendOK = false;
            BattleManager.Instance.alivePlayer = PhotonNetwork.CurrentRoom.Players.Count;
            GameManager.Instance.myIndex = myindex;
            GameManager.Instance.isAlive = true;
            photonView.RPC("SendIndex", RpcTarget.All, photonView.ViewID, myindex, id, nickName);
            photonView.RPC("StartGame", RpcTarget.All);
            if(shoe != null)
                photonView.RPC("SendShoeEffectNum", RpcTarget.All, myindex, (int)shoe.item.itemRank);
            
            EquipWeapon();
            
            // 모든 유저 DB 업데이트

            if (weaponNow == 1)
            {
                photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex, (int)GetWeaponNum(), (int)weapon1.magic);
            }
            else if (weaponNow == 2)
            {
                photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex, (int)GetWeaponNum(), (int)weapon2.magic);
            }
            
            WhatMagicEffect(myindex, (int)GetMagic());
            
            if (id != null && !isDbUpdate)
            {
                isDbUpdate = true;
                
                //Career Update
                MySqlConnector.Instance.doNonQuery("update Career set Game = Game + 1 where id = '" + id +"'");
            }
        }

        if (isWait || !isStart) return;
        if (!isalive) return;
        
        //if (charactercontroller == null) return;

        if (photonView.IsMine && GameManager.Instance.isAlive)
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
                if (isAttack) return;
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
                        myMagicNum = 3;
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
                if (isAttack) return;
                
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
                        myMagicNum = 3;
                        break;
                }

                if (weapon2 != null)
                    EquipWeapon();

                RefreshStat();
                WhatMagicEffect(myindex, (int)GetMagic());
                SetMagicImage();
                photonView.RPC("ChangeWeapon", RpcTarget.Others, myindex,(int)weapon2.item.weaponType, (int)weapon2.magic);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                DamageMessage dm;
                dm.damage = 100;
                dm.damager = myindex;
                dm.hitted = myindex;
                ApplyDamage(dm);
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

        if (Input.GetMouseButtonDown(0) && photonView.IsMine)
        {
            if (isAttack) return;
            if (BattleManager.Instance._isInven) return;
            
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
            
            // isAttack = true;
            // AttackStart(myindex);
            // AttackAnimation();
            // StartCoroutine(AttackEffect());
            if(weapon1 != null)
                photonView.RPC("SendAttack", RpcTarget.All, myindex, (int)weapon1.item.weaponType);
            
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
                
                //Look();
                MouseX = MouseX + (Input.GetAxis("Mouse X") * mouseSpeed);
                remoteRot = Quaternion.Euler(0, MouseX, 0);
                transform.rotation = remoteRot;

                if (isAttack) return;

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
                
                if (ismove)
                {
                    if (audio == null) return;
                    if (!audio.isPlaying)
                    {
                        audio.Play();
                    }
                }
            
            }
        }
        else
        {
            //ismove = (remoteDir.x > 0 || remoteDir.z > 0);
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            remoteRot = Quaternion.Euler(0, MouseX, 0);
            transform.rotation = remoteRot;

            if (audio == null) return;
            
            if (moveDir != Vector3.zero)
            {
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
            }
            else
            {
                 audio.Stop();
            }
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

        Transform playerTransform = BattleManager.Instance.players[GameManager.Instance.myIndex].GetComponent<BattlePlayer>().transform;

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
                    audio.PlayOneShot(attackSounds[2], 1f);
                    break;

                case Item.WeaponType.Sword:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(0));
                    audio.PlayOneShot(attackSounds[0], 1f);
                    break;

                case Item.WeaponType.Spear:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(1).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(1));
                    audio.PlayOneShot(attackSounds[1], 1f);
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
                    audio.PlayOneShot(attackSounds[2], 1f);
                    break;

                case Item.WeaponType.Sword:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(0));
                    audio.PlayOneShot(attackSounds[0], 1f);
                    break;

                case Item.WeaponType.Spear:
                    yield return new WaitForSeconds(0.2f);
                    attackEffectPos.transform.GetChild(1).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(1));
                    audio.PlayOneShot(attackSounds[1], 1f);
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
        
        BattleManager.Instance.HPText.GetComponent<TextMeshProUGUI>().text = health + " / " + startingHealth;
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
    void ApplyItems()
    {
        if (GameManager.Instance == null)
        {
            weapon1 = new RealItem();
            weapon1.item = BattleManager.Instance.sword[1];
            weapon1.magic = Magic.Light;

            weapon2 = new RealItem();
            weapon2.item = BattleManager.Instance.spear[1];
            weapon2.magic = Magic.Nothing;

            shoe = new RealItem();
            shoe.item = new Item();
            shoe.item.itemCategory = Item.ItemCategory.Shoes;
            shoe.item.itemRank = Item.ItemRank.Legendary;
        }
        else
        {
            
            helmet = GameManager.Instance.helmet;
            armor = GameManager.Instance.armor;
            shoe = GameManager.Instance.shoe;
            weapon1 = GameManager.Instance.weapon1;
            weapon2 = GameManager.Instance.weapon2;
            
            if (weapon1 != null)
            {
                myMagicNum = (int)weapon1.magic;
            }
            else if (weapon2 != null)
            {
                myMagicNum = (int)weapon2.magic; 
            }
            else
            {
                myMagicNum = (int)Magic.Nothing;
            }
        }

        if (GameManager.Instance != null)
        {
            
        startingHealth = GameManager.Instance.stat.health;
        offensivePower = GameManager.Instance.stat.attackPower;
        defensivePower = GameManager.Instance.stat.defensePower;
        moveSpeed = GameManager.Instance.stat.moveSpeed;
        }
        else
        {
            startingHealth = 100;
            offensivePower = 10;
            defensivePower = 10;
            moveSpeed = 8;
        }

        SetEquipItemImage();
        RefreshStat();
        SetMagicImage();
    }
    
    void SetEquipItemImage()
    {
        if (GameManager.Instance == null) return;
        if (isWait || isUI ) return;
        if (!photonView.IsMine) return;
        
        if(GameManager.Instance.helmet != null) BattleManager.Instance.helmetEquip.GetComponent<Image>().sprite = helmet.item.itemImage;
        if(GameManager.Instance.armor != null) BattleManager.Instance.armorEquip.GetComponent<Image>().sprite = armor.item.itemImage;
        if(GameManager.Instance.shoe != null) BattleManager.Instance.shoeEquip.GetComponent<Image>().sprite = shoe.item.itemImage;

        if (GameManager.Instance.weapon1 != null)
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

        if (GameManager.Instance.weapon2 != null)
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
            if (weapon1 == null) return Item.WeaponType.Nothing;
            return weapon1.item.weaponType;
        }
        else if (weaponNow == 2)
        {
            if (weapon2 == null) return Item.WeaponType.Nothing;
            return weapon2.item.weaponType;
        }

        return Item.WeaponType.Nothing;
    }

    Magic GetMagic()
    {
        if (weaponNow == 1)
        {
            if (weapon1 == null) return Magic.Nothing;
            return weapon1.magic;
        }

        if (weaponNow == 2)
        {
            if (weapon2 == null) return Magic.Nothing;
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

        if (damageMessage.hitted != GameManager.Instance.myIndex) return false;
        
        if (!base.ApplyDamage(damageMessage)) return false;


        if (damageMessage.damage > 0)
        {
            BattleManager.Instance.HitScreen();
            
            if (health < 0)
                health = 0;
        }
        else
        {
            if (health > startingHealth)
                health = startingHealth;
        }

        Inventory.instance.statParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Health : " + health + " / " + startingHealth;
        BattleManager.Instance.HPText.GetComponent<TextMeshProUGUI>().text = health + " / " + startingHealth;
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
        GameManager.Instance.isAlive = false;
        animator.SetBool("isRun", false);
        moveDir = Vector3.zero;
        moveSpeed = 0;
        isAttack = true;
        ismove = false;
    }
    IEnumerator die()
    {
        animator.SetTrigger("dying");
        
        Inventory.instance.statParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Health : " + 0 + " / " + startingHealth;
        BattleManager.Instance.HPText.GetComponent<TextMeshProUGUI>().text = 0 + " / " + startingHealth;
        MyHPBar.Instance.SetHPBar(startingHealth, 0);
        
        //photonView.RPC("MinusLiveCount", RpcTarget.Others);
        photonView.RPC("SendDie", RpcTarget.Others, myindex);
        
        if(GameManager.Instance != null && GameManager.Instance.id != null)
        {
            MySqlConnector.Instance.doNonQuery("update Career set Lose = Lose + 1 where id = '" + id +"'");
            MySqlConnector.Instance.doNonQuery("update Career set Death = Death + 1 where id = '" + id +"'");
        }
        
        yield return new WaitForSeconds(2.0f);
        
        Cursor.visible = true;
        BattleManager.Instance.openLoseCanvas();
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
            BattleManager.Instance.MagneticHitImage.SetActive(false);
            BattleManager.Instance.OffMag();
        }
        
        if (other.CompareTag("Weapon") && !other.transform.GetComponentInParent<BattlePlayer>().photonView.IsMine)
        {
            if (!GameManager.Instance.isAlive) return;
            if (ignoreAttackCor != null) return;
            
            ignoreAttackCor = StartCoroutine(NoDamageTimer());
            
            int damage = other.transform.GetComponentInParent<BattlePlayer>().offensivePower;
            float newDamage = damage - damage * (defensivePower * 0.6f) / 100;
            int newIntDamage = (int)newDamage;

            DamageMessage dm;
            dm.damager = other.transform.GetComponentInParent<BattlePlayer>().myindex;
            dm.damage = newIntDamage;
            dm.hitted = myindex;
            ApplyDamage(dm);

            ShowHitEffect(myindex, other.transform.GetComponentInParent<BattlePlayer>().myMagicNum);
            
            photonView.RPC("SendHit", RpcTarget.Others, myindex, other.transform.GetComponentInParent<BattlePlayer>().myMagicNum);
        }

        
        if (other.CompareTag("Item") && photonView.IsMine)
        {
            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = -20;
            dm.hitted = myindex;
            ApplyDamage(dm);

            //healEffect.SetActive(true);
            //healEffect.GetComponent<ParticleSystem>().Play();
            
            GameObject healEffect;
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = body.transform.position+ new Vector3(0, -2f,  0);

            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Item"))
        {
            //healEffect.SetActive(true);
            //healEffect.GetComponent<ParticleSystem>().Play();
            
            GameObject healEffect;
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = body.transform.position+ new Vector3(0, -2f,  0);

            Destroy(other.gameObject);
        }
    }
    
    IEnumerator NoDamageTimer()
    {
        yield return new WaitForSeconds(0.6f);
        ignoreAttackCor = null;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inside") && photonView.IsMine)
        {
            isSafe = false;
            BattleManager.Instance.MagneticHitImage.SetActive(true);
            BattleManager.Instance.ONMag();

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isSafe)
        {
            if (other.CompareTag("Outside") && photonView.IsMine)
            {
                if (!GameManager.Instance.isAlive) return;
                
                DamageMessage dm;
                dm.damager = myindex;
                dm.damage = 2;
                dm.hitted = myindex;
                ApplyDamage(dm);

                if(!audio.isPlaying)
                    audio.PlayOneShot(electricSound, 0.5f);
                
            }
        }
        
        if (other.CompareTag("Heal") && photonView.IsMine)
        {
            if (!GameManager.Instance.isAlive) return;
            
            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = -1;
            dm.hitted = myindex;
            ApplyDamage(dm);
            
            //if(!healEffect.gameObject.activeSelf)
                //healEffect.SetActive(true);
                
            // if(!healEffect.GetComponent<ParticleSystem>().isPlaying)
            //     healEffect.GetComponent<ParticleSystem>().Play();
            
            GameObject healEffect;    
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = body.transform.position + new Vector3(0, -2f,  0);;
        }
        else if (other.CompareTag("Heal"))
        { 
            //if(!healEffect.gameObject.activeSelf)
                //healEffect.SetActive(true);
            // if(!healEffect.GetComponent<ParticleSystem>().isPlaying)
            //     healEffect.GetComponent<ParticleSystem>().Play();
            
            GameObject healEffect;    
            healEffect = Instantiate(BattleManager.Instance.HealEffect);
            healEffect.transform.position = body.transform.position + new Vector3(0, -2f,  0);
            
        }
    }

    public void ShowHitEffect(int who, int magicNum)
    {
        GameObject hitEffect;
        hitEffect = Instantiate(BattleManager.Instance.HitEffects[magicNum]);
        hitEffect.transform.position = BattleManager.Instance.players[who].transform.position;
    }
    
    public void AttackStart(int who)
    {
        if (weaponNow == 1)
        {
            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer(who);
                    StartCoroutine(TurnOffHandHammer(who));
                    break;
                
                case Item.WeaponType.Sword:
                    TurnOnHandSword(who);
                    StartCoroutine(TurnOffHandSword(who));
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear(who);
                    StartCoroutine(TurnOffHandSpear(who));
                    break;
            }
        }
        else if (weaponNow == 2)
        {

            switch (weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    TurnOnHandHammer(who);
                    StartCoroutine(TurnOffHandHammer(who));
                    break;
                
                case Item.WeaponType.Sword:
                    TurnOnHandSword(who);
                    StartCoroutine(TurnOffHandSword(who));
                    break;
                
                case Item.WeaponType.Spear:
                    TurnOnHandSpear(who);
                    StartCoroutine(TurnOffHandSpear(who));
                    break;
            }
        }
    }

    public void TurnOnHandHammer(int who)
    {
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(0).gameObject.SetActive(true);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0).gameObject.SetActive(false);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSpear(int who)
    {
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(1).gameObject.SetActive(true);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1).gameObject.SetActive(false);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = true;
    }
    
    public void TurnOnHandSword(int who)
    {
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(2).gameObject.SetActive(true);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2).gameObject.SetActive(false);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = true;
    }

    IEnumerator TurnOffHandHammer(int who)
    {
        yield return new WaitForSeconds(0.9f);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(0).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(0).gameObject.SetActive(false);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(0).gameObject.SetActive(true);
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isAttack = false;
    }
    
    IEnumerator TurnOffHandSpear(int who)
    {
        yield return new WaitForSeconds(0.6f);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(1).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(1).gameObject.SetActive(false);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(1).gameObject.SetActive(true);
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isAttack = false;
    }
    
    IEnumerator TurnOffHandSword(int who)
    {
        yield return new WaitForSeconds(1f);
        
        col = BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().handR.transform.GetChild(2).gameObject.SetActive(false);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().back.transform.GetChild(2).gameObject.SetActive(true);
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isAttack = false;
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
            dm.hitted = myindex;
            ApplyDamage(dm);

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
            dm.hitted = myindex;
            ApplyDamage(dm);

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
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon1 = new RealItem();
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon1.item = new Item();
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon1.item.weaponType = weapon;
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon2 = new RealItem();
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().weapon2.item = new Item();
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
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().myMagicNum = 0;
                break;

            case (int)Magic.Water:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().myMagicNum = 1;
                break;

            case (int)Magic.Light:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(true);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().myMagicNum = 2;
                break;

            default:
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(0)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(1)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().whatMagicPos.transform.GetChild(2)
                    .gameObject.SetActive(false);
                BattleManager.Instance.players[who].GetComponent<BattlePlayer>().myMagicNum = 3;
                break;
        }
        
    }

    [PunRPC]
    void SendHit(int who, int magicNum)
    {
        ShowHitEffect(who, magicNum);
    }

    [PunRPC]
    void SendIndex(int viewID, int index, string id, string nick)
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
                BattleManager.Instance.players[index].GetComponent<BattlePlayer>().id = id;
                BattleManager.Instance.players[index].GetComponent<BattlePlayer>().nickName = nick;
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

    [PunRPC]
    void MinusLiveCount()
    {
        --BattleManager.Instance.alivePlayer;

        if (BattleManager.Instance.alivePlayer == 1 && isalive)
        {
            BattleManager.Instance.openWinCanvas();
            //이긴 사람 DB 승리 추가
        }
    }

    [PunRPC]
    void SendDie(int who)
    {
        if (BattleManager.Instance.players[who] == null) return;

        if (BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isalive)
            BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isalive = false;

        int aliveCount = 0;
        for (int i = 0; i < BattleManager.Instance.players.Length; ++i)
        {
            if (BattleManager.Instance.players[i] == null) continue;

            if (BattleManager.Instance.players[i].GetComponent<BattlePlayer>().isalive) ++aliveCount;
        }

        if (aliveCount == 1 && GameManager.Instance.isAlive)
        {

            if(GameManager.Instance != null && GameManager.Instance.id != null)
            {
                MySqlConnector.Instance.doNonQuery("update Career set Win = Win + 1 where id = '" + GameManager.Instance.id +"'");
            }
            
            BattleManager.Instance.openWinCanvas();
        }
    }

    [PunRPC]
    void SendShoeEffectNum(int who, int num)
    {
        if (BattleManager.Instance.players[who] == null) return;
        
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().shoesEffectPos.transform.GetChild(num).gameObject.SetActive(true);
    }

    [PunRPC]
    void SendAttack(int who, int weaponNum)
    {
        if (BattleManager.Instance.players[who] == null) return;
    
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().isAttack = true;
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().AttackStart(who);
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().AttackAnimation();
        BattleManager.Instance.players[who].GetComponent<BattlePlayer>().StartCoroutine(AttackEffect());
    }

    void SHB()
    {
        int i = 0;
        while (i < BattleManager.Instance.hamburgers)
        {
            int x = Random.Range(125, 275);
            int z = Random.Range(125, 275);
            float y = BattleManager.Instance.GetTerrainY(x, z);
            if (y > 15.0f)
            {
                continue;
            }
            else
            {
                Vector3 xyz = new Vector3(x, 15.0f, z);
                photonView.RPC("SendHamburger", RpcTarget.All, xyz);
                ++i;
            }
        }
    }

    [PunRPC]
    void SendHamburger(Vector3 xyz)
    {

        Instantiate(BattleManager.Instance.hamburger, xyz, Quaternion.identity);


    }

    public string GetMyId()
    {
        return id;
    }
}