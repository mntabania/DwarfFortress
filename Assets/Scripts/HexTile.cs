using UnityEngine;
using System.Collections;

public class HexTile : MonoBehaviour {

	public float elevationNoise;
	public float moistureNoise;
	public float temperature;

	public BIOMES biomeType;
	public ELEVATION elevationType;


//		switch(biomeType){
//		case BIOME.OCEAN:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f/255f,1f/255f,105f/255f);
//			break;
//		case BIOME.BEACH:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f/255f,1f/255f,105f/255f);
//			break;
//		case BIOME.SUBTROPICAL_DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(231f/255f,221f/255f,196f/255f);
//			break;
//		case BIOME.GRASSLAND:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(194f/255f,213f/255f,168f/255f);
//			break;
//		case BIOME.TEMPERATE_DECIDUOUS_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(178f/255f,202f/255f,168f/255f);
//			break;
//		case BIOME.TEMPERATE_RAIN_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(162f/255f,197f/255f,169f/255f);
//			break;
//		case BIOME.TEMPERATE_DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(226f/255f,231f/255f,201f/255f);
//			break;
//		case BIOME.SHRUBLAND:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(195f/255f,204f/255f,186f/255f);
//			break;
//		case BIOME.TAIGA:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(203f/255f,212f/255f,185f/255f);
//			break;
//		case BIOME.SCORCHED:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(153f/255f,153f/255f,153f/255f);
//			break;
//		case BIOME.BARE:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(186f/255f,186f/255f,186f/255f);
//			break;
//		case BIOME.TUNDRA:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(220f/255f,221f/255f,187f/255f);
//			break;
//		case BIOME.SNOW:
//			this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
//			break;
//		case BIOME.TROPICAL_RAIN_FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(156f/255f,188f/255f,167f/255f);
//			break;
//		case BIOME.FOREST:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(156f/255f,188f/255f,167f/255f);
//			break;
//		case BIOME.JUNGLE:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(194f/255f,213f/255f,168f/255f);
//			break;
//		case BIOME.SAVANNAH:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(231f/255f,221f/255f,196f/255f);
//			break;
//		case BIOME.DESERT:
//			this.gameObject.GetComponent<SpriteRenderer>().color = new Color(234f/255f,194f/255f,60f/255f);
//			break;
//		}
//	}

	private BIOME GetBiome(){

		if(elevationNoise <= 0.35f){
			if(elevationNoise < 0.30f){
				return BIOME.OCEAN;
			}
			return BIOME.BEACH;
		}else{
			if(elevationNoise > 0.3f && elevationNoise < 0.6f){
				if(moistureNoise < 0.16f){
					return BIOME.SUBTROPICAL_DESERT;
				}
				if(moistureNoise < 0.50f){
					return BIOME.GRASSLAND;
				}
				if(moistureNoise < 0.83f){
					return BIOME.TEMPERATE_DECIDUOUS_FOREST;
				}
				return BIOME.TEMPERATE_RAIN_FOREST;
			}
			else if(elevationNoise >= 0.6f && elevationNoise < 0.8f){
				if(moistureNoise < 0.33f){
					return BIOME.TEMPERATE_DESERT;
				}
				if(moistureNoise < 0.66f){
					return BIOME.SHRUBLAND;
				}
				return BIOME.TAIGA;
			}
			else if(elevationNoise >= 0.8f){
				if(moistureNoise < 0.1f){
					return BIOME.SCORCHED;
				}
				if(moistureNoise < 0.2f){
					return BIOME.BARE;
				}
				if(moistureNoise < 0.5f){
					return BIOME.TUNDRA;
				}
				return BIOME.SNOW;
			}
		}		
		return BIOME.TROPICAL_RAIN_FOREST;

	}

}
