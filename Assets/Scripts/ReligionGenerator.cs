using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ReligionGenerator : MonoBehaviour {

	public static ReligionGenerator Instance;

	public ReligionList religionList;

	void Awake(){
		Instance = this;
	}

	internal Religion GenerateReligion(){
		Religion religion = new Religion ();
		religion.gods = GetGods ();
		religion.focus = GetFocus ();
		religion.expectation = GetExpectation ();
		religion.forbidden = GetForbidden ();

		if(religion.gods.Length == 2){
			religion.sentence = "This religion worship " + religion.gods [0] + " and " + religion.gods [1] + ". They are focused on " + religion.focus [0] + " and " + religion.focus [1]
			+ ". People are expected to be " + religion.expectation [0] + " and " + religion.expectation [1] + ". They are not allowed " + religion.forbidden [0] + " and " + religion.forbidden [1] + ".";
				
		}else{
			religion.sentence = "This religion worships " + religion.gods [0] + ". They are focused on " + religion.focus [0] + " and " + religion.focus [1]
				+ ". People are expected to be " + religion.expectation [0] + " and " + religion.expectation [1] + ". They are not allowed " + religion.forbidden [0] + " and " + religion.forbidden [1] + ".";
		}

		return religion;
	}

	private string[] GetGods(){
		int noOfGods = UnityEngine.Random.Range (1, 3);

		string[] gods = new string[noOfGods];
		List<string> listReligion = new List<string> (religionList.gods);
		for(int i = 0; i < gods.Length; i++){
			int randomGod = UnityEngine.Random.Range (0, listReligion.Count);
			gods [i] = listReligion [randomGod];
			listReligion.RemoveAt (randomGod);
		}

		return gods;
	}

	private string[] GetFocus(){
		string[] focus = new string[2];
		List<string> listReligion = new List<string> (religionList.focus);
		for(int i = 0; i < focus.Length; i++){
			int randomFocus = UnityEngine.Random.Range (0, listReligion.Count);
			focus [i] = listReligion [randomFocus];
			listReligion.RemoveAt (randomFocus);
		}

		return focus;
	}

	private string[] GetExpectation(){
		string[] expectation = new string[2];
		List<string> listReligion = new List<string> (religionList.expectation);
		for(int i = 0; i < expectation.Length; i++){
			int randomExpectation = UnityEngine.Random.Range (0, listReligion.Count);
			expectation [i] = listReligion [randomExpectation];
			listReligion.RemoveAt (randomExpectation);
		}

		return expectation;
	}

	private string[] GetForbidden(){
		string[] forbidden = new string[2];
		List<string> listReligion = new List<string> (religionList.forbidden);
		for(int i = 0; i < forbidden.Length; i++){
			int randomForbidden = UnityEngine.Random.Range (0, listReligion.Count);
			forbidden [i] = listReligion [randomForbidden];
			listReligion.RemoveAt (randomForbidden);
		}

		return forbidden;
	}
}
