using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public RoyaltyChances royaltyChances;
	public MONTH birthMonth;
	public int birthWeek;
	public int birthYear;
	public int previousConversionMonth;
	public int previousConversionYear;
	public bool isIndependent;
	public bool isMarried;
	public bool isDirectDescendant;
	public bool isDead;

	public Royalty(KingdomTest kingdom, int age, GENDER gender, int generation){
		this.id = GetID () + 1;
		this.name = RandomNameGenerator.GenerateRandomName() + " " + this.id;
		this.age = age;
		this.gender = gender;
		this.generation = generation;
		this.kingdom = kingdom;
		this.trait = GetTrait();
//		this.trait = new TRAIT[]{TRAIT.VICIOUS, TRAIT.NONE};
		this.loyalLord = null;
		this.hatedLord = null;
		this.father = null;
		this.mother = null;
		this.spouse = null;
		this.children = new List<Royalty> ();
		this.royaltyChances = new RoyaltyChances ();
		this.birthMonth = (MONTH) PoliticsPrototypeManager.Instance.month;
		this.birthWeek = PoliticsPrototypeManager.Instance.week;
		this.birthYear = PoliticsPrototypeManager.Instance.year;
		this.isIndependent = false;
		this.isMarried = false;
		this.isDirectDescendant = false;
		this.isDead = false;
		this.kingdom.royaltyList.allRoyalties.Add (this);
		if(this.kingdom.assignedLord == null){
			this.loyalLord = this;
		}else{
			this.loyalLord = this.kingdom.assignedLord;
		}

		PoliticsPrototypeManager.Instance.RegisterRoyalty(this);
		SetLastID (this.id);

		RoyaltyEventDelegate.onIncreaseIllnessAndAccidentChance += IncreaseIllnessAndAccidentChance;
		RoyaltyEventDelegate.onChangeIsDirectDescendant += ChangeIsDirectDescendant;
		RoyaltyEventDelegate.onMassChangeLoyalty += MassChangeLoyalty;
		PoliticsPrototypeManager.Instance.turnEnded += TurnActions;
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
			Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + this.name + " LOYALTY IS TO " + this.name);
		}else{
			this.loyalLord = newLoyalty;
			Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + this.name + " LOYALTY IS TO " + newLoyalty.name);
		}
		this.previousConversionMonth = PoliticsPrototypeManager.Instance.month;
		this.previousConversionYear = PoliticsPrototypeManager.Instance.year;

	}

	internal void MassChangeLoyalty(Royalty newLoyalty, Royalty previousLoyalty){
		if (previousLoyalty.id == this.loyalLord.id) {
			if (this.kingdom.id != newLoyalty.kingdom.id) {
				this.loyalLord = this.kingdom.assignedLord;
			} else {
				this.loyalLord = newLoyalty;
			}
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
	
	internal void ChangeIsDirectDescendant(bool status){
		this.isDirectDescendant = status;
	}

	internal void IncreaseIllnessAndAccidentChance(){
		this.royaltyChances.illnessChance += 0.01f;
		this.royaltyChances.accidentChance += 0.01f;
	}
	internal void TurnActions(){
		CheckAge ();
		DeathReasons ();
	}
	internal void CheckAge(){
		if((MONTH)PoliticsPrototypeManager.Instance.month == this.birthMonth && PoliticsPrototypeManager.Instance.week == this.birthWeek && PoliticsPrototypeManager.Instance.year > this.birthYear){
			this.age += 1;
//			Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + "HAPPY BIRTHDAY " + this.name + "!!");
		}
	}
	internal void DeathReasons(){
		if(isDead){
			return;
		}
		float illness = UnityEngine.Random.Range (0f, 99f);
		if(illness <= this.royaltyChances.illnessChance){
			Death ();
			Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + this.name + " DIED OF ILLNESS!");
		}else{
			float accidents = UnityEngine.Random.Range (0f, 99f);
			if(accidents <= this.royaltyChances.accidentChance){
				Death ();
				Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + this.name + " DIED OF ACCIDENT!");
			}else{
				if(this.age >= 60){
					float oldAge = UnityEngine.Random.Range (0f, 99f);
					if(oldAge <= this.royaltyChances.oldAgeChance){
						Death ();
						Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + this.name + " DIED OF OLD AGE!");
					}else{
						this.royaltyChances.oldAgeChance += 0.05f;
					}
				}
			}
		}
	}
	internal void Death(){
		this.kingdom.royaltyList.allRoyalties.Remove (this);
		this.kingdom.royaltyList.successionRoyalties.Remove (this);
		this.isDead = true;
		RoyaltyEventDelegate.onIncreaseIllnessAndAccidentChance -= IncreaseIllnessAndAccidentChance;
		RoyaltyEventDelegate.onChangeIsDirectDescendant -= ChangeIsDirectDescendant;
		RoyaltyEventDelegate.onMassChangeLoyalty -= MassChangeLoyalty;
		PoliticsPrototypeManager.Instance.turnEnded -= TurnActions;

		if(this.id == this.kingdom.assignedLord.id){
			//ASSIGN NEW LORD, SUCCESSION
			if (this.kingdom.royaltyList.successionRoyalties.Count <= 0) {
				this.kingdom.AssignNewLord (null);
			} else {
				this.kingdom.AssignNewLord (this.kingdom.royaltyList.successionRoyalties [0]);
			}
		}
	}
		
	internal void Assassination(){
		List<RelationshipKingdoms> warRelations = this.kingdom.relationshipKingdoms.Where (x => x.isAtWar == true).ToList();
		List<RelationshipKingdoms> peaceRelations = this.kingdom.relationshipKingdoms.Where (x => x.isAtWar == false).ToList();

		if(warRelations.Count > 0){
			KingdomTest targetKingdom = warRelations [UnityEngine.Random.Range (0, warRelations.Count)].kingdom;
			if (targetKingdom.royaltyList.successionRoyalties.Count > 0) {
				if (targetKingdom.royaltyList.successionRoyalties [0].loyalLord.id == this.id) {
					Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT WAR...");
					int assassinateChance = UnityEngine.Random.Range (0, 100);
					if (assassinateChance < 20) {
						Assassinate (this, targetKingdom.assignedLord, targetKingdom);
					}
				} else {
					if (targetKingdom.SuccessionRoyaltiesHasLoyaltyTo (this)) {
						Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE NEXT IN LINE TO THE LORD AT WAR...");
						int assassinateChance = UnityEngine.Random.Range (0, 100);
						if (assassinateChance < 50) {
							Assassinate (this, targetKingdom.royaltyList.successionRoyalties [0], targetKingdom);
						} 
					} else {
						Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT WAR...");
						int assassinateChance = UnityEngine.Random.Range (0, 100);
						if (assassinateChance < 20) {
							Assassinate (this, targetKingdom.assignedLord, targetKingdom);
						} 
					}
				}
			} else {
				Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT WAR...");
				int assassinateChance = UnityEngine.Random.Range (0, 100);
				if (assassinateChance < 20) {
					Assassinate (this, targetKingdom.assignedLord, targetKingdom);
				} 
			}
		}else{
			if (peaceRelations.Count > 0) {
				RelationshipKingdoms targetKingdom = peaceRelations [UnityEngine.Random.Range (0, peaceRelations.Count)];
				if (targetKingdom.kingdom.royaltyList.successionRoyalties.Count > 0) {
					if (targetKingdom.kingdom.royaltyList.successionRoyalties [0].loyalLord.id == this.id) {
						Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT PEACE...");
						int assassinateChance = UnityEngine.Random.Range (0, 100);
						if (assassinateChance < 20) {
							Assassinate (this, targetKingdom.kingdom.assignedLord, targetKingdom.kingdom);
						}
					} else {
						if (targetKingdom.kingdom.SuccessionRoyaltiesHasLoyaltyTo (this)) {
							Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE NEXT IN LINE TO THE LORD AT PEACE...");
							int assassinateChance = UnityEngine.Random.Range (0, 100);
							if (assassinateChance < 50) {
								Assassinate (this, targetKingdom.kingdom.royaltyList.successionRoyalties [0], targetKingdom.kingdom);
							} 
						} else {
							Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT PEACE...");
							int assassinateChance = UnityEngine.Random.Range (0, 100);
							if (assassinateChance < 20) {
								Assassinate (this, targetKingdom.kingdom.assignedLord, targetKingdom.kingdom);
							} 
						}
					}
				} else {
					Debug.Log (this.name + " IS ATTEMPTING TO ASSASSINATE A LORD AT PEACE...");
					int assassinateChance = UnityEngine.Random.Range (0, 100);
					if (assassinateChance < 20) {
						Assassinate (this, targetKingdom.kingdom.assignedLord, targetKingdom.kingdom);
					} 
				}

				int caughtChance = UnityEngine.Random.Range (0, 100);
				if(caughtChance < 20){
					Debug.Log (this.name + " GOT CAUGHT!");
					targetKingdom.isAtWar = true;

					RelationshipKingdoms relationFromTarget = targetKingdom.kingdom.SearchRelationshipKingdomsById (this.kingdom.id);

					if(relationFromTarget != null){
						relationFromTarget.isAtWar = true;
					}
						
				}
			}
		}
	}
	private void Assassinate(Royalty assassin, Royalty victim, KingdomTest targetKingdom){

		victim.kingdom.royaltyList.allRoyalties.Remove (victim);
		victim.kingdom.royaltyList.successionRoyalties.Remove (victim);

		if (targetKingdom.assignedLord.id == victim.id){
			if (victim.kingdom.royaltyList.successionRoyalties.Count <= 0) {
				victim.kingdom.AssignNewLord (null);
			} else {
				victim.kingdom.AssignNewLord (victim.kingdom.royaltyList.successionRoyalties [0]);
			}
	
		}

		Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + assassin.name + " of " + assassin.kingdom.kingdomName + " ASSASSINATED " + victim.name + " of " + victim.kingdom.kingdomName);
	}

	internal void Conversion(){
		KingdomTest targetKingdom = this.kingdom.relationshipKingdoms [UnityEngine.Random.Range (0, this.kingdom.relationshipKingdoms.Count)].kingdom;
		Royalty targetRoyalty = null;
		int targetChance = UnityEngine.Random.Range (0, 100);
		if (targetKingdom.royaltyList.successionRoyalties.Count > 3) {
			if(targetChance < 20){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [0];
			}else if(targetChance >= 21 && targetChance < 50){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [1];
			}else if(targetChance >= 51 && targetChance < 90){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [2];
			}else{
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [UnityEngine.Random.Range (3, targetKingdom.royaltyList.successionRoyalties.Count)];
			}
		} else if (targetKingdom.royaltyList.successionRoyalties.Count > 2) {
			if(targetChance < 24){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [0];
			}else if(targetChance >= 24 && targetChance < 57){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [1];
			}else{
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [2];
			}
		} else if (targetKingdom.royaltyList.successionRoyalties.Count > 1) {
			if(targetChance < 49){
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [0];
			}else{
				targetRoyalty = targetKingdom.royaltyList.successionRoyalties [1];
			}
		} else if (targetKingdom.royaltyList.successionRoyalties.Count > 0) {
			targetRoyalty = targetKingdom.royaltyList.successionRoyalties [0];
		}


		if (targetRoyalty != null) {
			Debug.Log ("Attempting to convert " + targetRoyalty.name);
			if (!targetRoyalty.trait.Contains (TRAIT.LOYAL)) {
				if (targetRoyalty.hatedLord == null || targetRoyalty.hatedLord.id != this.id) {
					if (targetRoyalty.previousConversionMonth == 0) {
						if (targetRoyalty.trait.Contains (TRAIT.TRAITOR)) {
							int conversionChance = UnityEngine.Random.Range (0, 100);
							if (conversionChance < 80) {
								targetRoyalty.ChangeLoyalty (this);
							}
						} else {
							int conversionChance = UnityEngine.Random.Range (0, 100);
							if (conversionChance < 30) {
								targetRoyalty.ChangeLoyalty (this);
							}
						}
					} else {
						int monthDifference = PoliticsPrototypeManager.Instance.month - targetRoyalty.previousConversionMonth;
						int yearDifference = PoliticsPrototypeManager.Instance.year - targetRoyalty.previousConversionYear;
						int difference = monthDifference + (yearDifference * 12);

						if (difference >= 3) {
							if (targetRoyalty.trait.Contains (TRAIT.TRAITOR)) {
								int conversionChance = UnityEngine.Random.Range (0, 100);
								if (conversionChance < 80) {
									targetRoyalty.ChangeLoyalty (this);
								}
							} else {
								int conversionChance = UnityEngine.Random.Range (0, 100);
								if (conversionChance < 30) {
									targetRoyalty.ChangeLoyalty (this);
								}
							}	
						}

					}

				}
			} 
		} else {
			Debug.LogWarning("No Conversion Target!");
		}
	}

	internal bool IsRoyaltyCloseRelative(Royalty otherRoyalty){
		if (otherRoyalty.id == this.father.id || otherRoyalty.id == this.mother.id) {
			//royalty is father or mother
			return true;
		}

		if (this.father.father != null && this.father.mother != null && this.mother.father != null && this.mother.mother != null) {
			if (otherRoyalty.id == this.father.father.id || otherRoyalty.id == this.father.mother.id ||
			  otherRoyalty.id == this.mother.father.id || otherRoyalty.id == this.mother.mother.id) {
				//royalty is grand parent
				return true;
			}
		}

		for (int i = 0; i < this.father.children.Count; i++) {
			if(otherRoyalty.id == this.father.children[i].id){
				//royalty is sibling
				return true;
			}
		}

		if (this.father.father != null) {
			for (int i = 0; i < this.father.father.children.Count; i++) {
				if (otherRoyalty.id == this.father.father.children [i].id) {
					//royalty is uncle or aunt from fathers side
					return true;
				}
			}
		}

		if (this.mother.father != null) {
			for (int i = 0; i < this.mother.father.children.Count; i++) {
				if (otherRoyalty.id == this.mother.father.children [i].id) {
					//royalty is uncle or aunt from mothers side
					return true;
				}
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

		if (this.age < 16) {
			Debug.Log ("Royalties below age 16 can't be married!");
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

				if (currentBachelorette.kingdom.id == currentBachelor.loyalLord.kingdom.id) {
					mostElligibleBridesPoints.Add (BRIDE_CRITERIA.IS_LOYAL_TO_SAME_LORD);
				}

				if (currentBachelorette.kingdom.cities.Count > currentBachelor.kingdom.cities.Count) {
					mostElligibleBridesPoints.Add (BRIDE_CRITERIA.HAS_MORE_CITIES);
				}

				if (currentBachelor.hatedLord == null || currentBachelorette.kingdom.id != currentBachelor.hatedLord.kingdom.id) {
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
