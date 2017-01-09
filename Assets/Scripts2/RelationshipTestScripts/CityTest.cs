using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public List<HexTile> ownedBiomeTiles;
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;
	public string cityLogs;
	public CITIZEN_TYPE neededRole;
	public CITIZEN_TYPE uneededRole;
	public Citizen upgradeCitizenTarget;
	public Citizen newCitizenTarget;

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
		this.ownedBiomeTiles = new List<HexTile>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = null;
		this.hexTile = hexTile;
		this.cityLogs = string.Empty;
		this.neededRole = CITIZEN_TYPE.NONE;
		this.uneededRole = CITIZEN_TYPE.NONE;
		this.upgradeCitizenTarget = null;
		this.newCitizenTarget = null;
	}

	internal void ConsumeFood(int foodRequirement){
		this.foodCount -= foodRequirement;
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Consumed [ff0000]" + foodRequirement.ToString() + "[-] food.\n\n"; 
		if(this.foodCount < 0){
			this.cityState = CITY_STATE.STARVATION;
			cityLogs += GameManager.Instance.currentDay.ToString() + ": City is [ff0000] STARVING [-].\n\n"; 
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

	internal void AssignInitialCitizens(){
		for (int i = 0; i < citizens.Count; i++) {
			AssignCitizenToTile (citizens [i]);
		}
	}

	internal void AssignCitizenToTile(Citizen citizen){
		if (citizen.residence == RESIDENCE.OUTSIDE) {
			List<HexTile> neighbours = new List<HexTile>();
			for (int i = 0; i < this.ownedBiomeTiles.Count; i++) {
				neighbours.AddRange (this.ownedBiomeTiles [i].GetListTilesInRange(0.5f));
			}
			neighbours.AddRange(this.hexTile.GetListTilesInRange (0.5f));
			neighbours = neighbours.Distinct().ToList();
			int maxValue = 0;
			switch (citizen.type) {
			case CITIZEN_TYPE.FARMER:
				maxValue = neighbours.Max(x => x.farmingValue);
				neighbours = neighbours.Where(x => x.farmingValue == maxValue).ToList();
				break;
			case CITIZEN_TYPE.HUNTER:
				maxValue = neighbours.Max(x => x.huntingValue);
				neighbours = neighbours.Where(x => x.huntingValue == maxValue).ToList();
				break;
			case CITIZEN_TYPE.WOODSMAN:
				maxValue = neighbours.Max(x => x.woodValue);
				neighbours = neighbours.Where(x => x.woodValue == maxValue).ToList();
				break;
			case CITIZEN_TYPE.MINER:
				maxValue = neighbours.Max(x => x.stoneValue);
				neighbours = neighbours.Where(x => x.stoneValue == maxValue).ToList();
				break;
			case CITIZEN_TYPE.ALCHEMIST:
				maxValue = neighbours.Max(x => x.manaStoneValue);
				neighbours = neighbours.Where(x => x.manaStoneValue == maxValue).ToList();
				break;
			}
			int randomNeighbour = Random.Range (0, neighbours.Count);
			ownedBiomeTiles.Add(neighbours [randomNeighbour]);
			neighbours [randomNeighbour].isOccupied = true;
			citizen.assignedTile = neighbours[randomNeighbour];
			if(this.kingdomTile){
				neighbours [randomNeighbour].SetTileColor (this.kingdomTile.kingdom.tileColor);
			}
		} else {
			citizen.assignedTile = this.hexTile;
		}

	}

	int GetTotalChanceForUpgrade(){
		List<Citizen> citizensToChooseFrom = citizens.OrderBy(x => x.level);
		int lowestLevel = citizens.Min(x => x.level);
		int totalChances = 0;
		int[] currentChance = new int[]{100,60,20,5};
		int a = 0;
		for (int i = 0; i < citizensToChooseFrom.Count; i++) {
			if (citizensToChooseFrom [i].level == lowestLevel) {
				//Set Chance as 100
				totalChances += currentChance [a];
				citizensToChooseFrom [i].upgradeChance = currentChance [a];
			} else {
				lowestLevel = citizensToChooseFrom[i].level;
				a += 1;
				if (a >= currentChance.Length) {
					a = currentChance.Length - 1;
				}
				totalChances += currentChance [a];
				citizensToChooseFrom [i].upgradeChance = currentChance [a];
			}
		}
	}

	void SelectCitizenToUpgrade(){
		int choice = Random.Range (0, GetTotalChanceForUpgrade()+1);
		int upperBound = 0;
		int lowerBound = 0;
		for (int i = 0; i < citizens.Count; i++) {
			upperBound += citizens [i].upgradeChance;
			if (choice >= lowerBound && choice < upperBound) {
				upgradeCitizenTarget = citizens [i];
			} else {
				lowerBound = upperBound;
			}
		}
	}

	void SelectCitizenForCreation(){

	}

}
