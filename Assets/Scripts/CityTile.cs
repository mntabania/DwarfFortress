using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CityTile : MonoBehaviour {

	public City cityAttributes; 

	public List<HexTile> cityTilesByDistance;

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
}
