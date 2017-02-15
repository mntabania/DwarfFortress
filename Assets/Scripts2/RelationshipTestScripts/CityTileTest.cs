using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityTileTest : MonoBehaviour {

	public HexTile hexTile;
	public CityTest cityAttributes; 

	public List<HexTile> cityTilesByDistance;

	public List<HexTile> GetAllCitiesByDistance(){
		List<HexTile> allCityTiles = CityGenerator.Instance.cities.OrderBy(
			x => Vector2.Distance(this.transform.position, x.transform.position) 
		).ToList();
		allCityTiles.Remove(cityAttributes.hexTile);
		cityTilesByDistance = allCityTiles;
		return allCityTiles;
	}

	public HexTile FindNearestCityWithConnection(){
		GetAllCitiesByDistance ();
		for (int i = 0; i < cityTilesByDistance.Count; i++) {
			if (cityTilesByDistance [i].GetCityTile ().cityAttributes.numOfRoads > 0) {
				return cityTilesByDistance [i];
			}
		}
		return cityTilesByDistance [0];
	}

	public void SetCityAsActiveAndSetProduction(){
		GameManager.Instance.turnEnded += TurnActions;
		cityAttributes.GenerateInitialFood();
	}

	public void TurnActions(int currentDay){
		cityAttributes.ProduceResources();
		cityAttributes.ConsumeFood(cityAttributes.GetDailyFoodConsumption());
//		cityAttributes.SelectCitizenToUpgrade ();
		cityAttributes.UpdateCityExpenses();
//		cityAttributes.AssignNeededRole ();
//		cityAttributes.AssignUnneededRoles ();
		cityAttributes.ArmyMaintenance ();
		cityAttributes.AttemptToPurchaseTile ();

		if (currentDay % 7 == 0) { 
//			cityAttributes.SelectCitizenForCreation();
			cityAttributes.SelectHexTileToPurchase();
		}

		cityAttributes.AttemptToUpgradeCity ();
//		cityAttributes.AttemptToUpgradeCitizen ();
//		cityAttributes.AttemptToCreateNewCitizen ();
//		cityAttributes.AttemptToChangeCitizenRole ();
		cityAttributes.AttemptToIncreaseArmyCount ();
		cityAttributes.LaunchTradeMission();
		cityAttributes.AttemptToCreatePioneer();
		cityAttributes.AttemptToPerformCitizenAction();
//		cityAttributes.UpdateResourcesStatus();
	}
}
