using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MilitaryData {
	public int id;
	public CityTest enemyCity;
	public General enemyGeneral;
	public BATTLE_MOVE battleMove;
	public int yourArmyStrength;
//	public bool isResolved;

	public MilitaryData(CityTest enemyCity, General enemyGeneral, int yourArmyStrength, BATTLE_MOVE battleMove){
		this.id = GetID () + 1;
		this.enemyCity = enemyCity;
		this.enemyGeneral = enemyGeneral;
		this.battleMove = battleMove;
		this.yourArmyStrength = yourArmyStrength;
//		this.isResolved = false;
	}

	private int GetID(){
		return Utilities.lastMilitaryDataId;
	}

	private void SetLastID(int id){
		Utilities.lastMilitaryDataId = id;
	}
}
