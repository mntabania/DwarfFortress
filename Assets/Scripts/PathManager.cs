using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathManager : MonoBehaviour {

	public static PathManager Instance = null;

	public List<HexTile> roadTiles;
	public List<CityTile> pendingCityTiles;

	public GameObject Line;

	const float Spacing = 1f;

	void Awake(){
		Instance = this;
		roadTiles = new List<HexTile>();
		pendingCityTiles = new List<CityTile>();
	}

	public void GenerateConnections(List<HexTile> cities){
		pendingCityTiles.Add(cities[0].GetCityTile());
		for (int i = 0; i < pendingCityTiles.Count; i++) {
			CityTile currentCityTile = cities[i].GetCityTile();
			List<HexTile> listOrderedByDistance = currentCityTile.GetAllCitiesByDistance();
			int randomNumberOfRoads = currentCityTile.cityAttributes.GenerateNumberOfRoads();
			int createdRoads = currentCityTile.cityAttributes.connectedCities.Count;

			if (randomNumberOfRoads < createdRoads) {
				currentCityTile.cityAttributes.numOfRoads = createdRoads;
			}

			for (int j = 0; createdRoads < randomNumberOfRoads; j++) {
				CityTile tileToConnectTo = listOrderedByDistance [j].GetCityTile();
				if ((j + 1) == listOrderedByDistance.Count) {
					//if j has reached listOrderedByDistance's upper bound, connect to nearest city
					createdRoads++;
					if (!currentCityTile.cityAttributes.connectedCities.Contains (listOrderedByDistance [0].GetCityTile())) {
						ConnectCities (currentCityTile.cityAttributes.hexTile, listOrderedByDistance [0]);
						if (!pendingCityTiles.Contains (listOrderedByDistance [0].GetCityTile ())) {
							pendingCityTiles.Add (listOrderedByDistance [0].GetCityTile ());
						}
					}
					break;
				} else {
					if (tileToConnectTo.cityAttributes.numOfRoads < 3 && !currentCityTile.cityAttributes.connectedCities.Contains(tileToConnectTo)) {
						createdRoads++;
						ConnectCities (currentCityTile.cityAttributes.hexTile, tileToConnectTo.cityAttributes.hexTile);
						if (!pendingCityTiles.Contains(tileToConnectTo)) {
							pendingCityTiles.Add(tileToConnectTo);
						}
					}
				}
			}

			if (pendingCityTiles.Count != CityGenerator.Instance.cityCount) {
				Debug.Log ("MISSED SOME CITIES!");
				Debug.Log ("Create Lines for missed out cities");
				for (int x = 0; x < cities.Count; x++) {
					if (!pendingCityTiles.Contains (cities [x].GetCityTile())) {
						Debug.Log ("======Missed out city: " + cities [x].name + " ======");
						CityTile missedOutCity = cities [x].GetCityTile ();
						HexTile possibleConnectionTile = CityGenerator.Instance.FindNearestCityWithConnection (missedOutCity);
						if (possibleConnectionTile != null && !missedOutCity.cityAttributes.connectedCities.Contains(possibleConnectionTile.GetCityTile())) {
							ConnectCities (missedOutCity.cityAttributes.hexTile, possibleConnectionTile);
							if (!pendingCityTiles.Contains (missedOutCity)) {
								pendingCityTiles.Add (missedOutCity);
							}
							if (!pendingCityTiles.Contains(possibleConnectionTile.GetCityTile())) {
								pendingCityTiles.Add(possibleConnectionTile.GetCityTile());
							}
						}
					}
				}
			}
		}
		Debug.Log ("DONE!: " + pendingCityTiles.Count);
	}

	[ContextMenu("CHECK")]
	public void ForChecking(){
		Debug.Log(CityGenerator.Instance.FindNearestCityWithConnection(CityGenerator.Instance.cities[0].GetCityTile()).name);
	}

	public bool AreTheseTilesConnected(HexTile tile1, HexTile tile2){
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - tile2.tile.X, 2) + Math.Pow(t.Y - tile2.tile.Y, 2));

		tile2.isRoad = true;
		var path = PathFind.PathFind.FindPath(tile1.tile, tile2.tile, distance, estimate, false);
		tile2.isRoad = false;

		return path != null;
	}

	int GetAdjacentCitiesCount(HexTile city){
		int count = 0;
		for(int i = 0; i < CityGenerator.Instance.cities.Count; i++){
			if (CityGenerator.Instance.cities[i] != city && AreTheseTilesConnected (city, CityGenerator.Instance.cities[i])) {
				count++;
			}
		}
		return count;
	}

	void ConnectCities(HexTile originTile, HexTile targetTile){
		Debug.Log (originTile.name + " is now connected to: " + targetTile.name);
		PathManager.Instance.DeterminePath (originTile.tile, targetTile.tile);
		originTile.GetCityTile().cityAttributes.AddCityAsConnected (targetTile.GetCityTile());
		targetTile.GetCityTile().cityAttributes.AddCityAsConnected (originTile.GetCityTile());
	}

	/*
	 * Generate Path From one tile to another
	 * */
	public void DeterminePath(Tile start, Tile destination){
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destination.X, 2) + Math.Pow(t.Y - destination.Y, 2));

		List<HexTile> roadListByDistance = SortAllRoadsByDistance (start.hexTile, destination.hexTile); //Sort all road tiles in regards to how far they are from the start
		start.canPass = true;
		destination.canPass = true;
		for (int i = 0; i < roadListByDistance.Count; i++) {
			if (AreTheseTilesConnected (roadListByDistance [i], destination.hexTile)) {
				Debug.Log ("Connect to roadTile: " + roadListByDistance [i].name + " instead");
				if (roadListByDistance[i].isCity && roadListByDistance[i].GetCityTile().cityAttributes.connectedCities.Contains(start.hexTile.GetCityTile())) {
					break; //use the already created road between the 2 cities.
				}
				roadListByDistance[i].tile.canPass = true;
				var path = PathFind.PathFind.FindPath (start, roadListByDistance [i].tile, distance, estimate, true);
				DrawPath (path);
				roadListByDistance[i].tile.canPass = false;
				break;
			}
		}
		start.canPass = false;
		destination.canPass = false;
	}

	List<HexTile> SortAllRoadsByDistance(HexTile startTile, HexTile destinationTile){
		List<HexTile> tempRoadTiles = roadTiles;
		/*
		 * Code if you don't want to use cities included in some paths
		 * */
//		for (int i = 0; i < tempRoadTiles.Count; i++) {
//			if (tempRoadTiles [i].isCity) {
//				tempRoadTiles.Remove (tempRoadTiles [i]);
//			}
//		}
		tempRoadTiles.Add(destinationTile);

		List<HexTile> allRoadTiles = tempRoadTiles.OrderBy(
			x => Vector2.Distance(startTile.transform.position, x.transform.position) 
		).ToList();

		return allRoadTiles;
	}

	/*
	 * Draw the generated paths
	 * */
	private void DrawPath(IEnumerable<Tile> path) {
		Debug.Log ("DRAW PATH!");
		List<Tile> pathList = path.ToList();
		for (int i = 0; i < pathList.Count; i++) {
			if (!pathList[i].hexTile.isCity) {
				pathList[i].hexTile.isRoad = true;
				roadTiles.Add(pathList[i].hexTile);
			}

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
}
