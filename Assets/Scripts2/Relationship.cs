﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Relationship {
	public int id;
	public string name;
	public DECISION previousDecision;
	public int like;
	public LORD_RELATIONSHIP lordRelationship;
	public bool isFirstEncounter;

	public Relationship(int id, string name, DECISION previousDecision, int like){
		this.id = id;
		this.name = name;
		this.previousDecision = previousDecision;
		this.like = like;
		this.isFirstEncounter = true;
		this.lordRelationship = LORD_RELATIONSHIP.NEUTRAL;
	}
}
