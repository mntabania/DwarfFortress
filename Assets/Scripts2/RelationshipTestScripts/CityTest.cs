using UnityEngine;
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
	public int goldCount;
	public int foodCount;
	public int lumberCount;
	public int stoneCount;
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
	public string cityLogs;

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
		this.stoneCount = 0;
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
		this.cityLogs = string.Empty;
	}

	internal void ConsumeFood(int foodRequirement){
		this.foodCount -= foodRequirement;
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Consumed [ff0000]" + foodRequirement.ToString() + "[-] food.\n\n"; 
		if(this.foodCount < 0){
//			this.foodCount = 0;
			this.cityState = CITY_STATE.STARVATION;
			ComputeForDeath();
		}else{
			this.cityState = CITY_STATE.ABUNDANT;
		}
	}

	internal void ProduceGold(){
		int producedGold = this.richnessLevel + (UnityEngine.Random.Range (0, (int)((float)this.cityLevel * (0.2f * (float)this.richnessLevel))));
		this.goldCount += producedGold;
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + producedGold.ToString() + "[-] gold. Total is now: [7CFC00]" + this.goldCount.ToString()+ "[-]\n\n"; 
	}

	internal void ProduceResources(){
		for(int i = 0; i < this.citizens.Count; i++){
			int production = (int)((float)(5 + (5 * this.citizens [i].level)) * Random.Range(1f, 1.4f)) + mayorLikeRating;
			switch(this.citizens[i].type){
			case CITIZEN_TYPE.FARMER:
				this.foodCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] food.\n\n"; 
				break;
			case CITIZEN_TYPE.WOODSMAN:
				this.lumberCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] lumber.\n\n"; 
				break;
			case CITIZEN_TYPE.MINER:
				this.stoneCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] stone.\n\n"; 
				break;
			case CITIZEN_TYPE.ALCHEMIST:
				this.manaStoneCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] mana stones.\n\n"; 
				break;
			case CITIZEN_TYPE.ARTISAN:
				this.tradeGoodsCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] trade goods.\n\n"; 
				break;
			}
		}
	}

	internal int ComputeFoodConsumption(){
		int totalFoodConsumption = 0;
		for (int i = 0; i < citizens.Count; i++) {
			totalFoodConsumption += citizens [i].level * citizens [i].foodConsumption;
		}
		return totalFoodConsumption;
	}

	internal void ComputeForDeath(){
		int chance = Random.Range (0, 100);
		if (chance < 2) {
			//DIE MADAPAKA
			//RUSSIAN ROULETTE
			int russianRoulette = Random.Range (0, citizens.Count);
//			Debug.LogError ("SOMEONE DIED: " + citizens [russianRoulette].type.ToString());
			cityLogs += GameManager.Instance.currentDay.ToString() + ": A [FF0000]" + citizens[russianRoulette].type.ToString() + "[-] died.\n\n"; 
			citizens.Remove (citizens [russianRoulette]);
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
