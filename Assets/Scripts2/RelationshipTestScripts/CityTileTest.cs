using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityTileTest : MonoBehaviour {

	public JOB_TYPE citizenTypeToCreate;
	public RESOURCE resourceToAdd;
	public int resourceAmountToAdd;
	public int numOfTileToPurchase;
	public int citizenIndexToRemove;
	[Space(10)]
	public HexTile hexTile;
	public List<HexTile> cityTilesByDistance;
	[Space(10)]
	public CityTest cityAttributes; 

	public void DevelopNewCity(KingdomTileTest conqueror, List<General> visitingGenerals){
		this.cityAttributes = new CityTest (this.hexTile, conqueror);
		this.cityAttributes.OccupyCity();
		this.GetComponent<SpriteRenderer> ().color = this.cityAttributes.kingdomTile.kingdom.tileColor;
		this.cityAttributes.visitingGenerals.AddRange (visitingGenerals);
	}
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

	[ContextMenu("PurchaseTilesForTesting")]
	public void PurchaseTilesForTesting(){
		cityAttributes.PurchaseTilesForTesting(this.numOfTileToPurchase);
	}

	[ContextMenu("Force Update City Expenses")]
	public void ForceUpdateCityExpenses(){
		cityAttributes.UpdateCityExpenses();
		Debug.Log ("Updated City Expenses");
	}

	[ContextMenu("Compute Food Production")]
	public void ComputeFoodProduction(){
		Debug.Log(cityAttributes.GetAveDailyProduction(RESOURCE.FOOD, cityAttributes.citizens).ToString());
	}

	[ContextMenu("Add New Citizen")]
	public void ForceAddNewCitizen(){
		Citizen newCitizen = new Citizen (this.citizenTypeToCreate, this.cityAttributes);
		newCitizen.AssignCitizenToTile(this.cityAttributes.unoccupiedOwnedTiles);
		this.cityAttributes.citizens.Add(newCitizen);
		Debug.Log ("Created new Citizen: " + newCitizen.name);
		ForceUpdateCityExpenses();

	}

	[ContextMenu("Remove Citizen")]
	public void ForceRemoveCitizen(){
		Debug.Log ("Removed Citizen: " + this.cityAttributes.citizens.ElementAt (citizenIndexToRemove).name);
		this.cityAttributes.citizens.RemoveAt(citizenIndexToRemove);
		ForceUpdateCityExpenses();
	}

	[ContextMenu("Adjust Resource Count")]
	public void AddToResource(){
		this.cityAttributes.AdjustResourceCount (resourceToAdd, resourceAmountToAdd);
		Debug.Log ("Adjusted " + resourceToAdd.ToString () + " by " + resourceAmountToAdd);
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
		cityAttributes.AttemptToCreateNewGeneral();
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
		cityAttributes.CheckVisitingGenerals ();
//		cityAttributes.UpdateResourcesStatus();
	}
}
