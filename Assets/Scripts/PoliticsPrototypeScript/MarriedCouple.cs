using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MarriedCouple{

	public Royalty husband;
	public Royalty wife;
	public float chanceForPregnancy;
	public bool isPregnant;

	public int weeksDueForPregnancy;
	public PoliticsPrototypeManager.TurnEndedDelegate pregnancyActions;

	public List<Royalty> children{
		get{ return husband.children;}
	}
		
	public MarriedCouple(Royalty husband, Royalty wife){
		this.husband = husband;
		this.wife = wife;
		this.chanceForPregnancy = 0.5f;
		this.weeksDueForPregnancy = 36;
		pregnancyActions += WaitForBirth;
	}

	public void WaitForBirth(){
		if (wife.isDead) {
			Debug.Log ("The baby died because the mother died");
			PoliticsPrototypeManager.Instance.turnEnded -= pregnancyActions;
		}
		weeksDueForPregnancy -= 1;
		if (weeksDueForPregnancy <= 0) {
			weeksDueForPregnancy = 36;
			Royalty baby = MarriageManager.Instance.MakeBaby(husband, wife);
			this.isPregnant = false;
			Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + husband.name + " and " + wife.name + " had a baby, and named it :" + baby.name);
			PoliticsPrototypeManager.Instance.turnEnded -= pregnancyActions;
		}
	}
}
