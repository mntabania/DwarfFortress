using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipTestScript : MonoBehaviour {

	public GameObject[] hexTiles;
	public GameObject kingdomTilePrefab;
	public List<KingdomTileTest> kingdoms;

	// Use this for initialization
	void Start () {
		GenerateInitializeCities ();
		GenerateInitialKingdoms();
	}

	void GenerateInitializeCities(){
		for (int i = 0; i < hexTiles.Length; i++) {
			hexTiles [i].GetComponent<CityTileTest> ().cityAttributes = new CityTest (hexTiles [i].GetComponent<HexTile>(), BIOMES.GRASSLAND);
		}
		hexTiles[0].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[1].GetComponent<CityTileTest>());
		hexTiles[0].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[5].GetComponent<CityTileTest>());

		hexTiles[1].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[0].GetComponent<CityTileTest>());
		hexTiles[1].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[2].GetComponent<CityTileTest>());

		hexTiles[2].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[1].GetComponent<CityTileTest>());
		hexTiles[2].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[3].GetComponent<CityTileTest>());

		hexTiles[3].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[2].GetComponent<CityTileTest>());
		hexTiles[3].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[4].GetComponent<CityTileTest>());

		hexTiles[4].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[3].GetComponent<CityTileTest>());
		hexTiles[4].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[5].GetComponent<CityTileTest>());

		hexTiles[5].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[4].GetComponent<CityTileTest>());
		hexTiles[5].GetComponent<CityTileTest>().cityAttributes.connectedCities.Add(hexTiles[0].GetComponent<CityTileTest>());
	}

	void GenerateInitialKingdoms(){
		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){hexTiles[2].GetComponent<CityTileTest>()}, new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom1.GetComponent<KingdomTileTest>());

		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom2.transform.parent = this.transform;
		goKingdom2.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){hexTiles[5].GetComponent<CityTileTest>()}, new Color(40f/255f, 255f/255f, 0f/255f));
		goKingdom2.name = goKingdom2.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom2.GetComponent<KingdomTileTest>());
	}
	

}
