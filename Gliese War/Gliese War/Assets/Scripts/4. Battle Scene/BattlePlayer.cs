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
    public int maxHealth;
    public int currHealth;
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

    private bool isMagic;
    [SerializeField] private GameObject magicAreaPrefab;
    private Coroutine magicCor;
    [SerializeField] private GameObject[] magicEffect;
    private float magicCooltime;
    private bool isCool;
    private int magicNum = 0;

    public float moveFB { get; private set; } //             ̵   Է° 
    public float moveLR { get; private set; } //         ¿  ̵   Է° 
    public float rot { get; private set; } //        ȸ    Է° 
    public bool Mlattack { get; private set; } //         ߻ 1  Է° 
    public bool Mgattack { get; private set; } //         ߻ 2  Է° 
    public bool p_Jump { get; private set; } //              Է° 

    public float JumpPower;

    private Vector3 moveDir;

    public bool isNear;

    private Vector3 remoteDir;
    private Quaternion remoteRot;

    private bool isSafe = false;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    Vector3 _networkPosition;
    Quaternion _networkRotation;
    Rigidbody _rb;

    private void Start()
    {
        Debug.Log(photonView.IsMine);

        instance = this;
        if (photonView.IsMine)
            applyItems();
        charactercontroller = GetComponent<CharacterController>();
        moveDir = Vector3.zero;
        rot = 1.0f;
        isNear = false;
        life = 10;

        offensivePower = GameManager.Instance.stat.attackPower;
        defensivePower = GameManager.Instance.stat.defensePower;
        maxHealth = GameManager.Instance.stat.health;
        currHealth = maxHealth;
        moveSpeed = GameManager.Instance.stat.moveSpeed;

        SetBattleItemEquip();

        RefreshStat();

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
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.GetComponent<LivingEntity>().dead) return;

        //if (charactercontroller == null) return;

        if (photonView.IsMine)
        {
            moveFB = Input.GetAxis(moveFBAxisName);
            moveLR = Input.GetAxis(moveLRAxisName);
            Debug.Log("1moveLR : " + moveLR + ", moveFB : " + moveFB);

            ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));
            Mlattack = Input.GetButton(meleeAttackButtonName);
            Mgattack = Input.GetButton(magicAttackButtonName);
            p_Jump = Input.GetButton(JumpButtonName);

        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponNow = 1;

            if (weapon1 != null)
                EquipWeapon();

            RefreshStat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponNow = 2;
            if (weapon2 != null)
                EquipWeapon();

            RefreshStat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weapon1.item = TestManager.Instance.knife[1];
            EquipWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weapon1.item = TestManager.Instance.spear[1];
            EquipWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            weapon1.item = TestManager.Instance.hammer[1];
            EquipWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            magicNum = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            magicNum = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            magicNum = 2;
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (photonView.IsMine)
            {
                if (isMagic)
                {
                    GameObject magic = Instantiate(magicEffect[magicNum]); //1 Tornado , 2 Thunder  0 Fire
                    magic.transform.position = magicAreaPrefab.transform.position;
                    StopCoroutine(magicCor);
                    animator.SetTrigger("magicAttack");
                    magicAreaPrefab.SetActive(false);
                    isMagic = false;

                    isCool = true;
                    StartCoroutine(CheckCoolTime());

                    return;
                }
                Debug.Log(myindex);
                AttackStart();
                AttackAnimation();
                StartCoroutine(AttackEffect());
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
            magicAreaPrefab.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isMagic = false;
            StopCoroutine(magicCor);
            magicAreaPrefab.SetActive(false);
        }

        //if (charactercontroller == null) return;


        //if (!charactercontroller.isGrounded)
        //{
        //    if (!ignoreGravity)
        //        Fall();
        //}
        //else
        //{
        //if (photonView.IsMine)
        //{
        //    remoteDir = new Vector3(moveLR, 0, moveFB);
        //    Move();
        //    Look();
        //    Debug.Log("IsMine : " + photonView.IsMine + ", remoteRot : " + remoteRot);

        //    transform.rotation = remoteRot;

        //}
        //else
        //{
        //    Move();
        //    Look();
        //    Debug.Log("IsMine : " + photonView.IsMine + ", remoteRot : " + remoteRot);

        //    transform.rotation = remoteRot;
        //}

        //if (p_Jump)
        //{
        //    isAttack = false;
        //    animator.SetTrigger("doJump");
        //    Jump();

        //}
        //}

        //charactercontroller.Move(moveDir * Time.deltaTime * moveSpeed);

        animate_Run();
    }

    public void FixedUpdate()
    {
        if (photonView.IsMine)
        {

            remoteDir = new Vector3(moveLR, 0, moveFB);
            Debug.Log("2moveLR : " + moveLR + ", moveFB : " + moveFB);
            //Debug.Log("IsMine : " + photonView.IsMine + ", remoteDir : " + remoteDir);

            Move();
            _rb.position = Vector3.MoveTowards(_rb.position, _rb.position + remoteDir, Time.fixedDeltaTime * moveSpeed);

            Look();
            //Debug.Log("IsMine : " + photonView.IsMine + ", remoteRot : " + remoteRot);

            _rb.rotation = Quaternion.Euler(0, MouseX, 0);


        }
        else
        {
            _rb.position = Vector3.MoveTowards(_rb.position, _networkPosition, Time.fixedDeltaTime);
            _rb.rotation = Quaternion.Euler(_networkRotation.x, _networkRotation.y, _networkRotation.z);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rb.position);
            stream.SendNext(_rb.rotation);
            stream.SendNext(_rb.velocity);
        }
        else
        {
            _networkPosition = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
            _rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTimestamp));
            _networkPosition += (_rb.velocity * lag);
        }
    }

    IEnumerator SetMagicArea()
    {
        Debug.Log("On");
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        RaycastHit[] hit;

        while (true)
        {
            Vector3 direction = ((transform.position + new Vector3(0f, 1f, 0f)) - Camera.main.transform.position).normalized;
            hit = (Physics.RaycastAll(Camera.main.transform.position, direction, distance + 10f));

            for (int i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform.CompareTag("Terrain"))
                {
                    magicAreaPrefab.transform.position = hit[i].point;
                    break;
                }
            }

            yield return null;
        }
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
        player_lookTarget();

        //moveDir = charactercontroller.transform.TransformDirection(remoteDir);
    }

    private void Jump()
    {
        moveDir.y = JumpPower;
    }
    private void Fall()
    {
        moveDir.y -= Gravity * Time.deltaTime;
    }
    private void Look()
    {
        if (photonView.IsMine)
        {
            MouseX = MouseX + (Input.GetAxis("Mouse X") * mouseSpeed);
        }

        remoteRot = Quaternion.Euler(0, MouseX, 0);
    }
    private void player_lookTarget()
    {
        if (charactercontroller == null) return;

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
        maxHealth -= realItem.stat.health;
        currHealth -= realItem.stat.health;
        moveSpeed -= realItem.stat.moveSpeed;
        RefreshStat();
    }

    public void Equip(RealItem realItem)
    {
        //offensivePower += realItem.stat.attackPower;
        defensivePower += realItem.stat.defensePower;
        maxHealth += realItem.stat.health;
        currHealth += realItem.stat.health;
        moveSpeed += realItem.stat.moveSpeed;
        RefreshStat();
        SetShoesEffect();
    }

    private void RefreshStat()
    {
        Inventory.instance.statParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Health : " + currHealth + " / " + maxHealth;
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
        if (weapon1 == null)
        {
            weapon1 = new RealItem();
            weapon1.item = BattleManager.Instance.sword[1];
            weapon1.magic = Magic.Fire;
        }

        if (weapon2 == null)
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

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;

        return true;
    }

    public override void Die()
    {
        base.Die();

        StartCoroutine("die");

        BattleManager.Instance.BM_RemoveList(myindex);
    }
    IEnumerator die()
    {
        animator.SetTrigger("Dying");
        yield return new WaitForSeconds(0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inside"))
        {
            isSafe = true;
        }
        
        if (other.CompareTag("Weapon"))
        {
            BattleManager.Instance.HitScreen();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inside"))
        {
            isSafe = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isSafe)
        {
            if (other.CompareTag("Outside"))
            {
                Debug.Log("밖");
            }
        }
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
                    StartCoroutine(TurnOffHandKnife());
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
                    StartCoroutine(TurnOffHandKnife());
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

    IEnumerator TurnOffHandKnife()
    {
        yield return new WaitForSeconds(0.9f);

        col = handR.transform.GetChild(2).GetComponent<MeshCollider>();
        col.enabled = false;

        handR.transform.GetChild(2).gameObject.SetActive(false);
        back.transform.GetChild(2).gameObject.SetActive(true);

        isAttack = false;
    }
}