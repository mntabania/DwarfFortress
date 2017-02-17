﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CityTest{

	public int id;
	public string cityName;
	public BIOMES biomeType;
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
	public int goldValue;
	public int mayorLikeRating;
	public int citizenLimit;
	public int generalsLimit;
	public int unrest;
	public int armyMaintenanceAmount;
	public float farmerMultiplier;
	public float hunterMultiplier;
	public float alchemistMultiplier;
	public float quarrymanMultiplier;
	public float minerMultiplier;
	public float woodsmanMultiplier;
	public float goldMultiplier;
//	public int incomingMilitaryStrength;
//	public int neededArmyStrength;
	public int helpStrength;
	public CityTest targetCity;
	public bool hasTarget;
	public List<General> visitingGenerals;
//	public List<General> helpGenerals;
	public List<General> deployedGenerals;
	public CityActionChances cityActionChances;
	public CITY_STATE cityState;
	public List<Citizen> citizens = new List<Citizen>();
	public List<General> generals = new List<General>();
	public List<General> activeGenerals = new List<General>();
	public List<CityTileTest> connectedCities = new List<CityTileTest>();
	public List<HexTile> ownedBiomeTiles = new List<HexTile>();
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTileTest kingdomTile;
	public HexTile hexTile;
	public string cityLogs;
	public JOB_TYPE neededRole;
	public List<RESOURCE> unneededResources = new List<RESOURCE>();
	public List<JOB_TYPE> unneededRoles = new List<JOB_TYPE>();
	public JOB_TYPE newCitizenTarget;
	public Citizen upgradeCitizenTarget;
	public CityUpgradeRequirements cityUpgradeRequirements;
	public List<ResourceStatus> allResourcesStatus = new List<ResourceStatus>();
	public bool isDead;
	public int foodStockpileCount = 0;
	public HexTile targetHexTileToPurchase;

	protected int dayPioneerReachesCity = 0;
	protected CityTileTest pioneerCityTarget = null;

	/*
	 * Connected cities that
	 * are unoccupied.
	 * */
	public List<CityTileTest> unoccupiedConnectedCities{
		get { return this.connectedCities.Where (x => !x.hexTile.isOccupied).ToList();}
	}

	/*
	 * Owned Hex Tiles that
	 * are unoccupied.
	 * */
	protected List<HexTile> unoccupiedOwnedTiles{ 
		get { return this.ownedBiomeTiles.Where (x => !x.isOccupied).ToList(); }
	}

	/*
	 * List of initial citizens 
	 * of each city, used for initialization.
	 * */
	protected List<Citizen> InitialCitizens{
		get{
			return new List<Citizen> () {
				new Citizen (JOB_TYPE.FARMER, this),
				new Citizen (JOB_TYPE.QUARRYMAN, this),
				new Citizen (JOB_TYPE.WOODSMAN, this)
			};
		}
	}
		
	public CityTest(HexTile hexTile, KingdomTileTest kingdom){
		this.id = GetID()+1;
		this.cityName = hexTile.name;
		this.biomeType = hexTile.biomeType;
		this.cityLevel = 1;
		this.numOfRoads = 0;
		this.population = 0;
		this.richnessLevel = 60;
		this.foodCount = 15;
		this.lumberCount = 0;
		this.stoneCount = 0;
		this.manaStoneCount = 0;
		this.metalCount = 0;
		this.goldValue = GetGoldValue ();
		this.mayorLikeRating = 0;
		this.citizenLimit = 4;
		this.generalsLimit = 1;
		this.unrest = 0;
		this.armyMaintenanceAmount = 100;
		this.farmerMultiplier = 2f;
		this.hunterMultiplier = 2f;
		this.alchemistMultiplier = 2f;
		this.quarrymanMultiplier = 2f;
		this.minerMultiplier = 2f;
		this.woodsmanMultiplier = 2f;
		this.goldMultiplier = 1f;
		this.helpStrength = 0;
		this.targetCity = null;
		this.hasTarget = false;
		this.visitingGenerals = new List<General>();
		this.deployedGenerals = new List<General>();
//		this.helpGenerals = new List<DeployedGenerals>();
		this.cityActionChances = new CityActionChances ();
		this.cityState = CITY_STATE.ABUNDANT;
		this.connectedCities = new List<CityTileTest>();
		this.ownedBiomeTiles = new List<HexTile>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = kingdom;
		this.hexTile = hexTile;
		this.cityLogs = string.Empty;
		this.neededRole = JOB_TYPE.NONE;
		this.unneededRoles = new List<JOB_TYPE>();
		this.upgradeCitizenTarget = null;
		this.newCitizenTarget = JOB_TYPE.NONE;
		this.allResourcesStatus = GetInitialResourcesStatus ();
		this.isDead = false;
		this.ownedBiomeTiles.Add (this.hexTile);
//		this.citizens = InitialCitizens();
		SetLastID (this.id);
	}

	#region ID
	int GetID(){
		return Utilities.lastCityId;
	}

	void SetLastID(int id){
		Utilities.lastCityId = id;
	}
	#endregion

	/*
	 * Occupies current city.
	 * */
	internal void OccupyCity(){
		this.hexTile.isOccupied = true;
		this.citizens = this.InitialCitizens;
		CreateGeneral ();
		SelectHexTileToPurchase();
		GenerateInitialFood();
		UpdateCityUpgradeRequirements ();
		SelectCitizenForCreation ();
		this.kingdomTile.kingdom.lord.UpdateAdjacentLords();
		GeneralAI.onInstruct += ReceiveRequestFromGeneral;
//		LordMilitaryAI.onInstruct += this.ReceiveInstructionsFromLord;
	}
	private void ReceiveInstructionsFromLord(LORD_INSTRUCTIONS instructions, CityTest city, CityTest targetCity){
		if(city.id == this.id){
			switch (instructions){
			case LORD_INSTRUCTIONS.INCREASE_ARMY:
				break;
			case LORD_INSTRUCTIONS.PROVIDE_HELP:
				break;
			case LORD_INSTRUCTIONS.CREATE_GENERALS:
				break;
			case LORD_INSTRUCTIONS.ATTACK:
				break;
			case LORD_INSTRUCTIONS.DEFEND:
				break;
			}
		}
	}
	private void ReceiveRequestFromGeneral(GENERAL_STATUS status, General general){
		if(general.city.id == this.id){
			switch (status){
			case GENERAL_STATUS.BATTLE_DONE:
				break;
			}
		}
	}
//	internal List<Citizen> InitialCitizens(){
//		CityTest city = this;
//		List<Citizen> citizens = new List<Citizen> ();
//		citizens.Add(new Citizen (JOB_TYPE.FARMER, city));
//		citizens.Add(new Citizen (JOB_TYPE.QUARRYMAN, city));
//		citizens.Add(new Citizen (JOB_TYPE.WOODSMAN, city));
////		citizens.Add(new Citizen (JOB_TYPE.DEFENSE_GENERAL, city));
////		this.defenseGenerals.Add(new General(GENERAL_CLASSIFICATION.DEFENSE));
//		//Assign each initial citizen to a tile
//		for (int i = 0; i < this.citizens.Count; i++) {
//			List<HexTile> neighbours = new List<HexTile>();
//			for (int j = 0; j < this.ownedBiomeTiles.Count; j++) {
//				neighbours.AddRange (this.ownedBiomeTiles [i].GetListTilesInRange(0.5f));
//			}
//			neighbours = neighbours.Distinct().ToList();
//
//			this.citizens[i].AssignCitizenToTile(neighbours);
//		}
//	}
		
	internal List<ResourceStatus> GetInitialResourcesStatus(){
		List<ResourceStatus> resourcesStatus = new List<ResourceStatus> ();
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		for(int i = 0; i < allResources.Length; i++){
			resourcesStatus.Add(new ResourceStatus(allResources[i], RESOURCE_STATUS.NORMAL, 0));
		}
		return resourcesStatus;
	}

	/*
	 * Give city initial food for
	 * number of days before harvest +3. 
	 * */
	internal void GenerateInitialFood(){
		this.foodCount = GetNeededFoodForNumberOfDays (GameManager.Instance.daysUntilNextHarvest + 3);
	}

	/*
	 * Get needed food count for
	 * specific number of days.
	 * */
	int GetNeededFoodForNumberOfDays(int days){
		return GetDailyFoodConsumption() * days;
	}

	/*
	 * Returns total daily food
	 * consumption.
	 * */
	internal int GetDailyFoodConsumption(){
		int totalFoodConsumption = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			totalFoodConsumption += this.citizens [i].FoodConsumption();
		}
		return totalFoodConsumption;
	}

	/*
	 * Consume Food, called every day.
	 * Starvation causes unrest ti increase by 1 and
	 * has a slight chance to cause a citizen to die.
	 * */
	internal void ConsumeFood(int foodRequirement){
		if(isDead){
			return;
		}
		this.foodCount -= foodRequirement;
		if(this.foodCount < 0){
			this.cityState = CITY_STATE.STARVATION;
			cityLogs += GameManager.Instance.currentDay.ToString() + ": FOOD ration is running low. \n\n"; 
			this.unrest += 1;
			//Code for death
			int chance = UnityEngine.Random.Range (0, 100);
			if (chance < 2) {
				int russianRoulette = UnityEngine.Random.Range (0, this.citizens.Count);
				cityLogs += GameManager.Instance.currentDay.ToString() + ": The entire [FF0000]" + this.citizens[russianRoulette].job.jobType.ToString() + "[-] clan perished.\n\n"; 
				this.citizens.Remove (this.citizens [russianRoulette]);
				this.unrest += 10;
				UpdateResourcesStatus();
			}
		}else{
			this.cityState = CITY_STATE.ABUNDANT;
		}
	}
		
	/*
	 * Harvest food in stockpile.
	 * */
	internal void TriggerFoodHarvest(){
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Harvest Day, food is now [FF0000]" + this.foodStockpileCount.ToString() + "[-].\n\n";
		this.foodCount = this.foodStockpileCount;
		this.foodStockpileCount = 0;
	}

	/*
	 * Produce All Resources, called every day.
	 * */
	internal void ProduceResources(){
		if(isDead){
			return;
		}

		int halfGoldValue = (int)((float)this.goldValue / 2f);
		int producedGold = (int)((float)(halfGoldValue + UnityEngine.Random.Range(1, halfGoldValue + 1)) * goldMultiplier);
		this.goldCount += producedGold;

		for(int i = 0; i < this.citizens.Count; i++){
			int starvationModifier = 1;
			if (this.cityState == CITY_STATE.STARVATION) {
				starvationModifier = 2;
			}
			int[] resourcesProducedByCitizen = this.citizens[i].GetAllDailyProduction();
			this.goldCount += (int)(resourcesProducedByCitizen[0] / starvationModifier);
			this.foodStockpileCount += resourcesProducedByCitizen[1]; //Add food to stockpile instead
			this.lumberCount += (int)(resourcesProducedByCitizen[2] / starvationModifier);
			this.stoneCount += (int)(resourcesProducedByCitizen[3] / starvationModifier);
			this.manaStoneCount += (int)(resourcesProducedByCitizen[4] / starvationModifier);
			this.metalCount += (int)(resourcesProducedByCitizen[5] / starvationModifier);
		}
	}

	/*
	 * Get Total Gold Value of all
	 * owned tiles.
	 * */
	internal int GetGoldValue(){
		int goldValue = 0;
		for(int i = 0; i < this.ownedBiomeTiles.Count; i++){
			goldValue += this.ownedBiomeTiles [i].goldValue;
		}
		return goldValue;
	}

	/*
	 * Assign a specific citizen to a 
	 * tile.
	 * */
	void AssignCitizenToTile(Citizen citizen){
		if (citizen.job.residence == RESIDENCE.OUTSIDE) {
			List<HexTile> tilesToChooseFrom = this.ownedBiomeTiles.Where (x => !x.isOccupied).ToList();
			tilesToChooseFrom.OrderByDescending (x => x.GetRelevantResourceValueByJobType (citizen.job.jobType));
			if (tilesToChooseFrom.Count > 0) {
				if (tilesToChooseFrom [0].GetRelevantResourceValueByJobType (citizen.job.jobType) == 0) {
					Debug.Log("Resource Value of target tile is 0");
				}
				if (citizen.assignedTile == null || citizen.assignedTile.isCity) {
					citizen.SetCitizenTile (tilesToChooseFrom [0]);
				} else {
					int currentAssignedTileValue = citizen.assignedTile.GetRelevantResourceValueByJobType (citizen.job.jobType);
					int candidateTileValue = tilesToChooseFrom [0].GetRelevantResourceValueByJobType (citizen.job.jobType);
					if (candidateTileValue > currentAssignedTileValue) {
						citizen.assignedTile.SetTileAsUnoccupied ();
						citizen.SetCitizenTile (tilesToChooseFrom [0]);
					}
				}
			} else {
				if (citizen.assignedTile != null) {
					Debug.Log ("Coup Ledesma: " + citizen.assignedTile.GetRelevantResourceValueByJobType (citizen.job.jobType));
				}
			}
		} else {
			if (citizen.assignedTile != null) {
				citizen.assignedTile.SetTileAsUnoccupied ();
			}
			citizen.SetCitizenTile(this.hexTile);
		}

	}
		
	#region Citizen Creation Functions
	internal void SelectCitizenForCreation(){
		if(isDead){
			return;
		}
		int[] neededResources = NeededResources ();
		List<JobNeeds> neededJobs = GetListJobsTarget (neededResources);

//		for (int i = 0; i < neededJobs.Count; i++) {
//			List<Resource> citizenCreationCost = GetCitizenCreationCostPerType(neededJobs[i].jobType);
//			for (int j = 0; j < citizenCreationCost.Count; j++) {
//				if (citizenCreationCost[j].resourceType != RESOURCE.GOLD && GetNumberOfResourcesPerType(citizenCreationCost[j].resourceType) < citizenCreationCost[j].resourceQuantity) {
//					if (GetAveDailyProduction(citizenCreationCost [j].resourceType) <= 0) {
//						neededJobs.Remove(neededJobs[i]);
//					}
//				}
//			}
//		}

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
		} else{ //WARRIOR
//			if(GetNumberOfCitizensPerType (JOB_TYPE.DEFENSE_GENERAL) == 0){
//				this.newCitizenTarget = JOB_TYPE.DEFENSE_GENERAL;
//			}
//			this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//			if (GetNumberOfCitizensPerType (JOB_TYPE.OFFENSE_GENERAL) >= this.offenseGeneralsLimit) {
//				if (this.unoccupiedOwnedTiles.Count > 0) {
//					List<HexTile> tilesByHighestResource = unoccupiedOwnedTiles.OrderByDescending (x => x.GetHighestResourceValue ()).ToList ();
//					this.newCitizenTarget = tilesByHighestResource [0].GetBestJobForTile ();
//				} else {
//					this.newCitizenTarget = JOB_TYPE.NONE;
//				}
//			} else {
//				this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//			}
//			cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected job to be created: [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
//			if (GetNumberOfCitizensPerType (JOB_TYPE.DEFENSE_GENERAL) == 0) {
//				this.newCitizenTarget = JOB_TYPE.DEFENSE_GENERAL;
//			} else {
//				this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//			}
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

	private List<JobNeeds> GetListJobsTarget(int[] neededResources){ //gold, food, lumber, stone, manastone, metal

		int resourceDeficit = 0;
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		List<JobNeeds> neededJobs = new List<JobNeeds> ();
		for (int i = 0; i < allResources.Length; i++) {
			if (allResources [i] == RESOURCE.FOOD) {
				continue;
			}

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
	#endregion



	/*
	 * Assign needed role, can only be
	 * Farmer, Hunter or Pioneer
	 * */
	internal void AssignNeededRole(){
		if(isDead){
			return;
		}

		int averageDailyProd = GetAveDailyProduction (RESOURCE.FOOD);
		int daysUntilResourcesFinish = 0;
		if (averageDailyProd > 0) {
			int neededFood = (GetNeededFoodForNumberOfDays(33));
			daysUntilResourcesFinish = (int) (neededFood - this.foodStockpileCount) / averageDailyProd;
		}

		if(daysUntilResourcesFinish > GameManager.Instance.daysUntilNextHarvest || averageDailyProd <= 0){
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
		} else {
			if (this.neededRole != JOB_TYPE.PIONEER) {
				this.neededRole = JOB_TYPE.NONE;
			}
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

	private int[] NeededResources(){
		int[] neededResources = new int[]{0, 0, 0, 0, 0, 0}; //gold, food, lumber, stone, manastone, metal

//		//BASED ON UPGRADE CITIZEN
//		if (this.upgradeCitizenTarget == null) {
//			SelectCitizenToUpgrade();
//		}
//		for(int i = 0; i < this.upgradeCitizenTarget.GetUpgradeRequirements().resource.Count; i++){
//			switch(this.upgradeCitizenTarget.GetUpgradeRequirements().resource[i].resourceType){
//			case RESOURCE.GOLD:
//				neededResources[0] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			case RESOURCE.FOOD:
//				neededResources[1] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			case RESOURCE.LUMBER:
//				neededResources[2] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			case RESOURCE.STONE:
//				neededResources[3] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			case RESOURCE.MANA:
//				neededResources[4] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			case RESOURCE.METAL:
//				neededResources[5] += this.upgradeCitizenTarget.GetUpgradeRequirements().resource [i].resourceQuantity;
//				break;
//			}
//		}

		//BASED ON ARMY NEEDED RESOURCES
		if(this.kingdomTile != null){
			for(int i = 0; i < this.kingdomTile.kingdom.armyIncreaseUnitResource.Count; i++){
				switch (this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceType) {
				case RESOURCE.GOLD:
					neededResources[0] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				case RESOURCE.FOOD:
					neededResources[1] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				case RESOURCE.LUMBER:
					neededResources[2] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				case RESOURCE.STONE:
					neededResources[3] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				case RESOURCE.MANA:
					neededResources[4] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				case RESOURCE.METAL:
					neededResources[5] += this.kingdomTile.kingdom.armyIncreaseUnitResource[i].resourceQuantity;
					break;
				}
			}
		}
		//BASED ON HEX TILE TO PURCHASE
		if (this.targetHexTileToPurchase != null) {
			List<Resource> resourceReqs = GetHexTileCost (this.targetHexTileToPurchase);
			for (int i = 0; i < resourceReqs.Count; i++) {
				switch (resourceReqs[i].resourceType) {
				case RESOURCE.GOLD:
					neededResources[0] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.FOOD:
					neededResources[1] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.LUMBER:
					neededResources[2] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.STONE:
					neededResources[3] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.MANA:
					neededResources[4] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.METAL:
					neededResources[5] += resourceReqs[i].resourceQuantity;
					break;
				}
			}
		}

		//BASED ON NEEDED ROLE
		if (this.neededRole != JOB_TYPE.NONE) {
			List<Resource> resourceReqs = GetCitizenCreationCostPerType (this.neededRole);
			for (int i = 0; i < resourceReqs.Count; i++) {
				switch (resourceReqs[i].resourceType) {
				case RESOURCE.GOLD:
					neededResources[0] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.FOOD:
					neededResources[1] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.LUMBER:
					neededResources[2] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.STONE:
					neededResources[3] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.MANA:
					neededResources[4] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.METAL:
					neededResources[5] += resourceReqs[i].resourceQuantity;
					break;
				}
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
		
	internal void AttemptToCreateGeneral(){
		if(this.generals.Count < this.generalsLimit){
			CreateGeneral ();
		}
	}
	internal void CreateGeneral(){
		General newGeneral = new General (this);
		this.generals.Add (newGeneral);
	}
	internal void ArmyMaintenance(){
		if(GameManager.Instance.currentDay % 8 == 0){
			for(int i = 0; i < this.generals.Count; i++){
				if (this.goldCount >= this.armyMaintenanceAmount) {
					Debug.Log (this.generals[i].name + " ARMY IS MAINTAINED!");
					AdjustResourceCount (RESOURCE.GOLD, -this.armyMaintenanceAmount);
				}else{
					Debug.Log ("CAN'T MAINTAIN ARMY. COUNT WILL BE REDUCED!");
					this.generals [i].army.armyCount -= this.kingdomTile.kingdom.armyIncreaseUnits;
					if(this.generals[i].army.armyCount <= 0){
						this.generals.RemoveAt (i);
						i--;
					}
				}
			}
//			for(int i = 0; i < this.citizens.Count; i++){
//				if(this.citizens[i].job.jobType == JOB_TYPE.DEFENSE_GENERAL || this.citizens[i].job.jobType == JOB_TYPE.OFFENSE_GENERAL){
//					if (this.goldCount >= this.armyMaintenanceAmount) {
////						Debug.Log (this.citizens[i].name + " ARMY IS MAINTAINED!");
//						AdjustResourceCount (RESOURCE.GOLD, -this.armyMaintenanceAmount);
//					}else{
////						Debug.Log ("CAN'T MAINTAIN ARMY. COUNT WILL BE REDUCED!");
//						this.citizens [i].job.army.armyCount -= this.kingdomTile.kingdom.armyIncreaseUnits;
//						if(this.citizens[i].job.army.armyCount <= 0){
//							this.citizens.RemoveAt (i);
//							i--;
//						}
//					}
//				}
//			}
		}
	}
	internal void AttemptToIncreaseArmyCount(){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < this.cityActionChances.increaseArmyCountChance){
			if(HasEnoughResourcesForAction(this.kingdomTile.kingdom.armyIncreaseUnitResource)){
				if (this.generals.Count > 0) {
					List<General> generals = this.generals.Where (x => x.location == this.hexTile).ToList();
					this.cityActionChances.increaseArmyCountChance = this.cityActionChances.defaultIncreaseArmyCountChance;
					General chosenGeneral = generals [UnityEngine.Random.Range (0, generals.Count)];
					chosenGeneral.army.armyCount += this.kingdomTile.kingdom.armyIncreaseUnits;
					ReduceResources (this.kingdomTile.kingdom.armyIncreaseUnitResource);
				}
			}else{
//				Debug.Log ("DON'T HAVE ENOUGH RESOURCES FOR INCREASE ARMY COUNT!");
			}
		}else{
			this.cityActionChances.increaseArmyCountChance += 1;
		}
	}
	internal void UpdateCityUpgradeRequirements(){
		CityUpgradeRequirements req = new CityUpgradeRequirements ();

		int cityLevelThreshold = 5;

		int goldNeededForUpgrade = 2000 + (500 * this.cityLevel);
		int primaryResourceNeededForUpgrade = 500 + (100 * this.cityLevel);

		req.resource.Add (new Resource (RESOURCE.GOLD, goldNeededForUpgrade));
		req.resource.Add (new Resource (this.kingdomTile.kingdom.primaryRaceResource, primaryResourceNeededForUpgrade));

		if (this.cityLevel > cityLevelThreshold) {
			int secondaryResourceNeededForUpgrade = 200 + (50 * (this.cityLevel - cityLevelThreshold));
			req.resource.Add (new Resource (this.kingdomTile.kingdom.secondaryRaceResource, secondaryResourceNeededForUpgrade));
		}

		this.cityUpgradeRequirements = req;
	}
	internal void AttemptToUpgradeCity(){
		if (HasEnoughResourcesForAction(this.cityUpgradeRequirements.resource) && IsCitizenCapReached()) { //if city has the neccessary resources to upgrade and still has room for another citizen
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.increaseHousingChance) {
				//Increase Housing Triggered, Increase Citizen Limit by 1
				this.cityActionChances.increaseHousingChance =  this.cityActionChances.defaultIncreaseHousingChance;
				this.citizenLimit += 1;
				this.cityLevel += 1;
				this.generalsLimit += 1;
//				if((this.cityLevel % 4) == 0){
//					this.offenseGeneralsLimit += 1;
//				}
				cityLogs += GameManager.Instance.currentDay.ToString() + ": The City level has increased: [FF0000]" + this.cityName.ToString() + "[-] in exchange for ";
				for (int i = 0; i < this.cityUpgradeRequirements.resource.Count; i++) {
					cityLogs += this.cityUpgradeRequirements.resource [i].resourceQuantity + " " + this.cityUpgradeRequirements.resource [i].resourceType + "\n";
				}
				cityLogs += "\n";
				ReduceResources(this.cityUpgradeRequirements.resource);
				UpdateCityUpgradeRequirements();
				UpdateResourcesStatus();

			} else {
				//On not performing upgrade, increase chance to upgrade by 1
				this.cityActionChances.increaseHousingChance += 1;
			}
		}
	}
		
	internal void AttemptToCreateNewCitizen(){
		JOB_TYPE citizenToCreateJobType = JOB_TYPE.NONE;

		if (this.neededRole != JOB_TYPE.NONE && HasEnoughResourcesForAction(GetCitizenCreationCostPerType(this.neededRole))) {
			citizenToCreateJobType = this.neededRole;
		} else {
			citizenToCreateJobType = this.newCitizenTarget;
		}

		List<Resource> citizenCreationCost = GetCitizenCreationCostPerType (citizenToCreateJobType);
		if (citizenCreationCost == null || citizenCreationCost.Count <= 0) {
			return;
		}

		if(!IsCitizenCapReached() && HasEnoughResourcesForAction(citizenCreationCost) && HasTileForNewCitizen(citizenToCreateJobType)){
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.newCitizenChance) {
				
				this.cityActionChances.newCitizenChance = this.cityActionChances.defaultNewCitizenChance;
				Citizen newCitizen = new Citizen (citizenToCreateJobType, this);
				AssignCitizenToTile (newCitizen);
				this.citizens.Add (newCitizen);
				cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + citizenToCreateJobType.ToString() + "[-] was created in exchange for ";
				for (int i = 0; i < citizenCreationCost.Count; i++) {
					cityLogs += citizenCreationCost[i].resourceQuantity + " " + citizenCreationCost[i].resourceType.ToString() + "\n";
				}
				cityLogs += "\n";

				if(this.neededRole == citizenToCreateJobType){
					this.neededRole = JOB_TYPE.NONE;
				}

				if(this.newCitizenTarget == citizenToCreateJobType){
					this.newCitizenTarget = JOB_TYPE.NONE;
					SelectCitizenForCreation();
				}

				ReduceResources(citizenCreationCost);
				UpdateResourcesStatus();
				this.unneededResources = GetUnneededResources (NeededResources ());
				this.unneededRoles = GetUnneededRoles ();

				if (citizenToCreateJobType == JOB_TYPE.PIONEER) {
					//TODO: Change Distance Value To Pathfinding instead of Vector2.Distance
					pioneerCityTarget = kingdomTile.kingdom.NearestUnoccupiedCity();
					if (pioneerCityTarget == null) {
						for (int i = 0; i < citizens.Count; i++) {
							if (citizens[i].job.jobType == JOB_TYPE.PIONEER) {
								citizens.Remove(citizens[i]);
							}
						}
						return;
					}
					dayPioneerReachesCity = GameManager.Instance.currentDay + (int)Vector2.Distance(kingdomTile.kingdom.cities[0].transform.position, 
						pioneerCityTarget.hexTile.transform.position);
					GameManager.Instance.turnEnded += SendPioneer;
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Pioneer will reach city: [FF0000]" + pioneerCityTarget.hexTile.name + "[-] on day [FF0000]" + dayPioneerReachesCity.ToString() + "[-]\n\n";
				}
			} else {
				this.cityActionChances.newCitizenChance += 1;
			}

		}
	}

	void SendPioneer(int currentDay){
		if (dayPioneerReachesCity == (currentDay+1)) {
			//Pioneer has reached city
			cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]PIONEER[-] has reached city\n\n";
			if (!pioneerCityTarget.hexTile.isOccupied) {
				this.kingdomTile.AddCityToKingdom (pioneerCityTarget);
				pioneerCityTarget.cityAttributes.OccupyCity();
				pioneerCityTarget.hexTile.GetComponent<CityTileTest>().SetCityAsActiveAndSetProduction ();
				pioneerCityTarget.cityAttributes.foodStockpileCount += GetNeededFoodForNumberOfDays (33 - GameManager.Instance.daysUntilNextHarvest);
				GameManager.Instance.UpdateLordAdjacency();
				cityLogs += GameManager.Instance.currentDay.ToString () + ": PIONEER was [FF0000]successful[-] in expansion\n\n";
			} else {
				cityLogs += GameManager.Instance.currentDay.ToString () + ": PIONEER [FF0000]failed[-] in expansion\n\n";
			}
			for (int i = 0; i < citizens.Count; i++) {
				if (citizens[i].job.jobType == JOB_TYPE.PIONEER) {
					citizens.Remove(citizens[i]);
				}
			}
			GameManager.Instance.turnEnded -= SendPioneer;
		}

	}

	internal void AttemptToChangeCitizenRole(){
		JOB_TYPE citizenToCreateJobType = JOB_TYPE.NONE;

		if (this.neededRole != JOB_TYPE.NONE && HasEnoughResourcesForAction(GetCitizenCreationCostPerType(this.neededRole))) {
			citizenToCreateJobType = this.neededRole;
		} else if (this.newCitizenTarget != JOB_TYPE.NONE && HasEnoughResourcesForAction(GetCitizenCreationCostPerType(this.newCitizenTarget))) {
			citizenToCreateJobType = this.newCitizenTarget;
		} else {
			if (this.unoccupiedOwnedTiles.Count > 0) {
				List<HexTile> tilesByHighestResource = unoccupiedOwnedTiles.OrderByDescending (x => x.GetHighestResourceValue ()).ToList ();
				citizenToCreateJobType = tilesByHighestResource [0].GetBestJobForTile ();
			} else {
				return;
			}
		}
		List<Resource> citizenCreationCost = GetCitizenCreationCostPerType (citizenToCreateJobType);

		if(this.unneededRoles.Count > 0 && HasEnoughResourcesForAction(citizenCreationCost) && HasTileForNewCitizen(citizenToCreateJobType)){
			JOB_TYPE randomUnneededJob = GetRandomUnneededRole();
			if (randomUnneededJob == citizenToCreateJobType) {
				return;
			}
			int chance = UnityEngine.Random.Range(0,100);
			if (chance < this.cityActionChances.changeCitizenChance) {
				Citizen citizen = GetCitizenForChange (randomUnneededJob);
				if (citizen == null) {
					return;
				}

				citizen.ChangeJob(citizenToCreateJobType);
				if (this.neededRole == citizenToCreateJobType) {
					this.neededRole = JOB_TYPE.NONE;
				}

				if (this.newCitizenTarget == citizenToCreateJobType) {
					this.newCitizenTarget = JOB_TYPE.NONE;
					SelectCitizenForCreation();
				}
					
				this.cityActionChances.changeCitizenChance = this.cityActionChances.defaultChangeCitizenChance;
				citizen.ResetLevel();
				AssignCitizenToTile(citizen);
				ReduceResources(citizenCreationCost);
				RemoveUnneededResources (randomUnneededJob);
				this.unneededRoles.Remove(randomUnneededJob);
				this.unneededResources = GetUnneededResources (NeededResources ());
				this.unneededRoles = GetUnneededRoles ();
				cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]" + randomUnneededJob.ToString() + "[-] clan is now [FF0000]" + citizen.job.jobType.ToString() + "[-] in exchange for "; 
				for (int i = 0; i < citizenCreationCost.Count; i++) {
					cityLogs += citizenCreationCost [i].resourceQuantity + " " + citizenCreationCost [i].resourceType.ToString () + "\n";
				}
				cityLogs += "\n";

				if (citizen.job.jobType == JOB_TYPE.PIONEER) {
					//TODO: Change Distance Value To Pathfinding instead of Vector2.Distance
					pioneerCityTarget = kingdomTile.kingdom.NearestUnoccupiedCity();
					if (pioneerCityTarget == null) {
						for (int i = 0; i < citizens.Count; i++) {
							if (citizens[i].job.jobType == JOB_TYPE.PIONEER) {
								citizens.Remove(citizens[i]);
							}
						}
						return;
					}
					dayPioneerReachesCity = GameManager.Instance.currentDay + (int)Vector2.Distance(kingdomTile.kingdom.cities[0].transform.position, 
						pioneerCityTarget.hexTile.transform.position);
					GameManager.Instance.turnEnded += SendPioneer;
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Pioneer will reach city: [FF0000]" + pioneerCityTarget.hexTile.name + "[-] on day [FF0000]" + dayPioneerReachesCity.ToString() + "[-]\n\n";
				}
//				UpdateResourcesStatus ();
			} else {
				this.cityActionChances.changeCitizenChance += 1;
			}
		}
	}

	List<Resource> GetHexTileCost(HexTile hexTileToPurchase){
		Resource[] tilePurchaseCost;
		int ownedTilesThreshold = 10;
		if (this.ownedBiomeTiles.Count > ownedTilesThreshold) {
			tilePurchaseCost = new Resource[] {
				new Resource (RESOURCE.GOLD, (300 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.primaryResourceToPurchaseTile, 100 + (50 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.secondaryResourceToPurchaseTile, 80 + (40 * (this.ownedBiomeTiles.Count - ownedTilesThreshold)))
			};
		} else {
			tilePurchaseCost = new Resource[] {
				new Resource (RESOURCE.GOLD, (300 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.primaryResourceToPurchaseTile, 100 + (50 * this.ownedBiomeTiles.Count)),
			};
		}

		return tilePurchaseCost.ToList();
	}

	internal void AttemptToPurchaseTile(){
		if (this.targetHexTileToPurchase == null) {
			return;
		}
		List<Resource> tilePurchaseCost = GetHexTileCost(this.targetHexTileToPurchase);
		if (HasEnoughResourcesForAction(tilePurchaseCost.ToList())) {
			int chance = UnityEngine.Random.Range (0, 100);
			if (chance < this.cityActionChances.purchaseTileChance) {
				//Buy the tile
				this.cityActionChances.purchaseTileChance = this.cityActionChances.defaultPurchaseTileChance;
				this.ownedBiomeTiles.Add(this.targetHexTileToPurchase);
				this.targetHexTileToPurchase.SetTileColor(Color.red);
				ReduceResources(tilePurchaseCost.ToList());
				cityLogs += GameManager.Instance.currentDay.ToString () + ": Purchased Tile [FF0000]" + this.targetHexTileToPurchase.name + "[-] for [FF0000] ";
				for (int i = 0; i < tilePurchaseCost.Count; i++) {
					cityLogs += tilePurchaseCost [i].resourceQuantity + " " + tilePurchaseCost [i].resourceType + "\n";
				}
				cityLogs +="[-]\n";
				this.targetHexTileToPurchase = null;
			}
//			else {
//				this.cityActionChances.purchaseTileChance += 1;
//			}
		}

	}

	internal void SelectHexTileToPurchase(){
		RESOURCE scarceResource = GetRandomResourceTypeByStatus(RESOURCE_STATUS.SCARCE);
//		cityLogs += GameManager.Instance.currentDay.ToString () + ": Looking for tile with rich [FF0000]" + scarceResource.ToString() + "[-].\n\n";
		List<HexTile> neighbours = new List<HexTile>();
		for (int i = 0; i < this.ownedBiomeTiles.Count; i++) {
			List<HexTile> currentNeighbours = this.ownedBiomeTiles[i].GetListTilesInRange(0.5f);
			for (int j = 0; j < currentNeighbours.Count; j++) {
				if (this.ownedBiomeTiles.Contains (currentNeighbours [j])) {
					currentNeighbours.Remove (currentNeighbours [j]);
				}
			}
			neighbours.AddRange (currentNeighbours);
		}

		neighbours = neighbours.Distinct().ToList();

		if (scarceResource == RESOURCE.FOOD) {
			neighbours = neighbours.OrderByDescending (x => (x.farmingValue)).ToList();
			int highestFarmingValue = neighbours [0].farmingValue;
			neighbours = neighbours.OrderByDescending (x => (x.huntingValue)).ToList();
			int highestHuntingValue = neighbours [0].huntingValue;
			if (highestFarmingValue >= highestHuntingValue) {
				neighbours = neighbours.OrderByDescending (x => (x.farmingValue)).ToList();
			}
		} else if (scarceResource == RESOURCE.GOLD) {
			neighbours = neighbours.OrderByDescending (x => (x.goldValue)).ToList();
		} else if (scarceResource == RESOURCE.LUMBER) {
			neighbours = neighbours.OrderByDescending (x => (x.woodValue)).ToList();
		} else if (scarceResource == RESOURCE.MANA) {
			neighbours = neighbours.OrderByDescending (x => (x.manaValue)).ToList();
		} else if (scarceResource == RESOURCE.METAL) {
			neighbours = neighbours.OrderByDescending (x => (x.metalValue)).ToList();
		} else if (scarceResource == RESOURCE.STONE) {
			neighbours = neighbours.OrderByDescending (x => (x.stoneValue)).ToList();
		}

		this.targetHexTileToPurchase = neighbours [0];

//		return neighbours[0];
	} 

	internal JOB_TYPE GetRandomUnneededRole(){
//		List<JOB_TYPE> jobTypes = new List<JOB_TYPE> ();
//		jobTypes.AddRange(this.unneededRoles);
//		if (this.neededRole != JOB_TYPE.NONE) {
//			jobTypes.Remove (this.neededRole);
//		} else {
//			jobTypes.Remove (this.newCitizenTarget);
//		}

		if (this.unneededRoles.Count > 0) {
			
			return this.unneededRoles[UnityEngine.Random.Range (0, this.unneededRoles.Count)];
		}
		return JOB_TYPE.NONE;
	}

	internal void RemoveUnneededResources(JOB_TYPE jobType){
		for (int i = 0; i < Lookup.JOB_REF.Length; i++) {
			if(Lookup.GetJobInfo(i).resourcesProduced == null){
				continue;
			}
			if(Lookup.GetJobInfo(i).jobType == jobType){
				for(int j = 0; j < Lookup.GetJobInfo(i).resourcesProduced.Length; j++){
					this.unneededResources.Remove (Lookup.GetJobInfo (i).resourcesProduced [j]);
				}
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
		if (unneededCitizens.Count > 0) {
			return unneededCitizens [UnityEngine.Random.Range (0, unneededCitizens.Count)];
		}
		return null;
	}

	bool HasEnoughResourcesForAction(List<Resource> expenses){
		for (int i = 0; i < expenses.Count; i++) {
			Resource currentResource = expenses [i];
			if(this.GetNumberOfResourcesPerType(currentResource.resourceType) < currentResource.resourceQuantity){
				//this city lacks quantity of one resource, return false
				return false;
			}
		}
		return true;
	}

	bool HasTileForNewCitizen(JOB_TYPE jobType){
		if (jobType != JOB_TYPE.PIONEER) {
			if (this.unoccupiedOwnedTiles.Count <= 0) {
				return false;
			} else {
				List<HexTile> tilesWithPositiveResourceValueForJob = this.unoccupiedOwnedTiles.Where (x => x.GetRelevantResourceValueByJobType(jobType) > 0).ToList();
				if (tilesWithPositiveResourceValueForJob.Count > 0) {
					return true;
				}
				return false;
			}
		}
		return true;
	}

	bool IsCitizenCapReached(){
		if (this.citizens.Count < citizenLimit) {
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

	internal void TradeResources(CityTest tradeCity, RESOURCE resourceType, int amount){ //if seller positive amount, if buyer negative amount
//		if (resourceType == RESOURCE.GOLD) {
//			this.goldCount += amount;
//			tradeCity.goldCount -= amount;
//		}else 
		if (resourceType == RESOURCE.FOOD) {
			this.foodCount -= amount;
			tradeCity.foodCount += amount;
		} else if (resourceType == RESOURCE.LUMBER) {
			this.lumberCount -= amount;
			tradeCity.lumberCount += amount;
		} else if (resourceType == RESOURCE.MANA) {
			this.manaStoneCount -= amount;
			tradeCity.manaStoneCount += amount;
		} else if (resourceType == RESOURCE.STONE) {
			this.stoneCount -= amount;
			tradeCity.stoneCount += amount;
		} else if (resourceType == RESOURCE.METAL) {
			this.metalCount -= amount;
			tradeCity.metalCount += amount;
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
		for (int i = 0; i < this.citizens.Count; i++) {
			if (this.citizens [i].job.jobType == jobType) {
				count++;
			}
		}
		return count;
	}

	internal void UpdateResourcesStatus(){
		int[] neededResources = NeededResources ();
		for(int i = 0; i < this.allResourcesStatus.Count; i++){
			if(this.allResourcesStatus[i].resource == RESOURCE.FOOD){
				int daysFoodSupplyLasts = (int)(this.foodCount / GetDailyFoodConsumption ());
				int deficit = (int)Mathf.Abs(daysFoodSupplyLasts - GameManager.Instance.daysUntilNextHarvest) * GetDailyFoodConsumption ();

				if(daysFoodSupplyLasts > (GameManager.Instance.daysUntilNextHarvest + 3)){ //ABUNDANT
					deficit = (daysFoodSupplyLasts - (GameManager.Instance.daysUntilNextHarvest + 3)) * GetDailyFoodConsumption ();
					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
				}else if (daysFoodSupplyLasts < GameManager.Instance.daysUntilNextHarvest){ //SCARCE
					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
				}else{ //NORMAL
					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
				}

				if (deficit < 0) {
					deficit = deficit * -1;
				}
				this.allResourcesStatus[i].amount = deficit;
			}else{
				int excess = GetNumberOfResourcesPerType (this.allResourcesStatus [i].resource) - neededResources [i];

				if(excess > 0){ //ABUNDANT
					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
				}else if (excess < 0){ //SCARCE
					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
				}else{  //NORMAL
					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
				}
				if (excess < 0) {
					excess = excess * -1;
				}
				this.allResourcesStatus [i].amount = excess;
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

	#region Trade Mission Functions
	internal void LaunchTradeMission(){
		if (IsReadyForTrade ()) {
			int chance = UnityEngine.Random.Range(0, 100);
			if (chance < this.cityActionChances.tradeMissionChance) {
				this.cityActionChances.tradeMissionChance = this.cityActionChances.defaultTradeMissionChance;
				int chanceTradeHelpGift = UnityEngine.Random.Range (0, 100);
				if(chanceTradeHelpGift < 70){//TRADE
					TradeMission();
				}else if(chanceTradeHelpGift >= 70 && chanceTradeHelpGift < 85){//HELP
					AskHelp();
				}else{//GIFT
					GiveGift();
				}

			} else {
				this.cityActionChances.tradeMissionChance += 1;
			}
		}
	}
	internal void TradeMission(){
		if (IsResourceStatus (RESOURCE_STATUS.ABUNDANT, RESOURCE.GOLD)) {
			/*
			 * You are the buyer 
			 * */
			ResourceStatus scarceResource = GetResourceByStatus (RESOURCE_STATUS.SCARCE);
			if (scarceResource != null) {
				int neededGold = scarceResource.amount * GetCostPerResourceUnit(scarceResource.resource);
				if (neededGold < this.goldCount) {
//					cityLogs += GameManager.Instance.currentDay.ToString () +": Gold before purchase is " + this.goldCount + "\n\n";
//					cityLogs += GameManager.Instance.currentDay.ToString () +": " + scarceResource.resource.ToString() + 
//						" before purchase is " + GetNumberOfResourcesPerType(scarceResource.resource).ToString() + "\n\n";
					int caravanGold = neededGold;
					AdjustResourceCount(RESOURCE.GOLD, (caravanGold*-1));
					List<CityTest> cities = GetCitiesByStatus (RESOURCE_STATUS.ABUNDANT, scarceResource.resource);
					int distance = 6; //TODO: MAKE LIPAT THIS WHEN THE TIME IS RIGHT
					Buy (cities [0], RESOURCE.GOLD, scarceResource, caravanGold);
				} else {
					Debug.Log ("Gold Defficiency");
				}
			} else {
				Debug.Log (this.cityName + " Cannot find someone to buy from");
			}

		} else if (IsResourceStatus (RESOURCE_STATUS.SCARCE, RESOURCE.GOLD)) {
			/*
					 * You are the seller
					 * */
			ResourceStatus abundantResource = GetResourceByStatus (RESOURCE_STATUS.ABUNDANT);
			if (abundantResource != null) {
				int sellableResources = abundantResource.amount;
				if (sellableResources < this.GetNumberOfResourcesPerType(abundantResource.resource)) {
					int caravanResources = sellableResources;
//					cityLogs += GameManager.Instance.currentDay.ToString () +": Gold before selling is " + this.goldCount + "\n\n";
//					cityLogs += GameManager.Instance.currentDay.ToString () +": " + abundantResource.resource.ToString() + 
//						" before selling is " + GetNumberOfResourcesPerType(abundantResource.resource).ToString() + "\n\n";
					AdjustResourceCount(abundantResource.resource, (caravanResources*-1));
					List<CityTest> cities = GetCitiesByStatus (RESOURCE_STATUS.SCARCE, abundantResource.resource);
					int distance = 6; //TODO: MAKE LIPAT THIS WHEN THE TIME IS RIGHT
					Sell (cities [0], abundantResource, caravanResources);
				}
			}
		} else {
			//GOLD IS NORMAL
			Debug.Log(this.cityName + " Can't trade, gold is normal amount");
		}
	}
	void Buy(CityTest tradeCity, RESOURCE resourceToOffer, ResourceStatus resourceToBuy, int caravanGold){

		UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.kingdomTile.kingdom.lord.name + " wants to BUY " + resourceToBuy.resource.ToString () + " from " + tradeCity.kingdomTile.kingdom.lord.name + ".\n\n";
		if (tradeCity.GetNumberOfResourcesPerType (resourceToBuy.resource) > 0 && tradeCity.IsResourceStatus (RESOURCE_STATUS.ABUNDANT, resourceToBuy.resource)) {
//			int chance = UnityEngine.Random.Range(0, 100);
//			int successChance = 70 + (0 * 5);
			DECISION tradeCityDecision = tradeCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.TRADE, this.kingdomTile.kingdom.lord);

			if (tradeCityDecision == DECISION.NICE) {
				this.kingdomTile.kingdom.lord.AdjustLikeness (tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.NICE, LORD_EVENTS.TRADE, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.NICE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);


				int affordResource = (int)(this.GetResourceStatusByType (resourceToOffer).amount / GetCostPerResourceUnit (resourceToBuy.resource));
				int affordToSell = tradeCity.GetResourceStatusByType (resourceToBuy.resource).amount;
				if (affordResource > resourceToBuy.amount) {
					affordResource = resourceToBuy.amount;
				}

				if (affordToSell < affordResource) {
					affordResource = affordToSell;
				}

				int cost = affordResource * GetCostPerResourceUnit (resourceToBuy.resource);

				cityLogs += GameManager.Instance.currentDay.ToString () + ": Caravan Gold is " + caravanGold.ToString () + " Gold. \n\n";

				caravanGold -= cost;
				tradeCity.goldCount += cost;
				TradeResources (tradeCity, resourceToBuy.resource, (affordResource * -1));
				if (caravanGold > 0) {
					cityLogs += GameManager.Instance.currentDay.ToString () + ": Returned " + caravanGold.ToString () + " Gold. \n\n";
					AdjustResourceCount (RESOURCE.GOLD, caravanGold);
				}
				//TODO: Insert add like to both lords
				cityLogs += GameManager.Instance.currentDay.ToString () + ": Bought " + affordResource.ToString () + " " + resourceToBuy.resource.ToString () + " from " + tradeCity.cityName +
				" in exchange for " + cost.ToString () + " " + resourceToOffer.ToString () + "\n\n";

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + tradeCity.kingdomTile.kingdom.lord.name + " ACCEPTED the trade.\n\n";
			} else {
				//TODO: Reduce Like to target trade city

				this.kingdomTile.kingdom.lord.AdjustLikeness (tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + tradeCity.kingdomTile.kingdom.lord.name + " REJECTED the trade.\n\n";

			}
			Utilities.tradeCount++;
		} else {
				//TODO: Reduce Like to target trade city

			//ASSUMPTION: TRADE REJECTED
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: Trade has failed because " + tradeCity.kingdomTile.kingdom.lord.name + "'s " + resourceToBuy.resource.ToString() 
				+ " is not enough or SCARCE.\n\n";

//			this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE);
//			tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE);
		}
		
	}

	void Sell(CityTest tradeCity, ResourceStatus resourceToOffer, int caravanResources){
		UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.kingdomTile.kingdom.lord.name + " wants to SELL " + resourceToOffer.resource.ToString () + " to " + tradeCity.kingdomTile.kingdom.lord.name + "\n\n";

		ResourceStatus resourceToBeBought = tradeCity.GetResourceStatusByType (resourceToOffer.resource);

		if (tradeCity.GetNumberOfResourcesPerType (RESOURCE.GOLD) > 0 && tradeCity.IsResourceStatus (RESOURCE_STATUS.ABUNDANT, RESOURCE.GOLD)) {
//			int chance = UnityEngine.Random.Range(0, 100);
//			int successChance = 70 + (0 * 5);
			DECISION tradeCityDecision = tradeCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.TRADE, this.kingdomTile.kingdom.lord);
			if (tradeCityDecision == DECISION.NICE) {
				this.kingdomTile.kingdom.lord.AdjustLikeness (tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.NICE, LORD_EVENTS.TRADE, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.NICE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

				int affordResourceOfTradeCity = (int)(tradeCity.GetResourceStatusByType(RESOURCE.GOLD).amount / GetCostPerResourceUnit (resourceToOffer.resource));
				int affordToSell = this.GetResourceStatusByType (resourceToOffer.resource).amount;
				if (affordResourceOfTradeCity > resourceToBeBought.amount) {
					affordResourceOfTradeCity = resourceToBeBought.amount;
				}

				if (affordToSell < affordResourceOfTradeCity) {
					affordResourceOfTradeCity = affordToSell;
				}

				int cost = affordResourceOfTradeCity * GetCostPerResourceUnit (resourceToBeBought.resource);

				cityLogs += GameManager.Instance.currentDay.ToString () + ": Caravan Resources is " + caravanResources.ToString() + " " + resourceToOffer.resource.ToString() + ". \n\n";

				this.goldCount += cost;
				tradeCity.goldCount -= cost;

				caravanResources -= affordResourceOfTradeCity;
//				TradeResources (tradeCity, resourceToBeBought.resource, affordResourceOfTradeCity);

				if (caravanResources > 0) {
					cityLogs += GameManager.Instance.currentDay.ToString () + ": Returned " + caravanResources.ToString() + " " + resourceToBeBought.resource.ToString() + ". \n\n";
					AdjustResourceCount(resourceToBeBought.resource, caravanResources);
				}
				//TODO: Insert add like to both lords
				cityLogs += GameManager.Instance.currentDay.ToString () + ": Sold " + affordResourceOfTradeCity.ToString ()  + " " + resourceToOffer.resource.ToString () + " to " + tradeCity.cityName +
					" in exchange for " + cost.ToString () + " Gold \n\n";

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + tradeCity.kingdomTile.kingdom.lord.name + " ACCEPTED the trade.\n\n";

			} else {
				//TODO: Reduce Like to target trade city
				this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + tradeCity.kingdomTile.kingdom.lord.name + " REJECTED the trade.\n\n";

			}
			Utilities.tradeCount++;

		} else {
			//TODO: Reduce Like to target trade city
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: Trade has failed because " + tradeCity.kingdomTile.kingdom.lord.name + " doesn't have enough GOLD.\n\n";
//			this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE);
//			tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE);
		}
	}

	internal void AskHelp(){
		ResourceStatus scarceResource = GetResourceByStatus (RESOURCE_STATUS.SCARCE);
		if (scarceResource != null) {
//			int neededAmount = scarceResource.amount;
//				cityLogs += GameManager.Instance.currentDay.ToString () + ": Gold before purchase is " + this.goldCount + "\n\n";
			cityLogs += GameManager.Instance.currentDay.ToString () + ": " + scarceResource.resource.ToString () +
			" before help is " + GetNumberOfResourcesPerType (scarceResource.resource).ToString () + "\n\n";
//			int caravanAmount = neededAmount;
//			AdjustResourceCount (RESOURCE.GOLD, (caravanGold * -1));
			List<CityTest> cities = GetCitiesByStatus (RESOURCE_STATUS.ABUNDANT, scarceResource.resource);
			int distance = 6; //TODO: MAKE LIPAT THIS WHEN THE TIME IS RIGHT
			Help (cities [0], scarceResource);
		}else{
			cityLogs += GameManager.Instance.currentDay.ToString () + ": There is no one to whom "+ this.cityName +" can ask for help.\n\n";
		}
	}
	internal void Help(CityTest tradeCity, ResourceStatus askedResource){
		UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": HELP: " + this.kingdomTile.kingdom.lord.name + " is asking for " + askedResource.resource.ToString () + " from " + tradeCity.kingdomTile.kingdom.lord.name + ".\n\n";

		ResourceStatus resourceToBeProvided = tradeCity.GetResourceStatusByType (askedResource.resource);

		if(resourceToBeProvided.status == RESOURCE_STATUS.ABUNDANT){
			DECISION tradeCityDecision = tradeCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.HELP, this.kingdomTile.kingdom.lord);
			if(tradeCityDecision == DECISION.NICE){
				this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.NICE, LORD_EVENTS.HELP, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.NICE, DECISION.NEUTRAL, LORD_EVENTS.HELP, false);

				int finalResourceAmount = 0;
				if(askedResource.amount <= resourceToBeProvided.amount){
					finalResourceAmount = askedResource.amount;
				}else{
					finalResourceAmount = resourceToBeProvided.amount;
				}

				this.AdjustResourceCount(askedResource.resource, finalResourceAmount);
				tradeCity.AdjustResourceCount(askedResource.resource, -finalResourceAmount);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": HELP: " + tradeCity.kingdomTile.kingdom.lord.name + " ACCEPTED to PROVIDE HELP to " + this.kingdomTile.kingdom.lord.name + ".\n\n";


			}else{
				this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.HELP, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.HELP, false);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": HELP: " + tradeCity.kingdomTile.kingdom.lord.name + " REJECTED to PROVIDE HELP to " + this.kingdomTile.kingdom.lord.name + ".\n\n";


			}
			Utilities.helpCount++;

		}else{
			Debug.Log ("HELP REJECTED. DONT HAVE ENOUGH!");
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": HELP: " + tradeCity.kingdomTile.kingdom.lord.name + " CAN'T PROVIDE HELP because its " + askedResource.resource.ToString() + " is SCARCE.\n\n";

		}
	}

	internal void GiveGift(){
		ResourceStatus abundantResource = GetResourceByStatus (RESOURCE_STATUS.ABUNDANT);
		if (abundantResource != null) {
//			int excessAmount = abundantResource.amount;
			//				cityLogs += GameManager.Instance.currentDay.ToString () + ": Gold before purchase is " + this.goldCount + "\n\n";
			cityLogs += GameManager.Instance.currentDay.ToString () + ": " + abundantResource.resource.ToString () +
				" before gift is " + GetNumberOfResourcesPerType (abundantResource.resource).ToString () + "\n\n";
			//			int caravanAmount = neededAmount;
			//			AdjustResourceCount (RESOURCE.GOLD, (caravanGold * -1));
			List<CityTest> cities = GetCitiesByStatus (RESOURCE_STATUS.SCARCE, abundantResource.resource);
			int distance = 6; //TODO: MAKE LIPAT THIS WHEN THE TIME IS RIGHT
			Gift (cities [0], abundantResource);
		}else{
			cityLogs += GameManager.Instance.currentDay.ToString () + ": There is no one to whom "+ this.cityName +" can ask for help.\n\n";
		}
	}
	internal void Gift(CityTest tradeCity, ResourceStatus giftResource){
		UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": GIFT: " + this.kingdomTile.kingdom.lord.name + " wants to give " + giftResource.resource.ToString () + " to " + tradeCity.kingdomTile.kingdom.lord.name + ".\n\n";

		ResourceStatus resourceToBeAsked = tradeCity.GetResourceStatusByType (giftResource.resource);

		if(resourceToBeAsked.status == RESOURCE_STATUS.SCARCE){
			DECISION tradeCityDecision = tradeCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.GIFT, this.kingdomTile.kingdom.lord);
			if(tradeCityDecision == DECISION.NICE){
				this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.NICE, LORD_EVENTS.GIFT, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.NICE, DECISION.NEUTRAL, LORD_EVENTS.GIFT, false);

				int finalResourceAmount = 0;
				if(giftResource.amount <= resourceToBeAsked.amount){
					finalResourceAmount = giftResource.amount;
				}else{
					finalResourceAmount = resourceToBeAsked.amount;
				}

				this.AdjustResourceCount(giftResource.resource, -finalResourceAmount);
				tradeCity.AdjustResourceCount(giftResource.resource, finalResourceAmount);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": GIFT: " + tradeCity.kingdomTile.kingdom.lord.name + " is happy and ACCEPTED the " + giftResource.resource.ToString() + " given by " + this.kingdomTile.kingdom.lord.name + ".\n\n";


			}else{
				this.kingdomTile.kingdom.lord.AdjustLikeness(tradeCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.GIFT, true);
				tradeCity.kingdomTile.kingdom.lord.AdjustLikeness (this.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.GIFT, false);

				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": GIFT: " + tradeCity.kingdomTile.kingdom.lord.name + " REJECTED the " + giftResource.resource.ToString() + " given by " + this.kingdomTile.kingdom.lord.name + ".\n\n";


			}
			Utilities.giftCount++;

		}else{
			Debug.Log ("GIFT REJECTED. ALREADY HAVE ENOUGH!");
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": GIFT: " + tradeCity.kingdomTile.kingdom.lord.name + " has declined the " + giftResource.resource.ToString() + " because it's already ABUNDANT.\n\n";

		}
	}
	int GetCostPerResourceUnit(RESOURCE resourceType){
		switch (resourceType) {
		case RESOURCE.FOOD:
			return 10;
		case RESOURCE.LUMBER:
			return 20;
		case RESOURCE.STONE:
			return 20;
		case RESOURCE.MANA:
			return 20;
		case RESOURCE.METAL:
			return 20;
		}
		return 0;
	}

	List<CityTest> GetCitiesByStatus(RESOURCE_STATUS status, RESOURCE resource){
		List<CityTest> cities = new List<CityTest>();
		for (int i = 0; i < GameManager.Instance.kingdoms.Count; i++) {
			if (GameManager.Instance.kingdoms [i].kingdom.id != this.kingdomTile.kingdom.id) {
				for (int j = 0; j < GameManager.Instance.kingdoms [i].kingdom.cities.Count; j++) {
					CityTest currentCity = GameManager.Instance.kingdoms [i].kingdom.cities [j].cityAttributes;
					if (currentCity.IsResourceStatus (status, resource)) {
						cities.Add (currentCity);
					}
				}
			}
		}
		cities.OrderByDescending(x => x.goldCount).ToList();
		List<CityTest> filteredCities = new List<CityTest> ();
		filteredCities.Add (cities [0]);
		if (filteredCities.Count > 1) {
			filteredCities.Add (cities [1]);
		}
		return filteredCities.OrderBy(x => Vector2.Distance (hexTile.transform.position, x.hexTile.transform.position)).ToList();
	}

	internal bool IsResourceStatus(RESOURCE_STATUS status, RESOURCE resource){
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			ResourceStatus currentResource = this.allResourcesStatus [i];
			if (currentResource.resource == resource && currentResource.status == status) {
				return true;
			}
		}
		return false;
	}

	ResourceStatus GetResourceByStatus(RESOURCE_STATUS status){
		List<ResourceStatus> filteredResources = new List<ResourceStatus>();
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			ResourceStatus currentResource = this.allResourcesStatus [i];
			if (currentResource.status == status) {
				filteredResources.Add (currentResource);
			}
		}

		filteredResources = filteredResources.OrderByDescending(x => x.amount).ToList();
		RESOURCE_STATUS oppositeStatus = RESOURCE_STATUS.NORMAL;
		if(status == RESOURCE_STATUS.ABUNDANT){
			oppositeStatus = RESOURCE_STATUS.SCARCE;
		}else{
			oppositeStatus = RESOURCE_STATUS.ABUNDANT;
		}

		for (int i = 0; i < filteredResources.Count; i++) {
			for (int j = 0; j < GameManager.Instance.kingdoms.Count; j++) {
				if (GameManager.Instance.kingdoms [j].kingdom.id != this.kingdomTile.kingdom.id) {
					for (int k = 0; k < GameManager.Instance.kingdoms[j].kingdom.cities.Count; k++) {
						if (GameManager.Instance.kingdoms[j].kingdom.cities[k].cityAttributes.IsResourceStatus(oppositeStatus, filteredResources [i].resource)) {
							return filteredResources[i];
						}
					}
				}
			}
		}
		return null;
	}

	bool IsReadyForTrade(){
		bool hasScarce = false;
		bool hasAbundant = false;
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus [i].status == RESOURCE_STATUS.SCARCE) {
				hasScarce = true;
			}

			if (this.allResourcesStatus [i].status == RESOURCE_STATUS.ABUNDANT) {
				hasAbundant = true;
			}
		}

		if (hasScarce && hasAbundant) {
			return true;
		}
		return false;
	}

	RESOURCE GetRandomResourceTypeByStatus(RESOURCE_STATUS status){
		List<RESOURCE> resourceTypes = new List<RESOURCE>();
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus[i].status == status) {
				resourceTypes.Add (allResourcesStatus [i].resource);
			}
		}
		if (resourceTypes.Count > 0) {
			return resourceTypes [UnityEngine.Random.Range (0, resourceTypes.Count)];
		}
		return RESOURCE.FOOD;
	}

	internal ResourceStatus GetResourceStatusByType(RESOURCE resourceType){
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus [i].resource == resourceType) {
				return this.allResourcesStatus [i];
			}
		}
		return null;
	}
	#endregion

	void AdjustResourceCount(RESOURCE resourceType, int amount){
		switch (resourceType) {
		case RESOURCE.FOOD:
			this.foodCount += amount;
			break;
		case RESOURCE.GOLD:
			this.goldCount += amount;
			break;
		case RESOURCE.LUMBER:
			this.lumberCount += amount;
			break;
		case RESOURCE.STONE:
			this.stoneCount += amount;
			break;
		case RESOURCE.MANA:
			this.manaStoneCount += amount;
			break;
		case RESOURCE.METAL:
			this.metalCount += amount;
			break;
		}
	}
		
	public List<Resource> GetCitizenCreationCostPerType(JOB_TYPE jobType){
		if(jobType == JOB_TYPE.NONE){
			return null;
		}
		int numOfCitizensThreshold = 3;

		int numOfCitizensOfSameType = 0;
		for (int i = 0; i < this.citizens.Count; i++) {
			if (this.citizens [i].job.jobType == jobType) {
				numOfCitizensOfSameType++;
			}
		}
		List<Resource> citizenCreationCosts;
		RESOURCE primaryCreationResource = RESOURCE.FOOD;
		RESOURCE secondaryCreationResource = RESOURCE.FOOD;

		switch (jobType) {
		case JOB_TYPE.FARMER:
			primaryCreationResource = RESOURCE.LUMBER;
			secondaryCreationResource = RESOURCE.METAL;
			break;
		case JOB_TYPE.HUNTER:
			primaryCreationResource = RESOURCE.STONE;
			secondaryCreationResource = RESOURCE.MANA;
			break;
		case JOB_TYPE.WOODSMAN:
			primaryCreationResource = RESOURCE.LUMBER;
			secondaryCreationResource = RESOURCE.MANA;
			break;
		case JOB_TYPE.QUARRYMAN:
			primaryCreationResource = RESOURCE.STONE;
			secondaryCreationResource = RESOURCE.METAL;
			break;
		case JOB_TYPE.ALCHEMIST:
			primaryCreationResource = RESOURCE.LUMBER;
			secondaryCreationResource = RESOURCE.MANA;
			break;
		case JOB_TYPE.MINER:
			primaryCreationResource = RESOURCE.STONE;
			secondaryCreationResource = RESOURCE.METAL;
			break;
//		case JOB_TYPE.WARRIOR:
//			primaryCreationResource = RESOURCE.STONE;
//			secondaryCreationResource = RESOURCE.METAL;
//			break;
//		case JOB_TYPE.MAGE:
//			primaryCreationResource = RESOURCE.LUMBER;
//			secondaryCreationResource = RESOURCE.MANA;
//			break;
		case JOB_TYPE.PIONEER:
			citizenCreationCosts = new List<Resource> () {
				new Resource (RESOURCE.GOLD, 1000)
			};

			return citizenCreationCosts;
		}


		int primaryCreationResourceCount = 160 + (40 * numOfCitizensOfSameType);
		int goldCost = 500 + (50 * numOfCitizensOfSameType);

		if(numOfCitizensOfSameType == 0) {
			citizenCreationCosts = new List<Resource> () {
				new Resource (RESOURCE.GOLD, goldCost),
			};
		} else if (numOfCitizensOfSameType > numOfCitizensThreshold) {
			citizenCreationCosts = new List<Resource>(){
				new Resource(RESOURCE.GOLD, goldCost),
				new Resource (primaryCreationResource, primaryCreationResourceCount),
				new Resource(secondaryCreationResource, 160 + (40 * numOfCitizensOfSameType - numOfCitizensThreshold))
			};
		} else {
			citizenCreationCosts = new List<Resource> () {
				new Resource (RESOURCE.GOLD, goldCost),
				new Resource (primaryCreationResource, primaryCreationResourceCount)
			};
		}

		return citizenCreationCosts;
	}


	#region BATTLE
//	internal void TriggerCityAttack(CityTest targetCity){
//		if(this.targetCity != null){
//			Debug.Log ("THIS CITY ALREADY HAS A TARGET!");
//			return;
//		}
//		this.hasTarget = true;
//		this.targetCity = targetCity;
//
//		if(CanAttackBasedOnAggressiveness(targetCity)){
//			//Attack
//			int armyStrength = 0;
//			List<General> generalOrderedByStrength = this.generals.OrderByDescending (x => x.ArmyStrength ()).ToList();
//			for(int i = 0; i < generalOrderedByStrength.Count; i++){
//				armyStrength += generalOrderedByStrength[i].ArmyStrength();
//				generalOrderedByStrength [i].onAttack = true;
//				generalOrderedByStrength [i].daysBeforeArrival = 0;
//				generalOrderedByStrength [i].targetLocation = targetCity.hexTile;
//				targetCity.visitingGenerals.Add(generalOrderedByStrength[i]);
////				this.deployedGenerals.Add (generalOrderedByStrength [i]);
//				if(armyStrength >= targetCity.GetArmyStrength()){
//					break;
//				}
//			}
//
////			for(int i = 0; i < this.deployedGenerals.Count; i++){
////				this.generals.Remove (this.deployedGenerals [i]);
////			}
//
//		}else{
//			//Prepare
//			Preparation();
//		}
//	}
//	internal void Preparation(){
//		if(HasEnoughResourcesForAction(this.kingdomTile.kingdom.armyIncreaseUnitResource)){
//			if (this.generals.Count > 0) {
//				this.cityActionChances.increaseArmyCountChance = this.cityActionChances.defaultIncreaseArmyCountChance;
//				General chosenGeneral = this.generals.ElementAt(this.generals.Min(x => x.ArmyStrength()));
//				chosenGeneral.army.armyCount += this.kingdomTile.kingdom.armyIncreaseUnits;
//				ReduceResources (this.kingdomTile.kingdom.armyIncreaseUnitResource);
//			}
//		}else{
//			Debug.Log ("DON'T HAVE ENOUGH RESOURCES FOR INCREASE ARMY COUNT, GO TO ASK HELP!");
//			this.kingdomTile.kingdom.lord.ProvideBattleHelp (this, NeededArmyStrength (targetCity, BATTLE_MOVE.ATTACK));
//		}
//	}
	internal void CheckVisitingGenerals(){
		if(this.visitingGenerals.Count <= 0){
			return;
		}
		this.visitingGenerals.RemoveAll (x => x.targetLocation != this.hexTile && x.location != this.hexTile);

		for(int i = 0; i < this.visitingGenerals.Count; i++){
			if(this.visitingGenerals[i].location == this.hexTile){
				if(this.visitingGenerals[i].task == GENERAL_TASK.ON_ATTACK){
					
				}
			}
		}
	}
//	internal void CheckVisitingGenerals(){
//		if(this.visitingGenerals.Count <= 0){
//			return;
//		}
//		General victoriousGeneral = null;
//		General friendlyGeneral = null;
//		for (int i = 0; i < this.visitingGenerals.Count; i++) {
//			if (this.visitingGenerals [i].location.name == this.visitingGenerals [i].city.hexTile.name) {
//				if(!this.visitingGenerals [i].onHelp && !this.visitingGenerals [i].onAttack){
//					this.generals.Add (this.visitingGenerals [i]);
//					this.visitingGenerals.RemoveAt (i);
//					i--;
//				}
//			}
//		}
//		for (int i = 0; i < this.visitingGenerals.Count; i++) {
//			if (this.visitingGenerals [i].location.name == this.hexTile.name) {
//				if (this.visitingGenerals [i].onHelp) {
//					this.generals.Add (this.visitingGenerals [i]);
//					this.visitingGenerals.RemoveAt (i);
//					i--;
//				}
//			}
//		}
//		for(int i = 0; i < this.visitingGenerals.Count; i++){
//			if (this.visitingGenerals [i].location.name == this.hexTile.name) {
//				if (this.visitingGenerals [i].onAttack) {
//					if (!this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (this.kingdomTile.kingdom.lord)) {
//						this.visitingGenerals [i].onAttack = false;
//					}else{
//						if(this.generals.Count > 0){
//							GoToBattle (ref	this.visitingGenerals [i]);
//							if(this.generals.Count <= 0 && this.visitingGenerals [i].army.armyCount > 0){
//								//CITY DEFEATED AND CONQUERED --- IF ENEMY IS STILL ALIVE, AND YOU HAVE NO GENERALS LEFT
//								victoriousGeneral = this.visitingGenerals [i];
//								Debug.Log (this.cityName + " IS DEFEATED BY " + victoriousGeneral.city.cityName);
//							}
//						}else{
//							//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
//							if(victoriousGeneral == null){
//								victoriousGeneral = this.visitingGenerals [i];
//							}else{
//								friendlyGeneral = victoriousGeneral;
//								if (this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (friendlyGeneral.city.kingdomTile.kingdom.lord)) {
//									Debug.Log ("CITY IS FOR TAKING! NO MORE GENERALS! BATTLE FOR OWNERSHIP!");
//									Battle (ref this.visitingGenerals [i], ref friendlyGeneral);
//									if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
//										victoriousGeneral = null;
//									} else if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
//										victoriousGeneral = friendlyGeneral;
//									} else if (this.visitingGenerals [i].army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
//										victoriousGeneral = this.visitingGenerals [i];
//									}
//									Debug.Log ("WINNER: " + victoriousGeneral.city.cityName);
//								}
//							}
//						}
//						this.visitingGenerals [i].onAttack = false;
//					}
//				}
//			}
//		}
//
//		for(int i = 0; i < this.visitingGenerals.Count; i++){
//			if (this.visitingGenerals [i].army.armyCount <= 0) {
//				this.visitingGenerals [i].city.generals.Remove (this.visitingGenerals [i]);
//			}
//		}
//		this.visitingGenerals.RemoveAll (x => x.army.armyCount <= 0);
//
//		if(victoriousGeneral != null){
//			//CONQUER CITY
//			Debug.Log (this.cityName + " IS CONQUERED BY " + victoriousGeneral.city.cityName);
//			ConquerCity (victoriousGeneral.city.kingdomTile, this.visitingGenerals);
//
//		}
//	}
	internal void GoToBattle(ref General enemyGeneral){
		List<General> deadFriendlies = new List<General> ();

		General friendlyGeneral = null;
			
		List<General> generalOrderedByStrength = this.generals.OrderByDescending (x => x.ArmyStrength ()).ToList();

		for(int j = 0; j < generalOrderedByStrength.Count; j++){
			
			friendlyGeneral = generalOrderedByStrength [j];

			Battle (ref enemyGeneral, ref friendlyGeneral);
			if (friendlyGeneral.army.armyCount <= 0) {
				deadFriendlies.Add (friendlyGeneral);
			}
			if(enemyGeneral.army.armyCount <= 0){
				break;
			}
//						if (enemyGeneral.army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
//							deadEnemies.Add (enemyGeneral);
//							deadFriendlies.Add (friendlyGeneral);
//							break;
//						} else if (enemyGeneral.army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
//							deadEnemies.Add (enemyGeneral);
//							break;
//						} else if (enemyGeneral.army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
//							deadFriendlies.Add (friendlyGeneral);
//						} 
		}
		for (int j = 0; j < deadFriendlies.Count; j++) {
			this.generals.Remove (deadFriendlies [j]);
		}
	}
	internal void ConquerCity(KingdomTileTest conqueror, List<General> visitingGenerals){

		conqueror.kingdom.AddCityToKingdom (hexTile.GetCityTileTest());
		this.kingdomTile.kingdom.cities.Remove (hexTile.GetCityTileTest());

		hexTile.GetCityTileTest ().DevelopNewCity (conqueror, visitingGenerals);

	}
	internal void CheckMultipleArrivals(){
		
	}
	internal void Battle(ref General general1, ref General general2){
		float general1HPmultiplier = 1f;
		float general2HPmultiplier = 1f;

		if(general1.task != GENERAL_TASK.ON_ATTACK){
			general1HPmultiplier = 1.25f;
		}
		if(general2.task != GENERAL_TASK.ON_ATTACK){
			general2HPmultiplier = 1.25f;
		}

		int general1TotalHP = (int)(general1.army.armyCount * (general1.army.armyStats.hp * general1HPmultiplier));
		int general2TotalHP = (int)(general2.army.armyCount * (general2.army.armyStats.hp * general2HPmultiplier));

		int general1TotalAttack = general1.army.armyCount * general1.army.armyStats.attack;
		int general2TotalAttack = general1.army.armyCount * general1.army.armyStats.attack;

		while(general1.army.armyCount > 0 && general2.army.armyCount > 0){
			general2TotalHP -= general1TotalAttack;
			general1TotalHP -= general2TotalAttack;

			general1.army.armyCount = (int)Math.Ceiling((double)(general1TotalHP / general1.army.armyStats.hp));
			general2.army.armyCount = (int)Math.Ceiling((double)(general2TotalHP / general2.army.armyStats.hp));
		}

//		if(general1.job.army.armyCount == 0){
//			general1.city.citizens.Remove (general1);
//		}
//
//		if(general2.job.army.armyCount == 0){
//			general2.city.citizens.Remove (general2);
//		}
	}
	internal int NeededArmyStrength(CityTest targetCity, BATTLE_MOVE battleMove){
		if(battleMove == BATTLE_MOVE.ATTACK){
			int targetCityArmyStrength = (int)(targetCity.GetArmyStrength () * 1.25f);
			switch(this.kingdomTile.kingdom.lord.aggressiveness){
			case AGGRESSIVENESS.SUPER:
				return targetCityArmyStrength - this.GetArmyStrength ();
			case AGGRESSIVENESS.MILD:
				return (int)(targetCityArmyStrength * 1.25f) - this.GetArmyStrength ();
			case AGGRESSIVENESS.LIGHT:
				return (int)(targetCityArmyStrength * 1.5f) - this.GetArmyStrength ();
			}
		}else{
			int yourArmyStrength = (int)(this.GetArmyStrength () * 1.25f);
			switch(this.kingdomTile.kingdom.lord.aggressiveness){
			case AGGRESSIVENESS.SUPER:
				return targetCity.GetArmyStrength () - yourArmyStrength;
			case AGGRESSIVENESS.MILD:
				return (int)(targetCity.GetArmyStrength () * 1.25f) - yourArmyStrength;
			case AGGRESSIVENESS.LIGHT:
				return (int)(targetCity.GetArmyStrength () * 1.5f) - yourArmyStrength;
			}
		}
	
		return 0;
	}
	internal bool CanAttackBasedOnAggressiveness(CityTest targetCity){
		int targetCityArmyStrength = (int)(targetCity.GetArmyStrength () * 1.25f);
		switch(this.kingdomTile.kingdom.lord.aggressiveness){
		case AGGRESSIVENESS.SUPER:
			if (this.GetArmyStrength () > targetCityArmyStrength) {
				return true;
			}
			return false;
		case AGGRESSIVENESS.MILD:
			if (this.GetArmyStrength () > (int)(targetCityArmyStrength * 1.25f)) {
				return true;
			}
			return false;
		case AGGRESSIVENESS.LIGHT:
			if (this.GetArmyStrength () > (int)(targetCityArmyStrength * 1.5f)) {
				return true;
			}
			return false;
		}
		return false;
	}

	internal int GetArmyStrength(bool forAttack = false){
		int strength = 0;
		for(int i = 0; i < this.generals.Count; i++){
			if(this.generals[i].city.id == this.id){
				if(forAttack){
					if(this.generals[i].task == GENERAL_TASK.NONE){
						strength += this.generals[i].ArmyStrength();
					}
				}else{
					if (this.generals [i].location == this.hexTile) {
						strength += this.generals [i].ArmyStrength ();
					} else {
						if(this.generals[i].task == GENERAL_TASK.ON_DEFEND){
							strength += this.generals [i].ArmyStrength ();
						}
					}
				}
			}
		}
		for(int i = 0; i < this.visitingGenerals.Count; i++){
			if(this.visitingGenerals[i].location == this.hexTile){
				if(this.visitingGenerals[i].task != GENERAL_TASK.ON_ATTACK){
					strength += this.visitingGenerals [i].ArmyStrength ();
				}
			}else{
				if(this.visitingGenerals[i].task == GENERAL_TASK.ON_HELP){
					strength += this.visitingGenerals [i].ArmyStrength ();
				}
			}
		}
		return strength;
	}
	#endregion
	internal void TriggerCityDefense(General enemyGeneral){
		this.kingdomTile.kingdom.lord.militaryData.Add(new MilitaryData(null, enemyGeneral, 0, BATTLE_MOVE.DEFEND));
		this.kingdomTile.kingdom.lord.UpdateMilitaryData ();

		int strength = 0;
		for(int i = 0; i < this.generals.Count; i++){
			if(this.generals[i].location == this.hexTile){
				if(this.generals[i].task == GENERAL_TASK.NONE){
					strength += this.generals [i].ArmyStrength ();
					this.generals [i].task = GENERAL_TASK.ON_DEFEND;
					this.generals [i].daysBeforeReleaseTask = enemyGeneral.daysBeforeArrival + 1;
					this.generals [i].targetCity = null;
					this.generals [i].targetLocation = this.hexTile;
					if(strength >= enemyGeneral.ArmyStrength()){
						break;
					}
				}
			}
		}
		if(strength >= enemyGeneral.ArmyStrength()){
			return;
		}
		for(int i = 0; i < this.generals.Count; i++){
			if(this.generals[i].location != this.hexTile){
				if(this.generals[i].task == GENERAL_TASK.NONE){
					if(this.generals[i].task == GENERAL_TASK.NONE){
						//Check if distance will suffice the needed days
						int distance = 0;

						if(distance <= enemyGeneral.daysBeforeArrival){
							strength += this.generals [i].ArmyStrength ();
							this.generals [i].task = GENERAL_TASK.ON_DEFEND;
							this.generals [i].daysBeforeReleaseTask = enemyGeneral.daysBeforeArrival + 1;
							this.generals [i].targetCity = null;
							this.generals [i].targetLocation = this.hexTile;
							this.generals [i].daysBeforeArrival = distance;
							if(strength >= enemyGeneral.ArmyStrength()){
								break;
							}
						}
					}
				}
			}
		}
	}
	internal void GiveMeTask(General general){
		General askingGeneral = SearchGeneralByID (general.id);
		if(askingGeneral == null){
			return;
		}
		for(int i = 0; i < this.kingdomTile.kingdom.lord.militaryData.Count; i++){
			if(!this.kingdomTile.kingdom.lord.militaryData [i].isResolved){
				MilitaryData milData = this.kingdomTile.kingdom.lord.militaryData [i];
				ProcessTask (ref milData, ref askingGeneral);
				if(askingGeneral.task == GENERAL_TASK.NONE){
					continue;
				}else{
					break;
				}
			}
		}
	}
	internal void ProcessTask(ref MilitaryData militaryData, ref General general){
		if (militaryData.battleMove == BATTLE_MOVE.ATTACK) {
			general.targetCity = militaryData.enemyCity;
			general.targetLocation = militaryData.enemyCity.hexTile;
			general.daysBeforeArrival = 0;
			general.task = GENERAL_TASK.ON_ATTACK;
			militaryData.yourArmyStrength += general.ArmyStrength ();
			if(militaryData.yourArmyStrength >= militaryData.enemyCity.GetArmyStrength()){
				militaryData.isResolved = true;
			}
		}else{
			if(militaryData.enemyGeneral.ArmyStrength() > militaryData.enemyGeneral.targetCity.GetArmyStrength()){
				int neededStrength = militaryData.enemyGeneral.ArmyStrength () - militaryData.enemyGeneral.targetCity.GetArmyStrength ();
				int distance = 0;

				if (distance <= militaryData.enemyGeneral.daysBeforeArrival) {
					general.task = GENERAL_TASK.ON_HELP;
					general.targetCity = null;
					general.targetLocation = militaryData.enemyGeneral.targetCity.hexTile;
					general.daysBeforeArrival = distance;
					militaryData.enemyGeneral.targetCity.visitingGenerals.Add (general);
				}

				if(militaryData.enemyGeneral.targetCity.GetArmyStrength() >= militaryData.enemyGeneral.ArmyStrength()){
					militaryData.isResolved = true;
				}
			}
		}

			
	}

	internal General SearchGeneralByID(int id){
		for(int i = 0; i < this.generals.Count; i++){
			if(this.generals[i].id == id){
				return this.generals [i];
			}
		}
		return null;
	}
}


#region Citizen Upgrade Functions
//internal void SelectCitizenToUpgrade(){
//	if(isDead){
//		return;
//	}
//	if(this.upgradeCitizenTarget != null){
//		return;
//	}
//	int totalChance = GetTotalChanceForCitizenUpgrade ();
//	if(totalChance <= 0){
//		return;
//	}
//	int choice = UnityEngine.Random.Range (0, totalChance+1);
//	int upperBound = 0;
//	int lowerBound = 0;
//	for (int i = 0; i < this.citizens.Count; i++) {
//		upperBound += this.citizens [i].upgradeChance;
//		if (choice >= lowerBound && choice < upperBound) {
//			this.upgradeCitizenTarget = this.citizens [i];
//			//				cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected for citizen upgrade: [FF0000]" + this.upgradeCitizenTarget.job.jobType.ToString() + "[-]\n\n"; 
//			break;
//		} else {
//			lowerBound = upperBound;
//		}
//	}
//
//	UpdateResourcesStatus ();
//}
//
//int GetTotalChanceForCitizenUpgrade(){
//	List<Citizen> citizensToChooseFrom = new List<Citizen> (); 
//	citizensToChooseFrom.AddRange(citizens.OrderBy(x => x.level).ToList());
//	citizensToChooseFrom.RemoveAll (x => x.level >= 10);
//	int lowestLevel = citizens.Min(x => x.level);
//	int totalChances = 0;
//	int[] currentChance = new int[]{100,60,20,5};
//	int a = 0;
//	for (int i = 0; i < citizensToChooseFrom.Count; i++) {
//		if (citizensToChooseFrom [i].level == lowestLevel) {
//			//Set Chance as 100
//			totalChances += currentChance [a];
//			citizensToChooseFrom [i].upgradeChance = currentChance [a];
//		} else {
//			lowestLevel = citizensToChooseFrom[i].level;
//			a += 1;
//			if (a >= currentChance.Length) {
//				a = currentChance.Length - 1;
//			}
//			totalChances += currentChance [a];
//			citizensToChooseFrom [i].upgradeChance = currentChance [a];
//		}
//	}
//	return totalChances;
//}

//	internal void AttemptToUpgradeCitizen(){
//		if(HasEnoughResourcesForAction(this.upgradeCitizenTarget.GetUpgradeRequirements().resource)){
//			int chance = UnityEngine.Random.Range(0,100);
//			if (chance < this.cityActionChances.upgradeCitizenChance) {
//				this.cityActionChances.upgradeCitizenChance = this.cityActionChances.defaultUpgradeCitizenChance;
//				ReduceResources (this.upgradeCitizenTarget.GetUpgradeRequirements().resource);
//				this.upgradeCitizenTarget.UpgradeCitizen();
//				cityLogs += GameManager.Instance.currentDay.ToString() + ": The [FF0000]" + this.upgradeCitizenTarget.job.jobType.ToString() + "[-] clan's level has increased.\n\n";
//				this.upgradeCitizenTarget = null;
//			} else {
//				this.cityActionChances.upgradeCitizenChance += 1;
//			}
//		}
//	}
#endregion