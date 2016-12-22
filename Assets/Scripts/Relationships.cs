using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Relationships : MonoBehaviour {
	public static Relationships Instance;

	public List<KingdomRelations> kingdomRelations = new List<KingdomRelations> ();

	void Awake(){
		Instance = this;
	}

	internal void AddKingdomRelationship(Kingdom kingdom1, Kingdom kingdom2, bool warpeaceStatus){
		KingdomRelations kingdomRelations = new KingdomRelations ();
//		kingdomRelations.targetKingdom = kingdom1;
//		kingdomRelations.kingdom2 = kingdom2;
//		kingdomRelations.warpeaceStatus = warpeaceStatus;

		this.kingdomRelations.Add (kingdomRelations);
	}
}
