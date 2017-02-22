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

	public List<Citizen> unneededCitizens;
	protected int dayPioneerReachesCity = 0;
	protected CityTileTest pioneerCityTarget = null;

	public CITY_CITIZEN_ACTION nextCityCitizenAction;
	public JOB_TYPE citizenActionJobType;
	public HexTile citizenActionHexTile;
	public Citizen citizenToChange;
	public int pioneerPoints;

	protected List<Resource> generalCreationCost;

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
	public List<HexTile> unoccupiedOwnedTiles{ 
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
		this.unneededCitizens = new List<Citizen>();
		this.nextCityCitizenAction = CITY_CITIZEN_ACTION.NONE;
		this.citizenActionJobType = JOB_TYPE.NONE;
		this.citizenActionHexTile = null;
		this.citizenToChange = null;
		this.pioneerPoints = 1;

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
	 * Compute the constant cost of general creation.
	 * */
	void ComputeGeneralCreationCost(){
		if (this.kingdomTile != null) {
			switch (this.kingdomTile.kingdom.kingdomRace) {
			case RACE.HUMANS:
				this.generalCreationCost = new List<Resource> () {
					new Resource (RESOURCE.GOLD, 2000),
					new Resource (RESOURCE.STONE, 200)
				};
				break;
			case RACE.ELVES:
				this.generalCreationCost = new List<Resource> () {
					new Resource (RESOURCE.GOLD, 2000),
					new Resource (RESOURCE.LUMBER, 200)
				};
				break;
			case RACE.CROMADS:
				this.generalCreationCost = new List<Resource> () {
					new Resource (RESOURCE.GOLD, 2000),
					new Resource (RESOURCE.STONE, 200)
				};
				break;
			case RACE.MINGONS:
				this.generalCreationCost = new List<Resource> () {
					new Resource (RESOURCE.GOLD, 2000),
					new Resource (RESOURCE.LUMBER, 200)
				};
				break;
			}
		}
	}

	public void AddCityAsConnected(CityTileTest cityTile){
		this.connectedCities.Add(cityTile);
		this.numOfRoads = connectedCities.Count;
	}

	/*
	 * Occupies current city.
	 * */
	internal void OccupyCity(){
		this.hexTile.isOccupied = true;
		this.citizens = this.InitialCitizens;
//		for(int i = 0; i < 5; i++){
			CreateGeneral();
//		}
		ComputeGeneralCreationCost();
		SelectHexTileToPurchase();
		GenerateInitialFood();
		UpdateCityUpgradeRequirements ();
//		SelectCitizenForCreation ();
		this.kingdomTile.kingdom.lord.UpdateAdjacentLords();
		GeneralAI.onInstruct += ReceiveRequestFromGeneral;
//		LordMilitaryAI.onInstruct += this.ReceiveInstructionsFromLord;
		for (int i = 0; i < this.citizens.Count; i++) {
			List<HexTile> neighbours = new List<HexTile>();
			for (int j = 0; j < this.ownedBiomeTiles.Count; j++) {
				neighbours.AddRange (this.ownedBiomeTiles [i].GetListTilesInRange(0.5f));
			}
			neighbours = neighbours.Distinct().ToList();

			this.citizens[i].AssignCitizenToTile(neighbours);
		}

	}

	#region For Testing Scripts
	public void PurchaseTilesForTesting(int numOfTiles){
		for (int i = 0; i < numOfTiles; i++) {
			this.ownedBiomeTiles.Add(this.targetHexTileToPurchase);
			this.targetHexTileToPurchase.SetTileColor(Color.red);

			List<HexTile> neighbours = new List<HexTile>();
			for (int j = 0; j < this.ownedBiomeTiles.Count; j++) {
				List<HexTile> currentNeighbours = this.ownedBiomeTiles[j].GetListTilesInRange(0.5f);
				for (int k = 0; k < currentNeighbours.Count; k++) {
					if (this.ownedBiomeTiles.Contains (currentNeighbours [k]) || currentNeighbours [k].elevationType == ELEVATION.WATER) {
						currentNeighbours.Remove (currentNeighbours [k]);
					}
				}
				neighbours.AddRange (currentNeighbours);
			}

			neighbours = neighbours.Distinct().ToList();

			this.targetHexTileToPurchase = neighbours [UnityEngine.Random.Range (0, neighbours.Count)];
		}

		SelectHexTileToPurchase();

	}
	#endregion

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
		
	internal List<ResourceStatus> GetInitialResourcesStatus(){
		List<ResourceStatus> resourcesStatus = new List<ResourceStatus> ();
		RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
		for(int i = 0; i < allResources.Length; i++){
			resourcesStatus.Add(new ResourceStatus(allResources[i], RESOURCE_STATUS.NORMAL, 0, 0));
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
				this.UpdateCityExpenses();
				this.IdentifyCityCitizenAction();
			}
		}else{
			this.cityState = CITY_STATE.ABUNDANT;
		}
	}
		
	/*
	 * Harvest food in stockpile.
	 * */
	internal void TriggerFoodHarvest(){
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Harvest Day, food is now [FF0000]" + this.foodStockpileCount.ToString() + "[-] Food spoiled is [FF0000]" + 
			this.foodCount.ToString() + "[-].\n\n";
		this.foodCount = this.foodStockpileCount;
		this.foodStockpileCount = 0;
		this.IdentifyCityCitizenAction();
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
		if(this.citizens.Count > 0){
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
	}

	/*
	 * Get Total Gold Value of all
	 * owned tiles.
	 * */
	internal int GetGoldValue(){
		int goldValue = 0;
		if(this.ownedBiomeTiles.Count > 0){
			for(int i = 0; i < this.ownedBiomeTiles.Count; i++){
				goldValue += this.ownedBiomeTiles [i].goldValue;
			}
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
		

	internal void UpdateCityExpenses(){
//		cityLogs += GameManager.Instance.currentDay.ToString() + ": Update City Expenses.\n\n";
		int[] allNeededResources = GetNeededResources();

		for (int i = 0; i < allNeededResources.Length; i++) {
			RESOURCE currentResource = RESOURCE.GOLD;
			if (i == 1) {
				currentResource = RESOURCE.FOOD;
			} else if (i == 2) {
				currentResource = RESOURCE.LUMBER;
			} else if (i == 3) {
				currentResource = RESOURCE.STONE;
			} else if (i == 4) {
				currentResource = RESOURCE.METAL;
			} else if (i == 5) {
				currentResource = RESOURCE.MANA;
			}


			if (currentResource == RESOURCE.FOOD) {
				int dailyFoodConsumption = GetDailyFoodConsumption();
				int neededFood = (int)(GetNeededFoodForNumberOfDays(GameManager.Instance.daysUntilNextHarvest) * 1.3f);
				int foodStatusValue = (int)Mathf.Abs (this.foodCount - neededFood);
				if (this.foodCount > neededFood) {
					SetResourceStatus (RESOURCE.FOOD, RESOURCE_STATUS.ABUNDANT, foodStatusValue);
				} else {
					SetResourceStatus (RESOURCE.FOOD, RESOURCE_STATUS.SCARCE, foodStatusValue);
				}
			} else {
				bool isResourceNeeded = allNeededResources[i] > 0;
				int statusValue = (int)Mathf.Abs ((allNeededResources[i] * 1.3f) - GetNumberOfResourcesPerType (currentResource));
				if (isResourceNeeded && IsProducingResource (currentResource)) {
					if (GetNumberOfResourcesPerType (currentResource) < allNeededResources [i]) {//If producing needed resource but kulang
						if (currentResource == RESOURCE.GOLD) {
							SetResourceStatus (currentResource, RESOURCE_STATUS.SCARCE, statusValue);
						} else {
							//Compute Number of days to reach goal
							int dailyProductionOfResource = GetAveDailyProduction (currentResource, this.citizens);
							int daysUntilGoalIsReached = (int)(Mathf.Abs(allNeededResources[i] - GetNumberOfResourcesPerType(currentResource)) / dailyProductionOfResource);

							SetResourceStatus (currentResource, RESOURCE_STATUS.SCARCE, statusValue, daysUntilGoalIsReached);
						}
					} else if (GetNumberOfResourcesPerType (currentResource) > (allNeededResources [i] * 1.5f)) {//If producing needed resource and sobra w/ buffer
						SetResourceStatus (currentResource, RESOURCE_STATUS.ABUNDANT, statusValue, -1);
					} else {
						SetResourceStatus (currentResource, RESOURCE_STATUS.NORMAL, statusValue, 0);
					}
				} else if (isResourceNeeded && !IsProducingResource (currentResource)) {//If not producing needed resource
					SetResourceStatus (currentResource, RESOURCE_STATUS.SCARCE, statusValue, 100000);//Set resource as scarce

				} else if (!isResourceNeeded) {//If resource is not needed
					SetResourceStatus (currentResource, RESOURCE_STATUS.ABUNDANT, statusValue, -1);//Set resource as abundant
				} 
			}

			//for setting unneeded citizens
			for (int j = 0; j < this.allResourcesStatus.Count; j++) {
				if (this.allResourcesStatus[j].resource == RESOURCE.FOOD) {
					continue;
				}
				List<JOB_TYPE> jobTypesThatProduceResource = GetJobTypeByResourceProduced (this.allResourcesStatus[j].resource).ToList ();
				if (this.allResourcesStatus[j].status == RESOURCE_STATUS.SCARCE) {
					//Remove all citizens in unneededCitizens list that produce the current resource, since the resource is scarce.
					for (int k = 0; k < this.unneededCitizens.Count; k++) {
						if (jobTypesThatProduceResource.Contains(this.unneededCitizens[k].job.jobType)) {
							this.unneededCitizens.Remove(this.unneededCitizens[k]);
						}
					}
				} else if (this.allResourcesStatus[j].status == RESOURCE_STATUS.ABUNDANT){
					//Set all citizens that produce the abundant resource as unneeded
					for (int k = 0; k < this.citizens.Count; k++) {
						if (jobTypesThatProduceResource.Contains(this.citizens[k].job.jobType)) {
							if (!this.unneededCitizens.Contains (this.citizens[k])) {
								this.unneededCitizens.Add (this.citizens[k]);
							}
						}
					}
				}
			}

			//Remove dead citizens from unneeded citizens
			for (int j = 0; j < this.unneededCitizens.Count; j++) {
				if (!this.citizens.Contains (this.unneededCitizens [j])) {
					this.unneededCitizens.Remove (this.unneededCitizens [j]);
				}
			}
				
		}
	}

	internal int[] GetNeededResources(){
		int[] allNeededResources = new int[6]; //gold, food, lumber, stone, metal, mana

		//City Upgrade Cost
		for (int i = 0; i < cityUpgradeRequirements.resource.Count; i++) {
			switch(cityUpgradeRequirements.resource[i].resourceType){
			case RESOURCE.GOLD:
				allNeededResources[0] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				allNeededResources[1] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				allNeededResources[2] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				allNeededResources[3] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			case RESOURCE.METAL:
				allNeededResources[4] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			case RESOURCE.MANA:
				allNeededResources[5] += cityUpgradeRequirements.resource[i].resourceQuantity;
				break;
			}
		}

		//Pioneer Cost
		allNeededResources[0] += 2000 * pioneerPoints;

		//Army Upgrade Costs
		if(this.kingdomTile != null){
			if(this.generals.Count > 0){
				for (int i = 0; i < this.kingdomTile.kingdom.armyIncreaseUnitResource.Count; i++) {
					switch (this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceType) {
					case RESOURCE.GOLD:
						allNeededResources [0] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					case RESOURCE.FOOD:
						allNeededResources [1] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					case RESOURCE.LUMBER:
						allNeededResources [2] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					case RESOURCE.STONE:
						allNeededResources [3] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					case RESOURCE.METAL:
						allNeededResources [4] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					case RESOURCE.MANA:
						allNeededResources [5] += this.kingdomTile.kingdom.armyIncreaseUnitResource [i].resourceQuantity;
						break;
					}
				}
			}
		}

		//General Creation Costs
		for (int i = 0; i < this.generalCreationCost.Count; i++) {
			switch (this.generalCreationCost[i].resourceType) {
			case RESOURCE.GOLD:
				allNeededResources[0] += this.generalCreationCost[i].resourceQuantity;
				break;
			case RESOURCE.FOOD:
				allNeededResources[1] += this.generalCreationCost[i].resourceQuantity;
				break;
			case RESOURCE.LUMBER:
				allNeededResources[2] += this.generalCreationCost[i].resourceQuantity;
				break;
			case RESOURCE.STONE:
				allNeededResources[3] += this.generalCreationCost[i].resourceQuantity;
				break;
			case RESOURCE.METAL:
				allNeededResources[4] += this.generalCreationCost[i].resourceQuantity;
				break;
			case RESOURCE.MANA:
				allNeededResources[5] += this.generalCreationCost[i].resourceQuantity;
				break;
			}
		}

		//Purchase Tile Costs
		if (this.targetHexTileToPurchase != null) {
			List<Resource> resourceReqs = GetHexTileCost (this.targetHexTileToPurchase);
			for (int i = 0; i < resourceReqs.Count; i++) {
				switch (resourceReqs[i].resourceType) {
				case RESOURCE.GOLD:
					allNeededResources[0] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.FOOD:
					allNeededResources[1] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.LUMBER:
					allNeededResources[2] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.STONE:
					allNeededResources[3] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.METAL:
					allNeededResources[4] += resourceReqs[i].resourceQuantity;
					break;
				case RESOURCE.MANA:
					allNeededResources[5] += resourceReqs[i].resourceQuantity;
					break;
				}
			}
		}

		return allNeededResources;

	}

	void SetResourceStatus(RESOURCE resourceType, RESOURCE_STATUS resourceStatus, int statusValue, int daysToReachGoal = 0){
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus[i].resource == resourceType) {
				this.allResourcesStatus[i].status = resourceStatus;
				this.allResourcesStatus[i].amount = statusValue;
				this.allResourcesStatus[i].daysToReachGoal = daysToReachGoal;
			}
		}
	}

	bool IsProducingResource(RESOURCE resourceType){
		if (resourceType == RESOURCE.GOLD) {
			return true;
		}
		if (GetAveDailyProduction (resourceType, this.citizens) > 0) {
			return true;
		}
		return false;
	}


	internal void IdentifyCityCitizenAction(){
		#region Food Check
		cityLogs += GameManager.Instance.currentDay.ToString() + ": Identifying citizen action...\n\n";
		int currentFoodProduction = GetAveDailyProduction (RESOURCE.FOOD, this.citizens);
		int currentFoodConsumption = (int)(GetDailyFoodConsumption() * 1.1f);

		List<Citizen> tempCitizens = GetCitizensPerType(JOB_TYPE.FARMER);
		tempCitizens.OrderBy(x => x.GetAveDailyProduction(RESOURCE.FOOD));
		Citizen lowestFoodProducer = null;
		if (tempCitizens.Count > 0) {
			lowestFoodProducer = tempCitizens [0];
			//Remove lowest producing
			tempCitizens.RemoveAt (0);
		}

		if (currentFoodProduction > currentFoodConsumption && lowestFoodProducer != null) {
			int modifiedDailyFoodProduction = GetAveDailyProduction (RESOURCE.FOOD, tempCitizens);
			if (modifiedDailyFoodProduction > currentFoodConsumption) {
				if (!this.unneededCitizens.Contains (lowestFoodProducer)) {
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Added: " + lowestFoodProducer.name + "/" + 
						lowestFoodProducer.job.jobType.ToString() + " to unneeded citizens\n\n";
					this.unneededCitizens.Add (lowestFoodProducer);
				}
			} 
		} else {
			for (int i = 0; i < this.unneededCitizens.Count; i++) {
				if (this.unneededCitizens [i].job.jobType == JOB_TYPE.FARMER || this.unneededCitizens [i].job.jobType == JOB_TYPE.HUNTER) {
					this.unneededCitizens.Remove (this.unneededCitizens [i]);
				}
			}

			if (!this.IsCitizenCapReached() && this.unoccupiedOwnedTiles.Count > 0) {
				List<HexTile> tilesOrderedByHighestFoodValue = this.unoccupiedOwnedTiles.OrderByDescending (x => x.farmingValue).ThenByDescending (x => x.huntingValue).ToList ();
				//Check if there are positive hunting/farming values
				if (tilesOrderedByHighestFoodValue [0].huntingValue <= 0 && tilesOrderedByHighestFoodValue [0].farmingValue <= 0) {
					if (this.unneededCitizens.Count <= 0) {
						return;
					}
					List<Citizen> unneededCitizensOrderedByHighestFoodValue = this.unneededCitizens.OrderByDescending (x => x.assignedTile.farmingValue).ThenByDescending (x => x.assignedTile.huntingValue).ToList ();
					//Check if there are positive hunting/farming values
					for (int i = 0; i < unneededCitizensOrderedByHighestFoodValue.Count; i++) {
						if (unneededCitizensOrderedByHighestFoodValue [i].job.jobType == JOB_TYPE.FARMER ||
						    unneededCitizensOrderedByHighestFoodValue [i].job.jobType == JOB_TYPE.HUNTER) {
							continue;
						}

						if (unneededCitizensOrderedByHighestFoodValue [i].assignedTile.huntingValue > 0 ||
						    unneededCitizensOrderedByHighestFoodValue [i].assignedTile.farmingValue > 0) {

							//Set unneededCitizensOrderedByHighestFoodValue[i] to be changed
							this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CHANGE_CITIZEN;
							this.citizenToChange = unneededCitizensOrderedByHighestFoodValue [i];
							if (unneededCitizensOrderedByHighestFoodValue [i].assignedTile.huntingValue >
							    unneededCitizensOrderedByHighestFoodValue [i].assignedTile.farmingValue) {
								this.citizenActionJobType = JOB_TYPE.HUNTER;
							} else {
								this.citizenActionJobType = JOB_TYPE.FARMER;
							}

							cityLogs += GameManager.Instance.currentDay.ToString () + ": Trigger Point 1: Set " + this.citizenToChange.name + "/" +
							this.citizenToChange.job.jobType.ToString () + " to be changed to " + this.citizenActionJobType.ToString () + "\n\n";
							break;
						}
					}

				} else {

					//Create Famer/Hunter on tile tilesOrderedByHighestFoodValue[0]
					this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CREATE_CITIZEN;
					this.citizenActionHexTile = tilesOrderedByHighestFoodValue [0];
					if (tilesOrderedByHighestFoodValue [0].huntingValue > tilesOrderedByHighestFoodValue [0].farmingValue) {
						this.citizenActionJobType = JOB_TYPE.HUNTER;
					} else {
						this.citizenActionJobType = JOB_TYPE.FARMER;
					}

					cityLogs += GameManager.Instance.currentDay.ToString () + ": Trigger Point 1: Set " + this.citizenActionJobType.ToString () + " to be created on tile "
					+ this.citizenActionHexTile.name + "\n\n";

				}
			} else {
				if (this.unneededCitizens.Count <= 0) {
					return;
				}
				List<Citizen> unneededCitizensOrderedByHighestFoodValue = this.unneededCitizens.OrderByDescending (x => x.assignedTile.farmingValue).ThenByDescending (x => x.assignedTile.huntingValue).ToList ();
				//Check if there are positive hunting/farming values
				for (int i = 0; i < unneededCitizensOrderedByHighestFoodValue.Count; i++) {
					if (unneededCitizensOrderedByHighestFoodValue [i].job.jobType == JOB_TYPE.FARMER ||
						unneededCitizensOrderedByHighestFoodValue [i].job.jobType == JOB_TYPE.HUNTER) {
						continue;
					}

					if (unneededCitizensOrderedByHighestFoodValue [i].assignedTile.huntingValue > 0 ||
						unneededCitizensOrderedByHighestFoodValue [i].assignedTile.farmingValue > 0) {

						//Set unneededCitizensOrderedByHighestFoodValue[i] to be changed
						this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CHANGE_CITIZEN;
						this.citizenToChange = unneededCitizensOrderedByHighestFoodValue [i];
						if (unneededCitizensOrderedByHighestFoodValue [i].assignedTile.huntingValue >
							unneededCitizensOrderedByHighestFoodValue [i].assignedTile.farmingValue) {
							this.citizenActionJobType = JOB_TYPE.HUNTER;
						} else {
							this.citizenActionJobType = JOB_TYPE.FARMER;
						}

						cityLogs += GameManager.Instance.currentDay.ToString () + ": Trigger Point 1: Set " + this.citizenToChange.name + "/" +
							this.citizenToChange.job.jobType.ToString () + " to be changed to " + this.citizenActionJobType.ToString () + "\n\n";
						break;
					}
				}
			}
		}
		#endregion

		/*
		 * Check validity of current citizen action
		 * */
		if (this.citizenActionJobType != JOB_TYPE.NONE) {
			List<Resource> cityCitizenActionCost = GetCitizenCreationCostPerType (this.citizenActionJobType);
			//if can afford/produce needed resources for citizen action, keep citizen action
			if (!HasEnoughResourcesForAction (cityCitizenActionCost)) {
				for (int i = 0; i < cityCitizenActionCost.Count; i++) {
					if (!IsProducingResource (cityCitizenActionCost [i].resourceType)) {
						this.nextCityCitizenAction = CITY_CITIZEN_ACTION.NONE;
						this.citizenActionJobType = JOB_TYPE.NONE;
						this.citizenActionHexTile = null;
						this.citizenToChange = null;
						break;
					}
				}
				if (this.nextCityCitizenAction != CITY_CITIZEN_ACTION.NONE) {
					//is producing all needed resources, keep current action
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Keeping current city citizen action.\n\n";
					return;
				}
			} else {
				//can afford citizen action, keep current action
				cityLogs += GameManager.Instance.currentDay.ToString() + ": Keeping current city citizen action.\n\n";
				return;
			}
		}

		//Assign new citizen action, code will only reach here if the city cannot afford/produce the needed resources for the Farmer/Hunter citizen action

		//Determine Needed Resources
		List<RESOURCE> scarceResources = new List<RESOURCE>();
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus [i].resource != RESOURCE.FOOD && this.allResourcesStatus [i].resource != RESOURCE.GOLD) {
				if (this.allResourcesStatus [i].status == RESOURCE_STATUS.SCARCE) {
					scarceResources.Add (this.allResourcesStatus [i].resource);
				}
			}
		}
		scarceResources = scarceResources.OrderByDescending(x => GetResourceStatusByType(x).daysToReachGoal).ToList();
		if (scarceResources.Count > 0) {
			for (int i = 0; i < scarceResources.Count; i++) {
				if (scarceResources [i] == RESOURCE.GOLD || scarceResources[i] == RESOURCE.FOOD) {
					continue;
				}
				//Get all roles that can produce needed resource
				JOB_TYPE[] jobTypesThatCanProduceResource = GetJobTypeByResourceProduced (scarceResources [i]);
				for (int j = 0; j < jobTypesThatCanProduceResource.Length; j++) {
					List<Resource> cityCitizenActionCost = GetCitizenCreationCostPerType (jobTypesThatCanProduceResource [j]);
					//Filter roles that the city cannot create
					if (!HasEnoughResourcesForAction (cityCitizenActionCost)) {
						for (int k = 0; k < cityCitizenActionCost.Count; k++) {
							if (!IsProducingResource (cityCitizenActionCost [k].resourceType)) {
								jobTypesThatCanProduceResource [j] = JOB_TYPE.NONE;
								break;
							}
						}
					}
				}

				for (int j = 0; j < jobTypesThatCanProduceResource.Length; j++) {
					JOB_TYPE currentJobType = jobTypesThatCanProduceResource [j];
					if (!this.IsCitizenCapReached () && this.unoccupiedOwnedTiles.Count > 0) {
						List<HexTile> tilesOrderedByHighestResourceValue = this.unoccupiedOwnedTiles.
						OrderByDescending (x => x.GetRelevantResourceValueByJobType (currentJobType)).ToList ();
						//Check if there are positive resource values
						if (tilesOrderedByHighestResourceValue [0].GetRelevantResourceValueByJobType (currentJobType) <= 0) {
							if (this.unneededCitizens.Count <= 0) {
								cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger point 1: No unneeded citizens, no possible city citizen action.\n\n";
								continue;
							}
							List<Citizen> unneededCitizensOrderedByHighestResourceValue = this.unneededCitizens.
								OrderByDescending (x => x.assignedTile.GetRelevantResourceValueByJobType (currentJobType)).ToList();

							//Check if there are positive resource values
							for (int k = 0; k < unneededCitizensOrderedByHighestResourceValue.Count; k++) {
								if (unneededCitizensOrderedByHighestResourceValue [k].job.jobType == currentJobType) {
									continue;
								}
								if (unneededCitizensOrderedByHighestResourceValue [k].assignedTile.GetRelevantResourceValueByJobType (currentJobType) > 0) {

									//Set unneededCitizensOrderedByHighestResourceValue[0] to be changed
									this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CHANGE_CITIZEN;
									this.citizenToChange = unneededCitizensOrderedByHighestResourceValue [k];
									this.citizenActionJobType = currentJobType;

									cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point 2: Set " + this.citizenToChange.name + "/" + 
										this.citizenToChange.job.jobType.ToString() + " to be changed to " + this.citizenActionJobType.ToString() + "\n\n";
									return;
								}
							}
							cityLogs += GameManager.Instance.currentDay.ToString() + ": No Hex Tiles with positive value for: " + currentJobType.ToString() + ".\n\n";

						} else {
						
							//Create currentJobType on tile tilesOrderedByHighestResourceValue[0]
							this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CREATE_CITIZEN;
							this.citizenActionHexTile = tilesOrderedByHighestResourceValue [0];
							this.citizenActionJobType = currentJobType;

							cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point 2: Set " + this.citizenActionJobType.ToString() + " to be created on tile " 
								+ this.citizenActionHexTile.name + "\n\n";
							return;
		
						}
					} else {
						if (this.unneededCitizens.Count <= 0) {
							cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point 2: No unneeded citizens, no possible city citizen action.\n\n";
							return;
						}

						List<Citizen> unneededCitizensOrderedByHighestResourceValue = this.unneededCitizens.
							OrderByDescending (x => x.assignedTile.GetRelevantResourceValueByJobType (currentJobType)).ToList();

						//Check if there are positive resource values
						for (int k = 0; k < unneededCitizensOrderedByHighestResourceValue.Count; k++) {
							if (unneededCitizensOrderedByHighestResourceValue [k].job.jobType == currentJobType) {
								continue;
							}

							if (unneededCitizensOrderedByHighestResourceValue [k].assignedTile.GetRelevantResourceValueByJobType (currentJobType) > 0) {

								//Set unneededCitizensOrderedByHighestResourceValue[0] to be changed
								this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CHANGE_CITIZEN;
								this.citizenToChange = unneededCitizensOrderedByHighestResourceValue [k];
								this.citizenActionJobType = currentJobType;

								cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point 3: Set " + this.citizenToChange.name + "/" + 
									this.citizenToChange.job.jobType.ToString() + " to be changed to " + this.citizenActionJobType.ToString() + "\n\n";
								return;
							}
						}
						cityLogs += GameManager.Instance.currentDay.ToString() + ": No Hex Tiles with positive value for: " + currentJobType.ToString() + ".\n\n";
					}

				}
			}
		} else {
			//no needed resources

			//Check if can make merchant
			int numOfTraders = this.GetNumberOfCitizensPerType(JOB_TYPE.MERCHANT);
			int chanceToSetTraderAsAction = 0;
			int chance = UnityEngine.Random.Range(0, 100);
			if (numOfTraders == 0) {
				chanceToSetTraderAsAction = 50;
			} else if (numOfTraders == 1) {
				chanceToSetTraderAsAction = 20;
			} else {
				chanceToSetTraderAsAction = 5;
			}

			if (chance < chanceToSetTraderAsAction) {
				if (!this.IsCitizenCapReached () && this.unoccupiedOwnedTiles.Count > 0) {
					//Create merchant
					this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CREATE_CITIZEN;
					this.citizenActionHexTile = this.unoccupiedOwnedTiles[UnityEngine.Random.Range(0, this.unoccupiedOwnedTiles.Count)];
					this.citizenActionJobType = JOB_TYPE.MERCHANT;
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point M: Set " + this.citizenActionJobType.ToString() + 
						" to be created on " + this.citizenActionHexTile.name + "\n\n";
					return;
				} else {
					//Change unneeded citizen to merchant
					if (this.unneededCitizens.Count > 0) {
						this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CHANGE_CITIZEN;
						this.citizenActionJobType = JOB_TYPE.MERCHANT;
						this.citizenToChange = this.unneededCitizens [UnityEngine.Random.Range (0, this.unneededCitizens.Count)];
						cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point M: Set " + this.citizenToChange.name + "/" + 
							this.citizenToChange.job.jobType.ToString() + " to be changed to " + this.citizenActionJobType.ToString() + "\n\n";
						return;
					}
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Cannot create new merchant and no unneeded citizens .\n\n";
				}
			}

			//Get all jobtypes
			List<JOB_TYPE> allJobTypesCityCanCreate = Enum.GetValues(typeof(JOB_TYPE)).Cast<JOB_TYPE>().ToList();
			//Filter jobtypes that the city can't create
			for (int i = 0; i < allJobTypesCityCanCreate.Count; i++) {
				if (allJobTypesCityCanCreate [i] == JOB_TYPE.NONE) {
					continue;
				}
				List<Resource> jobTypeCreationCost = GetCitizenCreationCostPerType(allJobTypesCityCanCreate[i]);
				if (!HasEnoughResourcesForAction(jobTypeCreationCost)) {
					for (int k = 0; k < jobTypeCreationCost.Count; k++) {
						if (!IsProducingResource (jobTypeCreationCost [k].resourceType)) {
							allJobTypesCityCanCreate.Remove(allJobTypesCityCanCreate[i]);
							break;
						}
					}
				}
			}

			if (!this.IsCitizenCapReached () && this.unoccupiedOwnedTiles.Count > 0) {
				for (int i = 0; i < allJobTypesCityCanCreate.Count; i++) {
					List<HexTile> tilesOrderedByHighestResourceValue = this.unoccupiedOwnedTiles.
						OrderByDescending (x => x.GetRelevantResourceValueByJobType (allJobTypesCityCanCreate[i])).ToList ();
					//Check if there are positive resource values
					if (tilesOrderedByHighestResourceValue [0].GetRelevantResourceValueByJobType (allJobTypesCityCanCreate [i]) <= 0) {
						continue;
					} else {

						//Create currentJobType on tile tilesOrderedByHighestResourceValue[0]
						this.nextCityCitizenAction = CITY_CITIZEN_ACTION.CREATE_CITIZEN;
						this.citizenActionHexTile = tilesOrderedByHighestResourceValue [0];
						this.citizenActionJobType = allJobTypesCityCanCreate[i];
						cityLogs += GameManager.Instance.currentDay.ToString() + ": Trigger Point 3: Set " + this.citizenActionJobType.ToString() + " to be created on tile " 
							+ this.citizenActionHexTile.name + "\n\n";
						return;

					}
				}
			}
			cityLogs += GameManager.Instance.currentDay.ToString() + ": No Needed Resources and citizen cap is reached or no unoccupied tiles.\n\n";

		}


	}

	internal void AttemptToPerformCitizenAction(){
		if (this.nextCityCitizenAction == CITY_CITIZEN_ACTION.NONE) {
			return;
		}
		List<Resource> cityCitizenActionCost = this.GetCitizenCreationCostPerType(this.citizenActionJobType);
		if (HasEnoughResourcesForAction(cityCitizenActionCost)) {
			int chance = UnityEngine.Random.Range (0, 100);
			if (chance < this.cityActionChances.performCityCitizenActionChance) {
				//Perform City Action
				if (this.nextCityCitizenAction == CITY_CITIZEN_ACTION.CREATE_CITIZEN) {
					
					Citizen newCitizen = new Citizen(this.citizenActionJobType, this);
					newCitizen.SetCitizenTile(this.citizenActionHexTile);
					this.citizens.Add(newCitizen);

					cityLogs += GameManager.Instance.currentDay.ToString() + ": Created new: [FF0000]" + this.citizenActionJobType.ToString() + "[-] on tile " +
						"[FF0000]" + this.citizenActionHexTile.name + "[-] \n\n";

//					if (this.citizenActionJobType == JOB_TYPE.MERCHANT) {
//						SetMerchantActions((Merchant)newCitizen.job);
//					}

					this.citizenActionJobType = JOB_TYPE.NONE;
					this.nextCityCitizenAction = CITY_CITIZEN_ACTION.NONE;
					this.citizenActionHexTile = null;
				} else if (this.nextCityCitizenAction == CITY_CITIZEN_ACTION.CHANGE_CITIZEN) {
					cityLogs += GameManager.Instance.currentDay.ToString() + ": Changed : [FF0000]" + this.citizenToChange.job.jobType.ToString() + "[-] to " +
						"[FF0000]" + this.citizenActionJobType.ToString() + "[-] \n\n";

					this.unneededCitizens.Remove(this.citizenToChange);
					this.citizenToChange.ChangeJob(this.citizenActionJobType);

//					if (this.citizenActionJobType == JOB_TYPE.MERCHANT) {
//						SetMerchantActions((Merchant)this.citizenToChange.job);
//					}

					this.nextCityCitizenAction = CITY_CITIZEN_ACTION.NONE;
					this.citizenActionJobType = JOB_TYPE.NONE;
					this.citizenToChange = null;
				}

				this.ReduceResources(cityCitizenActionCost);
				this.cityActionChances.performCityCitizenActionChance = this.cityActionChances.defaultPerformCityCitizenActionChance;
				this.UpdateCityExpenses();
				this.IdentifyCityCitizenAction();
			} else {
				this.cityActionChances.performCityCitizenActionChance += 1;
			}
		}
	}

	void SetMerchantActions(Merchant merchant){
		Debug.LogError (merchant.citizen.name);
		List<Resource> excessResources = new List<Resource>();
		for (int i = 0; i < this.allResourcesStatus.Count; i++) {
			if (this.allResourcesStatus[i].status == RESOURCE_STATUS.ABUNDANT) {
				excessResources.Add (new Resource(this.allResourcesStatus[i].resource, this.allResourcesStatus[i].amount));
			}
		}
		merchant.tradeGoods = excessResources;
		merchant.currentTile = this.hexTile;

		List<CityTest> elligibleCitiesForTrade = new List<CityTest>();
		for (int i = 0; i < excessResources.Count; i++) {
			RESOURCE currentAbundantResource = excessResources[i].resourceType;
			if (currentAbundantResource == RESOURCE.GOLD) {

			} else {

			}
			
		}


	}

	internal void AttemptToCreatePioneer(){
		if (this.pioneerPoints <= 0 || this.pioneerCityTarget != null) {
			return;
		}

		int chance = UnityEngine.Random.Range(0, 100);
		if (chance < this.cityActionChances.expansionChance) {
			//TODO: Change Distance Value To Pathfinding instead of Vector2.Distance
			pioneerCityTarget = kingdomTile.kingdom.NearestUnoccupiedCity();
			if (pioneerCityTarget == null) {
				return;
			}
			dayPioneerReachesCity = GameManager.Instance.currentDay + (int)Vector2.Distance(kingdomTile.kingdom.cities[0].transform.position, 
				pioneerCityTarget.hexTile.transform.position);
//			GameManager.Instance.turnEnded += SendPioneer;
			cityLogs += GameManager.Instance.currentDay.ToString() + ": Pioneer will reach city: [FF0000]" + pioneerCityTarget.hexTile.name + "[-] on day [FF0000]" + dayPioneerReachesCity.ToString() + "[-]\n\n";
		}
	}

	internal void AttemptToCreateNewGeneral(){
		if (this.generals.Count < this.generalsLimit && HasEnoughResourcesForAction(this.generalCreationCost)) {
			int chance = UnityEngine.Random.Range (0, 100);
			if (chance < this.cityActionChances.createGeneralChance) {
				this.CreateGeneral();
				this.ReduceResources(this.generalCreationCost);
				this.cityActionChances.createGeneralChance = this.cityActionChances.defaultCreateGeneralChance;
			} else {
				this.cityActionChances.createGeneralChance += 1;
			}
		}
	}

	JOB_TYPE[] GetJobTypeByResourceProduced(RESOURCE resourceProduced){
		if (resourceProduced == RESOURCE.FOOD) {
			return new JOB_TYPE[]{JOB_TYPE.FARMER, JOB_TYPE.HUNTER};
		} else if (resourceProduced == RESOURCE.LUMBER) {
			return new JOB_TYPE[]{ JOB_TYPE.WOODSMAN };
		} else if (resourceProduced == RESOURCE.STONE) {
			return new JOB_TYPE[]{ JOB_TYPE.QUARRYMAN };
		} else if (resourceProduced == RESOURCE.MANA) {
			return new JOB_TYPE[]{ JOB_TYPE.ALCHEMIST };
		} else if (resourceProduced == RESOURCE.METAL) {
			return new JOB_TYPE[]{ JOB_TYPE.MINER };
		}
		return new JOB_TYPE[]{ JOB_TYPE.NONE };
	}

	List<Citizen> GetCitizensPerType(JOB_TYPE jobType){
		List<Citizen> citizenListPerType = new List<Citizen>();
		for (int i = 0; i < this.citizens.Count; i++) {
			if (jobType == JOB_TYPE.FARMER || jobType == JOB_TYPE.HUNTER) {
				if (this.citizens [i].job.jobType == JOB_TYPE.FARMER || this.citizens [i].job.jobType == JOB_TYPE.HUNTER) {
					citizenListPerType.Add (this.citizens [i]);
				} else {
					if(this.citizens[i].job.jobType == jobType){
						citizenListPerType.Add(this.citizens[i]);
					}
				}
			}
		}
		return citizenListPerType;
	}

	/*
	 * Assign needed role, can only be
	 * Farmer, Hunter or Pioneer
	 * */
	internal void AssignNeededRole(){
		if(isDead){
			return;
		}

		int averageDailyProd = GetAveDailyProduction (RESOURCE.FOOD, this.citizens);
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
			this.unneededResources = GetUnneededResources (GetNeededResources ());
			this.unneededRoles = GetUnneededRoles ();
		}else{
			this.cityActionChances.oversupplyChance += 2;
		}
	}


	private bool isResourceUnneeded (RESOURCE resource){
		if(this.unneededResources.Count > 0){
			for(int i = 0; i < this.unneededResources.Count; i++){
				if(resource == this.unneededResources[i]){
					return true;
				}
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

	internal void CreateGeneral(){
//		if (this.generals.Count < this.generalsLimit) {
			General newGeneral = new General (this);
			this.generals.Add (newGeneral);
//		}
	}
	internal void ArmyMaintenance(){
		if(GameManager.Instance.currentDay % 8 == 0){
			if(this.generals.Count > 0){
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
					if(generals.Count > 0){
						this.cityActionChances.increaseArmyCountChance = this.cityActionChances.defaultIncreaseArmyCountChance;
						General chosenGeneral = generals [UnityEngine.Random.Range (0, generals.Count)];
						chosenGeneral.army.armyCount += this.kingdomTile.kingdom.armyIncreaseUnits;
						ReduceResources (this.kingdomTile.kingdom.armyIncreaseUnitResource);
					}
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
				this.pioneerPoints += 1;
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
				UpdateCityExpenses();
//				UpdateResourcesStatus();
				this.IdentifyCityCitizenAction();

			} else {
				//On not performing upgrade, increase chance to upgrade by 1
				this.cityActionChances.increaseHousingChance += 1;
			}
		}
	}
		


	void SendPioneer(int currentDay){
		if (dayPioneerReachesCity == (currentDay+1)) {
			//Pioneer has reached city
			cityLogs += GameManager.Instance.currentDay.ToString() + ": [FF0000]PIONEER[-] has reached city\n\n";
			if (!this.pioneerCityTarget.hexTile.isOccupied) {
				this.kingdomTile.AddCityToKingdom (pioneerCityTarget);
				pioneerCityTarget.cityAttributes.OccupyCity();
				pioneerCityTarget.hexTile.GetComponent<CityTileTest>().SetCityAsActiveAndSetProduction ();
				pioneerCityTarget.cityAttributes.foodStockpileCount += GetNeededFoodForNumberOfDays (33 - GameManager.Instance.daysUntilNextHarvest);
				GameManager.Instance.UpdateLordAdjacency();
				cityLogs += GameManager.Instance.currentDay.ToString () + ": PIONEER was [FF0000]successful[-] in expansion\n\n";
			} else {
				cityLogs += GameManager.Instance.currentDay.ToString () + ": PIONEER [FF0000]failed[-] in expansion\n\n";
			}

			this.pioneerPoints -= 1;
			this.pioneerCityTarget = null;
//			for (int i = 0; i < citizens.Count; i++) {
//				if (citizens[i].job.jobType == JOB_TYPE.PIONEER) {
//					citizens.Remove(citizens[i]);
//				}
//			}
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
//					SelectCitizenForCreation();
				}
					
				this.cityActionChances.changeCitizenChance = this.cityActionChances.defaultChangeCitizenChance;
				citizen.ResetLevel();
				AssignCitizenToTile(citizen);
				ReduceResources(citizenCreationCost);
				RemoveUnneededResources (randomUnneededJob);
				this.unneededRoles.Remove(randomUnneededJob);
				this.unneededResources = GetUnneededResources (GetNeededResources ());
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
				new Resource (RESOURCE.GOLD, (100 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.primaryResourceToPurchaseTile, 100 + (20 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.secondaryResourceToPurchaseTile, 80 + (5 * (this.ownedBiomeTiles.Count - ownedTilesThreshold)))
			};
		} else {
			tilePurchaseCost = new Resource[] {
				new Resource (RESOURCE.GOLD, (100 * this.ownedBiomeTiles.Count)),
				new Resource (hexTileToPurchase.primaryResourceToPurchaseTile, 100 + (20 * this.ownedBiomeTiles.Count)),
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
				this.UpdateCityExpenses();
				this.IdentifyCityCitizenAction();
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
				if (this.ownedBiomeTiles.Contains (currentNeighbours [j]) || currentNeighbours [j].elevationType == ELEVATION.WATER) {
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
		if(this.citizens.Count > 0){
			for(int i = 0; i < this.citizens.Count; i++){
				if(this.citizens[i].job.jobType == unneededRole){
					unneededCitizens.Add (this.citizens [i]);
				}
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
			AdjustResourceCount (resource [i].resourceType, resource [i].resourceQuantity);
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

//	internal void UpdateResourcesStatus(){
//		int[] neededResources = GetNeededResources ();
//		for(int i = 0; i < this.allResourcesStatus.Count; i++){
//			if(this.allResourcesStatus[i].resource == RESOURCE.FOOD){
//				int daysFoodSupplyLasts = (int)(this.foodCount / GetDailyFoodConsumption ());
//				int deficit = (int)Mathf.Abs(daysFoodSupplyLasts - GameManager.Instance.daysUntilNextHarvest) * GetDailyFoodConsumption ();
//
//				if(daysFoodSupplyLasts > (GameManager.Instance.daysUntilNextHarvest + 3)){ //ABUNDANT
//					deficit = (daysFoodSupplyLasts - (GameManager.Instance.daysUntilNextHarvest + 3)) * GetDailyFoodConsumption ();
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
//				}else if (daysFoodSupplyLasts < GameManager.Instance.daysUntilNextHarvest){ //SCARCE
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
//				}else{ //NORMAL
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
//				}
//
//				if (deficit < 0) {
//					deficit = deficit * -1;
//				}
//				this.allResourcesStatus[i].amount = deficit;
//			}else{
//				int excess = GetNumberOfResourcesPerType (this.allResourcesStatus [i].resource) - neededResources [i];
//
//				if(excess > 0){ //ABUNDANT
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.ABUNDANT;
//				}else if (excess < 0){ //SCARCE
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.SCARCE;
//				}else{  //NORMAL
//					this.allResourcesStatus[i].status = RESOURCE_STATUS.NORMAL;
//				}
//				if (excess < 0) {
//					excess = excess * -1;
//				}
//				this.allResourcesStatus [i].amount = excess;
//			}
//		}
//	}

	//Get Daily Production Based On Resource Type
	internal int GetAveDailyProduction(RESOURCE resourceType, List<Citizen> citizensConcerned){
		int totalDailyProduction = 0;
		for (int i = 0; i < citizensConcerned.Count; i++) {
			totalDailyProduction += citizensConcerned[i].GetAveDailyProduction(resourceType);
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

	internal void AdjustResourceCount(RESOURCE resourceType, int amount){
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
//
//	internal void BattleGenerals(General enemyGeneral){
//		General victoriousGeneral = null;
//		General friendlyGeneral = null;
//		General visitingGeneral = enemyGeneral;
//
//		List<General> helpingGenerals = this.visitingGenerals.Where (x => x.task == GENERAL_TASK.ON_HELP && x.location == this.hexTile).ToList ();
//
//		if (!visitingGeneral.city.kingdomTile.kingdom.lord.CheckForSpecificWar (this.kingdomTile.kingdom.lord)) {
//			visitingGeneral.task = GENERAL_TASK.NONE;
//			visitingGeneral.taskID = 0;
//		}else{
//
//			if (helpingGenerals.Count > 0) {
//
//				List<General> newHelpingGenerals = new List<General> (helpingGenerals);
//				GoToBattle (ref newHelpingGenerals, ref visitingGeneral);
//				enemyGeneral = visitingGeneral;
//				helpingGenerals = newHelpingGenerals;
//
//				if (helpingGenerals.Count <= 0 && enemyGeneral.army.armyCount > 0) {
//					List<General> yourGenerals = this.generals.Where (x => x.location == this.hexTile).ToList ();
//
//					if (yourGenerals.Count > 0) {
//
//						List<General> newYourGenerals = new List<General> (yourGenerals);
//						GoToBattle (ref newYourGenerals, ref visitingGeneral);
//						enemyGeneral = visitingGeneral;
//						yourGenerals = newYourGenerals;
//
//						if (yourGenerals.Count <= 0 && enemyGeneral.army.armyCount > 0) {
//							//CITY DEFEATED AND CONQUERED --- IF ENEMY IS STILL ALIVE, AND YOU HAVE NO GENERALS LEFT
//							victoriousGeneral = enemyGeneral;
//							Debug.Log (this.cityName + " IS DEFEATED BY " + victoriousGeneral.city.cityName);
//						}
//					} else {
//						//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
//						if (victoriousGeneral == null) {
//							victoriousGeneral = enemyGeneral;
//						} else {
//							friendlyGeneral = victoriousGeneral;
//							if (this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar(friendlyGeneral.city.kingdomTile.kingdom.lord)) {
//								Debug.Log ("CITY IS FOR TAKING! NO MORE GENERALS! BATTLE FOR OWNERSHIP!");
//								Battle (ref visitingGeneral, ref friendlyGeneral);
//								this.visitingGenerals [i] = visitingGeneral;
//								if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
//									victoriousGeneral = null;
//								} else if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
//									victoriousGeneral = friendlyGeneral;
//								} else if (this.visitingGenerals [i].army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
//									victoriousGeneral = this.visitingGenerals [i];
//								}
//								Debug.Log ("WINNER: " + victoriousGeneral.city.cityName);
//							}
//						}
//					}
//				}
//			} else {
//				//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
//				List<General> yourGenerals = this.generals.Where (x => x.location == this.hexTile).ToList ();
//				if (yourGenerals.Count > 0) {
//
//					List<General> newYourGenerals = new List<General> (yourGenerals);
//					GoToBattle (ref newYourGenerals, ref visitingGeneral);
//					this.visitingGenerals [i] = visitingGeneral;
//					yourGenerals = newYourGenerals;
//
//					if (yourGenerals.Count <= 0 && this.visitingGenerals [i].army.armyCount > 0) {
//						//CITY DEFEATED AND CONQUERED --- IF ENEMY IS STILL ALIVE, AND YOU HAVE NO GENERALS LEFT
//						victoriousGeneral = this.visitingGenerals [i];
//						Debug.Log (this.cityName + " IS DEFEATED BY " + victoriousGeneral.city.cityName);
//					}
//				} else {
//					//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
//					if (victoriousGeneral == null) {
//						victoriousGeneral = this.visitingGenerals [i];
//					} else {
//						friendlyGeneral = victoriousGeneral;
//						if (this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (friendlyGeneral.city.kingdomTile.kingdom.lord)) {
//							Debug.Log ("CITY IS FOR TAKING! NO MORE GENERALS! BATTLE FOR OWNERSHIP!");
//
//							Battle (ref visitingGeneral, ref friendlyGeneral);
//							this.visitingGenerals [i] = visitingGeneral;
//
//							if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
//								victoriousGeneral = null;
//							} else if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
//								victoriousGeneral = friendlyGeneral;
//							} else if (this.visitingGenerals [i].army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
//								victoriousGeneral = this.visitingGenerals [i];
//							}
//							Debug.Log ("WINNER: " + victoriousGeneral.city.cityName);
//						}
//					}
//				}
//			}
//		}
//
//		for(int i = 0; i < this.visitingGenerals.Count; i++){
//			if (this.visitingGenerals [i].army.armyCount <= 0) {
//				this.visitingGenerals [i].city.kingdomTile.kingdom.lord.UpdateSpecificOffenseMilitaryData (this.visitingGenerals [i].taskID);
//				this.visitingGenerals [i].city.generals.Remove (this.visitingGenerals [i]);
//			}
//		}
//
//		this.visitingGenerals.RemoveAll (x => x.army.armyCount <= 0);
//
//		if(victoriousGeneral != null){
//			//CONQUER CITY
//			Debug.Log (this.cityName + " IS CONQUERED BY " + victoriousGeneral.city.cityName);
//			victoriousGeneral.city.kingdomTile.kingdom.lord.RemoveMilitaryData (BATTLE_MOVE.ATTACK, this, null);
//			this.kingdomTile.kingdom.lord.RemoveMilitaryData (BATTLE_MOVE.DEFEND, null, victoriousGeneral);
//			victoriousGeneral.taskID = 0;
//			victoriousGeneral.task = GENERAL_TASK.NONE;
//			victoriousGeneral.targetCity = null;
//			this.generals.Clear ();
//			ConquerCity (victoriousGeneral.city.kingdomTile, this.visitingGenerals);
//		}
//	}
	internal void CheckVisitingGenerals(){
		this.visitingGenerals.RemoveAll (x => x.targetLocation != this.hexTile && x.location != this.hexTile);

		if(this.visitingGenerals.Count <= 0){
			return;
		}
		bool hasBattled = false;
		General victoriousGeneral = null;
		General friendlyGeneral = null;
		General visitingGeneral = null;

		List<General> helpingGenerals = this.visitingGenerals.Where (x => x.task == GENERAL_TASK.ON_HELP && x.location == this.hexTile).ToList ();
		
		for(int i = 0; i < this.visitingGenerals.Count; i++){
			visitingGeneral = this.visitingGenerals [i];
			if(this.visitingGenerals[i].location == this.hexTile){
				if(this.visitingGenerals[i].task == GENERAL_TASK.ON_ATTACK){
					if (!this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (this.kingdomTile.kingdom.lord)) {
						this.visitingGenerals [i].task = GENERAL_TASK.NONE;
					}else{
						hasBattled = true;
						if (helpingGenerals.Count > 0) {
							List<General> newHelpingGenerals = new List<General> (helpingGenerals);
							GoToBattle (ref newHelpingGenerals, ref visitingGeneral);
							this.visitingGenerals [i] = visitingGeneral;
							helpingGenerals = newHelpingGenerals;

							if (helpingGenerals.Count <= 0 && this.visitingGenerals [i].army.armyCount > 0) {
								List<General> yourGenerals = this.generals.Where (x => x.location == this.hexTile).ToList ();

								if (yourGenerals.Count > 0) {
									
									List<General> newYourGenerals = new List<General> (yourGenerals);
									GoToBattle (ref newYourGenerals, ref visitingGeneral);
									this.visitingGenerals [i] = visitingGeneral;
									yourGenerals = newYourGenerals;

									if (yourGenerals.Count <= 0 && this.visitingGenerals [i].army.armyCount > 0) {
										//CITY DEFEATED AND CONQUERED --- IF ENEMY IS STILL ALIVE, AND YOU HAVE NO GENERALS LEFT
										victoriousGeneral = this.visitingGenerals [i];
										Debug.Log (this.cityName + " IS DEFEATED BY " + victoriousGeneral.city.cityName);
									}
								} else {
									//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
									if (victoriousGeneral == null) {
										victoriousGeneral = this.visitingGenerals [i];
									} else {
										friendlyGeneral = victoriousGeneral;
										if (this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar(friendlyGeneral.city.kingdomTile.kingdom.lord)) {
											Debug.Log ("CITY IS FOR TAKING! NO MORE GENERALS! BATTLE FOR OWNERSHIP!");
											Battle (ref visitingGeneral, ref friendlyGeneral);
											this.visitingGenerals [i] = visitingGeneral;
											if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
												victoriousGeneral = null;
											} else if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
												victoriousGeneral = friendlyGeneral;
											} else if (this.visitingGenerals [i].army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
												victoriousGeneral = this.visitingGenerals [i];
											}
											Debug.Log ("WINNER: " + victoriousGeneral.city.cityName);
										}
									}
								}
							}
						} else {
							//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
							List<General> yourGenerals = this.generals.Where (x => x.location == this.hexTile).ToList ();
							if (yourGenerals.Count > 0) {

								List<General> newYourGenerals = new List<General> (yourGenerals);
								GoToBattle (ref newYourGenerals, ref visitingGeneral);
								this.visitingGenerals [i] = visitingGeneral;
								yourGenerals = newYourGenerals;

								if (yourGenerals.Count <= 0 && this.visitingGenerals [i].army.armyCount > 0) {
									//CITY DEFEATED AND CONQUERED --- IF ENEMY IS STILL ALIVE, AND YOU HAVE NO GENERALS LEFT
									victoriousGeneral = this.visitingGenerals [i];
									Debug.Log (this.cityName + " IS DEFEATED BY " + victoriousGeneral.city.cityName);
								}
							} else {
								//CITY DEFEATED AND CONQUERED --- IF BOTH YOU AND YOUR ENEMY ARE DEAD FROM LAST ENCOUNTER, THIS ENEMY WILL PICK UP THE WIN BECAUSE YOU HAVE NO DEFENSE LEFT.
								if (victoriousGeneral == null) {
									victoriousGeneral = this.visitingGenerals [i];
								} else {
									friendlyGeneral = victoriousGeneral;
									if (this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (friendlyGeneral.city.kingdomTile.kingdom.lord)) {
										Debug.Log ("CITY IS FOR TAKING! NO MORE GENERALS! BATTLE FOR OWNERSHIP!");

										Battle (ref visitingGeneral, ref friendlyGeneral);
										this.visitingGenerals [i] = visitingGeneral;

										if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount <= 0) {
											victoriousGeneral = null;
										} else if (this.visitingGenerals [i].army.armyCount <= 0 && friendlyGeneral.army.armyCount > 0) {
											victoriousGeneral = friendlyGeneral;
										} else if (this.visitingGenerals [i].army.armyCount > 0 && friendlyGeneral.army.armyCount <= 0) {
											victoriousGeneral = this.visitingGenerals [i];
										}
										Debug.Log ("WINNER: " + victoriousGeneral.city.cityName);
									}
								}
							}
						}
					}
				}
			}
		}
		for(int i = 0; i < this.visitingGenerals.Count; i++){
			if (this.visitingGenerals [i].army.armyCount <= 0) {
				this.visitingGenerals [i].city.kingdomTile.kingdom.lord.UpdateSpecificOffenseMilitaryData (this.visitingGenerals [i].taskID);
				this.visitingGenerals [i].city.generals.Remove (this.visitingGenerals [i]);
			}
		}

		this.visitingGenerals.RemoveAll (x => x.army.armyCount <= 0);

		if(victoriousGeneral != null){
			//CONQUER CITY
			Debug.Log (this.cityName + " IS CONQUERED BY " + victoriousGeneral.city.cityName);
			victoriousGeneral.city.kingdomTile.kingdom.lord.RemoveMilitaryData (BATTLE_MOVE.ATTACK, this, null);
			this.kingdomTile.kingdom.lord.RemoveMilitaryData (BATTLE_MOVE.DEFEND, null, victoriousGeneral);
			victoriousGeneral.taskID = 0;
			victoriousGeneral.task = GENERAL_TASK.NONE;
			victoriousGeneral.targetCity = null;
			this.generals.Clear ();
			this.isDead = true;
			ConquerCity (victoriousGeneral.city.kingdomTile, this.visitingGenerals);
		}else{
			if (hasBattled) {
				this.kingdomTile.kingdom.lord.UpdateMilitaryData ();
			}
		}

//		GameManager.Instance.UpdateAllMilitaryData ();
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
//				if (this.visitingGenerals [i].task == GENERAL_TASK.ON_HELP) {
//					this.generals.Add (this.visitingGenerals [i]);
//					this.visitingGenerals.RemoveAt (i);
//					i--;
//				}
//			}
//		}
//		for(int i = 0; i < this.visitingGenerals.Count; i++){
//			if (this.visitingGenerals [i].location.name == this.hexTile.name) {
//				if (this.visitingGenerals [i].task == GENERAL_TASK.ON_ATTACK) {
//					if (!this.visitingGenerals [i].city.kingdomTile.kingdom.lord.CheckForSpecificWar (this.kingdomTile.kingdom.lord)) {
////						this.visitingGenerals [i].onAttack = false;
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
////						this.visitingGenerals [i].onAttack = false;
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
	internal void GoToBattle(ref List<General> friendlyGenerals, ref General enemyGeneral){
		List<General> deadFriendlies = new List<General> ();

		General friendlyGeneral = null;
			
		List<General> generalOrderedByStrength = friendlyGenerals.OrderByDescending (x => x.ArmyStrength ()).ToList();

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
		if(deadFriendlies.Count > 0){
			for (int j = 0; j < deadFriendlies.Count; j++) {
				friendlyGenerals.Remove (deadFriendlies [j]);
			}
		}
//		return this.generals;
	}
	internal void ConquerCity(KingdomTileTest conqueror, List<General> visitingGenerals){

		conqueror.kingdom.AddCityToKingdom (hexTile.GetCityTileTest());
		this.kingdomTile.kingdom.cities.Remove (hexTile.GetCityTileTest());

		hexTile.GetCityTileTest ().DevelopNewCity (conqueror, visitingGenerals);

	}
	internal void CheckMultipleArrivals(){
		
	}
	internal void Battle(ref General general1, ref General general2){
		Debug.Log ("BATTLE: " + general1.name + " and " + general2.name);
		Debug.Log ("enemy general army: " + general1.ArmyStrength ());
		Debug.Log ("friendly general army: " + general2.ArmyStrength ());

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
		Debug.Log ("RESULTS: " + general1.name + " army count left: " + general1.army.armyCount + "\n" + general2.name + " army count left: " + general2.army.armyCount);
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
		if(this.generals.Count > 0){
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
		}
		if(this.visitingGenerals.Count > 0){
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
		}

		return strength;
	}
	#endregion
	internal void TriggerCityDefense(General enemyGeneral){
		MilitaryData mildata = new MilitaryData (null, enemyGeneral, 0, BATTLE_MOVE.DEFEND);

		int strength = 0;

		if(this.generals.Count > 0){
			this.generals = this.generals.OrderByDescending (x => x.ArmyStrength ()).ToList();
			for(int i = 0; i < this.generals.Count; i++){
				if(this.generals[i].location == this.hexTile){
					if(this.generals[i].task == GENERAL_TASK.NONE){
						strength += this.generals [i].ArmyStrength ();
						this.generals [i].taskID = mildata.id;
						this.generals [i].task = GENERAL_TASK.ON_DEFEND;
						this.generals [i].daysBeforeReleaseTask = enemyGeneral.daysBeforeArrival + 1;
						this.generals [i].targetCity = null;
						this.generals [i].targetLocation = this.hexTile;
						this.generals [i].isOnTheWay = true;
						Debug.Log ("DEFEND: " + this.cityName + ": " + this.generals [i].name);
						if(strength >= enemyGeneral.ArmyStrength()){
							break;
						}
					}
				}
			}
			if(strength < enemyGeneral.ArmyStrength()){
				for(int i = 0; i < this.generals.Count; i++){
					if(this.generals[i].location != this.hexTile){
						if(this.generals[i].task == GENERAL_TASK.NONE){
							//Check if distance will suffice the needed days
							List<Tile> roads = new List<Tile>(GameManager.Instance.GetPath(this.generals[i].location.tile, this.hexTile.tile, false).Reverse().ToList());
							roads.RemoveAt (0);
							int distance = roads.Count;

							if(distance <= enemyGeneral.daysBeforeArrival){
								strength += this.generals [i].ArmyStrength ();
								this.generals [i].taskID = mildata.id;
								this.generals [i].task = GENERAL_TASK.ON_DEFEND;
								this.generals [i].daysBeforeReleaseTask = enemyGeneral.daysBeforeArrival + 1;
								this.generals [i].targetCity = null;
								this.generals [i].targetLocation = this.hexTile;
								this.generals [i].daysBeforeArrival = distance;
								this.generals [i].roads.Clear ();
								this.generals [i].roads = roads;
								this.generals [i].isOnTheWay = true;
								if(strength >= enemyGeneral.ArmyStrength()){
									break;
								}
							}
						}
					}
				}		
			}
		}


		this.kingdomTile.kingdom.lord.militaryData.Add(mildata);
		this.kingdomTile.kingdom.lord.UpdateMilitaryData ();
	}
	internal void GiveMeTask(General general){
		General askingGeneral = SearchGeneralByID (general.id);
		if(askingGeneral == null){
			return;
		}
		MilitaryData milDataForDef = this.kingdomTile.kingdom.lord.SearchForDefenseMilitaryData (this);
		if(milDataForDef != null){
			if (milDataForDef.enemyGeneral.ArmyStrength () > milDataForDef.enemyGeneral.targetCity.GetArmyStrength ()) {
//				int neededStrength = milDataForDef.enemyGeneral.ArmyStrength () - milDataForDef.enemyGeneral.targetCity.GetArmyStrength ();
				if(general.location != this.hexTile){
					List<Tile> roads = new List<Tile> (GameManager.Instance.GetPath (general.location.tile, this.hexTile.tile, false).Reverse ().ToList ());
					roads.RemoveAt (0);
					int distance = roads.Count;

					if (distance <= milDataForDef.enemyGeneral.daysBeforeArrival) {
						askingGeneral.taskID = milDataForDef.id;
						askingGeneral.task = GENERAL_TASK.ON_DEFEND;
						askingGeneral.daysBeforeReleaseTask = milDataForDef.enemyGeneral.daysBeforeArrival + 1;
						askingGeneral.targetCity = null;
						askingGeneral.targetLocation = this.hexTile;
						askingGeneral.daysBeforeArrival = distance;
						askingGeneral.roads.Clear ();
						askingGeneral.roads = roads;
						askingGeneral.isOnTheWay = true;
					}
				}else{
					askingGeneral.taskID = milDataForDef.id;
					askingGeneral.task = GENERAL_TASK.ON_DEFEND;
					askingGeneral.daysBeforeReleaseTask = milDataForDef.enemyGeneral.daysBeforeArrival + 1;
					askingGeneral.targetCity = null;
					askingGeneral.targetLocation = this.hexTile;
					askingGeneral.isOnTheWay = true;
				}

			}

//			if(milDataForDef.enemyGeneral.targetCity.GetArmyStrength() >= milDataForDef.enemyGeneral.ArmyStrength()){
//				milDataForDef.isResolved = true;
//			}
		}else{
			if(this.kingdomTile.kingdom.lord.militaryData.Count > 0){
				for(int i = 0; i < this.kingdomTile.kingdom.lord.militaryData.Count; i++){
//					if(!this.kingdomTile.kingdom.lord.militaryData [i].isResolved){
						MilitaryData milData = this.kingdomTile.kingdom.lord.militaryData [i];
						ProcessTask (ref milData, ref askingGeneral);
						if(askingGeneral.task == GENERAL_TASK.NONE){
							continue;
						}else{
							break;
						}
//					}
				}
			}
		}
	}
	internal void ProcessTask(ref MilitaryData militaryData, ref General general){
		if (militaryData.battleMove == BATTLE_MOVE.ATTACK) {
			if(this.targetCity == null){
				this.targetCity = militaryData.enemyCity;
			}
			if(this.targetCity == militaryData.enemyCity){
				if(militaryData.yourArmyStrength < militaryData.enemyCity.GetArmyStrength()){
					List<Tile> roads = new List<Tile>(GameManager.Instance.GetPath(general.location.tile, militaryData.enemyCity.hexTile.tile, false).Reverse().ToList());
					roads.RemoveAt (0);
					general.taskID = militaryData.id;
					general.targetCity = militaryData.enemyCity;
					general.targetLocation = militaryData.enemyCity.hexTile;
					general.daysBeforeArrival = roads.Count;
					general.task = GENERAL_TASK.ON_ATTACK;
					general.roads.Clear ();
					general.roads = roads;
					general.isOnTheWay = true;
					militaryData.enemyCity.visitingGenerals.Add (general);
					militaryData.enemyCity.TriggerCityDefense (general);
					militaryData.yourArmyStrength += general.ArmyStrength ();
				}
			}
//			else{
//				if(general.location != this.hexTile){
//					List<Tile> roads = new List<Tile>(GameManager.Instance.GetPath(general.location.tile, this.hexTile.tile, false).Reverse().ToList());
//					roads.RemoveAt (0);
//					int distance = roads.Count;
//					general.daysBeforeArrival = distance;
//					general.task = GENERAL_TASK.ON_DEFEND;
//					general.daysBeforeReleaseTask = general.daysBeforeArrival;
//					general.targetCity = null;
//					general.targetLocation = this.hexTile;
//					general.roads.Clear ();
//					general.roads = roads;
//				}
//			}
		}else{
			if(militaryData.enemyGeneral.ArmyStrength() > militaryData.enemyGeneral.targetCity.GetArmyStrength()){
//				int neededStrength = militaryData.enemyGeneral.ArmyStrength () - militaryData.enemyGeneral.targetCity.GetArmyStrength ();
				List<Tile> roads = new List<Tile>(GameManager.Instance.GetPath(general.location.tile,  militaryData.enemyGeneral.targetCity.hexTile.tile, false).Reverse().ToList());
				roads.RemoveAt (0);
				int distance = roads.Count;

				if (distance <= militaryData.enemyGeneral.daysBeforeArrival) {
					general.taskID = militaryData.id;
					general.task = GENERAL_TASK.ON_HELP;
					general.targetCity = null;
					general.targetLocation = militaryData.enemyGeneral.targetCity.hexTile;
					general.daysBeforeArrival = distance;
					general.roads.Clear ();
					general.roads = roads;
					general.isOnTheWay = true;
					militaryData.enemyGeneral.targetCity.visitingGenerals.Add (general);
				}

//				if(militaryData.enemyGeneral.targetCity.GetArmyStrength() >= militaryData.enemyGeneral.ArmyStrength()){
//					militaryData.isResolved = true;
//				}
			}
		}


	}
	internal General SearchGeneralByID(int id){
		if(this.generals.Count > 0){
			for(int i = 0; i < this.generals.Count; i++){
				if(this.generals[i].id == id){
					return this.generals [i];
				}
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

#region Old Create Citizen Functions
//internal void SelectCitizenForCreation(){
//	if(isDead){
//		return;
//	}
//	int[] neededResources = GetNeededResources ();
//	List<JobNeeds> neededJobs = GetListJobsTarget (neededResources);
//
//	//		for (int i = 0; i < neededJobs.Count; i++) {
//	//			List<Resource> citizenCreationCost = GetCitizenCreationCostPerType(neededJobs[i].jobType);
//	//			for (int j = 0; j < citizenCreationCost.Count; j++) {
//	//				if (citizenCreationCost[j].resourceType != RESOURCE.GOLD && GetNumberOfResourcesPerType(citizenCreationCost[j].resourceType) < citizenCreationCost[j].resourceQuantity) {
//	//					if (GetAveDailyProduction(citizenCreationCost [j].resourceType) <= 0) {
//	//						neededJobs.Remove(neededJobs[i]);
//	//					}
//	//				}
//	//			}
//	//		}
//
//	if(neededJobs.Count > 0){
//		int chance = UnityEngine.Random.Range (0, GetTotalChance (neededJobs.Count) + 1);
//		int upperBound = 0;
//		int lowerBound = 0;
//		for(int i = 0; i < neededJobs.Count; i++){
//			if(i == 0){
//				upperBound += 100;
//			}else if(i == 1){
//				upperBound += 60;
//			}else if(i == 2){
//				upperBound += 20;
//			}else{
//				upperBound += 5;
//			}
//
//			if(chance >= lowerBound && chance < upperBound){
//				this.newCitizenTarget = neededJobs[i].jobType;
//				//					cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected job to be created: [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
//				break;
//			}else{
//				lowerBound = upperBound;
//			}
//		}
//	} else{ //WARRIOR
//		//			if(GetNumberOfCitizensPerType (JOB_TYPE.DEFENSE_GENERAL) == 0){
//		//				this.newCitizenTarget = JOB_TYPE.DEFENSE_GENERAL;
//		//			}
//		//			this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//		//			if (GetNumberOfCitizensPerType (JOB_TYPE.OFFENSE_GENERAL) >= this.offenseGeneralsLimit) {
//		//				if (this.unoccupiedOwnedTiles.Count > 0) {
//		//					List<HexTile> tilesByHighestResource = unoccupiedOwnedTiles.OrderByDescending (x => x.GetHighestResourceValue ()).ToList ();
//		//					this.newCitizenTarget = tilesByHighestResource [0].GetBestJobForTile ();
//		//				} else {
//		//					this.newCitizenTarget = JOB_TYPE.NONE;
//		//				}
//		//			} else {
//		//				this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//		//			}
//		//			cityLogs += GameManager.Instance.currentDay.ToString() + ": Selected job to be created: [FF0000]" + this.newCitizenTarget.ToString() + "[-]\n\n"; 
//		//			if (GetNumberOfCitizensPerType (JOB_TYPE.DEFENSE_GENERAL) == 0) {
//		//				this.newCitizenTarget = JOB_TYPE.DEFENSE_GENERAL;
//		//			} else {
//		//				this.newCitizenTarget = JOB_TYPE.OFFENSE_GENERAL;
//		//			}
//	}
//}

//internal int GetTotalChance(int jobCount){
//	int totalChances = 0;
//	for (int i = 0; i < jobCount; i++) {
//		if(i == 0){
//			totalChances += 100;
//		}else if(i == 1){
//			totalChances += 60;
//		}else if(i == 2){
//			totalChances += 20;
//		}else{
//			totalChances += 5;
//		}
//	}
//	return totalChances;
//}

//private List<JobNeeds> GetListJobsTarget(int[] neededResources){ //gold, food, lumber, stone, manastone, metal
//
//	int resourceDeficit = 0;
//	RESOURCE[] allResources = (RESOURCE[]) Enum.GetValues (typeof(RESOURCE));
//	List<JobNeeds> neededJobs = new List<JobNeeds> ();
//	for (int i = 0; i < allResources.Length; i++) {
//		if (allResources [i] == RESOURCE.FOOD) {
//			continue;
//		}
//
//		resourceDeficit = neededResources [i] - GetNumberOfResourcesPerType(allResources[i]);
//
//		if(resourceDeficit > 0){
//			for(int j = 0; j < Lookup.JOB_REF.Length; j++){
//				if(Lookup.GetJobInfo(j).resourcesProduced == null){
//					continue;
//				}
//				for(int k = 0; k < Lookup.GetJobInfo(j).resourcesProduced.Length; k++){
//					if(Lookup.GetJobInfo(j).resourcesProduced[k] == allResources[i]){
//						int deficit = 0;
//						if(Lookup.GetJobInfo(j).GetDailyProduction(allResources[i]) > 0){
//							deficit =  (int)(resourceDeficit / Lookup.GetJobInfo(j).GetDailyProduction(allResources[i]));
//						}
//
//						neededJobs.Add (new JobNeeds(Lookup.GetJobInfo(j).jobType, new Resource(allResources[i],deficit)));
//						break;
//					}
//				}
//			}
//		}
//	}
//	neededJobs = neededJobs.OrderBy (x => x.resource.resourceQuantity).ToList ();
//	//		neededJobs = neededJobs.Distinct ().ToList ();
//
//	return neededJobs;
//}

//internal void AttemptToCreateNewCitizen(){
//	JOB_TYPE citizenToCreateJobType = JOB_TYPE.NONE;
//
//	if (this.neededRole != JOB_TYPE.NONE && HasEnoughResourcesForAction(GetCitizenCreationCostPerType(this.neededRole))) {
//		citizenToCreateJobType = this.neededRole;
//	} else {
//		citizenToCreateJobType = this.newCitizenTarget;
//	}
//
//	List<Resource> citizenCreationCost = GetCitizenCreationCostPerType (citizenToCreateJobType);
//	if (citizenCreationCost == null || citizenCreationCost.Count <= 0) {
//		return;
//	}
//
//	if(!IsCitizenCapReached() && HasEnoughResourcesForAction(citizenCreationCost) && HasTileForNewCitizen(citizenToCreateJobType)){
//		int chance = UnityEngine.Random.Range(0,100);
//		if (chance < this.cityActionChances.newCitizenChance) {
//
//			this.cityActionChances.newCitizenChance = this.cityActionChances.defaultNewCitizenChance;
//			Citizen newCitizen = new Citizen (citizenToCreateJobType, this);
//			AssignCitizenToTile (newCitizen);
//			this.citizens.Add (newCitizen);
//			cityLogs += GameManager.Instance.currentDay.ToString() + ": A new [FF0000]" + citizenToCreateJobType.ToString() + "[-] was created in exchange for ";
//			for (int i = 0; i < citizenCreationCost.Count; i++) {
//				cityLogs += citizenCreationCost[i].resourceQuantity + " " + citizenCreationCost[i].resourceType.ToString() + "\n";
//			}
//			cityLogs += "\n";
//
//			if(this.neededRole == citizenToCreateJobType){
//				this.neededRole = JOB_TYPE.NONE;
//			}
//
//			if(this.newCitizenTarget == citizenToCreateJobType){
//				this.newCitizenTarget = JOB_TYPE.NONE;
//				SelectCitizenForCreation();
//			}
//
//			ReduceResources(citizenCreationCost);
//			//				UpdateResourcesStatus();
//			this.unneededResources = GetUnneededResources (GetNeededResources ());
//			this.unneededRoles = GetUnneededRoles ();
//
//			if (citizenToCreateJobType == JOB_TYPE.PIONEER) {
//				//TODO: Change Distance Value To Pathfinding instead of Vector2.Distance
//				pioneerCityTarget = kingdomTile.kingdom.NearestUnoccupiedCity();
//				if (pioneerCityTarget == null) {
//					for (int i = 0; i < citizens.Count; i++) {
//						if (citizens[i].job.jobType == JOB_TYPE.PIONEER) {
//							citizens.Remove(citizens[i]);
//						}
//					}
//					return;
//				}
//				dayPioneerReachesCity = GameManager.Instance.currentDay + (int)Vector2.Distance(kingdomTile.kingdom.cities[0].transform.position, 
//					pioneerCityTarget.hexTile.transform.position);
//				GameManager.Instance.turnEnded += SendPioneer;
//				cityLogs += GameManager.Instance.currentDay.ToString() + ": Pioneer will reach city: [FF0000]" + pioneerCityTarget.hexTile.name + "[-] on day [FF0000]" + dayPioneerReachesCity.ToString() + "[-]\n\n";
//			}
//		} else {
//			this.cityActionChances.newCitizenChance += 1;
//		}
//
//	}
//}
#endregion