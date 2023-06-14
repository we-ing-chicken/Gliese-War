using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class playerScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private bool isJump;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            float axis = Input.GetAxisRaw("Vertical");
            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * 7,
                                            0,
                                            Input.GetAxisRaw("Vertical") * Time.deltaTime * 7));
            if (Input.GetKey(KeyCode.Space))
            {
                isJump = true;
                PV.RPC("Jump", RpcTarget.All, axis); //RPC 함수 호출 
            }
        }
    }

    //RPC 함수
    [PunRPC]
    void Jump(float axis)
    {
        if (!isJump)
            return;
        rigidbody.AddForce(Vector3.up * .5f, ForceMode.Impulse);
        isJump = false;
    }
}