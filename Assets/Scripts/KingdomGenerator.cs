using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class KingdomGenerator : MonoBehaviour {
	public static KingdomGenerator Instance;

	public GameObject goKingdomTile;
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
//		CreateTempCities ();
//		AddCapitalCities ();
//		capitalCities [0].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.HUMANS, new List<CityTile>(){capitalCities[0]}, "KINGDOM1", new Color(255f/255f, 0f/255f, 206f/255f));
//		capitalCities [1].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.ELVES, new List<CityTile>(){capitalCities[1]}, "KINGDOM2", new Color(40f/255f, 255f/255f, 0f/255f));
//		capitalCities [2].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.MINGONS, new List<CityTile>(){capitalCities[2]}, "KINGDOM3", new Color(0f/255f, 234f/255f, 255f/255f));
//		capitalCities [3].gameObject.AddComponent<KingdomTile> ().CreateKingdom (5f, RACE.CROMADS, new List<CityTile>(){capitalCities[3]}, "KINGDOM4", new Color(157f/255f, 0f/255f, 255f/255f));

		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (goKingdomTile);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.HUMANS, new List<CityTile>(){CityGenerator.Instance.capitalCities[0]}, "KINGDOM1", new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom1.GetComponent<KingdomTile>());


		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (goKingdomTile);
		goKingdom2.transform.parent = this.transform;
		goKingdom2.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.ELVES, new List<CityTile>(){CityGenerator.Instance.capitalCities[1]}, "KINGDOM2", new Color(40f/255f, 255f/255f, 0f/255f));
		goKingdom2.name = goKingdom2.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom2.GetComponent<KingdomTile>());


		GameObject goKingdom3 = (GameObject)GameObject.Instantiate (goKingdomTile);
		goKingdom3.transform.parent = this.transform;
		goKingdom3.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.MINGONS, new List<CityTile>(){CityGenerator.Instance.capitalCities[2]}, "KINGDOM3", new Color(0f/255f, 234f/255f, 255f/255f));
		goKingdom3.name = goKingdom3.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom3.GetComponent<KingdomTile>());


		GameObject goKingdom4 = (GameObject)GameObject.Instantiate (goKingdomTile);
		goKingdom4.transform.parent = this.transform;
		goKingdom4.GetComponent<KingdomTile>().CreateKingdom (5f, RACE.CROMADS, new List<CityTile>(){CityGenerator.Instance.capitalCities[3]}, "KINGDOM4", new Color(157f/255f, 0f/255f, 255f/255f));
		goKingdom4.name = goKingdom4.GetComponent<KingdomTile> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom4.GetComponent<KingdomTile>());


//		for(int i = 0; i < capitalCities.Count; i++){
//			kingdoms.Add (capitalCities [i].gameObject.GetComponent<KingdomTile> ());
//		}
//		CreateConnections ();
//		DrawConnections ();
		CreateInitialFactions ();
	}
	private void CreateInitialFactions(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			Religion religion = ReligionGenerator.Instance.GenerateReligion ();
			Culture culture = CultureGenerator.Instance.GenerateCulture ();
			if(religion == null || culture == null){
				Debug.Log ("NO MORE RELIGIONS OR CULTURE. CAN'T CREATE NEW.");
				break;
			}
			Faction faction = CreateNewFaction (this.kingdoms [i].kingdom.race, religion, culture);
			this.kingdoms [i].kingdom.factions.Add (faction);
			FactionStorage.Instance.AddFaction (faction);
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				this.kingdoms [i].kingdom.cities [j].cityAttributes.faction = this.kingdoms [i].kingdom.factions [0];
			}
		}
	}
	private Faction CreateNewFaction(RACE race, Religion religion, Culture culture){
		Faction faction = new Faction ();
		faction.id = Utilities.lastfactionid += 1;
		faction.factionName = "FACTION" + faction.id;
		faction.race = race;
		faction.religion = religion;
		faction.culture = culture;

		return faction;
	}
	internal void OnTurn(){
		TriggerEvents ();
	}
	private void TriggerEvents(){
//		CheckEnemyKingdoms ();
		GrowPopulation ();
		GenerateArmy ();
		TriggerExpandEvent ();
		ListAdjacentKingdoms ();
		TriggerDeclarePeaceEvent ();
		TriggerDeclareWarEvent ();
		TriggerInvadeEvent ();
		TriggerFactionEvents ();
	}
	private void TriggerExpandEvent(){
		for(int i = 0; i < this.kingdoms.Count; i++){
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("EXPAND");
			int expandPercentage = (5 + (2 * this.kingdoms[i].kingdom.ambition) + this.kingdoms[i].kingdom.performance);
			int expandChance = UnityEngine.Random.Range (0, 100);
			if(expandChance >= 0 && expandChance <= expandPercentage){
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
		toCityTile.cityAttributes.faction = fromCityTile.cityAttributes.faction;
		toCityTile.GetComponent<SpriteRenderer> ().color = kingdomTile.kingdom.tileColor;
		kingdomTile.kingdom.cities.Add (toCityTile);
		int populationDecrease = (int)(fromCityTile.cityAttributes.population * 0.2f);
		toCityTile.cityAttributes.population += populationDecrease;
		fromCityTile.cityAttributes.population -= populationDecrease;
	}
	private void GrowPopulation(){
		for(int i = 0; i < this.kingdoms.Count; i++){
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("GROW POPULATION");
			float growthPercentage = this.kingdoms[i].kingdom.populationGrowth / 100f;
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				City city = this.kingdoms [i].kingdom.cities [j].cityAttributes;
				float populationIncrease = city.population * growthPercentage;
				city.population += (int)populationIncrease;
			}
		}
	}
	private void GenerateArmy(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("GENERATE ARMY");
			float increaseArmyPercentage = UnityEngine.Random.Range (0.5f, 1.5f);
			int totalPopulation = 0;
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				totalPopulation += this.kingdoms [i].kingdom.cities [j].cityAttributes.population;
			}
			int armyIncrease = (int)(((increaseArmyPercentage/100f) * totalPopulation) * this.kingdoms [i].kingdom.performance);
			this.kingdoms [i].kingdom.army += armyIncrease;
		}
	}
	internal void ListAdjacentKingdoms(){
		for(int i = 0; i < this.kingdoms.Count; i++){
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("ListAdjacentKingdoms");
			this.kingdoms [i].kingdom.adjacentKingdoms.Clear();
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				for(int k = 0; k < this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities.Count; k++){
					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile != null){
						if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile.kingdom.id != this.kingdoms[i].kingdom.id){
							this.kingdoms [i].kingdom.adjacentKingdoms.Add (this.kingdoms [i].kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile);
						}
					}
				}
			}
			this.kingdoms [i].kingdom.adjacentKingdoms = this.kingdoms [i].kingdom.adjacentKingdoms.Distinct ().ToList();
		}
	}
	private void TriggerDeclareWarEvent(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("WAR");
			if(this.kingdoms [i].kingdom.adjacentKingdoms.Count <= 0){
				return;
			}
			List<KingdomTile> targetKingdoms = new List<KingdomTile> ();
			targetKingdoms = GetAdjacentKingdomsInOrderOfArmy (this.kingdoms [i]);
			bool isEnemy = false;
			for (int j = 0; j < targetKingdoms.Count; j++) {
//				if(targetKingdoms[j].kingdom.cities.Count <= 0){
//					continue;
//				}
				isEnemy = false;
				for(int k = 0; k < targetKingdoms[j].kingdom.enemyKingdoms.Count; k++){
					if(targetKingdoms[j].kingdom.enemyKingdoms[k].kingdom.kingdomName == this.kingdoms[i].kingdom.kingdomName){
						isEnemy = true;
						break;
					}
				}

				if(isEnemy){
					continue;
				}
				int warCount = this.kingdoms[i].kingdom.enemyKingdoms.Count;
				float warPercentage = 5f + ((((3f - (float)warCount) * (float)this.kingdoms [i].kingdom.ambition) - (float)this.kingdoms [i].kingdom.altruism) * ((float)this.kingdoms [i].kingdom.army / (float)targetKingdoms[j].kingdom.army));
				int warChance = UnityEngine.Random.Range (0, 100);
				if (warChance >= 0 && warChance <= warPercentage){
					DeclareWar (this.kingdoms[i], targetKingdoms[j]);
					Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED WAR WITH " + targetKingdoms[j].kingdom.kingdomName);
					break;
				}
			}
//			KingdomTile targetKingdom = GetAdjacentWeakestArmy (this.kingdoms [i].kingdom.adjacentKingdoms);
		}
//		Debug.Log ("WAR");

	}
	private void DeclareWar(KingdomTile currentKingdom, KingdomTile targetKingdom){
		currentKingdom.kingdom.enemyKingdoms.Add (targetKingdom);
		targetKingdom.kingdom.enemyKingdoms.Add (currentKingdom);
		currentKingdom.kingdom.citiesGained.Add (0);
		currentKingdom.kingdom.citiesLost.Add (0);
		targetKingdom.kingdom.citiesGained.Add (0);
		targetKingdom.kingdom.citiesLost.Add (0);
	}
	private List<KingdomTile> GetAdjacentKingdomsInOrderOfArmy(KingdomTile kingdomTile){
		List<KingdomTile> orderedKingdoms = new List<KingdomTile> ();
		for (int i = 0; i < kingdomTile.kingdom.adjacentKingdoms.Count; i++) {
			orderedKingdoms.Add (kingdomTile.kingdom.adjacentKingdoms[i]);
		}
		orderedKingdoms = orderedKingdoms.OrderBy (i => i.kingdom.army).ToList ();
		return orderedKingdoms;
	}

	private void TriggerInvadeEvent(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
			if(this.kingdoms [i].kingdom.enemyKingdoms.Count <= 0){
				continue;
			}
			KingdomTile targetKingdom = GetEnemyWeakestArmy (this.kingdoms [i].kingdom.enemyKingdoms);
			if(targetKingdom == null){
				continue;
			}
			List <CityTile> cityTilesConnected = GetCityTilesConnected (this.kingdoms [i], targetKingdom);
			int randomCityInvader = UnityEngine.Random.Range (0, cityTilesConnected.Count);

			CityTile cityInvader = cityTilesConnected [randomCityInvader];
			CityTile cityTarget = GetRandomConnectedCity(cityInvader, targetKingdom);


			float invadePercentage = 10f + ((4f * (10f - (float)this.kingdoms [i].kingdom.altruism)) * ((float)this.kingdoms [i].kingdom.army / (float)targetKingdom.kingdom.army));
			int invadeChance = UnityEngine.Random.Range (0, 100);
			if (invadeChance >= 0 && invadeChance <= invadePercentage) {
				Invade (this.kingdoms[i], targetKingdom, cityInvader, cityTarget);
			}
		}
//		Debug.Log ("INVADE");
	}
	private void Invade(KingdomTile invaderKingdom, KingdomTile targetKingdom, CityTile fromCityTile, CityTile toCityTile){
		Debug.Log (invaderKingdom.kingdom.kingdomName + " ATTEMPTS TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		float invaderArmyLossPercentage = UnityEngine.Random.Range (10f, 20f);
		float targetArmyLossPercentage = UnityEngine.Random.Range (5f, 10f);

		int invaderArmyLoss = (int)((invaderArmyLossPercentage/100f) * (float)invaderKingdom.kingdom.army);
		invaderKingdom.kingdom.army -= invaderArmyLoss;

		int targetArmyLoss = (int)((targetArmyLossPercentage/100f) * (float)targetKingdom.kingdom.army);
		targetKingdom.kingdom.army -= targetArmyLoss;

		float successRate = 50f * ((float)invaderKingdom.kingdom.army / (float)targetKingdom.kingdom.army);
		int successChance = UnityEngine.Random.Range (0, 100);
		if(successChance >= 0 && successChance <= successRate){
			toCityTile.cityAttributes.kingdomTile = invaderKingdom;
			toCityTile.cityAttributes.faction = fromCityTile.cityAttributes.faction;
			toCityTile.GetComponent<SpriteRenderer> ().color = invaderKingdom.kingdom.tileColor;
			invaderKingdom.kingdom.cities.Add (toCityTile);
			targetKingdom.kingdom.cities.Remove (toCityTile);
			for(int i = 0; i < invaderKingdom.kingdom.enemyKingdoms.Count; i++){
				if(invaderKingdom.kingdom.enemyKingdoms[i].kingdom.id == targetKingdom.kingdom.id){
					invaderKingdom.kingdom.citiesGained [i] += 1;
					break;
				}
			}
			for(int i = 0; i < targetKingdom.kingdom.enemyKingdoms.Count; i++){
				if(targetKingdom.kingdom.enemyKingdoms[i].kingdom.id == invaderKingdom.kingdom.id){
					targetKingdom.kingdom.citiesLost [i] += 1;
					break;
				}
			}
			CheckEnemyKingdoms ();
			Debug.Log (invaderKingdom.kingdom.kingdomName + " SUCCESSFULLY INVADED " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		}else{
			Debug.Log (invaderKingdom.kingdom.kingdomName + " HAS FAILED TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		}

		Debug.Log (invaderKingdom.kingdom.kingdomName + " ARMY LOSS: " + invaderArmyLoss + " (" + invaderArmyLossPercentage + "%)");
		Debug.Log (targetKingdom.kingdom.kingdomName + " ARMY LOSS: " + targetArmyLoss + " (" + targetArmyLossPercentage + "%)");

	}
	private List<CityTile> GetCityTilesConnected(KingdomTile kingdomTile, KingdomTile targetKingdom){
		List <CityTile> cityTilesConnected = new List<CityTile> ();
		for(int j = 0; j < kingdomTile.kingdom.cities.Count; j++){
			if(kingdomTile.kingdom.cities[j].cityAttributes.kingdomTile != null){
				for(int k = 0; k < kingdomTile.kingdom.cities[j].cityAttributes.connectedCities.Count; k++){
					if (kingdomTile.kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile != null){
						if (kingdomTile.kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile.kingdom.kingdomName == targetKingdom.kingdom.kingdomName) {
							cityTilesConnected.Add (kingdomTile.kingdom.cities [j]);
							break;
						}
					}
				}
			}
		}
		return cityTilesConnected;
	}
	private CityTile GetRandomConnectedCity(CityTile cityTile, KingdomTile targetKingdom){
		List <CityTile> cityTilesConnected = new List<CityTile> ();
		for(int j = 0; j < cityTile.cityAttributes.connectedCities.Count; j++){
			if (cityTile.cityAttributes.connectedCities [j].cityAttributes.kingdomTile != null) {
				if (cityTile.cityAttributes.connectedCities [j].cityAttributes.kingdomTile.kingdom.kingdomName == targetKingdom.kingdom.kingdomName) {
					cityTilesConnected.Add (cityTile.cityAttributes.connectedCities [j]);
				}
			}
		}
		int randomCityTarget = UnityEngine.Random.Range (0, cityTilesConnected.Count);
		return cityTilesConnected[randomCityTarget];
	}
	private KingdomTile GetEnemyWeakestArmy(List<KingdomTile> enemyKingdoms){
		int weakestArmy = 0;
		KingdomTile kingdomWithWeakestArmy = null;
		for(int i = 0; i < enemyKingdoms.Count; i++){
//			if(enemyKingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
			weakestArmy = enemyKingdoms [i].kingdom.army;
			kingdomWithWeakestArmy = enemyKingdoms [i];
			break;
		}
		for(int i = 0; i < enemyKingdoms.Count; i++){
//			if(enemyKingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
			int currentArmy = enemyKingdoms[i].kingdom.army;
			if(weakestArmy > currentArmy){
				weakestArmy = currentArmy;
				kingdomWithWeakestArmy = enemyKingdoms[i];
			}
		}
		return kingdomWithWeakestArmy;
	}

	private void TriggerDeclarePeaceEvent(){
		for(int i = 0; i < this.kingdoms.Count; i++){
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
			if(this.kingdoms[i].kingdom.enemyKingdoms.Count <= 0){
				continue;
			}
			KingdomTile targetKingdom = GetRandomPeaceKingdom (this.kingdoms [i]);
			int citiesGained = 0;
			int citiesLost = 0;
			for(int j = 0; j < this.kingdoms[i].kingdom.enemyKingdoms.Count; j++){
//				if(this.kingdoms[i].kingdom.enemyKingdoms[j].kingdom.cities.Count <= 0){
//					continue;
//				}
				if(this.kingdoms[i].kingdom.enemyKingdoms[j].kingdom.id == targetKingdom.kingdom.id){
					citiesGained = this.kingdoms [i].kingdom.citiesGained [j];
					citiesLost = this.kingdoms [i].kingdom.citiesLost [j];
					break;
				}
			}
			float peacePercentage = 5f + ((8f * ((float)citiesLost + (float)citiesGained)) * ((float)targetKingdom.kingdom.army / (float)this.kingdoms [i].kingdom.army));
			int peaceChance = UnityEngine.Random.Range (0, 100);

			if(peaceChance >= 0 && peaceChance <= peacePercentage){
				DeclarePeace (this.kingdoms [i], targetKingdom);
				Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED PEACE WITH " + targetKingdom.kingdom.kingdomName);
			}
		}
//		Debug.Log ("PEACE");

	}
	private void DeclarePeace(KingdomTile fromKingdomTile, KingdomTile toKingdomTile){
		for(int i = 0; i < fromKingdomTile.kingdom.enemyKingdoms.Count; i++){
			if(fromKingdomTile.kingdom.enemyKingdoms[i].kingdom.id == toKingdomTile.kingdom.id){
				fromKingdomTile.kingdom.citiesGained.RemoveAt (i);
				fromKingdomTile.kingdom.citiesLost.RemoveAt (i);
				fromKingdomTile.kingdom.enemyKingdoms.RemoveAt (i);
				break;
			}
		}
		for(int i = 0; i < toKingdomTile.kingdom.enemyKingdoms.Count; i++){
			if(toKingdomTile.kingdom.enemyKingdoms[i].kingdom.id == fromKingdomTile.kingdom.id){
				toKingdomTile.kingdom.citiesGained.RemoveAt (i);
				toKingdomTile.kingdom.citiesLost.RemoveAt (i);
				toKingdomTile.kingdom.enemyKingdoms.RemoveAt (i);
				break;
			}
		}
	}
	private KingdomTile GetRandomPeaceKingdom(KingdomTile kingdomTile){
		List <KingdomTile> enemyKingdoms = new List<KingdomTile> ();
		for(int j = 0; j < kingdomTile.kingdom.enemyKingdoms.Count; j++){
//			if(kingdomTile.kingdom.enemyKingdoms[j].kingdom.cities.Count <= 0){
//				continue;
//			}
			enemyKingdoms.Add (kingdomTile.kingdom.enemyKingdoms [j]);
		}
		int randomPeaceKingdom = UnityEngine.Random.Range (0, enemyKingdoms.Count);
		return enemyKingdoms[randomPeaceKingdom];
	}

	private void CheckEnemyKingdoms(){
		List <int> index = new List<int> ();
		List <KingdomTile> kingdoms = new List<KingdomTile> ();
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(this.kingdoms[i].kingdom.cities.Count <= 0){
				kingdoms.Add (this.kingdoms [i]);
			}
			index.Clear ();
			for(int j = 0; j < this.kingdoms[i].kingdom.enemyKingdoms.Count; j++){
				if(this.kingdoms[i].kingdom.enemyKingdoms[j].kingdom.cities.Count <= 0){
					index.Add (j);
				}
			}
			for (int j = 0; j < index.Count; j++) {
				this.kingdoms [i].kingdom.enemyKingdoms.RemoveAt (index[j]);
				this.kingdoms [i].kingdom.citiesGained.RemoveAt (index[j]);
				this.kingdoms [i].kingdom.citiesLost.RemoveAt (index[j]);
			}
		}

		for(int i = 0; i < kingdoms.Count; i++){
			this.kingdoms.Remove (kingdoms[i]);
		}
	}

	private void TriggerFactionEvents(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			int triggerChance = UnityEngine.Random.Range (0, 100);
			if(triggerChance >= 0 && triggerChance < 50){
				int randomCity = UnityEngine.Random.Range (0, this.kingdoms [i].kingdom.cities.Count);
				CityTile randomCityTile = this.kingdoms [i].kingdom.cities [randomCity];

				int factionEventsChance = UnityEngine.Random.Range (0, 100);
				if(factionEventsChance >= 0 && factionEventsChance < 25){ //change faction
					Debug.Log("CHANGE FACTION EVENT!");
					ChangeFaction (randomCityTile);
				}else if(factionEventsChance >= 25 && factionEventsChance < 50){ //split faction
					Debug.Log("SPLIT FACTION EVENT!");
					SplitFaction (this.kingdoms [i]);
				}else{ //influence faction
					Debug.Log("INFLUENCE FACTION EVENT!");
					InfluenceFaction (randomCityTile);
				}
			}
		}
	}

	private void ChangeFaction(CityTile cityTile){
		if(cityTile.cityAttributes.kingdomTile.kingdom.factions.Count < 2){
			Debug.Log (cityTile.cityAttributes.kingdomTile.kingdom.kingdomName + " HAS 1 OR LOWER FACTION COUNT. CAN'T CHANGE RELIGION OR CULTURE.");
			return;
		}
		int randomNumber = UnityEngine.Random.Range (0, 2);
		if(randomNumber == 0){ //religion change
			Religion religion = ReligionGenerator.Instance.GenerateReligion();
			if(religion == null){
				Debug.Log ("NO MORE RELIGIONS. CAN'T CREATE NEW.");
				return;
			}
			cityTile.cityAttributes.faction.religion = religion;
			Debug.Log (cityTile.cityAttributes.hexTile.name + " RELIGION HAS CHANGED!");
		}else{ //culture change
			Culture culture =  CultureGenerator.Instance.GenerateCulture();
			if(culture == null){
				Debug.Log ("NO MORE CULTURES. CAN'T CREATE NEW.");
				return;
			}
			cityTile.cityAttributes.faction.culture = culture;
			Debug.Log (cityTile.cityAttributes.hexTile.name + " CULTURE HAS CHANGED!");
		}
		Faction faction = FactionStorage.Instance.CheckFaction (cityTile.cityAttributes.faction);
		if(faction == null){
			Debug.Log ("FACTION NON EXISTENT. CREATING NEW FACTION....");
			cityTile.cityAttributes.faction = CreateNewFaction (cityTile.cityAttributes.faction.race, cityTile.cityAttributes.faction.religion, cityTile.cityAttributes.faction.culture);
			cityTile.cityAttributes.kingdomTile.kingdom.factions.Add (cityTile.cityAttributes.faction);
			FactionStorage.Instance.AddFaction (cityTile.cityAttributes.faction);
		}else{
			Debug.Log ("FACTION ALREADY EXISTS. RETRIEVING FACTION....");
			cityTile.cityAttributes.faction = faction;
		}
	}

	private void InfluenceFaction(CityTile fromCityTile){
		int randomNumber = UnityEngine.Random.Range (0, 2);
		CityTile targetCityTile = null;
		if(randomNumber == 0){ //influence religion
			List<int> religionIndexes = GetConnectedCitiesIndexesWithDiffReligion(fromCityTile);
			if(religionIndexes.Count > 0){
				int randomTargetCity = UnityEngine.Random.Range (0, religionIndexes.Count);
				CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [religionIndexes[randomTargetCity]];
				toCityTile.cityAttributes.faction.religion = fromCityTile.cityAttributes.faction.religion;
				targetCityTile = toCityTile;
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS RELIGION TO " + toCityTile.cityAttributes.hexTile.name);
			}
			else{
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION. CHECKING CULTURE....");
				List<int> cultureIndexes = GetConnectedCitiesIndexesWithDiffCulture(fromCityTile);
				if(cultureIndexes.Count > 0){
					int randomTargetCity = UnityEngine.Random.Range (0, cultureIndexes.Count);
					CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [cultureIndexes[randomTargetCity]];
					toCityTile.cityAttributes.faction.culture = fromCityTile.cityAttributes.faction.culture;
					targetCityTile = toCityTile;
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS CULTURE TO " + toCityTile.cityAttributes.hexTile.name);
				}else{
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION OR CULTURE. CAN'T INFLUENCE.");
				}
			}
		}else{ //influence culture
			List<int> cultureIndexes = GetConnectedCitiesIndexesWithDiffCulture(fromCityTile);
			if(cultureIndexes.Count > 0){
				int randomTargetCity = UnityEngine.Random.Range (0, cultureIndexes.Count);
				CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [cultureIndexes[randomTargetCity]];
				toCityTile.cityAttributes.faction.culture = fromCityTile.cityAttributes.faction.culture;
				targetCityTile = toCityTile;
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS CULTURE TO " + toCityTile.cityAttributes.hexTile.name);
			}else{
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE. CHECKING RELIGION....");
				List<int> religionIndexes = GetConnectedCitiesIndexesWithDiffReligion(fromCityTile);
				if(religionIndexes.Count > 0){
					int randomTargetCity = UnityEngine.Random.Range (0, religionIndexes.Count);
					CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [religionIndexes[randomTargetCity]];
					toCityTile.cityAttributes.faction.religion = fromCityTile.cityAttributes.faction.religion;
					targetCityTile = toCityTile;
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS RELIGION TO " + toCityTile.cityAttributes.hexTile.name);
				}
				else{
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE OR RELIGION. CAN'T INFLUENCE.");
				}
			}

		}
		if(targetCityTile != null){
			Faction faction = FactionStorage.Instance.CheckFaction (targetCityTile.cityAttributes.faction);
			if(faction == null){
				Debug.Log ("FACTION NON EXISTENT. CREATING NEW FACTION....");
				targetCityTile.cityAttributes.faction = CreateNewFaction (targetCityTile.cityAttributes.faction.race, targetCityTile.cityAttributes.faction.religion, targetCityTile.cityAttributes.faction.culture);
				targetCityTile.cityAttributes.kingdomTile.kingdom.factions.Add (targetCityTile.cityAttributes.faction);
				FactionStorage.Instance.AddFaction (targetCityTile.cityAttributes.faction);
			}else{
				Debug.Log ("FACTION ALREADY EXISTS. RETRIEVING FACTION....");
				targetCityTile.cityAttributes.faction = faction;
			}
		}
	}
	private List<int> GetConnectedCitiesIndexesWithDiffReligion(CityTile cityTile){
		List<int> indexes = new List<int> ();
		for(int i = 0; i < cityTile.cityAttributes.connectedCities.Count; i++){
			if(cityTile.cityAttributes.connectedCities[i].cityAttributes.faction != null){
				if(cityTile.cityAttributes.connectedCities[i].cityAttributes.faction.religion != cityTile.cityAttributes.faction.religion){
					indexes.Add (i);
				}
			}
		}
		return indexes;
	}
	private List<int> GetConnectedCitiesIndexesWithDiffCulture(CityTile cityTile){
		List<int> indexes = new List<int> ();
		for(int i = 0; i < cityTile.cityAttributes.connectedCities.Count; i++){
			if (cityTile.cityAttributes.connectedCities [i].cityAttributes.faction != null) {
				if (cityTile.cityAttributes.connectedCities [i].cityAttributes.faction.culture != cityTile.cityAttributes.faction.culture) {
					indexes.Add (i);
				}
			}
		}
		return indexes;
	}
	private void SplitFaction(KingdomTile kingdomTile){
		if(kingdomTile.kingdom.factions.Count > 1){
			int randomFaction = UnityEngine.Random.Range (0, kingdomTile.kingdom.factions.Count);
			Faction rebelFaction = kingdomTile.kingdom.factions [randomFaction];
			List <CityTile> newCityTiles = new List<CityTile> ();
			for(int i = 0; i < kingdomTile.kingdom.cities.Count; i++){
				if(kingdomTile.kingdom.cities[i].cityAttributes.faction == rebelFaction){
					newCityTiles.Add (kingdomTile.kingdom.cities [i]);
				}
			}
			GameObject goNewKingdom = (GameObject)GameObject.Instantiate (goKingdomTile);
			goNewKingdom.transform.parent = this.transform;
			goNewKingdom.GetComponent<KingdomTile>().CreateKingdom (kingdomTile.kingdom.populationGrowth, kingdomTile.kingdom.race, newCityTiles, "KINGDOM", new Color((float)UnityEngine.Random.Range(0,256)/255f, (float)UnityEngine.Random.Range(0,256)/255f, (float)UnityEngine.Random.Range(0,256)/255f));
			goNewKingdom.GetComponent<KingdomTile> ().kingdom.kingdomName = goNewKingdom.GetComponent<KingdomTile> ().kingdom.kingdomName + goNewKingdom.GetComponent<KingdomTile> ().kingdom.id;
			goNewKingdom.GetComponent<KingdomTile> ().kingdom.army = (int)(((float)newCityTiles.Count / (float)kingdomTile.kingdom.cities.Count) * (float)kingdomTile.kingdom.army);
			goNewKingdom.GetComponent<KingdomTile> ().kingdom.factions.Add (rebelFaction);
			goNewKingdom.name = goNewKingdom.GetComponent<KingdomTile> ().kingdom.kingdomName;

			kingdoms.Add (goNewKingdom.GetComponent<KingdomTile>());

			Debug.Log (rebelFaction.factionName + " HAS SPLIT WITH " + kingdomTile.kingdom.kingdomName);
			Debug.Log ("CREATED NEW KINGDOM: " + goNewKingdom.name);
			ListAdjacentKingdoms ();
		}else{
			Debug.Log (kingdomTile.kingdom.kingdomName + " HAS 1 OR LOWER FACTION COUNT. CAN'T SPLIT.");
		}
	}
}
