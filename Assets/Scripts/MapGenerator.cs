﻿using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	void Start(){
		GridMap.Instance.GenerateGrid();
		EquatorGenerator.Instance.GenerateEquator();
		Biomes.Instance.GenerateElevation();
		Biomes.Instance.GenerateBiome();
		Biomes.Instance.GenerateTileDetails ();
		CityGenerator.Instance.GenerateCities();
		KingdomGenerator.Instance.GenerateInitialKingdoms ();
//		KingdomGenerator.Instance.ListAdjacentKingdoms ();
	}

}
