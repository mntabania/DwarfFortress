using UnityEngine;
using System.Collections;

[System.Serializable]
public class Relationship {
//	public int id;
	public Lord lord;
	public DECISION previousDecision;
	public int like;
	public LORD_RELATIONSHIP lordRelationship;
	public LORD_EVENTS previousInteraction;
	public bool isFirstEncounter;
	public bool isAdjacent;
	public bool isAtWar;
	public int daysAtWar;

	public Relationship(Lord lord, DECISION previousDecision, int like){
//		this.id = id;
		this.lord = lord;
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
