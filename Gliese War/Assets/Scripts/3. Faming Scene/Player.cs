using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // �յ� �������� �ӵ�
    public int life;
    public float MouseX;

    private string moveFBAxisName = "Vertical"; // �յ� �������� ���� �Է��� �̸�
    private string moveLRAxisName = "Horizontal"; // �¿� �������� ���� �Է��� �̸�
    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�
    private string JumpButtonName = "Jump";

    private CharacterController charactercontroller;

    
    public float moveFB { get; private set; } // ������ �����̵� �Է°�
    public float moveLR { get; private set; } // ������ �¿��̵� �Է°�
    public float rot { get; private set; }    // ������ ȸ�� �Է°�
    public bool Mlattack { get; private set; } // ������ �߻� �Է°�
    public bool Mgattack { get; private set; } // ������ �߻� �Է°�
    public bool p_Jump { get; private set; } // ������ �߻� �Է°�

    private bool isJump;

    public float JumpPower;

    public Vector3 moveDistance;



    private void Start()
    {
        charactercontroller = GetComponent<CharacterController>();

        rot = 1.0f;
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

        // move�� ���� �Է� ����
        moveFB = Input.GetAxis(moveFBAxisName);
        Debug.Log(moveFB);

        // rotate�� ���� �Է� ����
        moveLR = Input.GetAxis(moveLRAxisName);
        Debug.Log(moveLR);

        // fire�� ���� �Է� ����
        Mlattack = Input.GetButton(meleeAttackButtonName);
        Mgattack = Input.GetButton(magicAttackButtonName);
        p_Jump = Input.GetButton(JumpButtonName);


    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        Look();
    }

    private void Move()
    {

        moveDistance = new Vector3(moveLR, 0, moveFB);
        charactercontroller.Move(transform.TransformDirection(moveDistance) * Time.deltaTime * moveSpeed);

        //playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);


    }
    private void Jump()
    {
        if(p_Jump && !isJump)
        {
            //playerRigidbody.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            isJump = true;
            Debug.Log(isJump);
        }
    }
    private void Look()
    {
        MouseX += Input.GetAxis("Mouse X");
        transform.rotation = Quaternion.Euler(0, MouseX, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
            Debug.Log(isJump);

        }
    }
}
