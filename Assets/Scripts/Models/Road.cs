using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Road : IRoad {

	public string roadName;
	public Tile[] path;
	public List<HexTile> connectingCities;

	public Road(Tile[] path, City city1, City city2){
		this.connectingCities = new List<HexTile> ();
		this.path = path;
		this.connectingCities.Add(city1.hexTile);
		this.connectingCities.Add(city2.hexTile);

		for (int i = 0; i < connectingCities.Count; i++) {
			roadName += connectingCities [i].name + " - ";
		}
	}
}
