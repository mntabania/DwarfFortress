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
	public CityActionChances cityActionChances;
	public CITY_STATE cityState;
	public List<Citizen> citizens;
	public List<CityTileTest> connectedCities;
	public List<HexTile> ownedBiomeTiles;
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;
	public string cityLogs;
	public CITIZEN_TYPE foodProductionRole;
	public CITIZEN_TYPE neededRole;
	public CITIZEN_TYPE unneededRole;
	public Citizen upgradeCitizenTarget;
	public Citizen newCitizenTarget;
	public CityUpgradeRequirements cityUpgradeRequirements;
	public bool isDead;

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
		this.cityActionChances = new CityActionChances ();
		this.cityState = CITY_STATE.ABUNDANT;
		this.citizens = new List<Citizen>();
		this.connectedCities = new List<CityTileTest>();
		this.ownedBiomeTiles = new List<HexTile>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = null;
		this.hexTile = hexTile;
		this.cityLogs = string.Empty;
		this.foodProductionRole = FoodProductionRole ();
		this.neededRole = CITIZEN_TYPE.NONE;
		this.unneededRole = CITIZEN_TYPE.NONE;
		this.upgradeCitizenTarget = null;
		this.newCitizenTarget = null;
		this.cityUpgradeRequirements = UpgradeRequirements (this.cityLevel);
		this.isDead = false;
	}
	internal CITIZEN_TYPE FoodProductionRole(){
		if(this.biomeType == BIOMES.SNOW){
			return CITIZEN_TYPE.FARMER;
		}
		return CITIZEN_TYPE.FARMER;

	}
	internal void ConsumeFood(int foodRequirement){
		if(isDead){
			return;
		}
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
		if(isDead){
			return;
		}
		int producedGold = this.richnessLevel + (UnityEngine.Random.Range (0, (int)((float)this.cityLevel * (0.2f * (float)this.richnessLevel))));
		this.goldCount += producedGold;
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + producedGold.ToString() + "[-] gold. Total is now: [7CFC00]" + this.goldCount.ToString()+ "[-]\n\n"; 
	}

	internal void ProduceResources(){
		if(isDead){
			return;
		}
		for(int i = 0; i < this.citizens.Count; i++){
			int production = 0;
			switch(this.citizens[i].type){
			case CITIZEN_TYPE.FARMER:
				production = Farmer.GetProduction (this.citizens [i].level, this.mayorLikeRating);
				this.foodCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] food.\n\n"; 
				break;
			case CITIZEN_TYPE.WOODSMAN:
				production = Woodsman.GetProduction (this.citizens [i].level, this.mayorLikeRating);
				this.lumberCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] lumber.\n\n"; 
				break;
			case CITIZEN_TYPE.MINER:
				production = Miner.GetProduction (this.citizens [i].level, this.mayorLikeRating);
				this.stoneCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] stone.\n\n"; 
				break;
			case CITIZEN_TYPE.ALCHEMIST:
				production = Alchemist.GetProduction (this.citizens [i].level, this.mayorLikeRating);
				this.manaStoneCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] mana stones.\n\n"; 
				break;
			case CITIZEN_TYPE.HUNTER:
				production = Hunter.GetProduction (this.citizens [i].level, this.mayorLikeRating);
				this.foodCount += production;
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Produced [7CFC00]" + production.ToString() + "[-] food.\n\n"; 
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
		if(isDead){
			return;
		}
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
		if(isDead){
			return;
		}
		for (int i = 0; i < citizens.Count; i++) {
			AssignCitizenToTile (citizens [i]);
		}
	}

	internal void AssignCitizenToTile(Citizen citizen){
		if(isDead){
			return;
		}
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
		List<Citizen> citizensToChooseFrom = citizens.OrderBy(x => x.level).ToList();
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
		return totalChances;
	}

	void SelectCitizenToUpgrade(){
		if(isDead){
			return;
		}
		int choice = Random.Range (0, GetTotalChanceForUpgrade()+1);
		int upperBound = 0;
		int lowerBound = 0;
		for (int i = 0; i < citizens.Count; i++) {
			upperBound += citizens [i].upgradeChance;
			if (choice >= lowerBound && choice < upperBound) {
				upgradeCitizenTarget = citizens [i];
				break;
			} else {
				lowerBound = upperBound;
			}
		}
	}

	void SelectCitizenForCreation(){
		if(isDead){
			return;
		}

	}
	private CITIZEN_TYPE GetNewCitizenTarget(){ //lumber, stone, manastone, food
		
	}


	internal void AssignNeededRole(){
		if(isDead){
			return;
		}
		if(this.foodCount <= -10){
			if(this.neededRole == CITIZEN_TYPE.NONE){
				this.neededRole = this.foodProductionRole;
			}
		}else{
			this.neededRole = CITIZEN_TYPE.NONE;
		}

		/* -------------DO NOT DELETE THIS-------------- */

//		int neededFood = ComputeFoodConsumption();
//		float averageFoodPerDay = 0;
//		for(int i = 0; i < this.citizens.Count; i++){
//			if(this.citizens[i].type == this.foodProductionRole){
//				averageFoodPerDay += GetAverageProductionInNoOfDays (this.citizens [i], 7);
//			}
////			if(this.citizens[i].type == CITIZEN_TYPE.FARMER || this.citizens[i].type == CITIZEN_TYPE.HUNTER){
////				averageFoodPerDay += GetAverageProductionInNoOfDays (this.citizens [i], 7);
////			}
//		}
//		if(averageFoodPerDay < neededFood){
//			this.neededRole = this.foodProductionRole;
//		}
	}
	private float GetAverageProductionInNoOfDays(Citizen citizen, int noOfDays){
		float averageProduction = 0f;
		switch(citizen.type){
		case CITIZEN_TYPE.FARMER:
			for(int i = 0; i < noOfDays; i++){
				averageProduction += Farmer.GetProduction (citizen.level, this.mayorLikeRating);
			}
			break;
		case CITIZEN_TYPE.HUNTER:
			for(int i = 0; i < noOfDays; i++){
				averageProduction += Hunter.GetProduction (citizen.level, this.mayorLikeRating);
			}
			break;
		}

		return averageProduction / (float)noOfDays;
		
	}

	internal void AssignUnneededRole(){
		if(isDead){
			return;
		}
		int chance = UnityEngine.Random.Range (0, 100);

		if(chance < this.cityActionChances.oversupplyChance){
			this.cityActionChances.oversupplyChance = this.cityActionChances.defaultOversupplyChance;
			this.unneededRole = GetUnneededRole (NeededResources ());
		}else{
			this.cityActionChances.oversupplyChance += 2;
		}
	}

	private int[] NeededResources(){
		int[] neededResources = new int[]{ 0, 0, 0, 0 }; //lumber, stone, manastone, food

		//BASED ON UPGRADE CITIZEN
		for(int i = 0; i < upgradeCitizenTarget.citizenUpgradeRequirements.resource.Count; i++){
			switch(upgradeCitizenTarget.citizenUpgradeRequirements.resource[i].resourceType){
			case RESOURCE.FOOD:
				neededResources[3] += upgradeCitizenTarget.citizenUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[0] += upgradeCitizenTarget.citizenUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[1] += upgradeCitizenTarget.citizenUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA_STONE:
				neededResources[2] += upgradeCitizenTarget.citizenUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.TRADE_GOOD:
				break;
			}
		}

		//BASED ON HOUSING UPGRADE
		for(int i = 0; i < cityUpgradeRequirements.resource.Count; i++){
			switch(cityUpgradeRequirements.resource[i].resourceType){
			case RESOURCE.FOOD:
				neededResources[3] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[0] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[1] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA_STONE:
				neededResources[2] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.TRADE_GOOD:
				break;
			}
		}

		return neededResources;
	}
	private CITIZEN_TYPE GetUnneededRole(int[] neededResources){ //lumber, stone, manastone
		CITIZEN_TYPE unneededCitizen = CITIZEN_TYPE.NONE;
		int highestExcess = 0;
		switch(this.citizens[0].type){
		case CITIZEN_TYPE.FARMER:
			break;
		case CITIZEN_TYPE.HUNTER:
			break;
		case CITIZEN_TYPE.WOODSMAN:
			highestExcess = this.lumberCount - neededResources [0];
			unneededCitizen = CITIZEN_TYPE.WOODSMAN;
			break;
		case CITIZEN_TYPE.MINER:
			highestExcess = this.stoneCount - neededResources [1];
			unneededCitizen = CITIZEN_TYPE.MINER;
			break;
		case CITIZEN_TYPE.ALCHEMIST:
			highestExcess = this.manaStoneCount - neededResources [2];
			unneededCitizen = CITIZEN_TYPE.ALCHEMIST;
			break;
		}

		int excess = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			switch(this.citizens[i].type){
			case CITIZEN_TYPE.FARMER:
				break;
			case CITIZEN_TYPE.HUNTER:
				break;
			case CITIZEN_TYPE.WOODSMAN:
				excess = this.lumberCount - neededResources [0];
				break;
			case CITIZEN_TYPE.MINER:
				excess = this.stoneCount - neededResources [1];
				break;
			case CITIZEN_TYPE.ALCHEMIST:
				excess = this.manaStoneCount - neededResources [2];
				break;
			}

			if(excess > highestExcess){
				highestExcess = excess;
				unneededCitizen = this.citizens [i].type;
			}
		}

		return unneededCitizen;
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

}
