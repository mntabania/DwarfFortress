﻿using UnityEngine;
using System.Collections;

public class Biomes : MonoBehaviour {
	public static Biomes Instance;

	public float initialTemperature;
	public float initialTemperature2;
	public float intervalTemperature;


	void Awake(){
		Instance = this;
	}

	internal void GenerateBiome(){
		CalculateTemperature();
		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().biomeType = GetBiomeSimple(GridMap.Instance.listHexes[i]);
			if(	GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType == ELEVATION.WATER){
				continue;
			}
			switch(GridMap.Instance.listHexes[i].GetComponent<HexTile>().biomeType){
			case BIOMES.SNOW:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.white;
				break;
			case BIOMES.TUNDRA:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.gray;
				break;
			case BIOMES.DESERT:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(223f/255f,152f/255f,0f/255f);
				break;
			case BIOMES.GRASSLAND:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(165f/255f,42f/255f,42f/255f);
				break;
			case BIOMES.WOODLAND:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(194f/255f,213f/255f,168f/255f);
				break;
			case BIOMES.FOREST:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(156f/255f,188f/255f,167f/255f);
				break;
			}
		}

	}

	internal void GenerateElevation(){
		CalculateElevationAndMoisture();

		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			switch(GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType){
			case ELEVATION.MOUNTAIN:
//				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(165f/255f,42f/255f,42f/255f);
				break;
			case ELEVATION.PLAIN:
//				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.green;
				break;
			case ELEVATION.WATER:
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.blue;

				break;
			}
		}
	}
	private void CalculateElevationAndMoisture(){
		float elevationFrequency = UnityEngine.Random.Range(0.1f,7f);
		float moistureFrequency = UnityEngine.Random.Range(0.1f,5f);

		float xRand = UnityEngine.Random.Range(1f,3f);
		float yRand = UnityEngine.Random.Range(0.1f,0.5f);


		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			string[] splittedName = GridMap.Instance.listHexes[i].name.Split(new char[]{','});
			int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};

			float nx = ((float)xy[0]/GridMap.Instance.width) - xRand;
			float ny = ((float)xy[1]/GridMap.Instance.height) - yRand;

			float elevationNoise = Mathf.PerlinNoise(nx * elevationFrequency, ny * elevationFrequency);
			ELEVATION elevationType = GetElevationType(elevationNoise);

			GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationNoise = elevationNoise;
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType = elevationType;
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().moistureNoise = Mathf.PerlinNoise(nx * moistureFrequency, ny * moistureFrequency);
		}
	}
	private void CalculateTemperature(){
		int distanceUp = 0;
		int distanceDown = 0;
		int distanceRight = 0;
		int distanceLeft = 0;

		int hexDistance1 = 7;
		int hexDistance2 = 14;
		int hexDistance3 = 20;

		EQUATOR_LINE equatorLine = EquatorGenerator.Instance.equatorLine;

		if(equatorLine == EQUATOR_LINE.HORIZONTAL){
			
			string[] splitted = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
			int[] split = {int.Parse(splitted[0]), int.Parse(splitted[1])};
			
			for(int i = 0; i < EquatorGenerator.Instance.listEquator.Count; i++){
				float temperature = initialTemperature;

				string[] splittedName = EquatorGenerator.Instance.listEquator[i].name.Split(new char[]{','});
				int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};
			
				for(int up = xy[1]; up < GridMap.Instance.height; up++){
					distanceUp = up - xy[1];
					if(distanceUp >= 0 && distanceUp <= hexDistance1){
						temperature = intervalTemperature * 1f;
					}else if(distanceUp > hexDistance1 && distanceUp <= hexDistance2){
						temperature = intervalTemperature * 2f;
					}else if(distanceUp > hexDistance2 && distanceUp <= hexDistance3){
						temperature = intervalTemperature * 3f;
					}else if(distanceUp > hexDistance3){
						temperature = intervalTemperature * 4f;
					}else{
						temperature = intervalTemperature * 1f;
					}
					GridMap.Instance.GetHex(xy[0] + "," + up).GetComponent<HexTile>().temperature = temperature;
				}

				for(int down = xy[1]; down >= 0; down--){
					distanceDown =  xy[1] - down;
					if(distanceDown >= 0 && distanceDown <= hexDistance1){
						temperature = intervalTemperature * 1f;
					}else if(distanceDown > hexDistance1 && distanceDown <= hexDistance2){
						temperature = intervalTemperature * 2f;
					}else if(distanceDown > hexDistance2 && distanceDown <= hexDistance3){
						temperature = intervalTemperature * 3f;
					}else if(distanceDown > hexDistance3){
						temperature = intervalTemperature * 4f;
					}else{
						temperature = intervalTemperature * 1f;
					}
					GridMap.Instance.GetHex(xy[0] + "," + down).GetComponent<HexTile>().temperature = temperature;
			
				}
			}

		}
		if(equatorLine == EQUATOR_LINE.VERTICAL || equatorLine == EQUATOR_LINE.DIAGONAL_RIGHT || equatorLine == EQUATOR_LINE.DIAGONAL_LEFT){

			string[] splitted = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
			int[] split = {int.Parse(splitted[0]), int.Parse(splitted[1])};

			for(int i = 0; i < EquatorGenerator.Instance.listEquator.Count; i++){
				float temperature = initialTemperature;

				string[] splittedName = EquatorGenerator.Instance.listEquator[i].name.Split(new char[]{','});
				int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};

				for(int right = xy[0]; right < GridMap.Instance.height; right++){
					distanceRight = right - xy[0];
					if(distanceRight >= 0 && distanceRight <= hexDistance1){
						temperature = intervalTemperature * 1f;
					}else if(distanceRight > hexDistance1 && distanceRight <= hexDistance2){
						temperature = intervalTemperature * 2f;
					}else if(distanceRight > hexDistance2 && distanceRight <= hexDistance3){
						temperature = intervalTemperature * 3f;
					}else if(distanceRight > hexDistance3){
						temperature = intervalTemperature * 4f;
					}else{
						temperature = intervalTemperature * 1f;
					}
					GridMap.Instance.GetHex(right + "," + xy[1]).GetComponent<HexTile>().temperature = temperature;
				}

				for(int left = xy[0]; left >= 0; left--){
					distanceLeft =  xy[0] - left;
					if(distanceLeft >= 0 && distanceLeft <= hexDistance1){
						temperature = intervalTemperature * 1f;
					}else if(distanceLeft > hexDistance1 && distanceLeft <= hexDistance2){
						temperature = intervalTemperature * 2f;
					}else if(distanceLeft > hexDistance2 && distanceLeft <= hexDistance3){
						temperature = intervalTemperature * 3f;
					}else if(distanceLeft > hexDistance3){
						temperature = intervalTemperature * 4f;
					}else{
						temperature = intervalTemperature * 1f;
					}
					GridMap.Instance.GetHex(left + "," + xy[1]).GetComponent<HexTile>().temperature = temperature;

				}
			}

		}
	}
	private ELEVATION GetElevationType(float elevationNoise){
		if(elevationNoise >= 0f && elevationNoise <= 0.30f){
			return ELEVATION.WATER;
		}else if(elevationNoise > 0.30f && elevationNoise <= 0.60f){
			return ELEVATION.PLAIN;
		}else{
			return ELEVATION.MOUNTAIN;
		}
	}
	private BIOMES GetBiomeSimple(GameObject goHex){
		float moistureNoise = goHex.GetComponent<HexTile>().moistureNoise;
		float temperature = goHex.GetComponent<HexTile>().temperature;

		if(temperature >= 0f && temperature <= 0.25f){
			if(moistureNoise >= 0f && moistureNoise <= 0.20f){
				return BIOMES.DESERT;
			}else if(moistureNoise > 0.20f && moistureNoise <= 0.40f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.40f && moistureNoise <= 0.60f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.60f){
				return BIOMES.FOREST;
			}
		}else if(temperature > 0.25f && temperature <= 0.50f){
			if(moistureNoise >= 0f && moistureNoise <= 0.20f){
				return BIOMES.DESERT;
			}else if(moistureNoise > 0.20f && moistureNoise <= 0.30f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.30f && moistureNoise <= 0.60f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.60f){
				return BIOMES.FOREST;
			}
		}else if(temperature > 0.50f && temperature <= 0.75f){
			if(moistureNoise >= 0f && moistureNoise <= 0.30f){
				return BIOMES.DESERT;
			}else if(moistureNoise > 0.30f && moistureNoise <= 0.50f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.50f && moistureNoise <= 0.60f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.60f){
				return BIOMES.SNOW;
			}
		}else if(temperature > 0.75f){
			if(moistureNoise >= 0f && moistureNoise <= 0.50f){
				return BIOMES.TUNDRA;
			}else if(moistureNoise > 0.50f){
				return BIOMES.SNOW;
			}
		}
		return BIOMES.SNOW;
	}
}



//string[] splitted = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
//int[] split = {int.Parse(splitted[0]), int.Parse(splitted[1])};
//
//for(int up = split[1]; up < GridMap.Instance.height; up++){
//	countUp += 1;
//}
//for(int down = split[1]; down >= 0; down--){
//	countDown += 1;
//}
//int hexLimitUp = countUp/4;
//int hexLimitDown = countDown/4;
//
//for(int i = 0; i < EquatorGenerator.Instance.listEquator.Count; i++){
//	float temperature = initialTemperature;
//	float temperature2 = initialTemperature2;
//	string[] splittedName = EquatorGenerator.Instance.listEquator[i].name.Split(new char[]{','});
//	int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};
//
//	int randomNoOfHexes = UnityEngine.Random.Range(1,hexLimitUp+1);
//	for(int up = xy[1]; up < GridMap.Instance.height; up++){
//		if(randomNoOfHexes == 0){
//			randomNoOfHexes = UnityEngine.Random.Range(1,hexLimitUp+1);
//			temperature += intervalTemperature;
//		}
//		GridMap.Instance.GetHex(xy[0] + "," + up).GetComponent<HexTile>().temperature = temperature;
//		randomNoOfHexes -= 1;
//	}
//
//
//	int randomNoOfHexes2 = UnityEngine.Random.Range(1,hexLimitDown+1);
//	for(int down = xy[1]; down >= 0; down--){
//		if(randomNoOfHexes2 == 0){
//			randomNoOfHexes2 = UnityEngine.Random.Range(1,hexLimitDown+1);
//			temperature2 += intervalTemperature;
//		}
//		GridMap.Instance.GetHex(xy[0] + "," + down).GetComponent<HexTile>().temperature = temperature2;
//		randomNoOfHexes2 -= 1;
//
//	}
//}