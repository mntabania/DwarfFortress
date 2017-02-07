using UnityEngine;
using System.Collections;

[System.Serializable]
public class Relationship {
	public int id;
	public string name;
	public DECISION previousDecision;
	public int like;
	public LORD_RELATIONSHIP lordRelationship;
	public LORD_EVENTS previousInteraction;
	public bool isFirstEncounter;
	public bool isAdjacent;
	public bool isAtWar;
	public int daysAtWar;

	public Relationship(int id, string name, DECISION previousDecision, int like){
		this.id = id;
		this.name = name;
		this.previousDecision = previousDecision;
		this.like = like;
		this.isFirstEncounter = true;
		this.lordRelationship = LORD_RELATIONSHIP.NEUTRAL;
		this.isAdjacent = false;
		this.isAtWar = false;
	}

	internal void IncreaseWartime(int currentDay){
		daysAtWar += 1;
	}
}
