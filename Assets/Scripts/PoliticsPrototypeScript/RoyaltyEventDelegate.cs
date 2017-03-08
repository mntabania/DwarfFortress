using UnityEngine;
using System.Collections;

public class RoyaltyEventDelegate: MonoBehaviour {

	public delegate void IncreaseIllnessAndAccidentChance();
	public static event IncreaseIllnessAndAccidentChance onIncreaseIllnessAndAccidentChance;

	public delegate void ChangeIsDirectDescendant(bool status);
	public static event ChangeIsDirectDescendant onChangeIsDirectDescendant;

	public delegate void MassChangeLoyalty(Royalty newLord, Royalty previousLord);
	public static event MassChangeLoyalty onMassChangeLoyalty;


	public static void TriggerIncreaseIllnessAndAccidentChance(){
		if(onIncreaseIllnessAndAccidentChance != null){
			onIncreaseIllnessAndAccidentChance ();
		}
	}

	public static void TriggerChangeIsDirectDescendant(bool status){
		if(onChangeIsDirectDescendant != null){
			onChangeIsDirectDescendant (status);
		}
	}

	public static void TriggerMassChangeLoyalty(Royalty newLord, Royalty previousLord){
		if(onMassChangeLoyalty != null){
			onMassChangeLoyalty (newLord, previousLord);
		}
	}
}
