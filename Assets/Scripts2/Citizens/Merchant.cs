using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Merchant : Job {

	internal CityTest targetCity;
	internal List<Tile> pathToTargetCity;
	internal List<Resource> tradeGoods;

	internal HexTile currentTile;
	protected int currentLocationIndex = 0;

	protected GameObject citizenAvatar;

	protected int daysWaitingForTradeGoods = 0;
	protected int numOfCitiesVisited = 0;

	protected int cityQuota = 3;

	public bool isOutsideCity;

	public Merchant(){
		this._jobType = JOB_TYPE.MERCHANT;
		this._residence = RESIDENCE.OUTSIDE;
		this.tradeGoods = new List<Resource>();
		this.pathToTargetCity = null;
		this.targetCity = null;
		this.isOutsideCity = false;
	}

	#region Trade Goods Acquisition
	internal void GetTradeGoods(){
		this.currentTile = this.citizen.city.hexTile;
		List<RESOURCE> abundantResources = this.citizen.city.GetAbundantResources();
		if (abundantResources.Count > 0) {
			for (int i = 0; i < abundantResources.Count; i++) {
				if (abundantResources [i] == RESOURCE.GOLD) {
					//exclude gold
					continue;
				}
				this.tradeGoods.Add (new Resource (abundantResources [i], this.citizen.city.GetResourceStatusByType(abundantResources[i]).amount));
				this.citizen.city.AdjustResourceCount(abundantResources [i], (this.citizen.city.GetResourceStatusByType (abundantResources [i]).amount * -1));
			}

		} 

		if (this.tradeGoods.Count > 0) {
			this.citizenAvatar = GameObject.Instantiate (Resources.Load ("CitizenAvatar", typeof(GameObject)), this.citizen.city.hexTile.transform) as GameObject;
			this.citizenAvatar.transform.localPosition = Vector3.zero;
			this.citizenAvatar.GetComponent<CitizenAvatar> ().citizen = this.citizen;
			this.ChooseTargetCity ();
		} else {
			GameManager.Instance.turnEnded += WaitForTradeGoods;
		}

	}

	internal void WaitForTradeGoods(int currentDay){
		daysWaitingForTradeGoods += 1;
		if (daysWaitingForTradeGoods == 5) {
			daysWaitingForTradeGoods = 0;
			GameManager.Instance.turnEnded -= WaitForTradeGoods;
			this.GetTradeGoods();
		}
	}
	#endregion

	void ChooseTargetCity(){
		List<CityTest> elligibleCitiesForTrade = new List<CityTest>();
		for (int i = 0; i < this.currentTile.GetCityTileTest().cityAttributes.connectedCities.Count; i++) {
			if (this.currentTile.GetCityTileTest().cityAttributes.connectedCities[i].cityAttributes.id == this.citizen.city.id || 
				!this.currentTile.GetCityTileTest().cityAttributes.connectedCities[i].cityAttributes.hexTile.isOccupied) {
				//Skip cities already in trade with or own city or unoccupiedCities
				continue;
			}
			CityTest currentConnectedCity = this.currentTile.GetCityTileTest().cityAttributes.connectedCities[i].cityAttributes;
			for (int j = 0; j < this.tradeGoods.Count; j++) {
				RESOURCE currentTradeGood = this.tradeGoods[j].resourceType;
				if (currentConnectedCity.GetResourceStatusByType(currentTradeGood).status == RESOURCE_STATUS.SCARCE) {
					if (!elligibleCitiesForTrade.Contains(currentConnectedCity)) {
						elligibleCitiesForTrade.Add(currentConnectedCity);
					}
				}
			}
		}
			
//		if (elligibleCitiesForTrade.Count > 0) {
//			this.targetCity = elligibleCitiesForTrade [UnityEngine.Random.Range(0, elligibleCitiesForTrade.Count)];
//			this.pathToTargetCity = GameManager.Instance.GetPath(this.citizen.city.hexTile.tile, this.targetCity.hexTile.tile, PATHFINDING_MODE.NORMAL).ToList();
//			this.pathToTargetCity.Reverse();
//
//			this.citizenAvatar = GameObject.Instantiate(Resources.Load ("CitizenAvatar", typeof(GameObject)), this.citizen.city.hexTile.transform) as GameObject;
//			this.citizenAvatar.transform.localPosition = Vector3.zero;
		if (elligibleCitiesForTrade.Count <= 0) {
			for (int i = 0; i < this.currentTile.GetCityTileTest().cityAttributes.connectedCities.Count; i++) {
				if (!this.currentTile.GetCityTileTest().cityAttributes.connectedCities[i].cityAttributes.hexTile.isOccupied) {
					continue;
				}
				elligibleCitiesForTrade.Add (this.currentTile.GetCityTileTest().cityAttributes.connectedCities[i].cityAttributes);
			}
		}

		elligibleCitiesForTrade.OrderByDescending(x => x.GetScarcityValue());

		if (elligibleCitiesForTrade [0].id == this.citizen.city.id) {
			elligibleCitiesForTrade.RemoveAt(0);
			List<CityTileTest> connectedCities = elligibleCitiesForTrade [0].connectedCities;
			for (int i = 0; i < connectedCities.Count; i++) {
				if (!this.currentTile.GetCityTileTest ().cityAttributes.connectedCities [i].cityAttributes.hexTile.isOccupied) {
					continue;
				}
				elligibleCitiesForTrade.Add (connectedCities[i].cityAttributes);
			}
		}

		this.targetCity = elligibleCitiesForTrade[0];
		this.pathToTargetCity = GameManager.Instance.GetPath(this.currentTile.tile, this.targetCity.hexTile.tile, PATHFINDING_MODE.NORMAL).Reverse().ToList();
		this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString() + ": Merchant will go to [FF0000]" + this.targetCity.cityName + "[-]. The Merchant been to " + this.numOfCitiesVisited.ToString() + " cities\n\n"; 
		GameManager.Instance.turnEnded += GoToDestination;

	}

	void GoToDestination(int currentDay){
		this.isOutsideCity = true;
		if (this.currentTile == this.targetCity.hexTile) {
			//Reached Destination
			StartTrade();
			currentLocationIndex = 0;
			this.numOfCitiesVisited += 1;
			GameManager.Instance.turnEnded -= GoToDestination;
			if (this.numOfCitiesVisited == this.cityQuota) {
				//Destroy Trader and return all trade goods back to city
				this.citizen.city.AdjustResources(this.tradeGoods, false);
				//TODO: Implement Object Pool for optimization
				GameObject.Destroy (this.citizenAvatar);
				this.citizen.city.RemoveCitizen(this.citizen);
			} else {
//				//merchant is at city to trade to, start trade then go back home
//				this.StartTrade();
//				this.targetCity = this.citizen.city;
//				this.pathToTargetCity = GameManager.Instance.GetPath(this.currentTile.tile, this.targetCity.hexTile.tile, PATHFINDING_MODE.NORMAL).ToList();
//				this.pathToTargetCity.Reverse();
//				GameManager.Instance.turnEnded += GoToDestination;
				ChooseTargetCity ();
			}
		} else {
			int increments = 2;
			for (int i = 0; i < increments; i++) {
				currentLocationIndex += 1;
				Tile nextTile = this.pathToTargetCity [currentLocationIndex];
				this.citizenAvatar.GetComponent<CitizenAvatar>().MakeCitizenMove(this.currentTile, nextTile.hexTile);
				this.currentTile = nextTile.hexTile;
				if (this.currentTile == this.targetCity.hexTile) {
					break;
				}
			}
		}
	}

	void StartTrade(){
		for (int i = 0; i < tradeGoods.Count; i++) {
			Resource currentTradeGood = tradeGoods[i];
			ResourceStatus resourceStatusOfOtherCity = this.targetCity.GetResourceStatusByType (currentTradeGood.resourceType);
			if (resourceStatusOfOtherCity.status == RESOURCE_STATUS.SCARCE) {
				int costPerResourceUnit = this.GetCostPerResourceUnit (currentTradeGood.resourceType);
				int totalCostOfWholeResource = resourceStatusOfOtherCity.amount * costPerResourceUnit;
				int numOfResourcesAfforded = (int)Math.Floor ((double)(targetCity.goldCount / costPerResourceUnit));

				if (numOfResourcesAfforded > currentTradeGood.resourceQuantity) {
					numOfResourcesAfforded = currentTradeGood.resourceQuantity;
				}

				if (numOfResourcesAfforded <= 0) {
					//target city cannot afford to buy the offered resource
					this.targetCity.cityLogs += GameManager.Instance.currentDay.ToString () + ": Merchant from " + this.citizen.city.cityName + " arrived, offering " 
						+ currentTradeGood.resourceType.ToString() + ", but rejected because the city cannot afford to buy.\n\n";
					continue;
				}

				DECISION tradeCityDecision = DECISION.NONE;
				if (targetCity.kingdomTile.kingdom.id != this.citizen.city.id) {
					tradeCityDecision = this.targetCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.TRADE, this.citizen.city.kingdomTile.kingdom.lord);
				}

				if (tradeCityDecision == DECISION.NICE || tradeCityDecision == DECISION.NONE) {
					this.ProcessTransaction (currentTradeGood.resourceType, numOfResourcesAfforded, numOfResourcesAfforded * costPerResourceUnit);
					this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString () + ": Merchant has sold [FF0000]" + numOfResourcesAfforded + " " + currentTradeGood.resourceType.ToString () + "[-] in exchange for [FF0000]"
					+ (numOfResourcesAfforded * costPerResourceUnit).ToString () + " GOLD[-].\n\n"; 

					this.targetCity.cityLogs += GameManager.Instance.currentDay.ToString () + ": City has bought [FF0000]" + numOfResourcesAfforded + " " + currentTradeGood.resourceType.ToString () + "[-] in exchange for [FF0000]"
					+ (numOfResourcesAfforded * costPerResourceUnit).ToString () + " GOLD[-].\n\n"; 

					UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.targetCity.kingdomTile.kingdom.lord.name + " ACCEPTED the trade.\n\n";
				} else {
					this.citizen.city.kingdomTile.kingdom.lord.AdjustLikeness (this.targetCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE, true);
					this.targetCity.kingdomTile.kingdom.lord.AdjustLikeness (this.citizen.city.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

					this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString () + ": Trade was rejected by lord of city [FF0000]" + this.targetCity.cityName + "[-].\n\n"; 
					this.targetCity.cityLogs += GameManager.Instance.currentDay.ToString () + ": Trade from " + this.citizen.city.cityName + " was rejected this city's lord.\n\n"; 

					UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.targetCity.kingdomTile.kingdom.lord.name + " REJECTED the trade.\n\n";

				}
			} else {
				this.targetCity.cityLogs += GameManager.Instance.currentDay.ToString () + ": Merchant from " + this.citizen.city.cityName + " arrived, offering " 
					+ currentTradeGood.resourceType.ToString() + ", but rejected since the city does not need more of that resource.\n\n";
			}
		}
	}

	void ProcessTransaction(RESOURCE resourceType, int quantity, int totalCost){
		this.targetCity.AdjustResourceCount(resourceType, quantity);
		this.targetCity.AdjustResourceCount(RESOURCE.GOLD, (totalCost * -1));

		this.AdjustItems(resourceType, (quantity * -1));

		this.citizen.city.AdjustResourceCount(RESOURCE.GOLD, totalCost);
	}

	void AdjustItems(RESOURCE resourceType, int earnings){
		bool adjustedSuccessfully = false;
		for (int i = 0; i < this.tradeGoods.Count; i++) {
			if (this.tradeGoods [i].resourceType == resourceType) {
				this.tradeGoods [i].resourceQuantity += earnings;
				adjustedSuccessfully = true;
				break;
			}
		}
		if (!adjustedSuccessfully) {
			this.tradeGoods.Add(new Resource (resourceType, earnings));
		}
	}

	int GetCostPerResourceUnit(RESOURCE resourceType){
		switch (resourceType) {
		case RESOURCE.FOOD:
			return 5;
		case RESOURCE.LUMBER:
			return 10;
		case RESOURCE.STONE:
			return 10;
		case RESOURCE.MANA:
			return 10;
		case RESOURCE.METAL:
			return 10;
		}
		return 0;
	}

}
