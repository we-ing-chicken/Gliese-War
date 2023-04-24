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


    public List<Item> items;

    public RealItem helmet;
    public RealItem armor;
    public RealItem shoe;
    public RealItem weapon1;
    public RealItem weapon2;
    public int weaponNow = 1;

    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject footL;
    [SerializeField] private GameObject footR;
    public GameObject handR;
    public GameObject back;
    
    [SerializeField] private GameObject attackEffectPos;
    [SerializeField] private GameObject[] attackEffect;

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

    private Vector3 moveDirection;

    private void Start()
    {
        instance = this;
        charactercontroller = GetComponent<CharacterController>();
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

        // fire�� ���� �Է� ����
        Mlattack = Input.GetButton(meleeAttackButtonName);
        Mgattack = Input.GetButton(magicAttackButtonName);
        p_Jump = Input.GetButton(JumpButtonName);
        animate();
        Test();

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

        if (!FarmingManager.Instance._isInven && !isAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttackAnimation();
                AttackEffect();
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (charactercontroller == null) return;
        Look();

        if (charactercontroller.isGrounded)
        {
            Move();
            if (p_Jump) Jump();
        }
        else
        {
            Fall();
        }

        charactercontroller.Move(moveDir * Time.deltaTime);

    }

    private void Move()
    {
        player_lookTarget();


        moveDir = charactercontroller.transform.TransformDirection(new Vector3(moveLR, 0, moveFB)) * moveSpeed;
        moveDirection = transform.right * moveDir.x + transform.forward * moveDir.z;

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
        transform.rotation = Quaternion.Euler(0, MouseX, 0);
    }
    private void player_lookTarget()
    {
        if(charactercontroller == null) return;
        Vector3 target = new Vector3();

        if(moveLR < 0 && moveFB < 0)    // left + back
        {
            Debug.Log("Left-Back");
            player_Rotate(lbTarget.position);
        }
        else if (moveLR < 0 && moveFB > 0)    // left + forward
        {
            Debug.Log("Left-Foward");
            player_Rotate(lfTarget.position);

        }
        else if (moveLR > 0 && moveFB < 0)    // right + back
        {
            Debug.Log("Right-Back");
            player_Rotate(rbTarget.position);

        }
        else if (moveLR > 0 && moveFB > 0)    // right + forward
        {
            Debug.Log("Right-Foward");
            player_Rotate(rfTarget.position);

        }
        else if (moveLR < 0)
        {
            Debug.Log("Left");
            player_Rotate(leftTarget.position);
        }
        else if (moveLR > 0)
        {
            Debug.Log("Right");
            player_Rotate(rightTarget.position);

        }
        else if (moveFB < 0)
        {
            Debug.Log("Back");
            player_Rotate(backwardTarget.position);

        }
        else if (moveFB > 0)
        {
            Debug.Log("Foward");
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
        animator.SetBool("isRun", moveDirection != Vector3.zero);
        
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

    private void AttackEffect()
    {
        if (weaponNow == 1)
        {
            if (weapon1 == null)
                return;
            
            switch (weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    //Instantiate(attackEffect[0], attackEffectPos.transform);
                    break;
                
                case Item.WeaponType.Knife:
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(QuitAttackEffect(0));
                    
                    break;
                
                case Item.WeaponType.Spear:
                    
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
                    //Instantiate(attackEffect[0], attackEffectPos.transform);
                    break;
                
                case Item.WeaponType.Knife:
                    attackEffectPos.transform.GetChild(0).gameObject.SetActive(true);
                    QuitAttackEffect(0);
                    
                    break;
                
                case Item.WeaponType.Spear:
                    
                    break;
            }
        }
    }

    IEnumerator QuitAttackEffect(int pos)
    {
        yield return new WaitForSeconds(0.5f);
        attackEffectPos.transform.GetChild(pos).gameObject.SetActive(false);
        yield return null;
    }

    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");
            isAttack = false;
           
            animator.SetTrigger("doJump");

        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            StartCoroutine(StartRevive());
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    Debug.Log("Run-down");
        //    moveSpeed *= 2.0f;

        //    animator.SetBool("isRun", true);

        //}
        //if (Input.GetKeyUp(KeyCode.LeftShift))
        //{
        //    Debug.Log("Run-up");
        //    moveSpeed /= 2.0f;

        //    animator.SetBool("isRun", false);

        //}
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
    }

    private void RefreshStat()
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
    

    private void OnCollisionStay(Collision collision)
    {
        // if (collision.transform.CompareTag("Monster"))
        // {
        //     Debug.Log("접촉");
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         Debug.Log("클릭");
        //
        //         collision.transform.GetComponent<NavMeshAgent>().enabled = false;
        //         collision.transform.GetComponent<Monster>().KnockBack();
        //         collision.transform.GetComponent<NavMeshAgent>().enabled = true;
        //     }
        // }
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
        
        //StartCoroutine(Damaged());
    }

    public int GetAttackPower()
    {
        return (offensivePower + GetWeaponStat());
    }

    IEnumerator StartRevive()
    {
        yield return new WaitForSeconds(1f);

        currHealth = maxHealth;
        FarmingManager.Instance.playerCurrentHPBar.value = (float)currHealth / maxHealth;
        
        charactercontroller.enabled = false;
        FarmingManager.Instance.StartFadeOut();
        instance.transform.position = FarmingManager.Instance.startPostion.transform.position;
        charactercontroller.enabled = true;
    }
}
