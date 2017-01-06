using UnityEngine;
using System.Collections;

public class UserInterfaceManager : MonoBehaviour {

	public static UserInterfaceManager Instance = null;

	public UILabel lblCityName;
	public UILabel lblKingdomName;
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
		currentDisplayingCityTile = cityTile;
		lblCityName.text = "Name: " + cityTile.cityAttributes.cityName;
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
