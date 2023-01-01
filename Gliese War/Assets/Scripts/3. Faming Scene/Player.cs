using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // �յ� �������� �ӵ�

    private string moveFBAxisName = "Vertical"; // �յ� �������� ���� �Է��� �̸�
    private string moveLRAxisName = "Horizontal"; // �¿� �������� ���� �Է��� �̸�
    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�

    private Rigidbody playerRigidbody; // �÷��̾� ĳ������ ������ٵ�

    
    public float moveFB { get; private set; } // ������ ������ �Է°�
    public float moveLR { get; private set; } // ������ ȸ�� �Է°�
    public bool Mlattack { get; private set; } // ������ �߻� �Է°�
    public bool Mgattack { get; private set; } // ������ �߻� �Է°�


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

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {

        Vector3 moveDistance = new Vector3(moveLR, 0, moveFB).normalized * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);


    }

}
