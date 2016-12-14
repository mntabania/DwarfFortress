using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		cities[0].cityAttributes.connectedCities = new List<CityTile>{cities[6],cities[9]};
		cities[1].cityAttributes.connectedCities = new List<CityTile>{cities[2],cities[7],cities[8]};
		cities[2].cityAttributes.connectedCities = new List<CityTile>{cities[5],cities[7]};
		cities[3].cityAttributes.connectedCities = new List<CityTile>{cities[4]};
		cities[4].cityAttributes.connectedCities = new List<CityTile>{cities[3],cities[9],cities[0]};
		cities[5].cityAttributes.connectedCities = new List<CityTile>{cities[7],cities[1]};
		cities[6].cityAttributes.connectedCities = new List<CityTile>{cities[4],cities[8]};
		cities[7].cityAttributes.connectedCities = new List<CityTile>{cities[1]};
		cities[8].cityAttributes.connectedCities = new List<CityTile>{cities[2]};
		cities[9].cityAttributes.connectedCities = new List<CityTile>{cities[2],cities[5],cities[3]};

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
		capitalCities [0].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.HUMANS, new List<CityTile>(){capitalCities[0]}, "KINGDOM1");
		capitalCities [1].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.ELVES, new List<CityTile>(){capitalCities[1]}, "KINGDOM2");
		capitalCities [2].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.MINGONS, new List<CityTile>(){capitalCities[2]}, "KINGDOM3");
		capitalCities [3].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.CROMADS, new List<CityTile>(){capitalCities[3]}, "KINGDOM4");

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
			CityTile fromCityTile = GetCityWithMostPopulation(this.kingdoms[i].kingdom);
			List<CityTile> citiesForExpansion = new List<CityTile>();
			citiesForExpansion = GetCitiesForExpansion (fromCityTile.cityAttributes);
			if(citiesForExpansion.Count > 0){
				int randomCity = UnityEngine.Random.Range (0, citiesForExpansion.Count);
				Expand (this.kingdoms[i], fromCityTile, citiesForExpansion[randomCity]);
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
	private CityTile GetCityWithMostPopulation(Kingdom kingdom){
		int highestPopulation = 0;
		CityTile cityWithHighestPopulation = null;
		for(int i = 0; i < kingdom.cities.Count; i++){
			int currentPopulation = kingdom.cities [i].cityAttributes.population;
			if(highestPopulation < currentPopulation){
				highestPopulation = currentPopulation;
				cityWithHighestPopulation = kingdom.cities [i];
			}
		}
		return cityWithHighestPopulation;
	}
	private void Expand(KingdomTile kingdomTile, CityTile fromCityTile, CityTile toCityTile){
		toCityTile.cityAttributes.kingdomTile = kingdomTile;
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
