using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Merchant : Job {

	internal CityTest targetCity;
	internal List<Tile> pathToTargetCity;
	internal List<Resource> tradeGoods;

	protected HexTile currentTile;
	protected int currentLocationIndex = 0;

	protected GameObject citizenAvatar;

	protected int daysWaitingForElligibleCity = 0;

	public Merchant(){
		this._jobType = JOB_TYPE.MERCHANT;
		this._residence = RESIDENCE.OUTSIDE;
		this.tradeGoods = new List<Resource>();
		this.pathToTargetCity = null;
		this.targetCity = null;
	}

	internal void WaitForElligibleCity(int currentDay){
		if (daysWaitingForElligibleCity == 5) {
			this.SetActions();
			daysWaitingForElligibleCity = 0;
		}
		daysWaitingForElligibleCity += 1;
	}

	internal void SetActions(){
		CityTest otherCity = null;
		this.tradeGoods.Clear();
		List<CityTest> elligibleCitiesForTrade = new List<CityTest>();
		List<RESOURCE> abundantResourcesOfThisCity = this.citizen.city.GetAbundantResources();
		List<RESOURCE> scarceResourcesOfThisCity = this.citizen.city.GetScarceResources();
		List<RESOURCE> abundantResourcesOfOtherCity = null;
		List<RESOURCE> scarceResourcesOfOtherCity = null;

		for (int i = 0; i < abundantResourcesOfThisCity.Count; i++) {
			RESOURCE abundantResourceOfThisCity = abundantResourcesOfThisCity[i];
			if (this.citizen.city.GetResourceStatusByType (abundantResourceOfThisCity).amount > 0) {
				this.tradeGoods.Add(new Resource(abundantResourceOfThisCity, this.citizen.city.GetResourceStatusByType(abundantResourceOfThisCity).amount));

				for (int j = 0; j < GameManager.Instance.cities.Count; j++) {
					otherCity = GameManager.Instance.cities [j].GetComponent<CityTileTest> ().cityAttributes;
					if (abundantResourceOfThisCity == RESOURCE.GOLD) {
						//buy from other city
						abundantResourcesOfOtherCity = otherCity.GetAbundantResources ();
						for (int k = 0; k < abundantResourcesOfOtherCity.Count; k++) {
							if (scarceResourcesOfThisCity.Contains (abundantResourcesOfOtherCity [k])) {
								elligibleCitiesForTrade.Add (otherCity);
								break;
							}
						}
					} else {
						//sell resource to other city
						scarceResourcesOfOtherCity = otherCity.GetScarceResources ();
						if (scarceResourcesOfOtherCity.Contains (abundantResourceOfThisCity)) {
							elligibleCitiesForTrade.Add (otherCity);
						}
					}

				}
			}
		}

		if (elligibleCitiesForTrade.Count > 0) {
			this.targetCity = elligibleCitiesForTrade [UnityEngine.Random.Range(0, elligibleCitiesForTrade.Count)];
			this.pathToTargetCity = GameManager.Instance.GetPath(this.citizen.city.hexTile.tile, this.targetCity.hexTile.tile, false).ToList();
			this.pathToTargetCity.Reverse();

			this.citizenAvatar = GameObject.Instantiate(Resources.Load ("CitizenAvatar", typeof(GameObject)), this.citizen.city.hexTile.transform) as GameObject;
			this.citizenAvatar.transform.localPosition = Vector3.zero;

			this.currentTile = this.citizen.city.hexTile;
			GameManager.Instance.turnEnded -= WaitForElligibleCity;

			this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString() + ": Merchant will go to: " + this.targetCity.cityName + " to trade ";
			for (int i = 0; i < this.tradeGoods.Count; i++) {
				this.citizen.city.cityLogs += this.tradeGoods[i].resourceQuantity.ToString() + " " + this.tradeGoods[i].resourceType.ToString() + " ";
			}
			this.citizen.city.cityLogs += "\n\n";

			this.citizen.city.AdjustResources(this.tradeGoods);
			GameManager.Instance.turnEnded += GoToDestination;
		} 
	}

	internal void GoToDestination(int currentDay){
		if (this.currentTile == this.targetCity.hexTile) {
			Debug.LogError ("REACHED DESTINATION");
			if (this.currentTile == this.citizen.city.hexTile) {
				//merchant is back home
				GameManager.Instance.turnEnded += WaitForElligibleCity;
				SetActions();
			} else {
				//merchant is at city to trade to
				this.citizen.city.AdjustResources(this.tradeGoods, false);
			}
		} else {
			currentLocationIndex += 1;
			Tile nextTile = this.pathToTargetCity [currentLocationIndex];
			while (this.citizenAvatar.transform.position != nextTile.hexTile.transform.position) {
				this.citizenAvatar.transform.position = Vector3.Lerp (this.citizenAvatar.transform.position, nextTile.hexTile.transform.position, 0.5f);
			}
			this.currentTile = nextTile.hexTile;
		}

	}


	void StartTrade(){
		int goldOtherCityCanSpend = this.targetCity.GetResourceStatusByType(RESOURCE.GOLD).amount;
		RESOURCE currentResourceOffered;
		int quantityOfResourceOffered;
		ResourceStatus resourceStatusOfResourceOfTargetCity;

		DECISION targetCityDecision = DECISION.NONE;
		int numOfResourcesThatCanBuy;
		int costOfCurrentPurchase;
		for (int i = 0; i < this.tradeGoods.Count; i++) {
			if (this.tradeGoods [i].resourceType == RESOURCE.GOLD) {

			} else {
				currentResourceOffered = this.tradeGoods[i].resourceType;
				quantityOfResourceOffered = this.tradeGoods [i].resourceQuantity;
				resourceStatusOfResourceOfTargetCity = this.targetCity.GetResourceStatusByType(currentResourceOffered);
				if (resourceStatusOfResourceOfTargetCity.status == RESOURCE_STATUS.SCARCE) {

					numOfResourcesThatCanBuy = Math.Floor ((double)(goldOtherCityCanSpend / GetCostPerResourceUnit (currentResourceOffered)));
					if (numOfResourcesThatCanBuy > quantityOfResourceOffered) {
						numOfResourcesThatCanBuy = quantityOfResourceOffered;
					}

					if (numOfResourcesThatCanBuy > 0) {
						//Determine if targetCity lord will trade with merchant
						targetCityDecision = this.targetCity.kingdomTile.kingdom.lord.ComputeDecisionBasedOnPersonality (LORD_EVENTS.TRADE, this.citizen.city.kingdomTile.kingdom.lord);
						if (targetCityDecision == DECISION.NICE) {
							this.citizen.city.kingdomTile.kingdom.lord.AdjustLikeness (this.targetCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.NICE, LORD_EVENTS.TRADE, true);
							this.targetCity.kingdomTile.kingdom.lord.AdjustLikeness (this.citizen.city.kingdomTile.kingdom.lord, DECISION.NICE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

							UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.targetCity.kingdomTile.kingdom.lord.name + " ACCEPTED the trade.\n\n";
						} else {
							this.citizen.city.kingdomTile.kingdom.lord.AdjustLikeness (this.targetCity.kingdomTile.kingdom.lord, DECISION.NEUTRAL, DECISION.RUDE, LORD_EVENTS.TRADE, true);
							this.targetCity.kingdomTile.kingdom.lord.AdjustLikeness (this.citizen.city.kingdomTile.kingdom.lord, DECISION.RUDE, DECISION.NEUTRAL, LORD_EVENTS.TRADE, false);

							UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": TRADE: " + this.targetCity.kingdomTile.kingdom.lord.name + " REJECTED the trade.\n\n";
							continue;
						}


						if (targetCityDecision == DECISION.NICE || targetCityDecision == DECISION.NONE) {
							costOfCurrentPurchase = numOfResourcesThatCanBuy * GetCostPerResourceUnit (currentResourceOffered);
							goldOtherCityCanSpend -= costOfCurrentPurchase;
							this.targetCity.AdjustResourceCount (RESOURCE.GOLD, costOfCurrentPurchase);
							this.targetCity.AdjustResourceCount (currentResourceOffered, numOfResourcesThatCanBuy);

							this.tradeGoods [i].resourceQuantity -= numOfResourcesThatCanBuy;
							this.IncreaseEarnings (costOfCurrentPurchase);
						}
					}
				}
			}
		}
	}

	void IncreaseEarnings(int earnings){
		for (int i = 0; i < this.tradeGoods.Count; i++) {
			if (this.tradeGoods [i].resourceType == RESOURCE.GOLD) {
				this.tradeGoods [i].resourceQuantity += earnings;
				break;
			}
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

}
