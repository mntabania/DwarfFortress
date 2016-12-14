﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICity {

	HexTile hexTile { get; set; }
	BIOMES biomeType { get; set; }
	CITY_TYPE cityType { get; set; }
	int richnessLevel { get; set; }
	int numOfRoads { get; set; }
	int population { get; set; }
	List<CityTile> connectedCities { get; set; }

	int GeneratePopulation();
	int GenerateRichness();

}
