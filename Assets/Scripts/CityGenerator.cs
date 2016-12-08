using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour {

	public static CityGenerator Instance = null;

	public int intCityCount;

	List<BIOMES> elligibleBiomes;

	void Awake(){
		Instance = this;
	}

	public void GenerateCities(){
		CheckForUnelligibleBiomes ();
		for (int i = 0; i < intCityCount; i++) {
			BIOMES chosenBiome = RollForCityBiome();
//			Debug.Log ("Chosen Biome-" + i + ": " + chosenBiome);
			if (!elligibleBiomes.Contains (chosenBiome)) {
				i--;
			} else {
				if (chosenBiome == BIOMES.SNOW) {
					PlaceCityOnBiomeTile (Biomes.Instance.snowHexTiles);
				} else if (chosenBiome == BIOMES.TUNDRA) {
					PlaceCityOnBiomeTile (Biomes.Instance.tundraHexTiles);
				} else if (chosenBiome == BIOMES.GRASSLAND) {
					PlaceCityOnBiomeTile (Biomes.Instance.grasslandHexTiles);
				} else if (chosenBiome == BIOMES.WOODLAND) {
					PlaceCityOnBiomeTile (Biomes.Instance.woodsHexTiles);
				} else if (chosenBiome == BIOMES.FOREST) {
					PlaceCityOnBiomeTile (Biomes.Instance.forestHexTiles);
				} else if (chosenBiome == BIOMES.DESERT) {
					PlaceCityOnBiomeTile (Biomes.Instance.desertHexTiles);
				}
			}

		}
	}

	/*
	 * Check if there is at least 1 tile from 
	 * each biome present, eliminate all biomes that don't have tiles
	 * from city selection
	 * */
	void CheckForUnelligibleBiomes(){
		elligibleBiomes = new List<BIOMES>();
		if(Biomes.Instance.snowHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.SNOW);
		}
		if(Biomes.Instance.tundraHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.TUNDRA);
		}
		if(Biomes.Instance.grasslandHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.GRASSLAND);
		}
		if(Biomes.Instance.woodsHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.WOODLAND);
		}
		if(Biomes.Instance.forestHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.FOREST);
		}
		if(Biomes.Instance.desertHexTiles.Count >= 1){
			elligibleBiomes.Add(BIOMES.DESERT);
		}
	}


	/*
	 * Randomly choose which biome a 
	 * city will spawn in, current chances are the ff:
	 * Snow - 5% - 0~4
	 * Tundra - 10% - 5~14
	 * Grassland - 40% - 15~54
	 * Woods - 30% - 55~84
	 * Forest - 10% - 85~94
	 * Desert - 5% - 95~100
	 * */
	BIOMES RollForCityBiome(){
		int roll = Random.Range (0, 101);
		if (roll >= 0 && roll <= 4) {
			//Snow Biome
			return BIOMES.SNOW;
		} else if (roll >= 5 && roll <= 14) {
			//Tundra
			return BIOMES.TUNDRA;
		} else if (roll >= 15 && roll <= 54) {
			//Grassland
			return BIOMES.GRASSLAND;
		} else if (roll >= 55 && roll <= 84) {
			//Woods
			return BIOMES.WOODLAND;
		} else if (roll >= 85 && roll <= 94) {
			//Forest
			return BIOMES.FOREST;
		} else if (roll >= 95 && roll <= 100) {
			//Desert
			return BIOMES.DESERT;
		}
		return BIOMES.GRASSLAND;
	}


	/*
	 * Randomly choose a tile from a specific
	 * biome, and mark it as a city
	 * */
	void PlaceCityOnBiomeTile(List<HexTile> biomeTiles){
		int randomTileIndex = Random.Range (0, biomeTiles.Count);
//		Debug.Log ("Random Tile Index: " + randomTileIndex);
		biomeTiles [randomTileIndex].gameObject.GetComponent<SpriteRenderer> ().color = Color.black;
	}


}
