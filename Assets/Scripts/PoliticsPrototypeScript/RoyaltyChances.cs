using UnityEngine;
using System.Collections;

[System.Serializable]
public class RoyaltyChances {

	internal float defaultIllnessChance = 0.05f;
	internal float defaultAccidentChance = 0.05f;
	internal float defaultOldAgeChance = 0.1f;

	public float illnessChance;
	public float accidentChance;
	public float oldAgeChance;

	public RoyaltyChances(){
		this.illnessChance = defaultIllnessChance;
		this.accidentChance = defaultAccidentChance;
		this.oldAgeChance = defaultOldAgeChance;
	}
}
