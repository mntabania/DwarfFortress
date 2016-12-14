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
		Debug.Log ("All City Tiles: " + allCityTiles.Count);
		return allCityTiles;
	}
}
