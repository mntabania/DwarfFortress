using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Biomes : MonoBehaviour {
	public static Biomes Instance;

	public float initialTemperature;
	public float initialTemperature2;
	public float intervalTemperature;
	[Space(10)]
	public float temperature;
	[Space(10)]
	public int[] hexInterval;
	public float[] temperatureInterval;
	[Space(10)]
	public Sprite forestSprite;
	public Sprite grasslandSprite;
	public Sprite woodlandSprite;
	public Sprite desertSprite;
	public Sprite tundraSprite;
	public Sprite snowSprite;
	public Sprite waterSprite;
	[Space(10)]
	public Sprite forestLeft;
	public Sprite forestRight;
	public Sprite forestLeftCorner;
	public Sprite forestRightCorner;
	public Sprite forestCenter;
	public Sprite forestCenter2;
	public Sprite forestCenter3;
	[Space(10)]
	public Sprite grasslandLeft;
	public Sprite grasslandRight;
	public Sprite grasslandLeftCorner;
	public Sprite grasslandRightCorner;
	public Sprite grasslandCenter;
	public Sprite grasslandCenter2;
	[Space(10)]
	public Sprite woodlandLeft;
	public Sprite woodlandRight;
	public Sprite woodlandLeftCorner;
	public Sprite woodlandRightCorner;
	public Sprite woodlandCenter;
	public Sprite woodlandCenter2;
	public Sprite woodlandCenter3;
	[Space(10)]
	public Sprite desertLeft;
	public Sprite desertRight;
	public Sprite desertLeftCorner;
	public Sprite desertRightCorner;
	public Sprite desertCenter;
	public Sprite desertCenter2;
	public Sprite desertCenter3;
	public Sprite desertCenter4;
	[Space(10)]
	public Sprite tundraLeft;
	public Sprite tundraRight;
	public Sprite tundraLeftCorner;
	public Sprite tundraRightCorner;
	public Sprite tundraCenter;
	[Space(10)]
	public Sprite snowLeft;
	public Sprite snowRight;
	public Sprite snowLeftCorner;
	public Sprite snowRightCorner;
	public Sprite snowCenter;
	public Sprite snowCenter2;
	[Space(10)]
	public Sprite mountainCenter;

	[Space(10)]
	public List<HexTile> snowHexTiles;
	public List<HexTile> tundraHexTiles;
	public List<HexTile> grasslandHexTiles;
	public List<HexTile> woodsHexTiles;
	public List<HexTile> forestHexTiles;
	public List<HexTile> desertHexTiles;

	void Awake(){
		Instance = this;
	}

	internal void GenerateBiome(){
		//CalculateNewTemperature();
		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			GameObject currentHexTileGO = GridMap.Instance.listHexes[i];
			HexTile currentHexTile = GridMap.Instance.listHexes[i].GetComponent<HexTile>();
			currentHexTile.biomeType = GetBiomeSimple(currentHexTileGO);
			if(currentHexTile.elevationType == ELEVATION.WATER){
				continue;
			}
			AssignHexTileToList (currentHexTile);
			switch(currentHexTile.biomeType){
			case BIOMES.SNOW:
				currentHexTile.SetTileSprites (snowSprite, snowLeft, snowRight, snowLeftCorner, snowRightCorner, new Sprite[]{snowCenter, snowCenter2});
				break;
			case BIOMES.TUNDRA:
				currentHexTile.SetTileSprites (tundraSprite, tundraLeft, tundraRight, tundraLeftCorner, tundraRightCorner,  new Sprite[]{tundraCenter});
				break;
			case BIOMES.DESERT:
				currentHexTile.SetTileSprites (desertSprite, desertLeft, desertRight, desertLeftCorner, desertRightCorner,  new Sprite[]{desertCenter, desertCenter2, desertCenter3, desertCenter4});
				break;
			case BIOMES.GRASSLAND:
				currentHexTile.SetTileSprites (grasslandSprite, grasslandLeft, grasslandRight, grasslandLeftCorner, grasslandRightCorner, new Sprite[]{grasslandCenter, grasslandCenter2});
				break;
			case BIOMES.WOODLAND:
				currentHexTile.SetTileSprites (woodlandSprite, woodlandLeft, woodlandRight, woodlandLeftCorner, woodlandRightCorner, new Sprite[]{woodlandCenter, woodlandCenter2, woodlandCenter3});
				break;
			case BIOMES.FOREST:
				currentHexTile.SetTileSprites (forestSprite, forestLeft, forestRight, forestLeftCorner, forestRightCorner, new Sprite[]{forestCenter, forestCenter2, forestCenter3});
				break;
			}
		}

		GenerateBareBiome();

	}

	internal void GenerateElevation(){
		CalculateElevationAndMoisture();

		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			switch(GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType){
			case ELEVATION.MOUNTAIN:
//				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(165f/255f,42f/255f,42f/255f);
				GridMap.Instance.listHexes[i].GetComponent<HexTile>().centerPiece.GetComponent<SpriteRenderer>().sprite = mountainCenter;
				break;
			case ELEVATION.PLAIN:
//				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.green;
				break;
			case ELEVATION.WATER:
//				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = Color.blue;
				GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().sprite = waterSprite;
				break;
			}
		}
	}

	private void CalculateElevationAndMoisture(){
		float elevationFrequency = 2.66f;
		float moistureFrequency = 2.94f;
		float tempFrequency = 2.4f;

		float elevationRand = UnityEngine.Random.Range(500f,2000f);
		float moistureRand = UnityEngine.Random.Range(500f,2000f);
		float temperatureRand = UnityEngine.Random.Range(500f,2000f);

		string[] splittedNameEq = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
		int equatorY = int.Parse (splittedNameEq [1]);

		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			string[] splittedName = GridMap.Instance.listHexes[i].name.Split(new char[]{','});
			int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};

			float nx = ((float)xy[0]/GridMap.Instance.width);
			float ny = ((float)xy[1]/GridMap.Instance.height);

			float elevationNoise = Mathf.PerlinNoise((nx + elevationRand) * elevationFrequency, (ny + elevationRand) * elevationFrequency);
			ELEVATION elevationType = GetElevationType(elevationNoise);

			GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationNoise = elevationNoise;
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType = elevationType;
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().moistureNoise = Mathf.PerlinNoise((nx + moistureRand) * moistureFrequency, (ny + moistureRand) * moistureFrequency);

			int distanceToEquator = Mathf.Abs (xy [1] - equatorY);
			float tempGradient = 1.2f / GridMap.Instance.height;
			GridMap.Instance.listHexes [i].GetComponent<HexTile>().temperature = distanceToEquator * tempGradient;
			GridMap.Instance.listHexes[i].GetComponent<HexTile>().temperature += (Mathf.PerlinNoise((nx + temperatureRand) * tempFrequency, (ny + temperatureRand) * tempFrequency)) * 0.6f;
		}
	}

	private void CalculateNewTemperature(){

		for(int i = 0; i < EquatorGenerator.Instance.listEquator.Count; i++){
			float temperature = this.temperature;

			string[] splittedName = EquatorGenerator.Instance.listEquator[i].name.Split(new char[]{','});
			int[] xy = {int.Parse(splittedName[0]), int.Parse(splittedName[1])};

			float tempCount = 0f;
			int hexCount = 0;
			int count = 0;

			for(int up = xy[1]; up < GridMap.Instance.height; up++){

				GridMap.Instance.GetHex(xy[0] + "," + up).GetComponent<HexTile>().temperature = temperature;

				hexCount++;
				if(hexCount > hexInterval[count]){
					count++;
					if(count >= hexInterval.Length){
						count = hexInterval.Length - 1;
					}
				}
				tempCount = temperatureInterval [count];
				temperature += tempCount;
			}

			tempCount = 0f;
			hexCount = 0;
			count = 0;
			temperature = this.temperature;
			for(int down = xy[1]; down >= 0; down--){
				GridMap.Instance.GetHex(xy[0] + "," + down).GetComponent<HexTile>().temperature = temperature;

				hexCount++;
				if(hexCount > hexInterval[count]){
					count++;
					if(count >= hexInterval.Length){
						count = hexInterval.Length - 1;
					}
				}
				tempCount = temperatureInterval [count];
				temperature += tempCount;


			}
		}
	}
	private void CalculateTemperature(){
		int distanceUp = 0;
		int distanceDown = 0;
		int distanceRight = 0;
		int distanceLeft = 0;

		int hexDistance1 = 13;
		int hexDistance2 = 20;
		int hexDistance3 = 35;

		EQUATOR_LINE equatorLine = EquatorGenerator.Instance.equatorLine;

		if(equatorLine == EQUATOR_LINE.HORIZONTAL){
			
//			string[] splitted = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
//			int[] split = {int.Parse(splitted[0]), int.Parse(splitted[1])};
			
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

//			string[] splitted = EquatorGenerator.Instance.listEquator[0].name.Split(new char[]{','});
//			int[] split = {int.Parse(splitted[0]), int.Parse(splitted[1])};

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
		if(elevationNoise <= 0.30f){
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

		if(temperature <= 0.35f) {
			if(moistureNoise <= 0.20f){
				return BIOMES.DESERT;
			}else if(moistureNoise > 0.20f && moistureNoise <= 0.40f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.40f && moistureNoise <= 0.55f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.55f){
				return BIOMES.FOREST;
			}
		} else if(temperature > 0.35f && temperature <= 0.65f){
			if(moistureNoise <= 0.20f){
				return BIOMES.DESERT;
			}else if(moistureNoise > 0.20f && moistureNoise <= 0.55f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.55f && moistureNoise <= 0.75f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.75f){
				return BIOMES.FOREST;
			}
		} else if(temperature > 0.65f && temperature <= 0.85f){
			if(moistureNoise <= 0.2f){
				return BIOMES.TUNDRA;
			}else if(moistureNoise > 0.2f && moistureNoise <= 0.55f){
				return BIOMES.GRASSLAND;
			}else if(moistureNoise > 0.55f && moistureNoise <= 0.75f){
				return BIOMES.WOODLAND;
			}else if(moistureNoise > 0.75f){
				return BIOMES.SNOW;
			}
		} else if(temperature > 0.85f){
			if(moistureNoise <= 0.4f){
				return BIOMES.TUNDRA;
			}else if(moistureNoise > 0.4f){
				return BIOMES.SNOW;
			}
		}
		return BIOMES.DESERT;
	}

	internal void GenerateBareBiome(){
		for(int i = 0; i < GridMap.Instance.listHexes.Count; i++){
			ELEVATION elevationType = GridMap.Instance.listHexes[i].GetComponent<HexTile>().elevationType;
			float moisture = GridMap.Instance.listHexes[i].GetComponent<HexTile>().moistureNoise;

			if(elevationType == ELEVATION.WATER){
				if(moisture <= 0.2f){
					GridMap.Instance.listHexes[i].GetComponent<HexTile>().biomeType = BIOMES.BARE;
					GridMap.Instance.listHexes[i].GetComponent<SpriteRenderer>().color = new Color(186f/255f, 154f/255f, 154f/255f);
				}
			}
		}
	}
	/*
	 * Place each hexagon tile in their respective
	 * biome lists for easy access to each specific biomes
	 * tiles.
	 * */
	void AssignHexTileToList(HexTile tile){
		BIOMES biomeType = tile.biomeType;
		switch (biomeType) {
		case BIOMES.SNOW:
			snowHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		case BIOMES.TUNDRA:
			tundraHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		case BIOMES.GRASSLAND:
			grasslandHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		case BIOMES.WOODLAND:
			woodsHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		case BIOMES.FOREST:
			forestHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		case BIOMES.DESERT:
			desertHexTiles.Add(tile);
			CityGenerator.Instance.tilesElligibleForCities.Add(tile);
			break;
		}
	}

	public void GenerateTileDetails(){
		HexTile currentHexTile = null;
		for (int i = 0; i < GridMap.Instance.listHexes.Count; i++) {
			currentHexTile = GridMap.Instance.listHexes [i].GetComponent<HexTile>();
			if (currentHexTile.elevationType != ELEVATION.WATER) {
				currentHexTile.GetenrateTileDetails ();
			}
		}
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