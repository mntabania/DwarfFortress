using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
using System.Linq;
using System;

public class PathManager : MonoBehaviour {

	public static PathManager Instance = null;

	public GameObject Line;

	List<GameObject> _path;
	const float Spacing = 1f;

	void Awake(){
		Instance = this;
	}

	[ContextMenu("PATHPATH")]
	public void PathPath(){
		DeterminePath (CityGenerator.Instance.cities [0].tile, CityGenerator.Instance.cities [1].tile);
		foreach(Tile t in CityGenerator.Instance.cities [0].tile.Neighbours)
			Debug.Log ("neighbour: " + t.hexTile.name);
	}

	public void DeterminePath(Tile start, Tile destination){
		Debug.Log ("DETERMINE PATH!");
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destination.X, 2) + Math.Pow(t.Y - destination.Y, 2));

		var path = PathFind.PathFind.FindPath(start, destination, distance, estimate);

		DrawPath(path);
	}

	private void DrawPath(IEnumerable<Tile> path) {
//		if (_path == null)
//			_path = new List<GameObject>();
//
//		_path.ForEach(Destroy);
//		_path = new List<GameObject>();
		path.ToList().ForEach(CreateLine);
	}

	void CreateLine(Tile tile) {
		tile.hexTile.path.SetActive(true);
//		var line = (GameObject)Instantiate(Line);
//		line.transform.position = GetWorldCoordinates(tile.Location.X, tile.Location.Y, 1f);
//		_path.Add(line);
	}
}
