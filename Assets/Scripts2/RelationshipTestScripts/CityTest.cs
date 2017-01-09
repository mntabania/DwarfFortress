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
	public CityUpgradeRequirements cityUpgradeRequirements;
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;
	public List<Citizen> citizens;
	public List<CityTileTest> connectedCities;

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
		this.cityUpgradeRequirements = UpgradeRequirements (this.cityLevel);
		this.citizens = new List<Citizen>();
		this.connectedCities = new List<CityTileTest>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = null;
		this.hexTile = hexTile;
	}

	internal void ConsumeFood(int foodRequirement){
		this.foodCount -= foodRequirement;
		if(this.foodCount < 0){
//			this.foodCount = 0;
			this.cityState = CITY_STATE.STARVATION;
			ComputeForDeath();
		}else{
			this.cityState = CITY_STATE.ABUNDANT;
		}
	}

	internal void ProduceGold(){
		this.goldCount += this.richnessLevel + (UnityEngine.Random.Range (0, (int)((float)this.cityLevel * (0.2f * (float)this.richnessLevel))));
	}

	internal void ProduceResources(){
		for(int i = 0; i < this.citizens.Count; i++){
			int production = (int)((float)(5 + (5 * this.citizens [i].level)) * Random.Range(1f, 1.45f)) + mayorLikeRating;
			switch(this.citizens[i].type){
			case CITIZEN_TYPE.FARMER:
				this.foodCount += production;
				break;
			case CITIZEN_TYPE.WOODSMAN:
				this.lumberCount += production;
				break;
			case CITIZEN_TYPE.MINER:
				this.stoneCount += production;
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
			Debug.LogError ("SOMEONE DIED: " + citizens [russianRoulette].type.ToString());
			citizens.Remove (citizens [russianRoulette]);
		}
	}
	internal void Upgrade(){
		int reqGold = this.cityUpgradeRequirements.gold;
		List<Resource> reqResources = this.cityUpgradeRequirements.resource;
		if(this.goldCount >= reqGold){
			for(int i = 0; i < reqResources.Count; i++){
				switch(reqResources[i].resourceType){
				case RESOURCE.LUMBER:
					if(reqResources[i].resourceQuantity >= this.lumberCount){
						
					}else{
						//TRADE OR WAIT
					}
					break;
				case RESOURCE.STONE:
					if(reqResources[i].resourceQuantity >= this.stoneCount){

					}else{

					}
					break;
				case RESOURCE.MANA_STONE:
					if(reqResources[i].resourceQuantity >= this.manaStoneCount){

					}else{

					}
					break;
				case RESOURCE.TRADE_GOOD:
					if(reqResources[i].resourceQuantity >= this.tradeGoodsCount){

					}else{

					}
					break;
				}
			}
		}else{
			//TRADE GOLD OR WAIT FOR GOLD COUNT
		}
	}
	internal CityUpgradeRequirements UpgradeRequirements(int level){
		CityUpgradeRequirements req = new CityUpgradeRequirements ();

		switch(level + 1){
		case 2:
			req.gold = 2000;
			req.resource.Add (new Resource (RESOURCE.LUMBER, 50));
			break;
		case 3:
			req.gold = 4000;
			req.resource.Add (new Resource (RESOURCE.LUMBER, 100));
			break;
		case 4:
			req.gold = 6000;
			req.resource.Add (new Resource (RESOURCE.LUMBER, 200));
			req.resource.Add (new Resource (RESOURCE.STONE, 100));
			break;
		case 5:
			req.gold = 8000;
			req.resource.Add (new Resource (RESOURCE.LUMBER, 400));
			req.resource.Add (new Resource (RESOURCE.STONE, 200));
			break;
		case 6:
			req.gold = 10000;
			req.resource.Add (new Resource (RESOURCE.LUMBER, 800));
			req.resource.Add (new Resource (RESOURCE.STONE, 400));
			req.resource.Add (new Resource (RESOURCE.MANA_STONE, 100));
			break;
		}

		return req;
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
