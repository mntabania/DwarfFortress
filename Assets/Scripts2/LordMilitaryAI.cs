using UnityEngine;
using System.Collections;

public class LordMilitaryAI : MonoBehaviour {

	public delegate void Instruct(LORD_INSTRUCTIONS instructions, CityTest city, CityTest targetCity);
	public static event Instruct onInstruct;

	public static void SendInstructions(LORD_INSTRUCTIONS instructions, CityTest city, CityTest targetCity){
		if(onInstruct != null){
			onInstruct (instructions, city, targetCity);
		}
	}
}
