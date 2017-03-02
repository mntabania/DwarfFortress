using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliticsPrototypeManager : MonoBehaviour {

	public static PoliticsPrototypeManager Instance = null;

	public GameObject kingdomTilePrefab;
	public GameObject kingdomsParent;
	public List<KingdomTileTest> kingdoms;

	void Awake(){
		Instance = this;
	}

	void Start(){
		GridMap.Instance.GenerateGrid();
		EquatorGenerator.Instance.GenerateEquator();
		Biomes.Instance.GenerateElevation();
		Biomes.Instance.GenerateBiome();
		Biomes.Instance.GenerateTileDetails ();
		CityGenerator.Instance.GenerateCities();

		for (int i = 0; i < CityGenerator.Instance.cities.Count; i++) {
//			Destroy(CityGenerator.Instance.cities[i].gameObject.GetComponent<CityTile>());
			CityGenerator.Instance.cities[i].gameObject.AddComponent<CityTileTest>();
			CityGenerator.Instance.cities[i].gameObject.GetComponent<CityTileTest>().hexTile = CityGenerator.Instance.cities[i];
		}

		this.GenerateKingdoms();
	}

	void GenerateKingdoms(){
		List<HexTile> elligibleCities = new List<HexTile>(CityGenerator.Instance.cities);

		int numOfKingdomsToCreate = 4;
		for (int i = 0; i < numOfKingdomsToCreate; i++) {
			GameObject goKingdom = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
			goKingdom.transform.parent = kingdomsParent.transform;
			List<CityTileTest> citiesForKingdom = new List<CityTileTest>();
			for (int j = 0; j < 4; j++) {
				int randomIndex = Random.Range(0, elligibleCities.Count);
				citiesForKingdom.Add(elligibleCities[randomIndex].GetComponent<CityTileTest>());
				elligibleCities.RemoveAt(randomIndex);
			}
			Color kingdomColor = Color.white;
			if (i == 0) {
				kingdomColor = Color.clear;
			} else if (i == 1) {
				kingdomColor = Color.black;
			} else if (i == 2) {
				kingdomColor = Color.magenta;
			} else {
				kingdomColor = Color.blue;
			}
			goKingdom.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, citiesForKingdom, kingdomColor);
			goKingdom.name = goKingdom.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
			this.kingdoms.Add (goKingdom.GetComponent<KingdomTileTest>());
		}
	}

}
