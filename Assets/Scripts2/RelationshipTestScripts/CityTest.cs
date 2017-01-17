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
	public int metalCount;
	public int tradeGoodsCount;
	public int goldValue;
	public int mayorLikeRating;
	public int citizenLimit;
	public CityActionChances cityActionChances;
	public CITY_STATE cityState;
	public List<Citizen> citizens = new List<Citizen>();
	public List<CityTileTest> connectedCities = new List<CityTileTest>();
	public List<HexTile> ownedBiomeTiles = new List<HexTile>();
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;
	public string cityLogs;
//	public JOB_TYPE foodProductionRole;
	public JOB_TYPE neededRole;
	public List<RESOURCE> unneededResources = new List<RESOURCE>();
	public List<JOB_TYPE> unneededRoles = new List<JOB_TYPE>();
	public JOB_TYPE newCitizenTarget;
	public Citizen upgradeCitizenTarget;
	public CityUpgradeRequirements cityUpgradeRequirements;
	public List<ResourceStatus> allResourcesStatus = new List<ResourceStatus>();
	public bool isDead;

	protected int foodStockpileCount = 0;

	public CityTest(HexTile hexTile, KingdomTileTest kingdom){
		this.id = 0;
		this.cityName = hexTile.name;
		this.biomeType = hexTile.biomeType;
		this.cityMayor = null;
		this.cityLevel = 1;
		this.numOfRoads = 0;
		this.population = 0;
		this.richnessLevel = 60;
		this.foodCount = 15;
		this.lumberCount = 0;
		this.stoneCount = 0;
		this.manaStoneCount = 0;
		this.metalCount = 0;
		this.tradeGoodsCount = 0;
		this.goldValue = GetGoldValue ();
		this.mayorLikeRating = 0;
		this.citizenLimit = 4;
		this.cityActionChances = new CityActionChances ();
		this.cityState = CITY_STATE.ABUNDANT;
		this.connectedCities = new List<CityTileTest>();
		this.ownedBiomeTiles = new List<HexTile>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = kingdom;
		this.hexTile = hexTile;
		this.cityLogs = string.Empty;
//		this.foodProductionRole = FoodProductionRole ();
		this.neededRole = JOB_TYPE.NONE;
		this.unneededRoles = new List<JOB_TYPE>();
		this.upgradeCitizenTarget = null;
		this.newCitizenTarget = JOB_TYPE.NONE;
		this.allResourcesStatus = GetInitialResourcesStatus ();
		this.cityUpgradeRequirements = UpgradeRequirements (this.cityLevel);
		this.isDead = false;
		this.ownedBiomeTiles.Add (this.hexTile);
		this.citizens = InitialCitizens();
		AssignInitialCitizens ();
		SelectCitizenToUpgrade ();
		SelectCitizenForCreation (false);

	}

	internal List<Citizen> InitialCitizens(){
		List<Citizen> citizens = new List<Citizen> ();
		citizens.Add(new Citizen (JOB_TYPE.FARMER, this));
		citizens.Add(new Citizen (JOB_TYPE.QUARRYMAN, this));
		citizens.Add(new Citizen (JOB_TYPE.WOODSMAN, this));
		citizens.Add(new Citizen (JOB_TYPE.WARRIOR, this));

		return citizens;
	}

	internal List<ResourceStatus> GetInitialResourcesStatus(){
		List<ResourceStatus> resourcesStatus = new List<ResourceStatus> ();
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		for(int i = 0; i < allResources.Length; i++){
			resourcesStatus.Add(new ResourceStatus(allResources[i], RESOURCE_STATUS.NORMAL));
		}
		return resourcesStatus;
	}

	internal void GenerateInitialFood(){
		this.foodCount = GetNeededFoodForNumberOfDays (40);
	}

	int GetNeededFoodForNumberOfDays(int days){
		int dailyFoodConsumption = ComputeFoodConsumption();
		return dailyFoodConsumption * days;
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

		int halfGoldValue = (int)((float)this.goldValue / 2f);
		int producedGold = halfGoldValue + UnityEngine.Random.Range(1, halfGoldValue + 1);
		this.goldCount += producedGold;

		for(int i = 0; i < this.citizens.Count; i++){
			int[] resourcesProducedByCitizen = this.citizens[i].GetAllDailyProduction();
			this.goldCount += resourcesProducedByCitizen[0];
//			this.foodCount += resourcesProducedByCitizen[1];
			this.foodStockpileCount += resourcesProducedByCitizen[1]; //Add food to stockpile instead
			this.lumberCount += resourcesProducedByCitizen[2];
			this.stoneCount += resourcesProducedByCitizen[3];
			this.manaStoneCount += resourcesProducedByCitizen[4];
			this.metalCount += resourcesProducedByCitizen[5];
		}
	}
	internal int GetGoldValue(){
		int goldValue = 0;
		for(int i = 0; i < this.ownedBiomeTiles.Count; i++){
			goldValue += this.ownedBiomeTiles [i].goldValue;
		}
		return goldValue;
	}

	internal void TriggerFoodHarvest(){
//		Debug.Log ("Food Harvest!: Spoiled - " + this.foodCount.ToString () + "/ New Food - " + this.foodStockpileCount.ToString ());
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Harvest Day, food is now [FF0000]" + this.foodStockpileCount.ToString() + "[-].\n\n";
		this.foodCount = this.foodStockpileCount;
		this.foodStockpileCount = 0;
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
			UpdateResourcesStatus();
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
//		neighbours.AddRange(this.hexTile.GetListTilesInRange (0.5f));
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

		UpdateResourcesStatus ();
	}
		
	int GetTotalChanceForUpgrade(){
		List<Citizen> citizensToChooseFrom = citizens.OrderBy(x => x.level).ToList();
		citizensToChooseFrom.RemoveAll (x => x.level >= 10);
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

	private List<JobNeeds> GetListJobsTarget(int[] neededResources){ //gold, food, lumber, stone, manastone, metal

		int resourceDeficit = 0;
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		List<JobNeeds> neededJobs = new List<JobNeeds> ();
		for (int i = 0; i < allResources.Length; i++) {
			
			resourceDeficit = neededResources [i] - GetNumberOfResourcesPerType(allResources[i]);

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
		int averageDailyProd = GetAveDailyProduction (RESOURCE.FOOD);
		int daysUntilResourcesFinish = 0;
		if (averageDailyProd > 0) {
			daysUntilResourcesFinish = (int) ((GetNeededFoodForNumberOfDays(30) - this.foodStockpileCount) / averageDailyProd);
		}
		if(daysUntilResourcesFinish > GameManager.Instance.daysUntilNextHarvest || daysUntilResourcesFinish <= 0){
			if(this.neededRole == JOB_TYPE.NONE){
				int averageFarmingValue = GetAveHexValue(BIOME_PRODUCE_TYPE.FARMING);
				int averageHuntingValue = GetAveHexValue(BIOME_PRODUCE_TYPE.HUNTING);
				if (averageFarmingValue > averageHuntingValue) {
					this.neededRole = JOB_TYPE.FARMER;
				} else if (averageFarmingValue < averageHuntingValue) {
					this.neededRole = JOB_TYPE.HUNTER;
				} else {
					int chance = UnityEngine.Random.Range (0, 2);
					if (chance == 0) {
						this.neededRole = JOB_TYPE.FARMER;
					} else {
						this.neededRole = JOB_TYPE.HUNTER;
					}
				}
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

		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus[i].status == RESOURCE_STATUS.ABUNDANT) {
				unneededResources.Add (this.allResourcesStatus[i].resource);
				cityLogs += GameManager.Instance.currentDay.ToString() + ": There is an oversupply of [FF0000]" + this.allResourcesStatus[i].resource.ToString() + "[-]\n\n";
			}
		}
//		int excess = 0;
//		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
//		for (int i = 0; i < allResources.Length; i++) {
//			if(allResources[i] == RESOURCE.FOOD){
//				excess = this.foodCount - (10 * ComputeFoodConsumption());
//			}else{
//				excess = GetNumberOfResourcesPerType(allResources[i]) - neededResources [i];
//			}
//
//			if (excess > 0) {
//				unneededResources.Add (allResources[i]);
//				cityLogs += GameManager.Instance.currentDay.ToString() + ": There is an oversupply of [FF0000]" + allResources[i].ToString() + "[-]\n\n"; 
//			}
//		}

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
//	internal List<JobNeeds> GetJobsWithExcessResources(int[] neededResources){
//		List<JobNeeds> unneededJobs = new List<JobNeeds>();
//		int excess = 0;
//		for(int i = 0; i < this.citizens.Count; i++){
//			switch(this.citizens[i].job.jobType){
//			case JOB_TYPE.FARMER:
//				excess = this.foodCount - neededResources [1];
//				if(excess >= 0){
//					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.FOOD, excess)));	
//				}
//				break;
//			case JOB_TYPE.HUNTER:
//				excess = this.foodCount - neededResources [1];
//				if(excess >= 0){
//					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.FOOD, excess)));	
//				}
//				break;
//			case JOB_TYPE.WOODSMAN:
//				excess = this.lumberCount - neededResources [2];
//				if(excess >= 0){
//					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.LUMBER, excess)));	
//				}
//				break;
//			case JOB_TYPE.MINER:
//				excess = this.stoneCount - neededResources [3];
//				if(excess >= 0){
//					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.STONE, excess)));	
//				}
//				break;
//			case JOB_TYPE.ALCHEMIST:
//				excess = this.manaStoneCount - neededResources [4];
//				if(excess >= 0){
//					unneededJobs.Add (new JobNeeds(this.citizens[i].job.jobType, new Resource(RESOURCE.MANA, excess)));	
//				}
//				break;
//			}
//		}
//		return unneededJobs.Distinct().OrderByDescending(x => x.resource.resourceQuantity).ToList();
//	}

	private int[] NeededResources(){
		int[] neededResources = new int[]{0, 0, 0, 0, 0, 0}; //gold, food, lumber, stone, manastone, metal

		//BASED ON UPGRADE CITIZEN
		if (this.upgradeCitizenTarget == null) {
			SelectCitizenToUpgrade();
		}
		for(int i = 0; i < this.upgradeCitizenTarget.GetUpgradeRequirements().resource.Count; i++){
			switch(this.upgradeCitizenTarget.GetUpgradeRequirements().resource[i].resourceType){
			case RESOURCE.GOLD:
				neededResources[0] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				neededResources[1] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[2] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[3] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA:
				neededResources[4] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			case RESOURCE.METAL:
				neededResources[5] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
				break;
			}
		}

		//BASED ON HOUSING UPGRADE
		for(int i = 0; i < this.cityUpgradeRequirements.resource.Count; i++){
			switch(this.cityUpgradeRequirements.resource[i].resourceType){
			case RESOURCE.GOLD:
				neededResources[0] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				neededResources[1] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				neededResources[2] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				neededResources[3] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.MANA:
				neededResources[4] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			case RESOURCE.METAL:
				neededResources[5] += this.cityUpgradeRequirements.resource [i].resourceQuantity;
				break;
			}
		}

		return neededResources;
	}
		
	internal CityUpgradeRequirements UpgradeRequirements(int level){
		CityUpgradeRequirements req = new CityUpgradeRequirements ();

		switch(level + 1){
		case 2:
//			req.resource.Add (new Resource (RESOURCE.GOLD, 2000));
			req.resource.Add (new Resource (RESOURCE.STONE, 900));
			break;
		case 3:
			req.resource.Add (new Resource (RESOURCE.STONE, 2000));
			break;
		case 4:
			req.resource.Add (new Resource (RESOURCE.STONE, 3500));
			break;
		case 5:
			req.resource.Add (new Resource (RESOURCE.STONE, 5000));
			break;
		case 6:
			req.resource.Add (new Resource (RESOURCE.STONE, 6500));
			req.resource.Add (new Resource (RESOURCE.MANA, 900));
			break;
		case 7:
			req.resource.Add (new Resource (RESOURCE.STONE, 8000));
			req.resource.Add (new Resource (RESOURCE.MANA, 2000));
			break;
		case 8:
			req.resource.Add (new Resource (RESOURCE.STONE, 11000));
			req.resource.Add (new Resource (RESOURCE.MANA, 3500));
			break;
		case 9:
			req.resource.Add (new Resource (RESOURCE.STONE, 15000));
			req.resource.Add (new Resource (RESOURCE.MANA, 5000));
			break;
		case 10:
			req.resource.Add (new Resource (RESOURCE.STONE, 20000));
			req.resource.Add (new Resource (RESOURCE.MANA, 6500));
			break;
		default:
			req.resource.Add (new Resource (RESOURCE.STONE, 20000));
			req.resource.Add (new Resource (RESOURCE.MANA, 6500));
			break;
		}

		return req;
	}
		
	internal void AttemptToIncreaseHousing(){
		if (CanCityAffordToUpgrade() && IsCitizenCapReached()) { //if city has the neccessary resources to upgrade and still has room for another citizen
			if(this.cityLevel >= 10){
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
				UpdateResourcesStatus ();
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
				this.upgradeCitizenTarget = null;
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
					cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + this.neededRole.ToString() + "[-] has emerged.\n\n"; 
					this.neededRole = JOB_TYPE.NONE;
				}else{
					Citizen newCitizen = new Citizen (this.newCitizenTarget, this);
					AssignCitizenToTile (newCitizen);
					this.citizens.Add (newCitizen);
					cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + this.newCitizenTarget.ToString() + "[-] has emerged.\n\n"; 
					this.newCitizenTarget = JOB_TYPE.NONE;
					SelectCitizenForCreation (false);
				}
				UpdateResourcesStatus ();
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
					cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]" + this.unneededRoles[randomRole].ToString() + "[-] clan is now [FF0000]" + this.neededRole.ToString() + "[-]\n\n"; 
					this.neededRole = JOB_TYPE.NONE;
				}else{
					citizen.ChangeJob (this.newCitizenTarget);
					cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]" + this.unneededRoles[randomRole].ToString() + "[-] clan is now [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
					this.newCitizenTarget = JOB_TYPE.NONE;
					SelectCitizenForCreation (false);
				}
				this.unneededRoles.Remove(this.unneededRoles[randomRole]);
//				UpdateResourcesStatus ();
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
			if (resource[i].resourceType == RESOURCE.GOLD) {
				this.goldCount -= resource[i].resourceQuantity;
			}else if (resource[i].resourceType == RESOURCE.FOOD) {
				this.foodCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.LUMBER) {
				this.lumberCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.MANA) {
				this.manaStoneCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.STONE) {
				this.stoneCount -= resource[i].resourceQuantity;
			} else if (resource[i].resourceType == RESOURCE.METAL) {
				this.metalCount -= resource[i].resourceQuantity;
			} 
		}

	}
	int GetNumberOfResourcesPerType(RESOURCE resourceType){
		if (resourceType == RESOURCE.GOLD) {
			return this.goldCount;
		}else if (resourceType == RESOURCE.FOOD) {
			return this.foodCount;
		} else if (resourceType == RESOURCE.LUMBER) {
			return this.lumberCount;
		} else if (resourceType == RESOURCE.MANA) {
			return this.manaStoneCount;
		} else if (resourceType == RESOURCE.STONE) {
			return this.stoneCount;
		} else if (resourceType == RESOURCE.METAL) {
			return this.metalCount;
		} 
//		else if (resourceType == RESOURCE.TRADE_GOOD) {
//			return this.tradeGoodsCount;
//		}
		return -1;
	}

	public int GetNumberOfCitizensPerType(JOB_TYPE jobType){
		int count = 0;
		for (int i = 0; i < citizens.Count; i++) {
			if (citizens [i].job.jobType == jobType) {
				count++;
			}
		}
		return count;
	}

	internal void UpdateResourcesStatus(){
		int[] neededResources = NeededResources ();
		for(int i = 0; i < this.allResourcesStatus.Count; i++){
			if(this.allResourcesStatus[i].resource == RESOURCE.FOOD){
				int days = (int)(this.foodCount / ComputeFoodConsumption ());
				if(days >= (GameManager.Instance.daysUntilNextHarvest + 3)){ //ABUNDANT
					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
				}else if (days <= GameManager.Instance.daysUntilNextHarvest){ //SCARCE
					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
				}else{ //NORMAL
					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
				}
			}else{
				int excess = GetNumberOfResourcesPerType (this.allResourcesStatus [i].resource) - neededResources [i];
				if(excess > 0){ //ABUNDANT
					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
				}else if (excess < 0){ //SCARCE
					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
				}else{  //NORMAL
					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
				}
			}
		}
	}

	//Get Daily Production Based On Resource Type
	internal int GetAveDailyProduction(RESOURCE resourceType){
		int totalDailyProduction = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			totalDailyProduction += this.citizens [i].GetAveDailyProduction(resourceType);
		}
		return totalDailyProduction;
	}

	internal int GetAveHexValue(BIOME_PRODUCE_TYPE produceType){
		int totalValue = 0;
		for (int i = 0; i < this.ownedBiomeTiles.Count; i++) {
			switch (produceType) {
			case BIOME_PRODUCE_TYPE.FARMING:
				totalValue += this.ownedBiomeTiles[i].farmingValue;
				break;
			case BIOME_PRODUCE_TYPE.HUNTING:
				totalValue += this.ownedBiomeTiles[i].huntingValue;
				break;
			}
		}
		totalValue = totalValue / this.ownedBiomeTiles.Count;
		return totalValue;
	}
}
