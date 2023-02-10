using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    private float Gravity = 9.8f;
    public int life;
    public float MouseX;
    public float mouseSpeed;

    private string moveFBAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    private string moveLRAxisName = "Horizontal"; // 좌우 움직임을 위한 입력축 이름
    private string meleeAttackButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    private string magicAttackButtonName = "Fire2"; // 발사를 위한 입력 버튼 이름
    private string JumpButtonName = "Jump";

    private CharacterController charactercontroller;
    
    public float moveFB { get; private set; } // 감지된 전후이동 입력값
    public float moveLR { get; private set; } // 감지된 좌우이동 입력값
    public float rot { get; private set; }    // 감지된 회전 입력값
    public bool Mlattack { get; private set; } // 감지된 발사1 입력값
    public bool Mgattack { get; private set; } // 감지된 발사2 입력값
    public bool p_Jump { get; private set; } // 감지된 점프 입력값

    public float JumpPower;

    public Vector3 moveDir;



    private void Start()
    {
        charactercontroller = GetComponent<CharacterController>();
        moveDir = Vector3.zero;
        rot = 1.0f;
    }

    private void Update()
    {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        //if (FarmingManager.Instance != null && FarmingManager._isEnd)
        //{
        //    moveFB = 0;
        //    moveLR = 0;
        //    Mlattack = false;
        //    return;
        //}

        if (charactercontroller == null) return;

        // move에 관한 입력 감지
        moveFB = Input.GetAxis(moveFBAxisName);

        // rotate에 관한 입력 감지
        moveLR = Input.GetAxis(moveLRAxisName);

        // fire에 관한 입력 감지
        Mlattack = Input.GetButton(meleeAttackButtonName);
        Mgattack = Input.GetButton(magicAttackButtonName);
        p_Jump = Input.GetButton(JumpButtonName);


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
        transform.rotation = Quaternion.Euler(0, MouseX, 0);
    }
}
