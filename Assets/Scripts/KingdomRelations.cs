using UnityEngine;
using System.Collections;

[System.Serializable]
public class KingdomRelations {
	public KingdomTile targetKingdom;
	public bool isAtWar;
	public bool isAdjacent;
	public int citiesGained;
	public int citiesLost;
	public int turnsAtPeace;
	public int turnsAtWar;


	public KingdomRelations(){
		this.targetKingdom = null;
		this.isAtWar = false;
		this.isAdjacent = false;
		this.citiesGained = 0;
		this.citiesLost = 0;
		this.turnsAtPeace = 0;
		this.turnsAtWar = 0;
	}
}
