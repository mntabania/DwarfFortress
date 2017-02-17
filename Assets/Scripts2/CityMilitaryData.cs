using UnityEngine;
using System.Collections;

[System.Serializable]
public class CityMilitaryData {

	public CityTest city;
	public int armyStrength;

	public CityMilitaryData(CityTest city, int armyStrength){
		this.city = city;
		this.armyStrength = armyStrength;
	}
}
