using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class playerScript : MonoBehaviourPunCallbacks//, IPunObservable
{
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

    //private Vector3 remotePos;
    //private Quaternion remoteRot;

    private float moveLR;
    private float moveFB;
    private bool ismove;

    public float move_speed;
    public float jump_force;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        //TODO : 캐릭터 생성 시 추가되도록.
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (PV.IsMine)
        //{
        //    transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
        //    return;
        //}
        //else
        {
            moveLR = Input.GetAxisRaw("Horizontal");
            moveFB = Input.GetAxisRaw("Vertical");
            ismove = (Input.GetButton("Horizontal") || Input.GetButton("Vertical"));


            anim.SetBool("isRun", ismove);

            player_lookTarget();

            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * move_speed,
                                            0,
                                            Input.GetAxisRaw("Vertical") * Time.deltaTime * move_speed));
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
                isJump = true;
            }
        }
    }

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

    ////RPC �Լ�
    //[PunRPC]
    void Jump()
    {
        if (!isJump) return;
        //Debug.Log("Jump!");

        anim.SetTrigger("jump");
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        isJump = false;
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    { 
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);
    //        //stream.SendNext(Jump);
    //    }
    //    else
    //    {
    //        remotePos = (Vector3)stream.ReceiveNext();
    //        remoteRot = (Quaternion)stream.ReceiveNext();
    //    }
    //}
}