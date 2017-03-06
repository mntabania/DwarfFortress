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
	public RoyaltyChances royaltyChances;
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
		this.royaltyChances = new RoyaltyChances ();
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
	internal void IncreaseIllnessAndAccidentChance(){
		this.royaltyChances.illnessChance += 0.01f;
		this.royaltyChances.accidentChance += 0.01f;
	}
	internal void IllnessAndAccidents(){
		float illness = UnityEngine.Random.Range (0f, 99f);
		if(illness <= this.royaltyChances.illnessChance){
			this.isDead = true;
			Death ();
			Debug.Log (this.name + " DIED OF ILLNESS!");
		}else{
			float accidents = UnityEngine.Random.Range (0f, 99f);
			if(accidents <= this.royaltyChances.accidentChance){
				this.isDead = true;
				Death ();
				Debug.Log (this.name + " DIED OF ACCIDENT!");
			}
		}
	}

	internal void OldAge(){
		if(this.age >= 60){
			float oldAge = UnityEngine.Random.Range (0f, 99f);
			if(oldAge <= this.royaltyChances.oldAgeChance){
				this.isDead = true;
				Death ();
				Debug.Log (this.name + " DIED OF OLD AGE!");
			}else{
				this.royaltyChances.oldAgeChance += 0.05f;
			}
		}
	}
	internal void Death(){
		if(this.id == this.kingdom.assignedLord.id){
			//ASSIGN NEW LORD, SUCCESSION
			this.kingdom.royaltyList.allRoyalties.Remove (this);

		}else{
			this.kingdom.royaltyList.allRoyalties.Remove (this);
		}
	}
}
