using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathManager : MonoBehaviour {

	public static PathManager Instance = null;

	public List<Road> allRoads;

	public GameObject Line;

	const float Spacing = 1f;

	void Awake(){
		Instance = this;
		allRoads = new List<Road> ();
	}

	public void GenerateConnections(List<HexTile> cities){
		for (int i = 0; i < cities.Count; i++) {
			CityTile currentCityTile = cities[i].GetCityTile();
			List<HexTile> listOrderedByDistance = currentCityTile.GetAllCitiesByDistance();
			int randomNumberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();
			int createdRoads = 0;
			for (int j = 0; createdRoads < randomNumberOfRoads; j++) {
				CityTile tileToConnectTo = listOrderedByDistance [j].GetCityTile();
				if ((j + 1) == listOrderedByDistance.Count) {
					//if j has reached listOrderedByDistance's upper bound, connect to nearest city
					RefinePaths (cities, currentCityTile.cityAttributes.hexTile, listOrderedByDistance[0]);
					ConnectCities (currentCityTile.cityAttributes.hexTile, listOrderedByDistance[0]);
					break;
				} else {
					if (tileToConnectTo.cityAttributes.numOfRoads < 3) {
						createdRoads++;
						RefinePaths (cities, currentCityTile.cityAttributes.hexTile, tileToConnectTo.cityAttributes.hexTile);
						ConnectCities (currentCityTile.cityAttributes.hexTile, tileToConnectTo.cityAttributes.hexTile);
					}
				}
			}
		}
	}

	public void RefinePaths(List<HexTile> cities, HexTile startTile, HexTile destinationTile){
		for (int i = 0; i < cities.Count; i++) {
			HexTile currentTile = cities[i];
			if (currentTile != startTile && currentTile != destinationTile) {
				for (int j = 0; j < currentTile.neighbours.Count; j++) {
					currentTile.neighbours [j].tile.canPass = false;
				}
			}
		}
	}

	void ConnectCities(HexTile originTile, HexTile targetTile){
		Debug.Log ("Connected to: " + targetTile.name);
		PathManager.Instance.DeterminePath (originTile.tile, targetTile.tile);
		originTile.GetCityTile().cityAttributes.AddCityAsConnected (targetTile.GetCityTile ());
		targetTile.GetCityTile ().cityAttributes.AddCityAsConnected (originTile.GetCityTile());
		ResetPassableTiles();
	}

	/*
	 * Generate Path From one tile to another
	 * */
	public void DeterminePath(Tile start, Tile destination){
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destination.X, 2) + Math.Pow(t.Y - destination.Y, 2));

		var path = PathFind.PathFind.FindPath(start, destination, distance, estimate);
		DrawPath(start.hexTile.GetComponent<CityTile>(), destination.hexTile.GetComponent<CityTile>(), path);
	}


	/*
	 * Draw the generated paths
	 * */
	private void DrawPath(CityTile startingCity, CityTile destinationCity, IEnumerable<Tile> path) {
		List<Tile> pathList = path.ToList();
		Road road = new Road(path.ToArray(), startingCity.cityAttributes, destinationCity.cityAttributes);
		startingCity.cityAttributes.roads.Add(road);
		destinationCity.cityAttributes.roads.Add(road);
		allRoads.Add (road);
		for (int i = 0; i < pathList.Count; i++) {
			if ((i + 1) < pathList.Count) {
				CreateLine(pathList[i], pathList [i+1]);
			} else {
				CreateLine(pathList[i], null);
			}
		}
	}

	/*
	 * Take into account which direction each path 
	 * is going to.
	 * */
	void CreateLine(Tile currentTile, Tile nextTile) {
		if (nextTile == null) {
			return;
		}
		Point difference = new Point((nextTile.X -currentTile.X), (nextTile.Y - currentTile.Y));
		HexTile currentHexTile = currentTile.hexTile;
		HexTile nextHexTile = nextTile.hexTile;
		if (currentTile.X % 2 == 0) {
			if (difference.X == 0 && difference.Y == 1) {
				//NORTH
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH);
			} else if (difference.X == 1 && difference.Y == 0) {
				//NORTH EAST
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH_EAST);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH_WEST);
			} else if (difference.X == 1 && difference.Y == -1) {
				//SOUTH EAST
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH_EAST);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH_WEST);
			} else if (difference.X == 0 && difference.Y == -1) {
				//SOUTH
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH);
			} else if (difference.X == -1 && difference.Y == -1) {
				//SOUTH WEST
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH_WEST);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH_EAST);
			} else if (difference.X == -1 && difference.Y == 0) {
				//NORTH WEST
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH_WEST);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH_EAST);
			}
		} else {
			if (difference.X == 0 && difference.Y == 1) {
				//NORTH
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH);
			} else if (difference.X == 1 && difference.Y == 1) {
				//NORTH EAST
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH_EAST);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH_WEST);
			} else if (difference.X == 1 && difference.Y == 0) {
				//SOUTH EAST
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH_EAST);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH_WEST);
			} else if (difference.X == 0 && difference.Y == -1) {
				//SOUTH
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH);
			} else if (difference.X == -1 && difference.Y == 0) {
				//SOUTH WEST
				currentHexTile.ActivatePath(PATH_DIRECTION.SOUTH_WEST);
				nextHexTile.ActivatePath(PATH_DIRECTION.NORTH_EAST);
			} else if (difference.X == -1 && difference.Y == 1) {
				//NORTH WEST
				currentHexTile.ActivatePath(PATH_DIRECTION.NORTH_WEST);
				nextHexTile.ActivatePath(PATH_DIRECTION.SOUTH_EAST);
			}
		}
//		currentHexTile.path.SetActive(true);
//		var line = (GameObject)Instantiate(Line);
//		line.transform.position = GetWorldCoordinates(tile.Location.X, tile.Location.Y, 1f);
//		_path.Add(line);
	}

	void ResetPassableTiles(){
		List<GameObject> allHexes = GridMap.Instance.listHexes;
		for (int i = 0; i < allHexes.Count; i++) {
			HexTile currentHexTile = allHexes[i].GetComponent<HexTile>();
			if (!currentHexTile.isCity) {
				currentHexTile.tile.canPass = true;
			}
		}
	}
}
