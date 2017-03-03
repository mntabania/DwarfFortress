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
	public TRAIT trait;
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
		this.birthMonth = GameManager.Instance.month;
		this.birthWeek = GameManager.Instance.week;
		this.birthYear = GameManager.Instance.year;
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
	internal TRAIT GetTrait(){
		int traitChance = UnityEngine.Random.Range (0, 100);
		if(traitChance < 40){
			return TRAIT.VICIOUS;
		}
		return TRAIT.NAIVE;
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
		for(int i = 0; i < GameManager.Instance.kingdoms.Count; i++){
			if(GameManager.Instance.kingdoms[i].kingdom.id != this.kingdom.id){
				hatedRoyaltyPool.Add (GameManager.Instance.kingdoms [i].kingdom.assignedLord);
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
}
