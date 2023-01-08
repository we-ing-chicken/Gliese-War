using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // �յ� �������� �ӵ�
    public int life;

    private string moveFBAxisName = "Vertical"; // �յ� �������� ���� �Է��� �̸�
    private string moveLRAxisName = "Horizontal"; // �¿� �������� ���� �Է��� �̸�
    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�
    private string JumpButtonName = "Jump";

    private Rigidbody playerRigidbody; // �÷��̾� ĳ������ ������ٵ�

    
    public float moveFB { get; private set; } // ������ ������ �Է°�
    public float moveLR { get; private set; } // ������ ȸ�� �Է°�
    public bool Mlattack { get; private set; } // ������ �߻� �Է°�
    public bool Mgattack { get; private set; } // ������ �߻� �Է°�
    public bool p_Jump { get; private set; } // ������ �߻� �Է°�

    private bool isJump;

    public float JumpPower;



    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // ���ӿ��� ���¿����� ����� �Է��� �������� �ʴ´�
        if (GameManager.Instance != null && GameManager.Instance.isGameover)
        {
            moveFB = 0;
            moveLR = 0;
            Mlattack = false;
            return;
        }

        // move�� ���� �Է� ����
        moveFB = Input.GetAxis(moveFBAxisName);
        // rotate�� ���� �Է� ����
        moveLR = Input.GetAxis(moveLRAxisName);
        // fire�� ���� �Է� ����
        Mlattack = Input.GetButton(meleeAttackButtonName);
        Mgattack = Input.GetButton(magicAttackButtonName);
        p_Jump = Input.GetButton(JumpButtonName);


    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Move()
    {

        Vector3 moveDistance = new Vector3(moveLR, 0, moveFB).normalized * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);


    }
    private void Jump()
    {
        if(p_Jump && !isJump)
        {
            playerRigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
}
