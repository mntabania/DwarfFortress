using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityTest{

	public int id;
	public string cityName;
	public BIOMES biomeType;
	public Mayor cityMayor;
	public int cityLevel;
	public int numOfRoads;
	public int population;
	public int foodCount;
	public int lumberCount;
	public int rockCount;
	public int manaStoneCount;
	public int tradeGoodsCount;
	public int mayorLikeRating;
	public List<Citizen> citizens;
	public List<CityTile> connectedCities;
	public Religion cityReligion;
	public Culture cityCulture;
	public KingdomTile kingdomTile;
	public HexTile hexTile;

	public CityTest(HexTile hexTile, BIOMES biomeType){
		this.id = 0;
		this.cityName = hexTile.name;
		this.biomeType = biomeType;
		this.cityMayor = new Mayor();
		this.cityLevel = 1;
		this.numOfRoads = 0;
		this.population = 0;
		this.foodCount = 0;
		this.lumberCount = 0;
		this.rockCount = 0;
		this.manaStoneCount = 0;
		this.tradeGoodsCount = 0;
		this.mayorLikeRating = 0;
		this.citizens = new List<Citizen>();
		this.connectedCities = new List<CityTile>();
		this.cityReligion = new Religion();
		this.cityCulture = new Culture();
		this.kingdomTile = new KingdomTile();
		this.hexTile = hexTile;
	}

//	public HexTile hexTile;
//	public BIOMES biomeType;
//	public CITY_TYPE cityType;
//	public int cityLevel;
//	public int richnessLevel;
//	public int numOfRoads;
//	public int population;
//	public int garrison;
//	public int gold;
//	public List<CityTile> connectedCities;
//	public KingdomTile kingdomTile;
//	public Faction faction;
}
