using UnityEngine;
using System;
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
//	public JOB_TYPE foodProductionRole;
	public JOB_TYPE neededRole;
	public List<RESOURCE> unneededResources;
	public List<JOB_TYPE> unneededRoles;
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
		this.foodCount = 15;
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
		this.unneededRoles = new List<JOB_TYPE>();
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
//		cityLogs += GameManager.Instance.currentDay.ToString() + ": Consumed [ff0000]" + foodRequirement.ToString() + "[-] food.\n\n"; 
		if(this.foodCount < 0){
			this.cityState = CITY_STATE.STARVATION;
//			cityLogs += GameManager.Instance.currentDay.ToString() + ": City is [ff0000] STARVING [-].\n\n"; 
			cityLogs += GameManager.Instance.currentDay.ToString() + ": FOOD ration is running low. \n\n"; 
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
		int chance = UnityEngine.Random.Range (0, 100);
		if (chance < 2) {
			int russianRoulette = UnityEngine.Random.Range (0, citizens.Count);
			cityLogs += GameManager.Instance.currentDay.ToString() + ": The entire [FF0000]" + citizens[russianRoulette].job.jobType.ToString() + "[-] clan perished.\n\n"; 
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
		if(this.upgradeCitizenTarget != null){
			return;
		}
		int totalChance = GetTotalChanceForUpgrade ();
		if(totalChance <= 0){
			return;
		}
		int choice = UnityEngine.Random.Range (0, totalChance+1);
		int upperBound = 0;
		int lowerBound = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			upperBound += this.citizens [i].upgradeChance;
			if (choice >= lowerBound && choice < upperBound) {
				this.upgradeCitizenTarget = this.citizens [i];
//				cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected for citizen upgrade: [FF0000]" + this.upgradeCitizenTarget.job.jobType.ToString() + "[-]\n\n"; 
				break;
			} else {
				lowerBound = upperBound;
			}
		}
	}
		
	int GetTotalChanceForUpgrade(){
		List<Citizen> citizensToChooseFrom = citizens.OrderBy(x => x.level).ToList();
		citizensToChooseFrom.RemoveAll (x => x.level >= 6);
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


	internal void SelectCitizenForCreation(bool is7Days){
		if(isDead){
			return;
		}
		if(!is7Days){
			if(this.newCitizenTarget != JOB_TYPE.NONE){
				return;
			}
		}
		int[] neededResources = NeededResources ();
		List<JobNeeds> neededJobs = GetListJobsTarget (neededResources);
		if(neededJobs.Count > 0){
			int chance = UnityEngine.Random.Range (0, GetTotalChance (neededJobs.Count) + 1);
			int upperBound = 0;
			int lowerBound = 0;
			for(int i = 0; i < neededJobs.Count; i++){
				if(i == 0){
					upperBound += 100;
				}else if(i == 1){
					upperBound += 60;
				}else if(i == 2){
					upperBound += 20;
				}else{
					upperBound += 5;
				}

				if(chance >= lowerBound && chance < upperBound){
					this.newCitizenTarget = neededJobs[i].jobType;
//					cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected job to be created: [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
					break;
				}else{
					lowerBound = upperBound;
				}
			}
		}
		else{ //WARRIOR
			this.newCitizenTarget = JOB_TYPE.WARRIOR;
//			cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected job to be created: [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
		}
	}

	internal int GetTotalChance(int jobCount){
		int totalChances = 0;
		for (int i = 0; i < jobCount; i++) {
			if(i == 0){
				totalChances += 100;
			}else if(i == 1){
				totalChances += 60;
			}else if(i == 2){
				totalChances += 20;
			}else{
				totalChances += 5;
			}
		}
		return totalChances;
	}

//	private int GetDailyProduction(RESOURCE resourceType){
//		int production = 0;
//		for (int i = 0; i < this.citizens.Count; i++) {
//			production += this.citizens [i].GetDailyProduction (resourceType);
//		}
//
//		return production;
//	}

	private List<JobNeeds> GetListJobsTarget(int[] neededResources){ //gold, food, lumber, stone, manastone

		int resourceDeficit = 0;
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		List<JobNeeds> neededJobs = new List<JobNeeds> ();
		for (int i = 0; i < allResources.Length; i++) {
			switch(allResources[i]){
			case RESOURCE.GOLD:
				resourceDeficit = neededResources [0] - this.goldCount;
				break;
			case RESOURCE.FOOD:
				resourceDeficit = neededResources [1] - this.foodCount;
				break;
			case RESOURCE.LUMBER:
				resourceDeficit = neededResources [2] - this.lumberCount;
				break;
			case RESOURCE.STONE:
				resourceDeficit = neededResources [3] - this.stoneCount;
				break;
			case RESOURCE.MANA_STONE:
				resourceDeficit = neededResources [4] - this.manaStoneCount;
				break;
			}

			if(resourceDeficit > 0){
				for(int j = 0; j < Lookup.JOB_REF.Length; j++){
					if(Lookup.GetJobInfo(j).resourcesProduced == null){
						continue;
					}
					for(int k = 0; k < Lookup.GetJobInfo(j).resourcesProduced.Length; k++){
						if(Lookup.GetJobInfo(j).resourcesProduced[k] == allResources[i]){
							int deficit = 0;
							if(Lookup.GetJobInfo(j).GetDailyProduction(allResources[i]) > 0){
								deficit =  (int)(resourceDeficit / Lookup.GetJobInfo(j).GetDailyProduction(allResources[i]));
							}

							neededJobs.Add (new JobNeeds(Lookup.GetJobInfo(j).jobType, new Resource(allResources[i],deficit)));
							break;
						}
					}
				}
			}
		}
		neededJobs = neededJobs.OrderBy (x => x.resource.resourceQuantity).ToList ();
//		neededJobs = neededJobs.Distinct ().ToList ();

		return neededJobs;
	}
		
	internal void AssignNeededRole(){
		if(isDead){
			return;
		}
		if(this.foodCount <= -10){
			if(this.neededRole == JOB_TYPE.NONE){
				this.neededRole = JOB_TYPE.FARMER;
			}
		}else{
			this.neededRole = JOB_TYPE.NONE;
		}

		/* -------------DO NOT DELETE THIS-------------- */

//		int neededFood = ComputeFoodConsumption();
//		float averageFoodPerDay = 0;
//		for(int i = 0; i < this.citizens.Count; i++){
//			if(this.citizens[i].type == this.foodProductionRole){
//				averageFoodPerDay += GetAverageProductionInNoOfDays (this.citizens [i], 7);
//			}
////			if(this.citizens[i].type == JOB_TYPE.FARMER || this.citizens[i].type == JOB_TYPE.HUNTER){
////				averageFoodPerDay += GetAverageProductionInNoOfDays (this.citizens [i], 7);
////			}
//		}
//		if(averageFoodPerDay < neededFood){
//			this.neededRole = this.foodProductionRole;
//		}
	}

//	private float GetAverageProductionInNoOfDays(Citizen citizen, int noOfDays){
//		float averageProduction = 0f;
//		switch(citizen.type){
//		case JOB_TYPE.FARMER:
//			for(int i = 0; i < noOfDays; i++){
//				averageProduction += Farmer.GetProduction (citizen.level, citizen.assignedTile.farmingValue, this.mayorLikeRating);
//			}
//			break;
//		case JOB_TYPE.HUNTER:
//			for(int i = 0; i < noOfDays; i++){
//				averageProduction += Hunter.GetProduction (citizen.level, citizen.assignedTile.huntingValue, this.mayorLikeRating);
//			}
//			break;
//		}
//
//		return averageProduction / (float)noOfDays;
//		
//	}

	internal void AssignUnneededRoles(){
		if(isDead){
			return;
		}
		int chance = UnityEngine.Random.Range (0, 100);

		if(chance < this.cityActionChances.oversupplyChance){
			this.cityActionChances.oversupplyChance = this.cityActionChances.defaultOversupplyChance;
			this.unneededResources = GetUnneededResources (NeededResources ());
			this.unneededRoles = GetUnneededRoles ();
		}else{
			this.cityActionChances.oversupplyChance += 2;
		}
	}


	private bool isResourceUnneeded (RESOURCE resource){
		for(int i = 0; i < this.unneededResources.Count; i++){
			if(resource == this.unneededResources[i]){
				return true;
			}
		}
		return false;
	}
	private List<RESOURCE> GetUnneededResources(int[] neededResources){ //gold, food, lumber, stone, manastone
		List<RESOURCE> unneededResources = new List<RESOURCE>();
		int excess = 0;
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		for (int i = 0; i < allResources.Length; i++) {
			switch (allResources[i]) {
			case RESOURCE.GOLD:
				excess = this.goldCount - neededResources [0];
				break;
			case RESOURCE.FOOD:
				excess = this.foodCount - (10 * ComputeFoodConsumption());
				break;
			case RESOURCE.LUMBER:
				excess = this.lumberCount - neededResources [2];
				break;
			case RESOURCE.STONE:
				excess = this.stoneCount - neededResources [3];
				break;
			case RESOURCE.MANA_STONE:
				excess = this.manaStoneCount - neededResources [4];
				break;
			}

			if (excess > 0) {
				unneededResources.Add (allResources[i]);
				cityLogs += GameManager.Instance.currentDay.ToString() + ": There is an oversupply of [FF0000]" + allResources[i].ToString() + "[-]\n\n"; 
			}
		}

		return unneededResources;
	}
	private List<JOB_TYPE> GetUnneededRoles(){ //gold, food, lumber, stone, manastone
		List<JOB_TYPE> unneededJobs = new List<JOB_TYPE>();
		List<JOB_TYPE> tempUnneededJobs = new List<JOB_TYPE>();
		bool isAllUnneeded = true;
		bool hasUnneeded = false;
		int excess = 0;
		for (int i = 0; i < Lookup.JOB_REF.Length; i++) {
			isAllUnneeded = true;
			if(Lookup.GetJobInfo(i).resourcesProduced == null){
				continue;
			}
			for(int j = 0; j < Lookup.GetJobInfo(i).resourcesProduced.Length; j++){
				if(!isResourceUnneeded(Lookup.GetJobInfo(i).resourcesProduced[j])){
					isAllUnneeded = false;
				}
			}

			if (isAllUnneeded) {
				unneededJobs.Add (Lookup.GetJobInfo(i).jobType);	
			}
		}

		if(unneededJobs.Count > 0){
			tempUnneededJobs.AddRange(unneededJobs);
			for(int i = 0; i < tempUnneededJobs.Count; i++){
				hasUnneeded = false;
				for(int j = 0; j < this.citizens.Count; j++){
					if(tempUnneededJobs[i] == this.citizens[j].job.jobType){
						hasUnneeded = true;
					}
				}

				if(!hasUnneeded){
					unneededJobs.Remove (tempUnneededJobs[i]);
				}
			}
		}
		return unneededJobs.Distinct ().ToList();
	}
	internal List<JobNeeds> GetJobsWithExcessResources(int[] neededResources){
		List<JobNeeds> unneededJobs = new List<JobNeeds>();
		int excess = 0;
		for(int i = 0; i < this.citizens.Count; i++){
			switch(this.citizens[i].job.jobType){
			case JOB_TYPE.FARMER:
				excess = this.foodCount - neededResources [1];
				if(excess >= 0){
					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.FOOD, excess)));	
				}
				break;
			case JOB_TYPE.HUNTER:
				excess = this.foodCount - neededResources [1];
				if(excess >= 0){
					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.FOOD, excess)));	
				}
				break;
			case JOB_TYPE.WOODSMAN:
				excess = this.lumberCount - neededResources [2];
				if(excess >= 0){
					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.LUMBER, excess)));	
				}
				break;
			case JOB_TYPE.MINER:
				excess = this.stoneCount - neededResources [3];
				if(excess >= 0){
					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.STONE, excess)));	
				}
				break;
			case JOB_TYPE.ALCHEMIST:
				excess = this.manaStoneCount - neededResources [4];
				if(excess >= 0){
					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.MANA_STONE, excess)));	
				}
				break;
			}
		}
		return unneededJobs.Distinct().OrderByDescending(x => x.resource.resourceQuantity).ToList();
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
		
	internal CityUpgradeRequirements UpgradeRequirements(int level){
		CityUpgradeRequirements req = new CityUpgradeRequirements ();

		switch(level + 1){
		case 2:
			req.resource.Add (new Resource (RESOURCE.GOLD, 2000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 50));
			break;
		case 3:
			req.resource.Add (new Resource (RESOURCE.GOLD, 4000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 100));
			break;
		case 4:
			req.resource.Add (new Resource (RESOURCE.GOLD, 6000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 200));
			req.resource.Add (new Resource (RESOURCE.STONE, 100));
			break;
		case 5:
			req.resource.Add (new Resource (RESOURCE.GOLD, 8000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 400));
			req.resource.Add (new Resource (RESOURCE.STONE, 200));
			break;
		case 6:
			req.resource.Add (new Resource (RESOURCE.GOLD, 10000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 800));
			req.resource.Add (new Resource (RESOURCE.STONE, 400));
			req.resource.Add (new Resource (RESOURCE.MANA_STONE, 100));
			break;
		default:
			req.resource.Add (new Resource (RESOURCE.GOLD, 10000));
			req.resource.Add (new Resource (RESOURCE.LUMBER, 800));
			req.resource.Add (new Resource (RESOURCE.STONE, 400));
			req.resource.Add (new Resource (RESOURCE.MANA_STONE, 100));
			break;
		}

		return req;
	}
		
	internal void AttemptToIncreaseHousing(){
		if (CanCityAffordToUpgrade() && IsCitizenCapReached()) { //if city has the neccessary resources to upgrade and still has room for another citizen
			if(this.cityLevel >= 6){
				this.cityActionChances.increaseHousingChance =  this.cityActionChances.defaultIncreaseHousingChance;
				return;
			}
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.increaseHousingChance) {
				//Increase Housing Triggered, Increase Citizen Limit by 1
				this.cityActionChances.increaseHousingChance =  this.cityActionChances.defaultIncreaseHousingChance;
				this.citizenLimit += 1;
				this.cityLevel += 1;
				ReduceResources (this.cityUpgradeRequirements.resource);
				this.cityUpgradeRequirements = UpgradeRequirements (this.cityLevel);
				cityLogs += GameManager.Instance.currentDay.ToString() + ": The City level has increased: [FF0000]" + this.cityName.ToString() + "[-]\n\n"; 

			} else {
				//On not performing upgrade, increase chance to upgrade by 1
				this.cityActionChances.increaseHousingChance += 1;
			}
		}
	}

	internal void AttemptToUpgradeCitizen(){
		if(CanAffordToUpgradeCitizen()){
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.upgradeCitizenChance) {
				this.cityActionChances.upgradeCitizenChance = this.cityActionChances.defaultUpgradeCitizenChance;
				ReduceResources (this.upgradeCitizenTarget.GetUpgradeRequirements().resource);
				this.upgradeCitizenTarget.UpgradeCitizen();
				cityLogs += GameManager.Instance.currentDay.ToString() + ": The [FF0000]" + this.upgradeCitizenTarget.job.jobType.ToString() + "[-] clan's level has increased.\n\n"; 
				SelectCitizenToUpgrade ();
			} else {
				this.cityActionChances.upgradeCitizenChance += 1;
			}
		}
	}

	internal void AttemptToCreateNewCitizen(){
		if(!IsCitizenCapReached()){
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.newCitizenChance) {
				this.cityActionChances.newCitizenChance = this.cityActionChances.defaultNewCitizenChance;

				if(this.neededRole != JOB_TYPE.NONE){
					Citizen newCitizen = new Citizen (this.neededRole, this);
					AssignCitizenToTile (newCitizen);
					this.citizens.Add (newCitizen);
					this.neededRole = JOB_TYPE.NONE;
					cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + this.neededRole.ToString() + "[-] has emerged.\n\n"; 
				}else{
					Citizen newCitizen = new Citizen (this.newCitizenTarget, this);
					AssignCitizenToTile (newCitizen);
					this.citizens.Add (newCitizen);
					cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + this.newCitizenTarget.ToString() + "[-] has emerged.\n\n"; 
					SelectCitizenForCreation (false);
				}
			} else {
				this.cityActionChances.newCitizenChance += 1;
			}

		}
	}

	internal void AttemptToChangeCitizenRole(){
		if(this.unneededRoles.Count > 0){
			int randomRole = UnityEngine.Random.Range (0, this.unneededRoles.Count);
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.changeCitizenChance) {
				this.cityActionChances.changeCitizenChance = this.cityActionChances.defaultChangeCitizenChance;
				Citizen citizen = GetCitizenForChange (this.unneededRoles[randomRole]);
				if(this.neededRole != JOB_TYPE.NONE){
					citizen.ChangeJob (this.neededRole);
					this.neededRole = JOB_TYPE.NONE;
					cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]" + this.unneededRoles[randomRole].ToString() + "[-] clan is now [FF0000]" + this.neededRole.ToString() + "[-]\n\n"; 
				}else{
					citizen.ChangeJob (this.newCitizenTarget);
					SelectCitizenForCreation (false);
					cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]" + this.unneededRoles[randomRole].ToString() + "[-] clan is now [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
				}

				this.unneededRoles.Remove(this.unneededRoles[randomRole]);
			} else {
				this.cityActionChances.changeCitizenChance += 1;
			}
		}
	}

	internal Citizen GetCitizenForChange(JOB_TYPE unneededRole){
		List<Citizen> unneededCitizens = new List<Citizen> ();
		for(int i = 0; i < this.citizens.Count; i++){
			if(this.citizens[i].job.jobType == unneededRole){
				unneededCitizens.Add (this.citizens [i]);
			}
		}

		return unneededCitizens [UnityEngine.Random.Range (0, unneededCitizens.Count)];
	}
	bool CanAffordToUpgradeCitizen(){
		for(int i = 0; i < this.upgradeCitizenTarget.GetUpgradeRequirements().resource.Count; i++){
			if(GetNumberOfResourcesPerType(this.upgradeCitizenTarget.GetUpgradeRequirements().resource[i].resourceType) < this.upgradeCitizenTarget.GetUpgradeRequirements().resource[i].resourceQuantity){
				return false;
			}
		}
		return true;
	}

	bool CanCityAffordToUpgrade(){
		CityUpgradeRequirements upgradeRequirements = this.cityUpgradeRequirements;
		for (int i = 0; i < upgradeRequirements.resource.Count; i++) {
			RESOURCE resourceType = upgradeRequirements.resource [i].resourceType;
			if(upgradeRequirements.resource [i].resourceQuantity >= GetNumberOfResourcesPerType(resourceType)){
				//The city lacks atleast one resource for upgrade, return false
				return false;
			}
		}
		//The loop has finished, meaning the city has all the needed resources to upgrade, return true
		return true;
	}

	bool IsCitizenCapReached(){
		if (citizens.Count < citizenLimit) {
			return false;
		} else {
			return true;
		}
	}
	internal void ReduceResources(List<Resource> resource){
		for(int i = 0; i < resource.Count; i++){
			if (resource[i].resourceType == RESOURCE.FOOD) {
				this.foodCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.LUMBER) {
				this.lumberCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.MANA_STONE) {
				this.manaStoneCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.STONE) {
				this.stoneCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.GOLD) {
				this.goldCount -= resource[i].resourceQuantity;
			} 
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
		} else if (resourceType == RESOURCE.GOLD) {
			return this.goldCount;
		} 
//		else if (resourceType == RESOURCE.TRADE_GOOD) {
//			return this.tradeGoodsCount;
//		}
		return -1;
	}

}
