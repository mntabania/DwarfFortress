using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class King {
	public int id;
	public string name;
	public int predictability;
	public int persistence;
	public int trustworthiness;
	public int selflessness;
	public int skill;
	public int racism;
	public int religiousTolerance;
	public CHARACTER character;
	public List<GOALS> goals;
	public List<List<string>> tasks;
	public List<PUBLIC_IMAGE> publicImages;
	public List<Relationship> relationshipKings;
	public List<Relationship> relationshipMayors;

	public King(string dummy){
		this.id = 1 + GetID ();
		this.name = "KING" + this.id;
		this.predictability = UnityEngine.Random.Range (0, 10);
		this.persistence = UnityEngine.Random.Range (0, 10);
		this.trustworthiness = UnityEngine.Random.Range (0, 10);
		this.selflessness = UnityEngine.Random.Range (0, 10);
		this.skill = UnityEngine.Random.Range (0, 10);
		this.racism = UnityEngine.Random.Range (0, 10);
		this.religiousTolerance = UnityEngine.Random.Range (0, 10);		
		this.character = (CHARACTER)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CHARACTER)).Length));		
		this.goals = new List<GOALS> ();
		this.tasks = new List<List<string>> ();
		this.publicImages = new List<PUBLIC_IMAGE> ();
		this.relationshipKings = new List<Relationship> ();
		this.relationshipMayors = new List<Relationship> ();

		SetLastID (this.id);
	}
	private int GetID(){
		return Utilities.lastKingId;
	}
	private void SetLastID(int id){
		Utilities.lastKingId = id;
	}
}