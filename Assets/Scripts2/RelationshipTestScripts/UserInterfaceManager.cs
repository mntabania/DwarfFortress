using UnityEngine;
using System.Collections;

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
	public UILabel lblLumberCount;
	public UILabel lblStoneCount;
	public UILabel lblManaStoneCount;
	public UILabel lblCitySummary;

	public UILabel lblPause;

	CityTileTest currentDisplayingCityTile;

	void Awake () {
		Instance = this;
	}

	public void SetCityInfoToShow(CityTileTest cityTile){
		lblUpgradeCitizenCost.text = "";
		lblUpgradeCityCost.text = "";
		currentDisplayingCityTile = cityTile;
		lblCityName.text = "Name: " + cityTile.cityAttributes.cityName;
		lblCityLevel.text = "Lvl: " + cityTile.cityAttributes.cityLevel.ToString();
		if (cityTile.cityAttributes.upgradeCitizenTarget != null) {
			lblUpgradeCitizenTarget.text = "Upgrade: " + cityTile.cityAttributes.upgradeCitizenTarget.job.jobType.ToString ();
		
			for (int i = 0; i < cityTile.cityAttributes.upgradeCitizenTarget.GetUpgradeRequirements ().resource.Count; i++) {
				Resource currentResource = cityTile.cityAttributes.upgradeCitizenTarget.GetUpgradeRequirements ().resource [i];
				lblUpgradeCitizenCost.text += currentResource.resourceType.ToString () + ": " + currentResource.resourceQuantity.ToString ();
			}
		}

		lblCreateCitizenTarget.text = "Create: " + cityTile.cityAttributes.newCitizenTarget.ToString();

		for (int i = 0; i < cityTile.cityAttributes.cityUpgradeRequirements.resource.Count; i++) {
			Resource currentResource = cityTile.cityAttributes.cityUpgradeRequirements.resource [i];
			lblUpgradeCityCost.text += currentResource.resourceType.ToString () + ": " + currentResource.resourceQuantity.ToString() +"\n";
		}

		lblGoldCount.text = "Gold: " + cityTile.cityAttributes.goldCount.ToString();
		lblFoodCount.text = "Food: " + cityTile.cityAttributes.foodCount.ToString();
		lblLumberCount.text = "Lumber: " + cityTile.cityAttributes.lumberCount.ToString();
		lblStoneCount.text = "Stone: " + cityTile.cityAttributes.stoneCount.ToString();
		lblManaStoneCount.text = "Mana Stone: " + cityTile.cityAttributes.manaStoneCount.ToString();
		if (cityTile.cityAttributes.kingdomTile) {
			lblKingdomName.text = "Kingdom: " + cityTile.cityAttributes.kingdomTile.kingdom.kingdomRace;
		} else {
			lblKingdomName.text = "Kingdom: None";
		}

		lblCitySummary.text = cityTile.cityAttributes.cityLogs;
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
	}

}
