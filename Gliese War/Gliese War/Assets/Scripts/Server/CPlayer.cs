using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PLAYER_STATE
{
	None = 0,
}

public class CPlayer : MonoBehaviour {
	
	public List<short> cell_indexes { get; private set; }
	public byte player_index { get; private set; }
	public PLAYER_STATE state { get; private set; }
	CPlayerAgent agent;
	
	void Awake()
	{
		this.cell_indexes = new List<short>();
		this.agent = new CPlayerAgent();
	}
	
	
	public void clear()
	{
	}
	
	public void initialize(byte player_index)
	{
		this.player_index = player_index;
	}
	
	public void remove(short cell)
	{
		this.cell_indexes.Remove(cell);
	}
}
