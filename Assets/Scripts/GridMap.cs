using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
using System.Linq;

public class GridMap : MonoBehaviour {
	public static GridMap Instance;

	public GameObject goHex;

	public float width;
	public float height;

	public float xOffset;
	public float yOffset;

	public int tileSize;

	public float elevationFrequency;
	public float moistureFrequency;

	public List<GameObject> listHexes;

	public Tile[,] GameBoard;

	void Awake(){
		Instance = this;
	}

	internal void GenerateGrid () {
		GameBoard = new Tile[(int)width, (int)height];
		for (int x = 0;  x < width; x++){
			for(int y = 0; y < height; y++){
				float yPosition = y * yOffset;
				if(x % 2 == 1){
					yPosition += yOffset/2;
				}
				GameObject hex = GameObject.Instantiate(goHex) as GameObject;
				hex.transform.parent = this.transform;
				hex.transform.position = new Vector3(x * xOffset, yPosition,0f);
				hex.transform.localScale = new Vector3(tileSize,tileSize,0f);
				hex.name = x + "," + y;
				listHexes.Add(hex);
				hex.GetComponent<HexTile> ().tile = new Tile (x, y);
				hex.GetComponent<HexTile> ().tile.hexTile = hex.GetComponent<HexTile> ();
				GameBoard [x, y] = hex.GetComponent<HexTile> ().tile;
			}
		}
		listHexes.ForEach(o => o.GetComponent<HexTile>().tile.FindNeighbours(GameBoard));
	}

	internal GameObject GetHex(string hexName){
		for(int i = 0; i < listHexes.Count; i++){
			if(hexName == listHexes[i].name){
				return listHexes[i];
			}
		}
		return null;
	}

	public IEnumerable<Tile> AllTiles
	{
		get
		{
			for (var x = 0; x < width; x++)
				for (var y = 0; y < height; y++)
					yield return GameBoard[x, y];
		}
	}
		//		elevationFrequency = UnityEngine.Random.Range(0.1f,7f);
//		moistureFrequency = UnityEngine.Random.Range(0.1f,5f);
//
//		xOffset = xOffset * tileSize;
//		yOffset = yOffset * tileSize;
//
//		float xRand = UnityEngine.Random.Range(1f,3f);
//		float yRand = UnityEngine.Random.Range(0.1f,0.5f);
//
//		Debug.Log(xRand);
//		Debug.Log(yRand);
//
//		for (int x = 0;  x < width; x++){
//			for(int y = 0; y < height; y++){
//				float xPosition = x * xOffset;
//				if(y % 2 == 1){
//					xPosition += xOffset/2;
//				}
//				GameObject hex = GameObject.Instantiate(goHex) as GameObject;
//				hex.transform.parent = this.transform;
//				hex.transform.position = new Vector3(xPosition,y * yOffset,0f);
//				hex.transform.localScale = new Vector3(tileSize,tileSize,0f);
//				float nx = ((float)x/width) - xRand;
//				float ny = ((float)y/height) - yRand;
//
//				hex.GetComponent<HexTile>().elevationNoise = Mathf.PerlinNoise(nx * elevationFrequency, ny * elevationFrequency);
//				hex.GetComponent<HexTile>().moistureNoise = Mathf.PerlinNoise(nx * moistureFrequency, ny * moistureFrequency);
//
//				hex.GetComponent<HexTile>().GenerateBiome();
//			}
//		}
	
}
