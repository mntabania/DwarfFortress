using UnityEngine;
using System.Collections;

public class GridMap : MonoBehaviour {

	public GameObject goHex;

	public float width;
	public float height;

	public float xOffset;
	public float yOffset;

	public int tileSize;

	public float elevationFrequency;
	public float moistureFrequency;


	// Use this for initialization
	void Start () {
		GenerateGrid();
	}
	
	// Update is called once per frame
	void Update () {

	}

	internal void GenerateGrid () {

		elevationFrequency = UnityEngine.Random.Range(0.1f,7f);
		moistureFrequency = UnityEngine.Random.Range(0.1f,5f);

		xOffset = xOffset * tileSize;
		yOffset = yOffset * tileSize;

		float xRand = UnityEngine.Random.Range(1f,3f);
		float yRand = UnityEngine.Random.Range(0.1f,0.5f);

		Debug.Log(xRand);
		Debug.Log(yRand);

		for (int x = 0;  x < width; x++){
			for(int y = 0; y < height; y++){
				float xPosition = x * xOffset;
				if(y % 2 == 1){
					xPosition += xOffset/2;
				}
				GameObject hex = GameObject.Instantiate(goHex) as GameObject;
				hex.transform.parent = this.transform;
				hex.transform.position = new Vector3(xPosition,y * yOffset,0f);
				hex.transform.localScale = new Vector3(tileSize,tileSize,0f);
				float nx = ((float)x/width) - xRand;
				float ny = ((float)y/height) - yRand;

				hex.GetComponent<HexTile>().elevationNoise = Mathf.PerlinNoise(nx * elevationFrequency, ny * elevationFrequency);
				hex.GetComponent<HexTile>().moistureNoise = Mathf.PerlinNoise(nx * moistureFrequency, ny * moistureFrequency);

				hex.GetComponent<HexTile>().GenerateBiome();
			}
		}
	}
}
