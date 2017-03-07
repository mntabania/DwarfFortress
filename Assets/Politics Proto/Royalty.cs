using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Royalty {
	public int id;
	public string name;
	public GENDER gender;
	public int age;
	public int generation;
	public KingdomTest kingdom;
	public TRAIT[] trait;
	public Royalty loyalLord;
	public Royalty hatedLord;
	public Royalty father;
	public Royalty mother;
	public Royalty spouse;
	public List<Royalty> children;
	public MONTH birthMonth;
	public int birthWeek;
	public int birthYear;
	public bool isIndependent;
	public bool isMarried;
	public bool isDead;

	public Royalty(KingdomTest kingdom, int age, GENDER gender, int generation){
		this.id = GetID () + 1;
		this.name = "ROYALTY" + this.id;
		this.age = age;
		this.gender = gender;
		this.generation = generation;
		this.kingdom = kingdom;
		this.trait = GetTrait();
		this.loyalLord = null;
		this.hatedLord = null;
		this.father = null;
		this.mother = null;
		this.spouse = null;
		this.children = new List<Royalty> ();
		this.birthMonth = (MONTH) PoliticsPrototypeManager.Instance.month;
		this.birthWeek = PoliticsPrototypeManager.Instance.week;
		this.birthYear = PoliticsPrototypeManager.Instance.year;
		this.isIndependent = false;
		this.isMarried = false;
		this.isDead = false;
		SetLastID (this.id);
		this.kingdom.royaltyList.allRoyalties.Add (this);
		ChangeLoyalty (this.kingdom.assignedLord);
		ChangeHatred ();
		PoliticsPrototypeManager.Instance.RegisterRoyalty(this);
	}

	private int GetID(){
		return Utilities.lastRoyaltyId;
	}

	private void SetLastID(int id){
		Utilities.lastRoyaltyId = id;
	}
	internal TRAIT[] GetTrait(){
		TRAIT[] traits = new TRAIT[2];
		int trait1Chance = UnityEngine.Random.Range (0, 100);
		if(trait1Chance < 40){
			traits[0] = TRAIT.VICIOUS;
		}else{
			traits[0] = TRAIT.NAIVE;
		}

		int trait2Chance = UnityEngine.Random.Range (0, 100);
		if(trait2Chance < 20){
			traits[1] = TRAIT.LOYAL;
		}else{
			int trait3Chance = UnityEngine.Random.Range (0, 100);
			if (trait3Chance < 20) {
				traits [1] = TRAIT.TRAITOR;
			}
		}

		return traits;
	}
	internal void AddParents(Royalty father, Royalty mother){
		this.father = father;
		this.mother = mother;
	}

	internal void AddChild(Royalty child){
		this.children.Add (child);
	}
	internal void ChangeLoyalty(Royalty newLoyalty){
		if(newLoyalty == null){
			this.loyalLord = this;
		}else{
			this.loyalLord = newLoyalty;
		}

	}

	internal void ChangeHatred(){
		List<Royalty> hatedRoyaltyPool = new List<Royalty> ();
		for(int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++){
			if(PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.id != this.kingdom.id){
				hatedRoyaltyPool.Add (PoliticsPrototypeManager.Instance.kingdoms [i].kingdom.assignedLord);
			}
		}
		if(hatedRoyaltyPool.Count > 0){
			this.hatedLord = hatedRoyaltyPool [UnityEngine.Random.Range(0, hatedRoyaltyPool.Count)];

		}
	}
	internal void AssignBirthday(MONTH month, int week, int year){
		this.birthMonth = month;
		this.birthWeek = week;
		this.birthYear = year;
	}

	internal bool IsRoyaltyCloseRelative(Royalty royalty){
		if (royalty.id == this.father.id || royalty.id == this.mother.id) {
			//royalty is father or mother
			return true;
		}

		if(royalty.id == this.father.father.id || royalty.id == this.father.mother.id ||
			royalty.id == this.mother.father.id || royalty.id == this.mother.mother.id){
			//royalty is grand parent
			return true;
		}

		for (int i = 0; i < this.father.children.Count; i++) {
			if(royalty.id == this.father.children[i].id){
				//royalty is sibling
				return true;
			}
		}

		for (int i = 0; i < this.father.father.children.Count; i++) {
			if(royalty.id == this.father.father.children[i].id){
				//royalty is uncle or aunt from fathers side
				return true;
			}
		}

		for (int i = 0; i < this.mother.father.children.Count; i++) {
			if(royalty.id == this.mother.father.children[i].id){
				//royalty is uncle or aunt from mothers side
				return true;
			}
		}

		return false;
	}

	internal void AttemptToMarry(){
		if (this.gender == GENDER.FEMALE) {
			Debug.Log ("Cannot trigger marriage on women");
			return;
		}

		if (this.isMarried) {
			Debug.Log ("Royalty is already married!");
			return;
		}
		List<Royalty> elligibleBachelorettes = MarriageManager.Instance.GetElligibleBachelorettesForMarriage();

		Royalty currentBachelor = this;
		Royalty mostElligibleBride = null;
		List<BRIDE_CRITERIA> mostElligibleBridesPoints = new List<BRIDE_CRITERIA>();
		//find bride
		for (int j = 0; j < elligibleBachelorettes.Count; j++) {
			Royalty currentBachelorette = elligibleBachelorettes[j];
			if (IsRoyaltyCloseRelative (currentBachelorette)) {
				continue;
			}
			if (mostElligibleBride == null) {
				mostElligibleBride = currentBachelorette;

				if (currentBachelorette.loyalLord.id == currentBachelor.loyalLord.id) {
					mostElligibleBridesPoints.Add (BRIDE_CRITERIA.IS_LOYAL_TO_SAME_LORD);
				}

				if (currentBachelorette.kingdom.cities.Count > currentBachelor.kingdom.cities.Count) {
					mostElligibleBridesPoints.Add (BRIDE_CRITERIA.HAS_MORE_CITIES);
				}

				if (currentBachelor.hatedLord == null || currentBachelorette.kingdom.lord.id != currentBachelor.hatedLord.id) {
					mostElligibleBridesPoints.Add (BRIDE_CRITERIA.IS_NOT_FROM_HATED_KINGDOM);
				}

			} else {
				List<BRIDE_CRITERIA> currentBacheloretteCriteria = new List<BRIDE_CRITERIA>();
				if (currentBachelorette.loyalLord.id == currentBachelor.loyalLord.id) {
					currentBacheloretteCriteria.Add (BRIDE_CRITERIA.IS_LOYAL_TO_SAME_LORD);
				}

				if (currentBachelorette.kingdom.cities.Count > currentBachelor.kingdom.cities.Count) {
					currentBacheloretteCriteria.Add (BRIDE_CRITERIA.HAS_MORE_CITIES);
				}

				if (currentBachelor.hatedLord == null || currentBachelorette.kingdom.lord.id != currentBachelor.hatedLord.id) {
					currentBacheloretteCriteria.Add (BRIDE_CRITERIA.IS_NOT_FROM_HATED_KINGDOM);
				}

				if (currentBacheloretteCriteria.Count > mostElligibleBridesPoints.Count) {
					mostElligibleBride = currentBachelorette;
					mostElligibleBridesPoints.Clear();
					mostElligibleBridesPoints.AddRange (currentBacheloretteCriteria);
				}
			}

			if (mostElligibleBridesPoints.Count >= 3) {
				//best possible bride already found
				break;
			}
		}

		if (mostElligibleBride != null) {
			MarriageManager.Instance.Marry (currentBachelor, mostElligibleBride);
		} else {
			Debug.Log ("Could not find bride for :" + currentBachelor.name);
		}
	}
}
