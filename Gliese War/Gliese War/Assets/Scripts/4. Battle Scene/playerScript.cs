using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.ProBuilder.Shapes;
using System.Collections;

public class playerScript : MonoBehaviourPunCallbacks//, IPunObservable
{
    public float MouseX;
    public float mouseSpeed;

    private PhotonView PV;
    private bool isJump;
    private new Rigidbody rigidbody;
    public Transform LeftTarget;
    public Transform RightTarget;
    public Transform ForwardTarget;
    public Transform BackWardTarget;
    public Transform LFTarget;
    public Transform RFTarget;
    public Transform LBTarget;
    public Transform RBTarget;

    public Transform playertransform;

    public Animator anim;

    public int life;
    public int offensivePower;
    public int defensivePower;
    public int maxHealth;
    public int currHealth;

    public List<Item> items;

    public RealItem helmet;
    public RealItem armor;
    public RealItem shoe;
    public RealItem weapon1;
    public RealItem weapon2;
    public int weaponNow = 1;

    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�

    public GameObject handR;
    public GameObject back;

    public GameObject attackEffectPos;
    [SerializeField] private GameObject[] attackEffect;

    public GameObject shoesEffectPos;

    public bool isAttack = false;

    private float moveLR;
    private float moveFB;
    private bool ismove;

    public float move_speed;
    public float jump_force;
    private string JumpButtonName = "Jump";

    private void Start()
    {

        rigidbody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        //TODO : 캐릭터 생성 시 추가되도록.
        //anim = GetComponent<Animator>();

        offensivePower = 10;
        defensivePower = 10;
        maxHealth = 100;
        currHealth = 100;
        move_speed = 8;

        RefreshStat();

    }

    private void Update()
    {
        if (PV.IsMine)
        {
            moveLR = Input.GetAxisRaw("Horizontal");
            moveFB = Input.GetAxisRaw("Vertical");
            ismove = (Input.GetButton("Horizontal") || Input.GetButton("Vertical"));


            anim.SetBool("isRun", ismove);

            Look();
            player_lookTarget();

            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * move_speed,
                                            0,
                                            Input.GetAxisRaw("Vertical") * Time.deltaTime * move_speed));
            if (Input.GetButtonDown(JumpButtonName))
            {
                isJump = true;
                Jump();
                
            }
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
            "Speed : " + move_speed;
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
        if (!isAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Attack!");
                AttackAnimation();
                StartCoroutine(AttackEffect());
            }
        }

        FarmingManager.Instance.SetEquipWeaponImage();
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
                    anim.SetTrigger("attackHammer");
                    break;

                case Item.WeaponType.Knife:
                    anim.SetTrigger("attackSword");
                    break;

                case Item.WeaponType.Spear:
                    anim.SetTrigger("attackSpear");
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
                    anim.SetTrigger("attackHammer");
                    break;

                case Item.WeaponType.Knife:
                    anim.SetTrigger("attackSword");
                    break;

                case Item.WeaponType.Spear:
                    anim.SetTrigger("attackSpear");
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

    /*public void UnwearHelmet()
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
    }*/

    private void player_lookTarget()
    {

        if (moveLR < 0 && moveFB < 0)    // left + back
        {
            player_Rotate(LBTarget.position);
        }
        else if (moveLR < 0 && moveFB > 0)    // left + forward
        {
            player_Rotate(LFTarget.position);

        }
        else if (moveLR > 0 && moveFB < 0)    // right + back
        {
            player_Rotate(RBTarget.position);

        }
        else if (moveLR > 0 && moveFB > 0)    // right + forward
        {
            player_Rotate(RFTarget.position);

        }
        else if (moveLR < 0)
        {
            player_Rotate(LeftTarget.position);
        }
        else if (moveLR > 0)
        {
            player_Rotate(RightTarget.position);

        }
        else if (moveFB < 0)
        {
            player_Rotate(BackWardTarget.position);

        }
        else if (moveFB > 0)
        {
            player_Rotate(ForwardTarget.position);
        }
    }
    private void player_Rotate(Vector3 movePoint)
    {
        Vector3 relativePosition = movePoint - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        playertransform.rotation = Quaternion.Lerp(playertransform.rotation, rotation, Time.deltaTime * 10);
    }

    private void Look()
    {
        MouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        transform.rotation = Quaternion.Euler(0, MouseX, 0);
    }
    ////RPC �Լ�
    //[PunRPC]
    void Jump()
    {
        if (!isJump) return;
        isJump = false;

        anim.SetTrigger("doJump");
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        
    }

    public bool isMine()
    {
        bool isMine = PV.IsMine;
        return isMine;
    }
}