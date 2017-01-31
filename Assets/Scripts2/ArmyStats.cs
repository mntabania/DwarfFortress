using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArmyStats {

	public int hp;
	public int attack;

	public ArmyStats(int hp, int attack){
		this.hp = hp;
		this.attack = attack;
	}
}
