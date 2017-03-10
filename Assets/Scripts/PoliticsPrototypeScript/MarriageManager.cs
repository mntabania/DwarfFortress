using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MarriageManager : MonoBehaviour {

	public static MarriageManager Instance;

	public PoliticsPrototypeManager.TurnEndedDelegate marriageEvents;

	void Awake(){
		Instance = this;
//		marriageEvents += CheckForMarriage;
		marriageEvents += CheckForPregnancy;
	}

	void Start(){
		PoliticsPrototypeManager.Instance.turnEnded += marriageEvents;
	}

	List<Royalty> GetElligibleBachelorsForMarriage(){
		List<Royalty> elligibleBachelors = new List<Royalty>();
		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			elligibleBachelors.AddRange(PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.elligibleBachelors);
		}
		return elligibleBachelors;
	}

	public List<Royalty> GetElligibleBachelorettesForMarriage(){
		List<Royalty> elligibleBachelorettes = new List<Royalty>();
		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			elligibleBachelorettes.AddRange(PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.elligibleBachelorettes);
		}
		return elligibleBachelorettes.OrderBy(x => x.age).ToList(); //younger women are prioritized
	}


	void CheckForPregnancy(){
		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			for (int j = 0; j < PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.marriedCouples.Count; j++) {
				MarriedCouple couple = PoliticsPrototypeManager.Instance.kingdoms [i].kingdom.marriedCouples [j];
				if (couple.husband.isDead || couple.wife.isDead || couple.isPregnant) {
					continue;
				}
				if (couple.husband.age < 60 && couple.wife.age < 40 && couple.children.Count < 3 ) {
					float chance = Random.Range(0f, 100f);
					if (chance < couple.chanceForPregnancy) {
						couple.chanceForPregnancy = 0.5f;
						Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + couple.husband.name + " and " + couple.wife.name + " will have a baby in 9 months");
						couple.isPregnant = true;
						PoliticsPrototypeManager.Instance.turnEnded += couple.pregnancyActions;
//						MakeBaby(couple.husband, couple.wife);
					} else {
						couple.chanceForPregnancy += 0.5f;
					}
				}
			}
		}
	}

	void CheckForMarriage(){
		List<Royalty> elligibleBachelors = GetElligibleBachelorsForMarriage();
		List<Royalty> elligibleBachelorettes = GetElligibleBachelorettesForMarriage();
		for (int i = 0; i < elligibleBachelors.Count; i++) {
			Royalty currentBachelor = elligibleBachelors[i];
			Royalty mostElligibleBride = null;
			List<BRIDE_CRITERIA> mostElligibleBridesPoints = new List<BRIDE_CRITERIA>();
			//find bride
			for (int j = 0; j < elligibleBachelorettes.Count; j++) {
				if (currentBachelor.IsRoyaltyCloseRelative (elligibleBachelorettes [j])) {
					//currentBachelorette is a close relative, DO NOT MARRY
					continue;
				}
				Royalty currentBachelorette = elligibleBachelorettes[j];
				if (mostElligibleBride == null) {
					mostElligibleBride = currentBachelorette;

					if (currentBachelorette.loyalLord.id == currentBachelor.loyalLord.id) {
						mostElligibleBridesPoints.Add (BRIDE_CRITERIA.IS_LOYAL_TO_SAME_LORD);
					}

					if (currentBachelorette.kingdom.cities.Count > currentBachelor.kingdom.cities.Count) {
						mostElligibleBridesPoints.Add (BRIDE_CRITERIA.HAS_MORE_CITIES);
					}

					if (currentBachelorette.kingdom.lord.id != currentBachelor.hatedLord.id) {
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

					if (currentBachelorette.kingdom.lord.id != currentBachelor.hatedLord.id) {
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
				Marry (currentBachelor, mostElligibleBride);
				elligibleBachelorettes = GetElligibleBachelorettesForMarriage();
			} else {
				Debug.Log ("Could not find bride for :" + currentBachelor.name);
			}
		}
	}

	internal Royalty MakeBaby(Royalty father, Royalty mother, int age = 0, bool isLord = false){
		GENDER gender = (GENDER)(UnityEngine.Random.Range (0, System.Enum.GetNames (typeof(GENDER)).Length));
//		int age = 0;
		if(isLord){
			int randomGender = UnityEngine.Random.Range (0, 100);
			if(randomGender < 20){
				gender = GENDER.FEMALE;
			}else{
				gender = GENDER.MALE;
			}

//			age = UnityEngine.Random.Range (16, 36);
		}
		Royalty child = new Royalty(father.kingdom, age, gender, father.generation + 1);
		if(father.isDirectDescendant || mother.isDirectDescendant){
			child.isDirectDescendant = true;
		}
		father.AddChild (child);
		mother.AddChild (child);
		child.AddParents(father, mother);
		if(child.isDirectDescendant){
			child.kingdom.UpdateLordSuccession ();
		}
//		father.kingdom.royaltyList.allRoyalties.Add (child);

		return child;
	}

	internal Royalty CreateSpouse(Royalty otherSpouse){
		GENDER gender = GENDER.FEMALE;
		if(otherSpouse.gender == GENDER.FEMALE){
			gender = GENDER.MALE;
		}
		Royalty spouse = new Royalty(otherSpouse.kingdom, UnityEngine.Random.Range(16, (otherSpouse.age + 1)), gender, otherSpouse.generation);

		Marry (otherSpouse, spouse);
		return spouse;
	}

	internal void Marry(Royalty husband, Royalty wife){
		Debug.Log (PoliticsPrototypeManager.Instance.month + "/" + PoliticsPrototypeManager.Instance.week + "/" + PoliticsPrototypeManager.Instance.year + ": " + husband.name + " got married to " + wife.name);
		husband.spouse = wife;
		wife.spouse = husband;
		husband.isMarried = true;
		wife.isMarried = true;
		husband.isIndependent = true;
		wife.isIndependent = true;

		//the wife will transfer to the court of the husband
		wife.kingdom = husband.kingdom;
//		wife.loyalLord = husband.kingdom.assignedLord;
//		husband.kingdom.royaltyList.allRoyalties.Add(wife);
//		wife.kingdom.royaltyList.allRoyalties.Remove(wife);

		husband.kingdom.marriedCouples.Add(new MarriedCouple (husband, wife));
	}
}
