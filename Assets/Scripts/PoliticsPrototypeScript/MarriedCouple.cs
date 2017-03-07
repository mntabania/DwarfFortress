using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MarriedCouple{

	public Royalty husband;
	public Royalty wife;
	public float chanceForPregnancy;

	public List<Royalty> children{
		get{ return husband.children;}
	}
		
	public MarriedCouple(Royalty husband, Royalty wife){
		this.husband = husband;
		this.wife = wife;
		this.chanceForPregnancy = 0.5f;
	}
}
