using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipTestScript : MonoBehaviour {

	public GameObject[] hexTiles;
	public GameObject kingdomTilePrefab;
	public List<KingdomTile> kingdoms;

	// Use this for initialization
	void Start () {
		GenerateInitialKingdoms();
	}

	void GenerateInitialKingdoms(){
		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.HUMANS, new List<CityTile>(){hexTiles[2].GetComponent<CityTile>()}, "KINGDOM1", new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom1.GetComponent<KingdomTile>());

		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom2.transform.parent = this.transform;
		goKingdom2.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.ELVES, new List<CityTile>(){hexTiles[4].GetComponent<CityTile>()}, "KINGDOM2", new Color(40f/255f, 255f/255f, 0f/255f));
		goKingdom2.name = goKingdom2.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom2.GetComponent<KingdomTile>());
	}
	

}
