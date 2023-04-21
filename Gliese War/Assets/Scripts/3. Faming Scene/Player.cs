using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private CharacterController charactercontroller;

    public Material flashRed;


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

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (weaponNow == 1)
                weaponNow = 2;
            else
                weaponNow = 1;

            RefreshStat();
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

    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Monster"))
        {
            Debug.Log("접촉");
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("클릭");

                collision.transform.GetComponent<NavMeshAgent>().enabled = false;
                collision.transform.GetComponent<Monster>().KnockBack();
                collision.transform.GetComponent<NavMeshAgent>().enabled = true;
            }
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
        
        //StartCoroutine(Damaged());
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
