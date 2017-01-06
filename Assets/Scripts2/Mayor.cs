using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Mayor {
	public int id;
	public string name;
	public int predictability;
	public int persistence;
	public int trustworthiness;
	public int selflessness;
	public int skill;
	public int racism;
	public int religiousTolerance;
	public int likeCitizen;
	public CHARACTER character;
	public List<GOALS> goals;
	public List<List<string>> tasks;
	public List<PUBLIC_IMAGE> publicImages;
	public List<Relationship> relationshipKings;
	public List<Relationship> relationshipMayors;


	public Mayor(){
		this.id = 1 + GetID ();
		this.name = "MAYOR" + this.id;
		this.predictability = 0;
		this.persistence = 0;
		this.trustworthiness = 0;
		this.selflessness = 0;
		this.skill = 0;
		this.racism = 0;
		this.religiousTolerance = 0;
		this.likeCitizen = 0;
		this.character = CHARACTER.LOGICAL;
		this.goals = new List<GOALS> ();
		this.tasks = new List<List<string>> ();
		this.publicImages = new List<PUBLIC_IMAGE> ();
		this.relationshipKings = new List<Relationship> ();
		this.relationshipMayors = new List<Relationship> ();

		SetLastID (this.id);
	}

	private int GetID(){
		return Utilities.lastMayorId;
	}
	private void SetLastID(int id){
		Utilities.lastMayorId = id;
	}
}
