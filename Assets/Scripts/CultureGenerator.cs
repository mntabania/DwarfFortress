using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CultureGenerator : MonoBehaviour {

	public static CultureGenerator Instance;

	public CultureList cultureList;

	void Awake(){
		Instance = this;
	}

	internal Culture GenerateCulture(){
		Culture culture = new Culture ();
		int randomName = UnityEngine.Random.Range (0, cultureList.names.Count);

		culture.cultureName = cultureList.names[randomName];
		culture.valuedTrait = cultureList.valuedTrait [UnityEngine.Random.Range (0, cultureList.valuedTrait.Length)];
		culture.popularQuirk = cultureList.popularQuirk [UnityEngine.Random.Range (0, cultureList.popularQuirk.Length)];
		culture.primaryEntertainment = cultureList.primaryEntertainment [UnityEngine.Random.Range (0, cultureList.primaryEntertainment.Length)];
		culture.respectedProfession = cultureList.respectedProfession [UnityEngine.Random.Range (0, cultureList.respectedProfession.Length)];
		culture.discrimination = cultureList.discrimination [UnityEngine.Random.Range (0, cultureList.discrimination.Length)];
		culture.majorTaboo = cultureList.majorTaboo [UnityEngine.Random.Range (0, cultureList.majorTaboo.Length)];
		culture.cultureSentence = "This culture values " + culture.valuedTrait + ". They are known for " + culture.popularQuirk + ". They enjoy " + culture.primaryEntertainment + ". " + culture.respectedProfession 
			+ " are very important in their society. They discriminate " + culture.discrimination + ". " + culture.majorTaboo + " is forbidden";

		cultureList.names.RemoveAt (randomName);

		return culture;
	}
}
