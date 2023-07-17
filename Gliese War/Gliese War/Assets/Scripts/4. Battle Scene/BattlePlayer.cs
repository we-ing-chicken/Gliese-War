using Cinemachine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

    public bool isAttack = false;

    private CharacterController charactercontroller;
    
    private bool isMagic;   // while right mouse down
    private bool isCool;    // after use magic
    private Coroutine magicCor; // set magic area position coroutine 
    [SerializeField] private GameObject[] magicEffect;
    private Vector3 magicPosition;
    private int myMagicNum = 0; // my magic num
    
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

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
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

        RefreshStat();

        myMagicNum = 1;
        magicCooltime = 5f;
        isCool = false;

        EquipWeapon();

        if (photonView.IsMine)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            CServercamTest sct = Camera.main.GetComponent<CServercamTest>();

            sct.bp = GetComponent<BattlePlayer>();
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;

        }
    }

    private void Update()
    {
        if (transform.GetComponent<LivingEntity>().dead) return;

        //if (charactercontroller == null) return;

        if (photonView.IsMine)
        {
            moveFB = Input.GetAxis(moveFBAxisName);
            moveLR = Input.GetAxis(moveLRAxisName);

            ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));
            Mgattack = Input.GetButton(magicAttackButtonName);
            p_Jump = Input.GetButton(JumpButtonName);

        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weapon1 == null) return;
            
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
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (weapon2 == null) return;
            
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
            isMagic = false;
            StopCoroutine(magicCor);
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
            //remoteDir = new Vector3(moveLR, 0, moveFB).normalized;
            //player_lookTarget();

            

            //Move();
            Vector3 lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
            Vector3 lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;
            moveDir = lookForward * moveFB + lookRight * moveLR;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            playertransform.LookAt(playertransform.position + moveDir);

            //Look();
            MouseX = MouseX + (Input.GetAxis("Mouse X") * mouseSpeed);
            remoteRot = Quaternion.Euler(0, MouseX, 0);
            transform.rotation = remoteRot;

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
        animate_Run();
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

    IEnumerator CheckCoolTime()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= magicCooltime)
            {
                isCool = false;
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

    private void animate_Run()
    {
            animator.SetBool("isRun", ismove);
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
                    yield return new WaitForSeconds(0.3f);
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
                    yield return new WaitForSeconds(0.3f);
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
            weapon2.item = BattleManager.Instance.sword[1];
            weapon2.magic = Magic.Water;
        }
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
        MyHPBar.Instance.SetHPBar(startingHealth, health);
        return true;
    }

    public override void Die()
    {
        base.Die();

        StartCoroutine("die");

        BattleManager.Instance.BM_RemoveList(myindex);
        SceneManager.LoadScene(1);
    }
    IEnumerator die()
    {
        animator.SetTrigger("Dying");
        yield return new WaitForSeconds(0.3f);
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
            //MyHPBar.Instance.SetHPBar(startingHealth, health);
            BattleManager.Instance.HitScreen();
        }

        if (other.CompareTag("Item"))
        {
            DamageMessage dm;
            dm.damager = myindex;
            dm.damage = -20;
            ApplyDamage(dm);
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
    }
    
    public void GetDamage(int val)
    {
        health -= val;
        MyHPBar.Instance.SetHPBar(startingHealth, health);
        BattleManager.Instance.HitScreen();
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
                    TurnOnHandSword();
                    StartCoroutine(TurnOffHandSword());
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
        yield return new WaitForSeconds(0.5f);
        
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
        yield return new WaitForSeconds(0.9f);
        
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
}