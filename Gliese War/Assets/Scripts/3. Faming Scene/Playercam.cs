using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercam : MonoBehaviour
{
    public Player player; // �ٶ� �÷��̾� ������Ʈ�Դϴ�.
    public float xmove = 0;  // X�� ���� �̵���
    public float ymove = 0;  // Y�� ���� �̵���
    public float distance = 3;
    public float ymove_max = 60.0f;
    public float ymove_min = -5.0f;

    // Update is called once per frame
    void Update()
    {
        xmove = player.MouseX; // ���콺�� �¿� �̵����� xmove �� �����մϴ�.
        
        ymove -= Input.GetAxis("Mouse Y"); // ���콺�� ���� �̵����� ymove �� �����մϴ�.
        //ymove = ymove <= ymove_min ? ymove_min : ymove;
        //ymove = ymove >= ymove_max ? ymove_max : ymove;
        ymove = Mathf.Clamp(ymove, ymove_min, ymove_max);

        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // �̵����� ���� ī�޶��� �ٶ󺸴� ������ �����մϴ�.
        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // ī�޶� �ٶ󺸴� �չ����� Z ���Դϴ�. �̵����� ���� Z ������� ���͸� ���մϴ�.
        transform.position = player.transform.position - transform.rotation * reverseDistance; // �÷��̾��� ��ġ���� ī�޶� �ٶ󺸴� ���⿡ ���Ͱ��� ������ ��� ��ǥ�� �����մϴ�.
    }
}