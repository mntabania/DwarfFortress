using UnityEngine;
using System.Collections;

public class KingdomListItem : MonoBehaviour {

	[HideInInspector] public KingdomTileTest kingdom;

	public void ShowKingdomInfo(){
		PoliticsPrototypeUIManager.Instance.ShowKingdomInfo(kingdom.kingdom);
	}
}
