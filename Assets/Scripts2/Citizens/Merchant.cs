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

	public bool isOutsideCity;

	public Merchant(){
		this._jobType = JOB_TYPE.MERCHANT;
		this._residence = RESIDENCE.OUTSIDE;
		this.tradeGoods = new List<Resource>();
		this.pathToTargetCity = null;
		this.targetCity = null;
		this.isOutsideCity = false;
	}

	internal void WaitForElligibleCity(int currentDay){
		if (daysWaitingForElligibleCity == 5) {
			this.SetActions();
			daysWaitingForElligibleCity = 0;
			if (this.targetCity == null) {
				bool areAllMerchantsIdle = true;
				List<Citizen> merchants = this.citizen.city.GetCitizensByType (JOB_TYPE.MERCHANT);
				for (int i = 0; i < merchants.Count; i++) {
					if (merchants[i].id != this.citizen.id) {
						if(((Merchant)merchants[i].job).targetCity != null){
							areAllMerchantsIdle = false;
						}
					}
				}
				if (areAllMerchantsIdle) {
					this.citizen.city.unneededCitizens.Add(this.citizen);
				}
			}
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

					if (otherCity.id == this.citizen.city.id) {
						continue;
					}
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

		List<Citizen> merchants = this.citizen.city.GetCitizensByType (JOB_TYPE.MERCHANT);
		for (int i = 0; i < merchants.Count; i++) {
			if (merchants[i].id != this.citizen.id) {
				Merchant otherMerchant = (Merchant)merchants[i].job;
				if (elligibleCitiesForTrade.Contains(otherMerchant.targetCity)) {
					elligibleCitiesForTrade.Remove(otherMerchant.targetCity);
				}
			}
		}

		if (elligibleCitiesForTrade.Count > 0) {
			this.targetCity = elligibleCitiesForTrade [UnityEngine.Random.Range(0, elligibleCitiesForTrade.Count)];
			this.pathToTargetCity = GameManager.Instance.GetPath(this.citizen.city.hexTile.tile, this.targetCity.hexTile.tile, false).ToList();
			this.pathToTargetCity.Reverse();

			this.citizenAvatar = GameObject.Instantiate(Resources.Load ("CitizenAvatar", typeof(GameObject)), this.citizen.city.hexTile.transform) as GameObject;
			this.citizenAvatar.transform.localPosition = Vector3.zero;
			this.citizenAvatar.GetComponent<CitizenAvatar>().citizen = this.citizen;

			this.currentTile = this.citizen.city.hexTile;
			GameManager.Instance.turnEnded -= WaitForElligibleCity;

			this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString() + ": Merchant will go to: " + this.targetCity.cityName + " to trade ";
			for (int i = 0; i < this.tradeGoods.Count; i++) {
				this.citizen.city.cityLogs += this.tradeGoods[i].resourceQuantity.ToString() + " " + this.tradeGoods[i].resourceType.ToString() + " ";
			}
			this.citizen.city.cityLogs += "\n\n";

			this.citizen.city.AdjustResources(this.tradeGoods);
			GameManager.Instance.turnEnded += GoToDestination;
			this.isOutsideCity = true;
		} 
	}

	internal void GoToDestination(int currentDay){
		if (this.currentTile == this.targetCity.hexTile) {
			currentLocationIndex = 0;
			GameManager.Instance.turnEnded -= GoToDestination;

			if (this.currentTile == this.citizen.city.hexTile) {
				//merchant is back home
				this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString() + ": Merchant has returned with ";
				for (int i = 0; i < this.tradeGoods.Count; i++) {
					this.citizen.city.cityLogs += this.tradeGoods[i].resourceQuantity.ToString() + " " + this.tradeGoods[i].resourceType.ToString() + " ";
				}
				this.citizen.city.cityLogs += "\n\n";

				this.isOutsideCity = false;
				this.targetCity = null;
				this.pathToTargetCity.Clear();
				GameObject.Destroy(this.citizenAvatar);
				this.citizen.city.AdjustResources(this.tradeGoods, false);
				GameManager.Instance.turnEnded += WaitForElligibleCity;

				SetActions();
			} else {
				//merchant is at city to trade to, start trade then go back home
				this.StartTrade();
				this.targetCity = this.citizen.city;
				this.pathToTargetCity = GameManager.Instance.GetPath(this.currentTile.tile, this.targetCity.hexTile.tile, false).ToList();
				this.pathToTargetCity.Reverse();
				GameManager.Instance.turnEnded += GoToDestination;
			}
		} else {
			int increments = 2;
			for (int i = 0; i < increments; i++) {
				currentLocationIndex += 1;
				Tile nextTile = this.pathToTargetCity [currentLocationIndex];
				while (this.citizenAvatar.transform.position != nextTile.hexTile.transform.position) {
					this.citizenAvatar.transform.position = Vector3.Lerp (this.citizenAvatar.transform.position, nextTile.hexTile.transform.position, 0.5f);
				}
				this.currentTile = nextTile.hexTile;
				if (this.currentTile == this.targetCity.hexTile) {
					break;
				}
			}

		}

	}


	void StartTrade(){
		List<Resource> soldList = new List<Resource>();
		List<Resource> boughtList = new List<Resource>();

		List<RESOURCE> scarceResourcesOfTargetCity = this.targetCity.GetScarceResources();
		int goldTargetCityCanSpend = this.targetCity.goldCount;

		List<RESOURCE> scarceResourcesOfThisCity = this.citizen.city.GetScarceResources();


		for (int i = 0; i < this.tradeGoods.Count; i++) {
			Resource currentResourceOnOffer = this.tradeGoods[i];
			int costOfResourcePerUnit = GetCostPerResourceUnit (currentResourceOnOffer.resourceType);

			if (currentResourceOnOffer.resourceType == RESOURCE.GOLD) {
				//trader will buy resources from city
				int goldThisCityCanSpend = currentResourceOnOffer.resourceQuantity;
				for (int j = 0; j < scarceResourcesOfThisCity.Count; j++) {
					//check all scarce resources of this city, then check if the scarce resource is abundant in target city
					if (scarceResourcesOfThisCity [j] == RESOURCE.GOLD) {
						continue;
					}

					if (goldThisCityCanSpend <= 0) {
						//this city has no more budget to buy
						break;
					}
					ResourceStatus resourceTargetCityIsOffering = this.targetCity.GetResourceStatusByType(scarceResourcesOfThisCity[j]);
					if (resourceTargetCityIsOffering.status == RESOURCE_STATUS.ABUNDANT) {
						ResourceStatus statusOfThisCityScarceResource = this.citizen.city.GetResourceStatusByType(scarceResourcesOfThisCity[j]);
						costOfResourcePerUnit = GetCostPerResourceUnit (resourceTargetCityIsOffering.resource);

						int numOfResourcesThatCanBuy = (int)Math.Floor ((double)(goldThisCityCanSpend / costOfResourcePerUnit));
						if (numOfResourcesThatCanBuy > statusOfThisCityScarceResource.amount) {
							numOfResourcesThatCanBuy = statusOfThisCityScarceResource.amount;
						}

						if (numOfResourcesThatCanBuy > resourceTargetCityIsOffering.amount) {
							numOfResourcesThatCanBuy = resourceTargetCityIsOffering.amount;
						}

						if (numOfResourcesThatCanBuy > 0) {
							int totalCostOfItem = numOfResourcesThatCanBuy * costOfResourcePerUnit;
							goldThisCityCanSpend -= totalCostOfItem;
							boughtList.Add (new Resource (resourceTargetCityIsOffering.resource, numOfResourcesThatCanBuy));
						}
					}
				}
			} else {
				//trader will sell resource
				if (goldTargetCityCanSpend <= 0) {
					//target city has no more budget to buy
					continue;
				}

				if (scarceResourcesOfTargetCity.Contains (currentResourceOnOffer.resourceType)) {
					//target city is scarce on the offered resource, target city will buy
					ResourceStatus statusOfOtherCityScarceResource = this.targetCity.GetResourceStatusByType(currentResourceOnOffer.resourceType);

					int numOfResourcesThatCanBuy = (int)Math.Floor ((double)(goldTargetCityCanSpend / costOfResourcePerUnit));
					if (numOfResourcesThatCanBuy > statusOfOtherCityScarceResource.amount) {
						numOfResourcesThatCanBuy = statusOfOtherCityScarceResource.amount;
					}

					if (numOfResourcesThatCanBuy > currentResourceOnOffer.resourceQuantity) {
						numOfResourcesThatCanBuy = currentResourceOnOffer.resourceQuantity;
					}

					if (numOfResourcesThatCanBuy > 0) {
						int totalCostOfItem = numOfResourcesThatCanBuy * costOfResourcePerUnit;
						goldTargetCityCanSpend -= totalCostOfItem;
						soldList.Add (new Resource (currentResourceOnOffer.resourceType, numOfResourcesThatCanBuy));
					}
				}
			}
		}

		this.ProcessOrders(soldList, boughtList);
	}

	void ProcessOrders(List<Resource> soldList, List<Resource> boughtList){
		//process sold
		if (soldList.Count > 0) {
			this.targetCity.cityLogs += GameManager.Instance.currentDay.ToString () + ": City bought ";
			for (int i = 0; i < soldList.Count; i++) {
				int costPerUnit = GetCostPerResourceUnit (soldList [i].resourceType);
				int totalCostOfItem = costPerUnit * soldList [i].resourceQuantity;
				//Pay gold
				this.AdjustItems (RESOURCE.GOLD, totalCostOfItem);
				this.targetCity.AdjustResourceCount (RESOURCE.GOLD, (totalCostOfItem * -1));
				//Get items
				this.AdjustItems (soldList [i].resourceType, (soldList [i].resourceQuantity * -1));
				this.targetCity.AdjustResourceCount (soldList [i].resourceType, soldList [i].resourceQuantity);

				this.targetCity.cityLogs += soldList [i].resourceQuantity.ToString () + " " + soldList [i].resourceType.ToString () + " for " + totalCostOfItem.ToString () + " GOLD\n\n";
			}
			this.targetCity.cityLogs += " from merchant from " + this.citizen.city.cityName + "\n\n";
		}

		if (boughtList.Count > 0) {
			//process bought
			this.citizen.city.cityLogs += GameManager.Instance.currentDay.ToString () + ": City bought ";
			for (int i = 0; i < boughtList.Count; i++) {
				int costPerUnit = GetCostPerResourceUnit (boughtList [i].resourceType);
				int totalCostOfItem = costPerUnit * boughtList [i].resourceQuantity;
				//Pay gold
				this.AdjustItems (RESOURCE.GOLD, (totalCostOfItem * -1));
				this.targetCity.AdjustResourceCount (RESOURCE.GOLD, totalCostOfItem);
				//Get items
				this.AdjustItems (boughtList [i].resourceType, boughtList [i].resourceQuantity);
				this.targetCity.AdjustResourceCount (boughtList [i].resourceType, (boughtList [i].resourceQuantity * -1));

				this.citizen.city.cityLogs += boughtList [i].resourceQuantity.ToString () + " " + boughtList [i].resourceType.ToString () + " for " + totalCostOfItem.ToString () + " GOLD\n";
			}
			this.targetCity.cityLogs += "from " + this.targetCity.cityName + "\n\n";
		}
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
