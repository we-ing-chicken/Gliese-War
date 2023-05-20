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

public class CPlayer : MonoBehaviour {
	public byte player_index { get; private set; }
	public PLAYER_STATE state { get; private set; }
	
	void Awake()
	{
	}
	
	
	public void clear()
	{
	}
	
	public void initialize(byte player_index)
	{
	}
	
	public void remove(short cell)
	{
	}

    private void Update()
    {
        if(Input.anyKeyDown)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.MOVING_REQ);
			
		}
    }
}
