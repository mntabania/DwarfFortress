using UnityEngine;
using System.Collections;

public class RoyaltyEventDelegate: MonoBehaviour {

	public delegate void IncreaseIllnessAndAccidentChance();
	public static event IncreaseIllnessAndAccidentChance onIncreaseIllnessAndAccidentChance;

	public static void TriggerIncreaseIllnessAndAccidentChance(){
		if(onIncreaseIllnessAndAccidentChance != null){
			onIncreaseIllnessAndAccidentChance ();
		}
	}
}
