using UnityEngine;
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

	public int farmingValue = 0;
	public int huntingValue = 0;
	public int woodValue = 0;
	public int stoneValue = 0;
	public int manaValue = 0;
	public int metalValue = 0;
	public int goldValue = 0;

	public BIOMES biomeType;
	public ELEVATION elevationType;

	public bool isCity = false;
	public bool isRoad = false;
	public bool isOccupied = false;

	public RESOURCE primaryResourceToPurchaseTile;
	public RESOURCE secondaryResourceToPurchaseTile;

	int[] allResourceValues;

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

	[ContextMenu("Show Combat Tiles")]
	public void ShowCombatTiles(){
		Utilities.targetCity = GameManager.Instance.targetHexTile.GetComponent<CityTileTest> ().cityAttributes;
		foreach (Tile t in tile.CombatTiles) {
			t.hexTile.SetTileColor (Color.red);
			Debug.Log ("Neighbours: " + t.hexTile.name);
		}
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

	public void SetTileBiomeType(BIOMES biomeType){
		this.biomeType = biomeType;
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
			if (nearHexes[i].gameObject == null || nearHexes[i].gameObject == this.gameObject) {
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
	public CityTileTest GetCityTileTest(){
		return gameObject.GetComponent<CityTileTest>();
	}

	public void GenerateResourceValues(){
		Dictionary<BIOME_PRODUCE_TYPE, int[]> chancesDict = Utilities.biomeResourceChances[biomeType];
		for (int i = 0; i < chancesDict.Keys.Count; i++) {
			int choice = Random.Range (0, 100);
			BIOME_PRODUCE_TYPE currentProduceType = chancesDict.Keys.ElementAt(i);
			int[] chancesForCurrentProduceType = chancesDict [currentProduceType];

			int upperBound = 0;
			int lowerBound = 0;
			int generatedResourceValue = 0;
			for (int j = 0; j < chancesForCurrentProduceType.Length; j++) {
				upperBound += chancesForCurrentProduceType [j];
				if (choice >= lowerBound && choice < upperBound) {
					if (j == 0) {
						if (currentProduceType == BIOME_PRODUCE_TYPE.GOLD) {
							generatedResourceValue = Random.Range (51, 81);
						} else {
//							generatedResourceValue = Random.Range (21, 36);
							generatedResourceValue = 20;
						}
					} else if (j == 1) {
						if (currentProduceType == BIOME_PRODUCE_TYPE.GOLD) {
							generatedResourceValue = Random.Range (81, 101);
						} else {
//							generatedResourceValue = Random.Range (36, 46);
							generatedResourceValue = 30;
						}
					} else if (j == 2) {
						if (currentProduceType == BIOME_PRODUCE_TYPE.GOLD) {
							generatedResourceValue = Random.Range (1, 51);
						} else {
//							generatedResourceValue = Random.Range (10, 21);
							generatedResourceValue = 10;
						}
					} else if (j == 3) {
						generatedResourceValue = 0;
					}
					break;
				}
				lowerBound = upperBound;
			}

			if (currentProduceType == BIOME_PRODUCE_TYPE.FARMING) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					generatedResourceValue -= 5;
					if (generatedResourceValue < 0) {
						generatedResourceValue = 1;
					}
				}
				farmingValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.HUNTING) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					if (generatedResourceValue > 0) {
						generatedResourceValue += 5;
					}
				}
				huntingValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.WOOD) {
				woodValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.STONE) {
				stoneValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.MANA) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					if (generatedResourceValue > 0) {
						generatedResourceValue += 5;
					}
				}
				manaValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.METAL) {
				if (elevationType == ELEVATION.MOUNTAIN) {
					if (generatedResourceValue > 0) {
						generatedResourceValue += 5;
					}
				}
				metalValue = generatedResourceValue;
			} else if (currentProduceType == BIOME_PRODUCE_TYPE.GOLD) {
				goldValue = generatedResourceValue;
			}
		}

		allResourceValues = new int[]{ farmingValue, huntingValue, woodValue, stoneValue, manaValue, metalValue };
	}	

	public int GetRelevantResourceValueByJobType(JOB_TYPE jobType){
		switch (jobType) {
		case JOB_TYPE.FARMER:
			return farmingValue;
		case JOB_TYPE.HUNTER:
			return huntingValue;
		case JOB_TYPE.WOODSMAN:
			return woodValue;
		case JOB_TYPE.QUARRYMAN:
			return stoneValue;
		case JOB_TYPE.MINER:
			return metalValue;
		case JOB_TYPE.ALCHEMIST:
			return manaValue;
		}
		return -1;
	}

	public JOB_TYPE GetBestJobForTile(){
		int bestJobIndex = 0;
		int highestResourceValue = 0;
		for (int i = 0; i < allResourceValues.Length; i++) {
			if (allResourceValues [i] > highestResourceValue) {
				highestResourceValue = allResourceValues [i];
				bestJobIndex = i;
			}
		}

		if (bestJobIndex == 0) {
			return JOB_TYPE.FARMER;
		} else if (bestJobIndex == 1) {
			return JOB_TYPE.HUNTER;
		} else if (bestJobIndex == 2) {
			return JOB_TYPE.WOODSMAN;
		} else if (bestJobIndex == 3) {
			return JOB_TYPE.QUARRYMAN;
		} else if (bestJobIndex == 4) {
			return JOB_TYPE.ALCHEMIST;
		} else if (bestJobIndex == 5) {
			return JOB_TYPE.MINER;
		}
		return JOB_TYPE.FARMER;
	}

	public int GetHighestResourceValue(){
		int highestResourceValue = 0;
		for (int i = 0; i < allResourceValues.Length; i++) {
			if (allResourceValues [i] > highestResourceValue) {
				highestResourceValue = allResourceValues [i];
			}
		}
		return highestResourceValue;
	}
		
	public void SetTileAsUnoccupied(){
		if (!isCity) {
			this.isOccupied = false;
			SetTileColor (Color.red);
		}
	}

//	private BIOME GetBiome(){
//		if(elevationNoise <= 0.35f){
//			if(elevationNoise < 0.30f){
//				return BIOME.OCEAN;
//			}
//			return BIOME.BEACH;
//		}else{
//			if(elevationNoise > 0.3f && elevationNoise < 0.6f){
//				if(moistureNoise < 0.16f){
//					return BIOME.SUBTROPICAL_DESERT;
//				}
//				if(moistureNoise < 0.50f){
//					return BIOME.GRASSLAND;
//				}
//				if(moistureNoise < 0.83f){
//					return BIOME.TEMPERATE_DECIDUOUS_FOREST;
//				}
//				return BIOME.TEMPERATE_RAIN_FOREST;
//			}
//			else if(elevationNoise >= 0.6f && elevationNoise < 0.8f){
//				if(moistureNoise < 0.33f){
//					return BIOME.TEMPERATE_DESERT;
//				}
//				if(moistureNoise < 0.66f){
//					return BIOME.SHRUBLAND;
//				}
//				return BIOME.TAIGA;
//			}
//			else if(elevationNoise >= 0.8f){
//				if(moistureNoise < 0.1f){
//					return BIOME.SCORCHED;
//				}
//				if(moistureNoise < 0.2f){
//					return BIOME.BARE;
//				}
//				if(moistureNoise < 0.5f){
//					return BIOME.TUNDRA;
//				}
//				return BIOME.SNOW;
//			}
//		}		
//		return BIOME.TROPICAL_RAIN_FOREST;
//
//	}

	void OnMouseDown(){
		if (isCity) {
			UserInterfaceManager.Instance.SetCityInfoToShow (gameObject.GetComponent<CityTileTest> ());
		}
	}
}
