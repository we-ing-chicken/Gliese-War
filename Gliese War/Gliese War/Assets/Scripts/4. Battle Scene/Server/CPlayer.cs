using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PLAYER_STATE
{
	None = 0,
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
}
