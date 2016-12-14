using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class City : ICity{

	public HexTile hexTile;
	public BIOMES biomeType;
	public CITY_TYPE cityType;
	public int richnessLevel;
	public int numOfRoads;
	public int population;
	public List<CityTile> connectedCities;

	public City(HexTile hexTile, BIOMES biomeType){
		this.hexTile = hexTile;
		this.biomeType = biomeType;
		this.cityType = CITY_TYPE.NORMAL;
		this.richnessLevel = GenerateRichness();
		this.population = GeneratePopulation();
		this.numOfRoads = 0;
		this.connectedCities = new List<CityTile>();
	}

	public int GeneratePopulation(){
		return Random.Range(5000, 10001);
	}

	public int GenerateRichness(){
		float rand = Random.value;
		if (rand <= .1f) {
			return Random.Range (0, 31);
		}
		if (rand <= .2f) {
			return Random.Range (40, 101);
		}
		if (rand <= .8f) {
			return Random.Range (30, 41);
		}
		return Random.Range (40, 101);

	}

	public int GenerateNumberOfRoads(){
		int linesRandomizer = Random.Range (0, 101);
		if (linesRandomizer >= 0 && linesRandomizer < 11) {
			this.numOfRoads = 1;
			return 1;
		} else if (linesRandomizer >= 11 && linesRandomizer < 81) {
			this.numOfRoads = 2;
			return 2;
		} else {
			this.numOfRoads = 3;
			return 3;
		}
	}

	public void AddCityAsConnected(CityTile cityTile){
		this.connectedCities.Add(cityTile);
	}



}
