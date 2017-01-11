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
//	public CITIZEN_TYPE foodProductionRole;
	public JOB_TYPE neededRole;
	public JOB_TYPE unneededRole;
	public JOB_TYPE newCitizenTarget;
	public Citizen upgradeCitizenTarget;
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
//		this.foodProductionRole = FoodProductionRole ();
		this.neededRole = JOB_TYPE.NONE;
		this.unneededRole = JOB_TYPE.NONE;
		this.upgradeCitizenTarget = null;
		this.newCitizenTarget = JOB_TYPE.NONE;
		this.cityUpgradeRequirements = UpgradeRequirements (this.cityLevel);
		this.isDead = false;
	}

	internal int ComputeFoodConsumption(){
		int totalFoodConsumption = 0;
		for (int i = 0; i < citizens.Count; i++) {
			totalFoodConsumption += citizens [i].FoodConsumption();
		}
		return totalFoodConsumption;
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

	internal void ProduceResources(){
		if(isDead){
			return;
		}

		int producedGold = this.richnessLevel + (UnityEngine.Random.Range (0, (int)((float)this.cityLevel * (0.2f * (float)this.richnessLevel))));
		this.goldCount += producedGold;

		for(int i = 0; i < this.citizens.Count; i++){
			int[] resourcesProducedByCitizen = this.citizens[i].GetAllDailyProduction();
			this.goldCount += resourcesProducedByCitizen[0];
			this.foodCount += resourcesProducedByCitizen[1];
			this.lumberCount += resourcesProducedByCitizen[2];
			this.stoneCount += resourcesProducedByCitizen[3];
			this.manaStoneCount += resourcesProducedByCitizen[4];
		}
	}

	internal void ComputeForDeath(){
		if(isDead){
			return;
		}
		int chance = Random.Range (0, 100);
		if (chance < 2) {
			int russianRoulette = Random.Range (0, citizens.Count);
			cityLogs += GameManager.Instance.currentDay.ToString() + ": A [FF0000]" + citizens[russianRoulette].job.jobType.ToString() + "[-] died.\n\n"; 
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
		List<HexTile> neighbours = new List<HexTile>();
		neighbours.Add (this.hexTile);
		for (int i = 0; i < this.ownedBiomeTiles.Count; i++) {
			neighbours.AddRange (this.ownedBiomeTiles [i].GetListTilesInRange(0.5f));
		}
		neighbours.AddRange(this.hexTile.GetListTilesInRange (0.5f));
		neighbours = neighbours.Distinct().ToList();

		citizen.AssignCitizenToTile (neighbours);
	}

	internal void SelectCitizenToUpgrade(){
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


	internal void SelectCitizenForCreation(){
		if(isDead){
			return;
		}
		int[] neededResources = NeededResources ();
		List<CITIZEN_TYPE> neededCitizens = GetListCitizensTarget (neededResources);
		if(neededCitizens.Count > 0){
			float highestNoOfTurns = 0f;
			CITIZEN_TYPE currentHighestCitizenType = CITIZEN_TYPE.NONE;
			for (int i = 0; i < neededCitizens.Count; i++) {
				switch(neededCitizens[i]){
				case CITIZEN_TYPE.WOODSMAN:
					int aveLumber = (int)((float)(neededResources [0] - this.lumberCount) / GetDailyProduction(CITIZEN_TYPE.WOODSMAN));
					if(aveLumber > highestNoOfTurns){
						highestNoOfTurns = aveLumber;
						currentHighestCitizenType = CITIZEN_TYPE.WOODSMAN;
					}
					break;
				case CITIZEN_TYPE.MINER:
					int aveStone = (int)((float)(neededResources [0] - this.lumberCount) / GetDailyProduction(CITIZEN_TYPE.WOODSMAN));
					if(aveStone > highestNoOfTurns){
						highestNoOfTurns = aveStone;
						currentHighestCitizenType = CITIZEN_TYPE.MINER;
					}					
					break;
				case CITIZEN_TYPE.ALCHEMIST:
					int aveManaStone = (int)((float)(neededResources [0] - this.lumberCount) / GetDailyProduction(CITIZEN_TYPE.WOODSMAN));
					if(aveManaStone > highestNoOfTurns){
						highestNoOfTurns = aveManaStone;
						currentHighestCitizenType = CITIZEN_TYPE.ALCHEMIST;
					}					
					break;
				}
			}

			this.newCitizenTarget = currentHighestCitizenType;
		}
		else{ //WARRIOR
			this.newCitizenTarget = CITIZEN_TYPE.WARRIOR;

		}
	}
	private int GetDailyProduction(RESOURCE resourceType){
		int production = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			production += this.citizens [i].GetDailyProduction (resourceType);
		}

		return production;
	}
	private List<CITIZEN_TYPE> GetListCitizensTarget(int[] neededResources){ //lumber, stone, manastone
		
		List<CITIZEN_TYPE> neededCitizens = new List<CITIZEN_TYPE> ();
//		int highestExcess = 0;
//		switch(this.citizens[0].type){
//		case CITIZEN_TYPE.FARMER:
//			break;
//		case CITIZEN_TYPE.HUNTER:
//			break;
//		case CITIZEN_TYPE.WOODSMAN:
//			highestExcess = this.lumberCount - neededResources [0];
//			unneededCitizen = CITIZEN_TYPE.WOODSMAN;
//			break;
//		case CITIZEN_TYPE.MINER:
//			highestExcess = this.stoneCount - neededResources [1];
//			unneededCitizen = CITIZEN_TYPE.MINER;
//			break;
//		case CITIZEN_TYPE.ALCHEMIST:
//			highestExcess = this.manaStoneCount - neededResources [2];
//			unneededCitizen = CITIZEN_TYPE.ALCHEMIST;
//			break;
//		}

		int excess = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			switch(this.citizens[i].type){
//			case CITIZEN_TYPE.FARMER:
//				break;
//			case CITIZEN_TYPE.HUNTER:
//				break;
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

			if(excess < 0){
				neededCitizens.Add (this.citizens [i].type);
			}
		}

		return neededCitizens.Distinct().ToList();
	}


	internal void AssignNeededRole(){
		if(isDead){
			return;
		}
		if(this.foodCount <= -10){
			if(this.neededRole == CITIZEN_TYPE.NONE){
				this.neededRole = CITIZEN_TYPE.FARMER;
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
				averageProduction += Farmer.GetProduction (citizen.level, citizen.assignedTile.farmingValue, this.mayorLikeRating);
			}
			break;
		case CITIZEN_TYPE.HUNTER:
			for(int i = 0; i < noOfDays; i++){
				averageProduction += Hunter.GetProduction (citizen.level, citizen.assignedTile.huntingValue, this.mayorLikeRating);
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
		int[] neededResources = new int[]{0, 0, 0, 0, 0,}; //gold, food, lumber, stone, manastone

		//BASED ON UPGRADE CITIZEN
		for(int i = 0; i < upgradeCitizenTarget.GetUpgradeRequirements().resource.Count; i++){
			switch(upgradeCitizenTarget.GetUpgradeRequirements().resource[i].resourceType){
			case RESOURCE.GOLD:
				neededResources[0] += upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				neededResources[1] += upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[2] += upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[3] += upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA_STONE:
				neededResources[4] += upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			}
		}

		//BASED ON HOUSING UPGRADE
		for(int i = 0; i < cityUpgradeRequirements.resource.Count; i++){
			switch(cityUpgradeRequirements.resource[i].resourceType){
			case RESOURCE.GOLD:
				neededResources[0] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				neededResources[1] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[2] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[3] += cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA_STONE:
				neededResources[4] += cityUpgradeRequirements.resource [i].resourceQuantity;
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
		
	internal void AttemptToIncreaseHousing(){
		if (CanCityAffordToUpgrade() && !IsCitizenCapReached()) { //if city has the neccessary resources to upgrade and still has room for another citizen
			int chance = Random.Range(0,100);
			if (chance < cityActionChances.increaseHousingChance) {
				//Increase Housing Triggered, Increase Citizen Limit by 1
				citizenLimit += 1;
			} else {
				//On not performing upgrade, increase chance to upgrade by 1
				cityActionChances.increaseHousingChance += 1;
			}
		}
	}

	internal void AttemptToUpgradeCitizen(){

	}

	bool CanAffordToUpgradeCitizen(){
		return true;
	}

	bool CanCityAffordToUpgrade(){
		CityUpgradeRequirements upgradeRequirements = this.cityUpgradeRequirements;
		if (upgradeRequirements.gold <= this.goldCount) {
			for (int i = 0; i < upgradeRequirements.resource.Count; i++) {
				RESOURCE resourceType = upgradeRequirements.resource [i].resourceType;
				if(upgradeRequirements.resource [i].resourceQuantity > GetNumberOfResourcesPerType(resourceType)){
					//The city lacks atleast one resource for upgrade, return false
					return false;
				}
			}
			//The loop has finished, meaning the city has all the needed resources to upgrade, return true
			return true;
		}
		//The city does not have enough gold, return false
		return false;
	}

	bool IsCitizenCapReached(){
		if (citizens.Count < citizenLimit) {
			return false;
		} else {
			return true;
		}
	}

	int GetNumberOfResourcesPerType(RESOURCE resourceType){
		if (resourceType == RESOURCE.FOOD) {
			return this.foodCount;
		} else if (resourceType == RESOURCE.LUMBER) {
			return this.lumberCount;
		} else if (resourceType == RESOURCE.MANA_STONE) {
			return this.manaStoneCount;
		} else if (resourceType == RESOURCE.STONE) {
			return this.stoneCount;
		} else if (resourceType == RESOURCE.TRADE_GOOD) {
			return this.tradeGoodsCount;
		}
		return -1;
	}

}
