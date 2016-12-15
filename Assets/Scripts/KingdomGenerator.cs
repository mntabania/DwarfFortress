using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class KingdomGenerator : MonoBehaviour {
	public static KingdomGenerator Instance;

	public int[] cityInterval;
	public List<KingdomTile> kingdoms;
	public List<CityTile> capitalCities;
	public List<HexTile> cityHexes;
	public List<CityTile> cities;

	void Awake(){
		Instance = this;
	}

	private void CreateTempCities(){
		for (int i = 0; i < 10; i++){
			cityHexes.Add (GridMap.Instance.listHexes [i + cityInterval[i]].GetComponent<HexTile> ());
		}
		for (int i = 0; i < cityHexes.Count; i++) {
			cityHexes[i].gameObject.GetComponent<SpriteRenderer>().color = Color.black;
			cityHexes[i].isCity = true;
			cityHexes[i].gameObject.AddComponent<CityTile>();
			cityHexes[i].gameObject.GetComponent<CityTile>().cityAttributes = new City(cityHexes[i], cityHexes[i].biomeType);
			this.cities.Add (cityHexes [i].gameObject.GetComponent<CityTile> ());
		}

	}
	private void CreateConnections(){
		cities[0].cityAttributes.connectedCities = new List<CityTile>{cities[6],cities[9],cities[4]};
		cities[1].cityAttributes.connectedCities = new List<CityTile>{cities[2],cities[7],cities[8],cities[5]};
		cities[2].cityAttributes.connectedCities = new List<CityTile>{cities[5],cities[7],cities[1],cities[8],cities[9]};
		cities[3].cityAttributes.connectedCities = new List<CityTile>{cities[4],cities[9]};
		cities[4].cityAttributes.connectedCities = new List<CityTile>{cities[3],cities[9],cities[0],cities[6]};
		cities[5].cityAttributes.connectedCities = new List<CityTile>{cities[7],cities[1],cities[2],cities[9]};
		cities[6].cityAttributes.connectedCities = new List<CityTile>{cities[4],cities[8],cities[0]};
		cities[7].cityAttributes.connectedCities = new List<CityTile>{cities[1],cities[2],cities[5]};
		cities[8].cityAttributes.connectedCities = new List<CityTile>{cities[2],cities[1],cities[6]};
		cities[9].cityAttributes.connectedCities = new List<CityTile>{cities[2],cities[5],cities[3],cities[0],cities[4]};

	}
	private void DrawConnections(){
		for(int i = 0; i < cities.Count; i++){
			for(int z = 0; z < cities[i].cityAttributes.connectedCities.Count; z++){
				GLDebug.DrawLine (cities[i].transform.position, cities[i].cityAttributes.connectedCities[z].transform.position, Color.black, 10000f);

			}
		}
	}
	private void AddCapitalCities(){
		capitalCities.Add (cityHexes[0].gameObject.GetComponent<CityTile> ());
		capitalCities.Add (cityHexes[3].gameObject.GetComponent<CityTile> ());
		capitalCities.Add (cityHexes[5].gameObject.GetComponent<CityTile> ());
		capitalCities.Add (cityHexes[7].gameObject.GetComponent<CityTile> ());

		capitalCities [0].cityAttributes.population = capitalCities [0].cityAttributes.GeneratePopulation();
		capitalCities [1].cityAttributes.population = capitalCities [1].cityAttributes.GeneratePopulation();
		capitalCities [2].cityAttributes.population = capitalCities [2].cityAttributes.GeneratePopulation();
		capitalCities [3].cityAttributes.population = capitalCities [3].cityAttributes.GeneratePopulation();
	}

	internal void GenerateInitialKingdoms(){
		CreateTempCities ();
		AddCapitalCities ();
		capitalCities [0].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.HUMANS, new List<CityTile>(){capitalCities[0]}, "KINGDOM1", new Color(255f/255f, 0f/255f, 206f/255f));
		capitalCities [1].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.ELVES, new List<CityTile>(){capitalCities[1]}, "KINGDOM2", new Color(40f/255f, 255f/255f, 0f/255f));
		capitalCities [2].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.MINGONS, new List<CityTile>(){capitalCities[2]}, "KINGDOM3", new Color(0f/255f, 234f/255f, 255f/255f));
		capitalCities [3].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.CROMADS, new List<CityTile>(){capitalCities[3]}, "KINGDOM4", new Color(157f/255f, 0f/255f, 255f/255f));

		for(int i = 0; i < capitalCities.Count; i++){
			kingdoms.Add (capitalCities [i].gameObject.GetComponent<KingdomTile> ());
		}
		CreateConnections ();
		DrawConnections ();
	}
	internal void OnTurn(){
		TriggerEvents ();
	}
	private void TriggerEvents(){
		GrowPopulation ();
		TriggerExpandEvent ();
	}
	private void TriggerExpandEvent(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			List<CityTile> fromCityTile = GetCitiesInOrderOfPopulation(this.kingdoms[i].kingdom);
			List<CityTile> citiesForExpansion = new List<CityTile>();
			for(int j = 0; j < fromCityTile.Count; j++){
				citiesForExpansion = GetCitiesForExpansion (fromCityTile[j].cityAttributes);
				if(citiesForExpansion.Count > 0){
					int randomCity = UnityEngine.Random.Range (0, citiesForExpansion.Count);
					Expand (this.kingdoms[i], fromCityTile[j], citiesForExpansion[randomCity]);
					Debug.Log (this.kingdoms [i].kingdom.kingdomName + " EXPANDED FROM " + fromCityTile [j].cityAttributes.hexTile.name + " TO " + citiesForExpansion [randomCity].cityAttributes.hexTile.name);
					break;
				}
			}

		}
	}
	private List<CityTile> GetCitiesForExpansion(City city){
		List<CityTile> citiesForExpansion = new List<CityTile> ();
		for(int j = 0; j < city.connectedCities.Count; j++){
			if(city.connectedCities[j].cityAttributes.kingdomTile == null){
				citiesForExpansion.Add (city.connectedCities [j]);
			}
		}
		return citiesForExpansion;
	}
	private List<CityTile> GetCitiesInOrderOfPopulation(Kingdom kingdom){
		List<CityTile> orderedCity = new List<CityTile> ();
		for (int i = 0; i < kingdom.cities.Count; i++) {
			orderedCity.Add (kingdom.cities [i]);
		}
		orderedCity = orderedCity.OrderByDescending (i => i.cityAttributes.population).ToList ();
		return orderedCity;
//		int highestPopulation = 0;
//		CityTile cityWithHighestPopulation = null;
//		for(int i = 0; i < kingdom.cities.Count; i++){
//			int currentPopulation = kingdom.cities [i].cityAttributes.population;
//			if(highestPopulation < currentPopulation){
//				highestPopulation = currentPopulation;
//				cityWithHighestPopulation = kingdom.cities [i];
//			}
//		}
//		return cityWithHighestPopulation;
	}
	private void Expand(KingdomTile kingdomTile, CityTile fromCityTile, CityTile toCityTile){
		toCityTile.cityAttributes.kingdomTile = kingdomTile;
		toCityTile.GetComponent<SpriteRenderer> ().color = kingdomTile.kingdom.tileColor;
		kingdomTile.kingdom.cities.Add (toCityTile);
		int populationDecrease = (int)(fromCityTile.cityAttributes.population * 0.2f);
		toCityTile.cityAttributes.population += populationDecrease;
		fromCityTile.cityAttributes.population -= populationDecrease;
	}
	private void GrowPopulation(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			float growthPercentage = this.kingdoms[i].kingdom.populationGrowth / 100f;
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				City city = this.kingdoms [i].kingdom.cities [j].cityAttributes;
				float populationIncrease = city.population * growthPercentage;
				city.population += (int)populationIncrease;
			}
		}
	}
}
