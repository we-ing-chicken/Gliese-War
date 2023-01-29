using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercam : MonoBehaviour
{
    public Player player; // 바라볼 플레이어 오브젝트입니다.
    public float xmove = 0;  // X축 누적 이동량
    public float ymove = 0;  // Y축 누적 이동량
    public float distance = 3;
    public float ymove_max = 60.0f;
    public float ymove_min = -5.0f;

    // Update is called once per frame
    void Update()
    {
        xmove = player.MouseX; // 마우스의 좌우 이동량을 xmove 에 누적합니다.
        
        ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.
        //ymove = ymove <= ymove_min ? ymove_min : ymove;
        //ymove = ymove >= ymove_max ? ymove_max : ymove;
        ymove = Mathf.Clamp(ymove, ymove_min, ymove_max);

        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // 이동량에 따라 카메라의 바라보는 방향을 조정합니다.
        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
        transform.position = player.transform.position - transform.rotation * reverseDistance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.
    }
}