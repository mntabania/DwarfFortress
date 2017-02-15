using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {

	public static UserInterfaceManager Instance = null;

	public UILabel lblCityName;
	public UILabel lblKingdomName;
	public UILabel lblCityLevel;
	public UILabel lblUpgradeCitizenTarget;
	public UILabel lblUpgradeCitizenCost;
	public UILabel lblCreateCitizenTarget;
	public UILabel lblCreateCitizenCost;
	public UILabel lblUpgradeCityCost;
	public UILabel lblNumOfDays;
	public UILabel lblGoldCount;
	public UILabel lblFoodCount;
	public UILabel lblFoodStockPileCount;
	public UILabel lblLumberCount;
	public UILabel lblStoneCount;
	public UILabel lblManaStoneCount;
	public UILabel lblCitySummary;
	public UILabel lblExternalAffairs;
	public UILabel lblMetalCount;
	public UILabel lblUnrest;
	public UILabel lblCityCitizenAction;

	public UILabel lblFarmerCount;
	public UILabel lblHunterCount;
	public UILabel lblWoodsmanCount;
	public UILabel lblMinerCount;
	public UILabel lblAlchemistCount;
	public UILabel lblWarriorCount;
	public UILabel lblArcherCount;
	public UILabel lblMageCount;
	public UILabel lblQuarrymanCount;
	public UILabel lblBrawlerCount;

	public GameObject goCitizenInfo;
	public UILabel lblCitizenCap;
	public UILabel lblCitizenInfo;

	public UILabel lblPause;

	CityTileTest currentDisplayingCityTile;

	internal List<string> externalAffairsLogList = new List<string>(){string.Empty};
	public int currentIndex = 0;

	void Awake () {
		Instance = this;
	}

	public void SetCityInfoToShow(CityTileTest cityTile){
		lblUpgradeCitizenCost.text = "";
		lblUpgradeCityCost.text = "";
		lblCreateCitizenCost.text = "";
		currentDisplayingCityTile = cityTile;
		lblCityName.text = "Name: " + cityTile.cityAttributes.cityName;
		lblCityLevel.text = "Lvl: " + cityTile.cityAttributes.cityLevel.ToString();

//		lblUpgradeCitizenTarget.text = "Needed Role: " + cityTile.cityAttributes.neededRole.ToString ();
//		List<Resource> neededRoleCost = cityTile.cityAttributes.GetCitizenCreationCostPerType (cityTile.cityAttributes.neededRole);
//		if (neededRoleCost != null) {
//			for (int i = 0; i < neededRoleCost.Count; i++) {
//				lblUpgradeCitizenCost.text += neededRoleCost [i].resourceType.ToString () + ": " + neededRoleCost [i].resourceQuantity.ToString () + "\n";
//			}
//		}
//
//
//		if (cityTile.cityAttributes.newCitizenTarget != JOB_TYPE.NONE) {
//			lblCreateCitizenTarget.text = "Create: " + cityTile.cityAttributes.newCitizenTarget.ToString();
//		}
////		else {
////			lblCreateCitizenTarget.text = "Create: NONE";
////		}
//
//		List<Resource> createCitizenResources = cityTile.cityAttributes.GetCitizenCreationCostPerType (cityTile.cityAttributes.newCitizenTarget);
//		if(createCitizenResources != null){
//			for (int i = 0; i < createCitizenResources.Count; i++) {
//				lblCreateCitizenCost.text += createCitizenResources[i].resourceQuantity + " " + createCitizenResources[i].resourceType.ToString() + "\n";
//			}
//		}

		if (cityTile.cityAttributes.nextCityCitizenAction == CITY_CITIZEN_ACTION.CREATE_CITIZEN) {
			lblCityCitizenAction.text = "CREATE new " + cityTile.cityAttributes.citizenActionJobType.ToString () + " on tile "
			+ cityTile.cityAttributes.citizenActionHexTile.name;
		} else if (cityTile.cityAttributes.nextCityCitizenAction == CITY_CITIZEN_ACTION.CHANGE_CITIZEN) {
			lblCityCitizenAction.text = "CHANGE " + cityTile.cityAttributes.citizenToChange.name + "/" + cityTile.cityAttributes.citizenToChange.job.jobType.ToString () +
			" to " + cityTile.cityAttributes.citizenActionJobType.ToString ();
		} else {
			lblCityCitizenAction.text = "No Citizen Action";
		}



		for (int i = 0; i < cityTile.cityAttributes.cityUpgradeRequirements.resource.Count; i++) {
			Resource currentResource = cityTile.cityAttributes.cityUpgradeRequirements.resource [i];
			lblUpgradeCityCost.text += currentResource.resourceType.ToString () + ": " + currentResource.resourceQuantity.ToString() +"\n";
		}

		lblGoldCount.text = "Gold: " + cityTile.cityAttributes.goldCount.ToString();
		lblFoodCount.text = "Food: " + cityTile.cityAttributes.foodCount.ToString();
		lblFoodStockPileCount.text = "Stock: " + cityTile.cityAttributes.foodStockpileCount.ToString();
		lblLumberCount.text = "Lumber: " + cityTile.cityAttributes.lumberCount.ToString();
		lblStoneCount.text = "Stone: " + cityTile.cityAttributes.stoneCount.ToString();
		lblManaStoneCount.text = "Mana: " + cityTile.cityAttributes.manaStoneCount.ToString();
		lblMetalCount.text = "Metal: " + cityTile.cityAttributes.metalCount.ToString();
		if (cityTile.cityAttributes.kingdomTile) {
			lblKingdomName.text = "Kingdom: " + cityTile.cityAttributes.kingdomTile.kingdom.kingdomRace;
		} else {
			lblKingdomName.text = "Kingdom: None";
		}
			
		lblCitizenCap.text = cityTile.cityAttributes.citizens.Count.ToString () + "/" + cityTile.cityAttributes.citizenLimit.ToString();
		lblFarmerCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.FARMER).ToString();
		lblHunterCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.HUNTER).ToString();
		lblWoodsmanCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.WOODSMAN).ToString();
		lblMinerCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.MINER).ToString();
		lblAlchemistCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.ALCHEMIST).ToString();
		lblWarriorCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.DEFENSE_GENERAL).ToString();
		lblArcherCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.OFFENSE_GENERAL).ToString();
//		lblMageCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.MAGE).ToString();
		lblQuarrymanCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.QUARRYMAN).ToString();
//		lblBrawlerCount.text = cityTile.cityAttributes.GetNumberOfCitizensPerType(JOB_TYPE.BRAWLER).ToString();

		lblUnrest.text = "Unrest: " + cityTile.cityAttributes.unrest.ToString();
		lblCitySummary.text = cityTile.cityAttributes.cityLogs;
	}


	public void HoverOnCitizen(GameObject go){
		JOB_TYPE jobType = (JOB_TYPE) System.Enum.Parse (typeof(JOB_TYPE), go.name);
		lblCitizenInfo.text = jobType.ToString() + "'s: \n\n";
		for (int i = 0; i < currentDisplayingCityTile.cityAttributes.citizens.Count; i++) {
			Citizen currentCitizen = currentDisplayingCityTile.cityAttributes.citizens [i];
			if (currentCitizen.job.jobType == jobType) {
				lblCitizenInfo.text += "Name: " + currentCitizen.name + "\n";
				lblCitizenInfo.text += "Level: " + currentCitizen.level.ToString() + "\n";
				lblCitizenInfo.text += "Assigned Tile: " + currentCitizen.assignedTile.name + "\n";
				lblCitizenInfo.text += "Upgrade Reqs: ";
				for (int j = 0; j < currentCitizen.GetUpgradeRequirements().resource.Count; j++) {
					Resource currentResource = currentCitizen.GetUpgradeRequirements ().resource [j];
					lblCitizenInfo.text += currentResource.resourceType.ToString() + " - " + currentResource.resourceQuantity.ToString() + "\n";
				}
				lblCitizenInfo.text += "\n";
			}
		}
		goCitizenInfo.SetActive (true);
	}

	public void HoverOutCitizen(){
		goCitizenInfo.SetActive (false);
	}

	public void UpdateDayCounter(int numOfDays){
		lblNumOfDays.text = "Day #: " + numOfDays.ToString();
	}

	public void PauseDay(){
		GameManager.Instance.TogglePause();
		if (GameManager.Instance.isDayPaused) {
			lblPause.text = "Unpause";
		} else {
			lblPause.text = "Pause";
		}
	}

	void Update(){
		if (currentDisplayingCityTile != null) {
			SetCityInfoToShow (currentDisplayingCityTile);
		}

		lblExternalAffairs.text = externalAffairsLogList [currentIndex];
	}

	public void OnClickNextPage(){
		currentIndex++;
		if(currentIndex > (externalAffairsLogList.Count - 1)){
			currentIndex = externalAffairsLogList.Count - 1;
		}
		lblExternalAffairs.text = externalAffairsLogList [currentIndex];
	}

	public void OnClickPrevPage(){
		currentIndex--;
		if(currentIndex < 0){
			currentIndex = 0;
		}
		lblExternalAffairs.text = externalAffairsLogList [currentIndex];
	}
}
