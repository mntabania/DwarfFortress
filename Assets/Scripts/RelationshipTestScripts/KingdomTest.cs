using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingdomTest{

	public int id;
	public string kingdomName;
	public King king;
	public List<CityTest> cities;
	public RACE kingdomRace;
	public bool isDead;
	public List<Relationship> relationshipKingdoms;

	public KingdomTest(){
		this.id = 0;
		this.kingdomName = "KINGDOM";
		this.king = new King();
		this.cities = new List<CityTest>();
		this.kingdomRace = RACE.HUMANS;
		this.isDead = false;
		this.relationshipKingdoms = new List<Relationship>();
	}

}
