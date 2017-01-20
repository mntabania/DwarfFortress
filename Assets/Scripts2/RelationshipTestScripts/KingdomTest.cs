using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class KingdomTest{

	public int id;
	public string kingdomName;
	public Lord lord;
	public List<CityTileTest> cities;
	public RACE kingdomRace;
	public bool isDead;
	public Color tileColor;
	public List<Relationship> relationshipKingdoms;

	public RESOURCE primaryRaceResource;
	public RESOURCE secondaryRaceResource;

	public KingdomTest(float populationGrowth, RACE race, List<CityTileTest> cities, Color tileColor){
		this.id = GetID() + 1;
		this.kingdomName = "KINGDOM" + this.id;
		this.lord = new Lord(this);
		this.cities = cities;
		this.kingdomRace = race;
		this.isDead = false;
		this.tileColor = tileColor;
		this.relationshipKingdoms = new List<Relationship>();
		SetLastID (this.id);
		DetermineCityUpgradeResourceType();
	}

	private int GetID(){
		return Utilities.lastkingdomid;
	}
	private void SetLastID(int id){
		Utilities.lastkingdomid = id;
	}

	internal void AddCityToKingdom(CityTileTest city){
		cities.Add (city);
		city.GetComponent<HexTile> ().SetTileColor (tileColor);
	}

	void DetermineCityUpgradeResourceType(){
		switch (this.kingdomRace) {
		case RACE.HUMANS:
			this.primaryRaceResource = RESOURCE.STONE;
			this.secondaryRaceResource = RESOURCE.METAL;
			break;
		case RACE.ELVES:
			this.primaryRaceResource = RESOURCE.LUMBER;
			this.secondaryRaceResource = RESOURCE.MANA;
			break;
		case RACE.MINGONS:
			this.primaryRaceResource = RESOURCE.LUMBER;
			this.secondaryRaceResource = RESOURCE.METAL;
			break;
		case RACE.CROMADS:
			this.primaryRaceResource = RESOURCE.STONE;
			this.secondaryRaceResource = RESOURCE.MANA;
			break;
		}
	}

}
