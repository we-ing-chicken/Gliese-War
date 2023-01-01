using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도

    private string moveFBAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    private string moveLRAxisName = "Horizontal"; // 좌우 움직임을 위한 입력축 이름
    private string meleeAttackButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    private string magicAttackButtonName = "Fire2"; // 발사를 위한 입력 버튼 이름

    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디

    
    public float moveFB { get; private set; } // 감지된 움직임 입력값
    public float moveLR { get; private set; } // 감지된 회전 입력값
    public bool Mlattack { get; private set; } // 감지된 발사 입력값
    public bool Mgattack { get; private set; } // 감지된 발사 입력값


    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        if (GameManager.Instance != null && GameManager.Instance.isGameover)
        {
            moveFB = 0;
            moveLR = 0;
            Mlattack = false;
            return;
        }

        // move에 관한 입력 감지
        moveFB = Input.GetAxis(moveFBAxisName);
        // rotate에 관한 입력 감지
        moveLR = Input.GetAxis(moveLRAxisName);
        // fire에 관한 입력 감지
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
