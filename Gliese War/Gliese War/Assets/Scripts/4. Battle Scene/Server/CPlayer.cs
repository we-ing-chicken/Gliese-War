using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;
using Server_Unity;
//using UnityEngine.Windows;

public enum PLAYER_STATE
{
	None = 0,

	ImLocal = 1,
	END
}

public class CPlayer : MonoBehaviour 
{
    Vector3 preposition;
    public bool isMine = false;
    public byte player_index { get; private set; }
	public PLAYER_STATE state { get; private set; }

    private float Gravity = 9.8f;
    public int life;
    public float MouseX;
    public float mouseSpeed;

    public int offensivePower;
    public int defensivePower;
    public int maxHealth;
    public int currHealth;
    public int moveSpeed;

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

    [SerializeField] private bool ismove = false;
    [SerializeField] private bool ignoreGravity = false;

    //private CharacterController charactercontroller;
    public Rigidbody rb;

    public float moveFB { get; private set; } // ������ �����̵� �Է°�
    public float moveLR { get; private set; } // ������ �¿��̵� �Է°�
    public float rot { get; private set; } // ������ ȸ�� �Է°�
    public bool Mlattack { get; private set; } // ������ �߻�1 �Է°�
    public bool Mgattack { get; private set; } // ������ �߻�2 �Է°�
    public bool p_Jump { get; private set; } // ������ ���� �Է°�

    public float JumpPower;

    public Vector3 moveDir;

    private string moveFBAxisName = "Vertical"; // �յ� �������� ���� �Է��� �̸�
    private string moveLRAxisName = "Horizontal"; // �¿� �������� ���� �Է��� �̸�
    private string meleeAttackButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    private string magicAttackButtonName = "Fire2"; // �߻縦 ���� �Է� ��ư �̸�
    private string JumpButtonName = "Jump";

    void Awake()
	{
	}

    private void Start()
    {
        //charactercontroller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        moveDir = Vector3.zero;
        rot = 1.0f;
        life = 10;

        offensivePower = 10;
        defensivePower = 10;
        maxHealth = 100;
        currHealth = 100;
        moveSpeed = 8;
        JumpPower = 5;
    }

    public void clear()
	{
	}
	
	public void initialize(byte my_player_index)
	{
        player_index = my_player_index;
	}
	
	public void remove(short cell)
	{
	}

    private void Update()
    {
        preposition = transform.position;

        //if (charactercontroller == null) return;

        if (isMine)
        {
            moveFB = Input.GetAxis(moveFBAxisName);

            moveLR = Input.GetAxis(moveLRAxisName);

            ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));

            p_Jump = Input.GetButton(JumpButtonName);
            Look();
        }
        animate();

    }

    private void FixedUpdate()
    {
        //if (charactercontroller == null) return;

        //if (!charactercontroller.isGrounded)
        //{
        //    if (!ignoreGravity)
        //        Fall();
        //}
        //else
        //{
        //    Move();
        //    if (p_Jump)
        //    {
        //            animator.SetTrigger("doJump");
        //            Jump();
        //    }
        //}
        //charactercontroller.Move(moveDir * Time.deltaTime);

        Vector3 movement = new Vector3(moveLR, 0.0f, moveFB);

        transform.Translate(movement * moveSpeed * Time.deltaTime);
        rb.useGravity = false;

        if (movement != Vector3.zero)
        {
            
            StartCoroutine(send_MOVING_REQ());
        }
        SendButton();
    }

    IEnumerator send_MOVING_REQ()
    {
        CPacket msg = CPacket.create((short)PROTOCOL.MOVING_REQ);
        msg.push(transform.position.x);
        msg.push(transform.position.y);
        msg.push(transform.position.z);
        CNetworkManager.Instance.send(msg);
        yield return new WaitForEndOfFrame();
    }

    private void Move()
    {
        player_lookTarget();

        
        //moveDir = charactercontroller.transform.TransformDirection(new Vector3(moveLR, 0, moveFB)) * moveSpeed;
    }

    private void Jump()
    {
        moveDir.y = JumpPower;
    }

    private void Fall()
    {
        moveDir.y -= Gravity * Time.deltaTime;
    }

    private void player_lookTarget()
    {
        //if (charactercontroller == null) return;

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

    private void Look()
    {
        MouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        transform.rotation = Quaternion.Euler(0, MouseX, 0);


    }

    private void player_Rotate(Vector3 movePoint)
    { 
        Vector3 relativePosition = movePoint - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        playertransform.rotation = Quaternion.Lerp(playertransform.rotation, rotation, Time.deltaTime * 10);
    }

    void SendButton()
    {

    }

    private void animate()
    {
        if(isMine) 
        { 
            animator.SetBool("isRun", ismove);
        }
        else
        {
            animator.SetBool("isRun", preposition != transform.position);
        }
    }
}
