using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MilitaryData {
	public CityTest enemyCity;
	public General enemyGeneral;
	public BATTLE_MOVE battleMove;
	public int yourArmyStrength;
	public bool isResolved;

	public MilitaryData(CityTest enemyCity, General enemyGeneral, int yourArmyStrength, BATTLE_MOVE battleMove){
		this.enemyCity = enemyCity;
		this.enemyGeneral = enemyGeneral;
		this.battleMove = battleMove;
		this.yourArmyStrength = yourArmyStrength;
		this.isResolved = false;
	}
}
