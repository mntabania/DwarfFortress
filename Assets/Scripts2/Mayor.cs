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
	public CHARACTER character;
	public List<GOALS> goals;
	public List<List<string>> tasks;

	public Mayor(){
		this.id = 1 + GetID ();
		this.name = "MAYOR" + this.id;
		this.predictability = UnityEngine.Random.Range (0, 10);
		this.persistence = UnityEngine.Random.Range (0, 10);
		this.trustworthiness = UnityEngine.Random.Range (0, 10);
		this.selflessness = UnityEngine.Random.Range (0, 10);
		this.skill = UnityEngine.Random.Range (0, 10);
		this.racism = UnityEngine.Random.Range (0, 10);
		this.religiousTolerance = UnityEngine.Random.Range (0, 10);
		this.character = (CHARACTER)(UnityEngine.Random.Range (0, Enum.GetNames(typeof(CHARACTER)).Length));
		this.goals = new List<GOALS> ();
		this.tasks = new List<List<string>> ();

		SetLastID (this.id);
	}

	private int GetID(){
		return Utilities.lastMayorId;
	}
	private void SetLastID(int id){
		Utilities.lastMayorId = id;
	}
}
