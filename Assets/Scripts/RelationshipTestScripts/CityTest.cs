﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CityTest{

	public int id;
	public string cityName;
	public BIOMES biomeType;
	public Mayor cityMayor;
	public int cityLevel;
	public int numOfRoads;
	public int population;
	public int richnessLevel;
	public int gold;
	public int foodCount;
	public int lumberCount;
	public int rockCount;
	public int manaStoneCount;
	public int tradeGoodsCount;
	public int mayorLikeRating;
	public int citizenLimit;
	public CITY_STATE cityState;
	public List<Citizen> citizens;
	public List<CityTileTest> connectedCities;
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;

	public CityTest(HexTile hexTile, BIOMES biomeType){
		this.id = 0;
		this.cityName = hexTile.name;
		this.biomeType = biomeType;
		this.cityMayor = null;
		this.cityLevel = 1;
		this.numOfRoads = 0;
		this.population = 0;
		this.richnessLevel = 60;
		this.foodCount = 0;
		this.lumberCount = 0;
		this.rockCount = 0;
		this.manaStoneCount = 0;
		this.tradeGoodsCount = 0;
		this.mayorLikeRating = 0;
		this.citizenLimit = 4;
		this.cityState = CITY_STATE.ABUNDANT;
		this.citizens = new List<Citizen>();
		this.connectedCities = new List<CityTileTest>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = null;
		this.hexTile = hexTile;
	}

	internal void CheckCityState(int foodRequirement){
		this.foodCount -= foodRequirement;

		if(this.foodCount < 0){
			this.foodCount = 0;
			this.cityState = CITY_STATE.FAMINE;
		}else{
			this.cityState = CITY_STATE.ABUNDANT;
		}
	}
	internal void ProduceGold(){
		this.gold += this.richnessLevel + (UnityEngine.Random.Range (0, (int)((float)this.cityLevel * (0.2f * (float)this.richnessLevel))));
	}
	internal void ProduceResources(){
		for(int i = 0; i < this.citizens.Count; i++){
			int production = (int)(((UnityEngine.Random.Range (0.25f, 0.5f) * (float)this.citizens [i].productionValue) + (5 * this.citizens [i].level)) + mayorLikeRating);
			switch(this.citizens[i].type){
			case CITIZEN_TYPE.FARMER:
				this.foodCount += production;
				break;
			case CITIZEN_TYPE.WOODSMAN:
				this.lumberCount += production;
				break;
			case CITIZEN_TYPE.MINER:
				this.rockCount += production;
				break;
			case CITIZEN_TYPE.ALCHEMIST:
				this.manaStoneCount += production;
				break;
			case CITIZEN_TYPE.ARTISAN:
				this.tradeGoodsCount += production;
				break;
			}
		}
	}
//	public HexTile hexTile;
//	public BIOMES biomeType;
//	public CITY_TYPE cityType;
//	public int cityLevel;
//	public int richnessLevel;
//	public int numOfRoads;
//	public int population;
//	public int garrison;
//	public int gold;
//	public List<CityTile> connectedCities;
//	public KingdomTile kingdomTile;
//	public Faction faction;
}
