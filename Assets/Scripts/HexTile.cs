﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexTile : MonoBehaviour {

	public Tile tile;

	public GameObject path;
	public GameObject northPath;
	public GameObject northEastPath;
	public GameObject southEastPath;
	public GameObject southPath;
	public GameObject southWestPath;
	public GameObject northWestPath;

	public float elevationNoise;
	public float moistureNoise;
	public float temperature;

	public int woodValue = 0;
	public int stoneValue = 0;
	public int manaStoneValue = 0;
	public int farmingValue = 0;
	public int huntingValue = 0;

	public BIOMES biomeType;
	public ELEVATION elevationType;

	public bool isCity = false;
	public bool isRoad = false;
	public bool isOccupied = false;

//		switch(biomeType){
//		case BIOME.OCEAN:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f/255f,1f/255f,105f/255f);
//			break;
//		case BIOME.BEACH:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f/255f,1f/255f,105f/255f);
//			break;
//		case BIOME.SUBTROPICAL_DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(231f/255f,221f/255f,196f/255f);
//			break;
//		case BIOME.GRASSLAND:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(194f/255f,213f/255f,168f/255f);
//			break;
//		case BIOME.TEMPERATE_DECIDUOUS_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(178f/255f,202f/255f,168f/255f);
//			break;
//		case BIOME.TEMPERATE_RAIN_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(162f/255f,197f/255f,169f/255f);
//			break;
//		case BIOME.TEMPERATE_DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(226f/255f,231f/255f,201f/255f);
//			break;
//		case BIOME.SHRUBLAND:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(195f/255f,204f/255f,186f/255f);
//			break;
//		case BIOME.TAIGA:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(203f/255f,212f/255f,185f/255f);
//			break;
//		case BIOME.SCORCHED:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(153f/255f,153f/255f,153f/255f);
//			break;
//		case BIOME.BARE:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(186f/255f,186f/255f,186f/255f);
//			break;
//		case BIOME.TUNDRA:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(220f/255f,221f/255f,187f/255f);
//			break;
//		case BIOME.SNOW:
//			this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
//			break;
//		case BIOME.TROPICAL_RAIN_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(156f/255f,188f/255f,167f/255f);
//			break;
//		case BIOME.FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(156f/255f,188f/255f,167f/255f);
//			break;
//		case BIOME.JUNGLE:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(194f/255f,213f/255f,168f/255f);
//			break;
//		case BIOME.SAVANNAH:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(231f/255f,221f/255f,196f/255f);
//			break;
//		case BIOME.DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(234f/255f,194f/255f,60f/255f);
//			break;
//		}
//	}

	[ContextMenu("Check Tile Can Pass")]
	public void CheckIfTileIsPassable(){
		Debug.Log (name + " is Passable?: " + tile.canPass);
	}

	[ContextMenu("Show Neighbours")]
	public void ShowNeighbours(){
		Debug.Log ("======" + this.name + " - " + tile.ValidTiles.ToList().Count + "======");
		foreach (Tile t in tile.ValidTiles) {
			Debug.Log ("Neighbours: " + t.hexTile.name);
		}
	}

	[ContextMenu("Set Tile as Unpassable")]
	public void SetTileAsUnpassable(){
		tile.canPass = false;
	}

	public void ActivatePath(PATH_DIRECTION direction){
		switch (direction) {
		case PATH_DIRECTION.NORTH:
			northPath.SetActive (true);
			break;
		case PATH_DIRECTION.NORTH_EAST:
			northEastPath.SetActive (true);
			break;
		case PATH_DIRECTION.NORTH_WEST:
			northWestPath.SetActive (true);
			break;
		case PATH_DIRECTION.SOUTH:
			southPath.SetActive (true);
			break;
		case PATH_DIRECTION.SOUTH_EAST:
			southEastPath.SetActive (true);
			break;
		case PATH_DIRECTION.SOUTH_WEST:
			southWestPath.SetActive (true);
			break;
		}
	}

	public void SetTileColor(Color color){
		gameObject.GetComponent<SpriteRenderer> ().color = color;
	}

	/*
	 * TODO: Revise this to use the new formula 
	 * in getting a hexagon's neighbours. Much more
	 * optimized that way
	 * 
	 * Returns all Hex tiles within a radius
	 * */
	public HexTile[] GetTilesInRange(float radius){
		Collider2D[] nearHexes = Physics2D.OverlapCircleAll (new Vector2(transform.position.x, transform.position.y), radius);
		HexTile[] nearTiles = new HexTile[nearHexes.Length];
		for (int i = 0; i < nearTiles.Length; i++) {
			nearTiles[i] = nearHexes[i].gameObject.GetComponent<HexTile> ();
		}
		return nearTiles;
	}

	public List<HexTile> GetListTilesInRange(float radius){
		Collider2D[] nearHexes = Physics2D.OverlapCircleAll (new Vector2(transform.position.x, transform.position.y), radius);
		List<HexTile> nearTiles = new List<HexTile> ();
		for (int i = 0; i < nearHexes.Length; i++) {
			if (nearHexes[i].gameObject == null) {
				continue;
			}
			if (!nearHexes[i].gameObject.GetComponent<HexTile> ().isCity) {
				if(!nearHexes[i].gameObject.GetComponent<HexTile> ().isOccupied){
					nearTiles.Add(nearHexes[i].gameObject.GetComponent<HexTile>());
				}
			}
		}
		return nearTiles;
	}

	public CityTile GetCityTile(){
		return gameObject.GetComponent<CityTile>();
	}

	public void GenerateResourceValues(){
		Debug.Log ("Generate Resource Values!");
		Dictionary<JOB_TYPE, int[]> chancesDict = biomeResourceChances[biomeType];
		for (int i = 0; i < chancesDict.Keys.Count; i++) {
			int choice = Random.Range (0, 100);
			JOB_TYPE currentJobType = chancesDict.Keys.ElementAt(i);
			int[] chancesForCurrentJobType = chancesDict [currentJobType];

			int upperBound = 0;
			int lowerBound = 0;
			int generatedResourceValue = 0;
			for (int j = 0; j < chancesForCurrentJobType.Length; j++) {
				upperBound += chancesForCurrentJobType [j];
				if (choice >= lowerBound && choice < upperBound) {
					if (j == 0) {
						generatedResourceValue = Random.Range (21, 36);
					} else if (j == 1) {
						generatedResourceValue = Random.Range (36, 46);
					} else if (j == 2) {
						generatedResourceValue = Random.Range (10, 21);
					}
					break;
				}
				lowerBound = upperBound;
			}

			if (currentJobType == JOB_TYPE.FARMER) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					generatedResourceValue -= Random.Range (10, 21);
				}
				farmingValue = generatedResourceValue;
			} else if (currentJobType == JOB_TYPE.HUNTER) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					generatedResourceValue += Random.Range (10, 21);
				}
				huntingValue = generatedResourceValue;
			} else if (currentJobType == JOB_TYPE.WOODSMAN) {
				woodValue = generatedResourceValue;
			} else if (currentJobType == JOB_TYPE.MINER) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					generatedResourceValue += Random.Range (10, 21);
				}
				stoneValue = generatedResourceValue;
			} else if (currentJobType == JOB_TYPE.ALCHEMIST) {
				manaStoneValue = generatedResourceValue;
			}
		}
	}	

	private BIOME GetBiome(){
		if(elevationNoise <= 0.35f){
			if(elevationNoise < 0.30f){
				return BIOME.OCEAN;
			}
			return BIOME.BEACH;
		}else{
			if(elevationNoise > 0.3f && elevationNoise < 0.6f){
				if(moistureNoise < 0.16f){
					return BIOME.SUBTROPICAL_DESERT;
				}
				if(moistureNoise < 0.50f){
					return BIOME.GRASSLAND;
				}
				if(moistureNoise < 0.83f){
					return BIOME.TEMPERATE_DECIDUOUS_FOREST;
				}
				return BIOME.TEMPERATE_RAIN_FOREST;
			}
			else if(elevationNoise >= 0.6f && elevationNoise < 0.8f){
				if(moistureNoise < 0.33f){
					return BIOME.TEMPERATE_DESERT;
				}
				if(moistureNoise < 0.66f){
					return BIOME.SHRUBLAND;
				}
				return BIOME.TAIGA;
			}
			else if(elevationNoise >= 0.8f){
				if(moistureNoise < 0.1f){
					return BIOME.SCORCHED;
				}
				if(moistureNoise < 0.2f){
					return BIOME.BARE;
				}
				if(moistureNoise < 0.5f){
					return BIOME.TUNDRA;
				}
				return BIOME.SNOW;
			}
		}		
		return BIOME.TROPICAL_RAIN_FOREST;

	}

	void OnMouseDown(){
//		UserInterfaceManager.Instance.SetCityInfoToShow (gameObject.GetComponent<CityTileTest>());
	}Dictionary<BIOMES, Dictionary<JOB_TYPE, int[]>> biomeResourceChances = new Dictionary<BIOMES, Dictionary<JOB_TYPE, int[]>>(){
		{BIOMES.GRASSLAND, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{20, 75, 5}},
				{JOB_TYPE.HUNTER, new int[]{5, 0, 95}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{20, 5, 75}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},

		{BIOMES.WOODLAND, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{75, 20, 5}},
				{JOB_TYPE.HUNTER, new int[]{75, 20, 5}},
				{JOB_TYPE.WOODSMAN, new int[]{75, 20, 5}},
				{JOB_TYPE.MINER, new int[]{20, 5, 75}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},

		{BIOMES.FOREST, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{20, 75, 5}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{5, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.DESERT, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{20, 5, 75}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{75, 20, 5}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.TUNDRA, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{20, 5, 75}},
				{JOB_TYPE.HUNTER, new int[]{20, 5, 75}},
				{JOB_TYPE.WOODSMAN, new int[]{20, 5, 75}},
				{JOB_TYPE.MINER, new int[]{50, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{20, 5, 75}},
			}
		},

		{BIOMES.SNOW, new Dictionary<JOB_TYPE, int[]>(){
				{JOB_TYPE.FARMER, new int[]{5, 0, 95}},
				{JOB_TYPE.HUNTER, new int[]{5, 0, 95}},
				{JOB_TYPE.WOODSMAN, new int[]{5, 0, 95}},
				{JOB_TYPE.MINER, new int[]{5, 0, 95}},
				{JOB_TYPE.ALCHEMIST, new int[]{5, 0, 95}},
			}
		},
	};



}
