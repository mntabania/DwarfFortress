using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : ICity {

	public HexTile hexTile { get; set; }
	public BIOMES biomeType { get; set; }
	public int richnessLevel { get; set; }
	public int numOfRoads { get; set; }
	public List<CityTile> connectedCities { get; set; }

	public City(HexTile hexTile, BIOMES biomeType){
		this.hexTile = hexTile;
		this.biomeType = biomeType;
		this.richnessLevel = GenerateRichness();
		numOfRoads = 0;
		connectedCities = new List<CityTile>();
	}

	public int GenerateRichness(){
		float rand = Random.value;
		if (rand <= .1f) {
			return Random.Range (0, 31);
		}
		if (rand <= .2f) {
			return Random.Range (40, 101);
		}
		if (rand <= .8f) {
			return Random.Range (30, 41);
		}
		return Random.Range (40, 101);

	}



}
