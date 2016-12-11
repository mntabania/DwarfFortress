using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICity {

	HexTile hexTile { get; set; }
	BIOMES biomeType { get; set; }
	int richnessLevel { get; set; }
	int numOfRoads { get; set; }
	List<CityTile> connectedCities { get; set; }

	int GenerateRichness();
}
