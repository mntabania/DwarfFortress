using UnityEngine;
using System.Collections;

[System.Serializable]
public class General {
	public int id;
	public string name;
	public CityTest city;
	public HexTile location;
	public Army army;
	public bool onAttack;

	public General(CityTest city){
		this.id = GetID() + 1;
		this.name = "GENERAL" + this.id;
		this.city = city;
		this.location = city.hexTile;
		this.army = new Army (city);
		this.onAttack = false;

		SetLastID (this.id);
	}
	internal int ArmyStrength(){
		return this.army.armyCount * this.army.armyStats.hp;
	}
	private int GetID(){
		return Utilities.lastGeneralId;
	}

	private void SetLastID(int id){
		Utilities.lastGeneralId = id;
	}
}
