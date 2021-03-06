﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Relationship {
//	public int id;
	public Lord lord;
	public DECISION previousDecision;
	public int like;
	public LORD_RELATIONSHIP lordRelationship;
	public LORD_EVENTS previousInteraction = LORD_EVENTS.NONE;
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
		if (daysAtWar == 90) {
			Debug.LogError("War with lord: " + this.lord.name.ToString () + " has ended on day: " + currentDay.ToString() + "!");
			this.like += 20;
			this.lordRelationship = GetLordRelationship(this.like);
			daysAtWar = 0;
			isAtWar = false;
			GameManager.Instance.turnEnded -= this.IncreaseWartime;
		}

	}

	internal LORD_RELATIONSHIP GetLordRelationship(int likeness){
		if(likeness <= -81){
			return LORD_RELATIONSHIP.RIVAL;
		}else if(likeness >= -80 && likeness <= -41){
			return LORD_RELATIONSHIP.ENEMY;
		}else if(likeness >= -40 && likeness <= -21){
			return LORD_RELATIONSHIP.COLD;
		}else if(likeness >= -20 && likeness <= 20){
			return LORD_RELATIONSHIP.NEUTRAL;
		}else if(likeness >= 21 && likeness <= 40){
			return LORD_RELATIONSHIP.WARM;
		}else if(likeness >= 41 && likeness <= 80){
			return LORD_RELATIONSHIP.FRIEND;
		}else{
			return LORD_RELATIONSHIP.ALLY;
		}
	}
}
