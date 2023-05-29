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

public class Player : MonoBehaviour
{
    static public Player instance;
    private float Gravity = 9.8f;
    public int life;
    public float MouseX;
    public float mouseSpeed;
    public bool isUI = false;
    public bool isFarming = true;

    private string moveFBAxisName = "Vertical"; // �յ� �������� ���� �Է��� �̸�
    private string moveLRAxisName = "Horizontal"; // �¿� �������� ���� �Է��� �̸�
    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�
    private string JumpButtonName = "Jump";

    public int offensivePower;
    public int defensivePower;
    public int maxHealth;
    public int currHealth;
    public int moveSpeed; // �յ� �������� �ӵ�
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
    
    [SerializeField] private GameObject attackEffectPos;
    [SerializeField] private GameObject[] attackEffect;
    
    [SerializeField] private GameObject shoesEffectPos;

    public bool isAttack = false;

    private CharacterController charactercontroller;

    public float moveFB { get; private set; } // ������ �����̵� �Է°�
    public float moveLR { get; private set; } // ������ �¿��̵� �Է°�
    public float rot { get; private set; } // ������ ȸ�� �Է°�
    public bool Mlattack { get; private set; } // ������ �߻�1 �Է°�
    public bool Mgattack { get; private set; } // ������ �߻�2 �Է°�
    public bool p_Jump { get; private set; } // ������ ���� �Է°�

    public float JumpPower;

    public Vector3 moveDir;

    public bool isNear;

    private void Start()
    {
        instance = this;
        charactercontroller = GetComponent<CharacterController>();
        //if (ignoreGravity)
        //    charactercontroller.
        moveDir = Vector3.zero;
        rot = 1.0f;
        isNear = false;
        life = 10;

        offensivePower = 10;
        defensivePower = 10;
        maxHealth = 100;
        currHealth = 100;
        moveSpeed = 8;

        //flashRed = GetComponent<MeshRenderer>().material;

        RefreshStat();
    }

    private void Update()
    {
        // ���ӿ��� ���¿����� ����� �Է��� �������� �ʴ´�
        //if (FarmingManager.Instance != null && FarmingManager._isEnd)
        //{
        //    moveFB = 0;
        //    moveLR = 0;
        //    Mlattack = false;
        //    return;
        //}

        if (charactercontroller == null) return;

        // move�� ���� �Է� ����
        moveFB = Input.GetAxis(moveFBAxisName);

        // rotate�� ���� �Է� ����
        moveLR = Input.GetAxis(moveLRAxisName);

        ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));

        // fire�� ���� �Է� ����
        Mlattack = Input.GetButton(meleeAttackButtonName);
        Mgattack = Input.GetButton(magicAttackButtonName);
        p_Jump = Input.GetButton(JumpButtonName);
        animate();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponNow = 1;
            
            if(weapon1 != null)
                EquipWeapon();
            
            RefreshStat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponNow = 2;
            if(weapon2 != null)
                EquipWeapon();
            
            RefreshStat();
        }

        if (isFarming)
        {
            if (!FarmingManager.Instance._isInven && !isAttack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    AttackAnimation();
                    StartCoroutine(AttackEffect());
                }
            }
        }
        else
        {
            if (!isAttack)
            {
                //if (Input.GetMouseButtonDown(0))
                //{
                //    AttackAnimation();
                //    StartCoroutine(AttackEffect());
                //}
            }
        }
        
    }
    
    private void FixedUpdate()
    {
        if(isFarming)
            if (FarmingManager.Instance._isFading) return;
        //else
            //if (BattleManager.Instance._isFading) return;
        if (charactercontroller == null) return;
        Look();

        if (!charactercontroller.isGrounded)
        {
            if(!ignoreGravity)
                Fall();
        }
        else
        {
            Move();
            if (p_Jump)
            {
                isAttack = false;
                if (!isUI)
                {
                    animator.SetTrigger("doJump");
                    Jump();
                }

            }
        }

        charactercontroller.Move(moveDir * Time.deltaTime);
        //tranform.position 전송
    }

    private void Move()
    {
        player_lookTarget();


        moveDir = charactercontroller.transform.TransformDirection(new Vector3(moveLR, 0, moveFB)) * moveSpeed;
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
        MouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        if(!isUI)
            transform.rotation = Quaternion.Euler(0, MouseX, 0);
    }
    private void player_lookTarget()
    {
        if(charactercontroller == null) return;

        if(moveLR < 0 && moveFB < 0)    // left + back
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

    private void animate()
    {
        if(!isUI)
            animator.SetBool("isRun", ismove);
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
                
                case Item.WeaponType.Knife:
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
                
                case Item.WeaponType.Knife:
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
                
                case Item.WeaponType.Knife:
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
                
                case Item.WeaponType.Knife:
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
        if (isFarming)
        {
            Inventory.instance.statParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                "Health : " + currHealth + " / " + maxHealth;
            Inventory.instance.statParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                "Attack : " + (offensivePower + GetWeaponStat());
            Inventory.instance.statParent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                "Defense : " + defensivePower;
            Inventory.instance.statParent.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                "Speed : " + moveSpeed;
        }
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
        temp.transform.localEulerAngles = new Vector3(0f,-90f,180f);
        temp.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
    }

    public void WearArmor()
    {
        if (body.transform.childCount == 1)
            Destroy(body.transform.GetChild(0).gameObject);

        GameObject temp = Instantiate(Inventory.instance.armor[0].itemPrefab, body.transform);
        //temp.transform.position = new Vector3(0f, 0.15f, 0f);
        temp.transform.localEulerAngles = new Vector3(0,-90f,180f);
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
        temp.transform.localEulerAngles = new Vector3(0,90f,180f);
        temp.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        
        GameObject temp2 = Instantiate(Inventory.instance.shoes[0].itemPrefab, footR.transform);
        //temp2.transform.position += new Vector3(0.05f, 0f, 0.05f);
        temp2.transform.localEulerAngles = new Vector3(0,90f,180f);
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
                
                case Item.WeaponType.Knife:
                    EquipKnife();
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
                
                case Item.WeaponType.Knife:
                    EquipKnife();
                    break;
                
                case Item.WeaponType.Spear:
                    EquipSpear();
                    break;
            }
        }

        FarmingManager.Instance.SetEquipWeaponImage();
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
    
    private void EquipKnife()
    {
        back.transform.GetChild(2).gameObject.SetActive(true);
        back.transform.GetChild(0).gameObject.SetActive(false);
        back.transform.GetChild(1).gameObject.SetActive(false);
    }
    
    public void UnwearHelmet()
    {
        Destroy(head.transform.GetChild(1).gameObject);
    }
    
    public void UnwearArmor()
    {
        Destroy(body.transform.GetChild(1).gameObject);
    }
    
    public void UnwearShoes()
    {
        Destroy(footL.transform.GetChild(1).gameObject);
        Destroy(footR.transform.GetChild(1).gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MonsterAttack"))
        {
            GetDamage(5);
            FarmingManager.Instance.HitScreen();
        }
    }

    public void GetDamage(int damage)
    {
        currHealth -= damage;
        RefreshStat();
        FarmingManager.Instance.playerCurrentHPBar.value = (float)currHealth / maxHealth;

        if (currHealth <= 0)
        {
            animator.SetTrigger("doDie");
            StartCoroutine(StartRevive());
        }

    }

    public int GetAttackPower()
    {
        return (offensivePower + GetWeaponStat());
    }

    IEnumerator StartRevive()
    {
        FarmingManager.Instance.StartFadeOut();
        yield return new WaitForSeconds(2f);

        currHealth = maxHealth;
        FarmingManager.Instance.playerCurrentHPBar.value = (float)currHealth / maxHealth;
        
        charactercontroller.enabled = false;
        yield return new WaitForSeconds(0.1f);
        instance.transform.position = FarmingManager.Instance.startPostion.transform.position;
        yield return new WaitForSeconds(0.1f);
        charactercontroller.enabled = true;
        yield return null;
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
}
