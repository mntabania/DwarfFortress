using UnityEngine;
using System.Collections;

public class MarriageManager : MonoBehaviour {

	public static MarriageManager Instance;

	void Awake(){
		Instance = this;
	}

	internal Royalty MakeBaby(Royalty father, Royalty mother, int age, bool isLord = false){
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

		father.AddChild (child);
		mother.AddChild (child);
//		father.kingdom.royaltyList.allRoyalties.Add (child);

		return child;
	}

	internal Royalty CreateSpouse(Royalty otherSpouse){
		GENDER gender = GENDER.FEMALE;
		if(otherSpouse.gender == GENDER.FEMALE){
			gender = GENDER.MALE;
		}
		Royalty spouse = new Royalty(otherSpouse.kingdom, UnityEngine.Random.Range(16, (otherSpouse.age + 1)), gender, otherSpouse.generation);

		MarrySpouse (otherSpouse, spouse);
		return spouse;
	}
	internal void MarrySpouse(Royalty otherSpouse, Royalty newSpouse){
		otherSpouse.spouse = newSpouse;
		newSpouse.spouse = otherSpouse;
		otherSpouse.isMarried = true;
		newSpouse.isMarried = true;
		otherSpouse.isIndependent = true;
		newSpouse.isIndependent = true;
	}
}
