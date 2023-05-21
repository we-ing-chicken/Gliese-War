using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;
using Server_Unity;

public enum PLAYER_STATE
{
	None = 0,

	ImLocal = 1,
	END
}

public class CPlayer : MonoBehaviour 
{
    static public CPlayer instance;
    public byte player_index { get; private set; }
	public PLAYER_STATE state { get; private set; }

	public CBattleRoom cbattleroom;

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

    private CharacterController charactercontroller;

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
        instance = this;
        charactercontroller = GetComponent<CharacterController>();

        moveDir = Vector3.zero;
        rot = 1.0f;
        life = 10;

        offensivePower = 10;
        defensivePower = 10;
        maxHealth = 100;
        currHealth = 100;
        moveSpeed = 8;
    }

    public void clear()
	{
	}
	
	public void initialize(byte my_player_index, CBattleRoom cb)
	{
        player_index = my_player_index;
        cbattleroom = cb;
	}
	
	public void remove(short cell)
	{
	}

    private void Update()
    {
        if (charactercontroller == null) return;

        moveFB = Input.GetAxis(moveFBAxisName);

        moveLR = Input.GetAxis(moveLRAxisName);

        ismove = (Input.GetButton(moveFBAxisName) || Input.GetButton(moveLRAxisName));

        p_Jump = Input.GetButton(JumpButtonName);

        animate();

        if (Input.anyKeyDown)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.MOVING_REQ);
			
		}
    }

    private void FixedUpdate()
    {
        if (charactercontroller == null) return;

        SendButton();
    }

    void SendButton()
    {

    }

    private void animate()
    {

        animator.SetBool("isRun", ismove);
    }
}
