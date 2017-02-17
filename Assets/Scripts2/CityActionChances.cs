using UnityEngine;
using System.Collections;

[System.Serializable]
public class CityActionChances{
	internal int defaultOversupplyChance = 2;
	internal int defaultIncreaseHousingChance = 2;
	internal int defaultUpgradeCitizenChance = 2;
	internal int defaultNewCitizenChance = 2;
	internal int defaultChangeCitizenChance = 5;
	internal int defaultTradeMissionChance = 2;
	internal int defaultPurchaseTileChance = 3;
	internal int defaultIncreaseArmyCountChance = 2;
	internal int defaultPerformCityCitizenActionChance = 2;
	internal int defaultExpansionChance = 4;
	internal int defaultCreateGeneralChance = 2;

	public int oversupplyChance;
	public int increaseHousingChance;
	public int upgradeCitizenChance;
	public int newCitizenChance;
	public int changeCitizenChance;
	public int tradeMissionChance;
	public int purchaseTileChance;
	public int increaseArmyCountChance;
	public int performCityCitizenActionChance;
	public int expansionChance;
	public int createGeneralChance;

	public CityActionChances(){
		this.oversupplyChance = this.defaultOversupplyChance;
		this.increaseHousingChance = this.defaultIncreaseHousingChance;
		this.upgradeCitizenChance = this.defaultUpgradeCitizenChance;
		this.newCitizenChance = this.defaultNewCitizenChance;
		this.changeCitizenChance = this.defaultChangeCitizenChance;
		this.tradeMissionChance = this.defaultTradeMissionChance;
		this.purchaseTileChance = this.defaultPurchaseTileChance;
		this.increaseArmyCountChance = this.defaultIncreaseArmyCountChance;
		this.performCityCitizenActionChance = this.defaultPerformCityCitizenActionChance;
		this.expansionChance = this.defaultExpansionChance;
		this.createGeneralChance = this.defaultCreateGeneralChance;
	}
}
