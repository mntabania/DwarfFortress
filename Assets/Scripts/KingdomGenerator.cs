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
		GenerateInitialKingdomRelations();
		UpdateAdjacentKingdoms ();
		CreateInitialFactions ();
	}

	internal void OnTurn(){
		TriggerEvents ();
	}
	private void TriggerEvents(){
//		CheckEnemyKingdoms ();
		WorldEvents();
		GrowPopulation ();
		GenerateGold ();
		GenerateArmy ();
		GenerateGarrison ();
		TriggerExpandEvent ();
//		ListAdjacentKingdoms ();
//		TriggerDeclarePeaceEvent ();
		TriggerDeclarePeaceEventNew();
//		TriggerDeclareWarEvent ();
		TriggerDeclareWarEventNew();
//		TriggerInvadeEvent ();
		TriggerInvadeEventNew();
		UpdateAdjacentKingdoms ();
//		TriggerFactionEvents ();
		DarkAge();
		GoldenAge();
		CreateNewFactionFromExistingOnes();
		SplitKingdoms();
		CheckFactions ();
		CheckWarPeaceCounter ();
	}

	#region UPDATE ADJACENT KINGDOMS
	internal void UpdateAdjacentKingdoms(){
		List<KingdomTile> adjacentKingdoms = new List<KingdomTile> ();

		for (int i = 0; i < this.kingdoms.Count; i++) {
			adjacentKingdoms.Clear ();
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				for(int k = 0; k < this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities.Count; k++){
					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile != null){
						if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile.kingdom.id != this.kingdoms[i].kingdom.id){
							adjacentKingdoms.Add (this.kingdoms [i].kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile);
						}
					}
				}
			}
			adjacentKingdoms = adjacentKingdoms.Distinct ().ToList();

			for (int j = 0; j < this.kingdoms [i].kingdom.kingdomRelations.Count; j++) {
				if(adjacentKingdoms.Contains(this.kingdoms [i].kingdom.kingdomRelations[j].targetKingdom)){
					this.kingdoms [i].kingdom.kingdomRelations [j].isAdjacent = true;
				}else{
					this.kingdoms [i].kingdom.kingdomRelations [j].isAdjacent = false;
				}
			}
		}
	}
	#endregion

	#region GROW POPULATION
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
	#endregion

	#region GENERATE ARMY
	private void GenerateArmy(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			float increaseArmyPercentage = UnityEngine.Random.Range (0.5f, 1.5f);
			int totalPopulation = 0;
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				totalPopulation += this.kingdoms [i].kingdom.cities [j].cityAttributes.population;
			}
			int armyIncrease = (int)(((increaseArmyPercentage/100f) * totalPopulation) * this.kingdoms [i].kingdom.performance);
			this.kingdoms [i].kingdom.army += armyIncrease;
		}
	}
	#endregion

	#region GENERATE GARRISON
	private void GenerateGarrison(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				float increaseGarrisonPercentage = UnityEngine.Random.Range (0.5f, 1.5f);
				int garrisonIncrease = ((int)((increaseGarrisonPercentage / 100f) * (float)this.kingdoms [i].kingdom.cities [j].cityAttributes.population)) * this.kingdoms [i].kingdom.performance;
				this.kingdoms [i].kingdom.cities [j].cityAttributes.garrison += garrisonIncrease;
			}
		}
	}
	#endregion

	#region GENERATE GOLD
	private void GenerateGold(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				int goldIncrease = this.kingdoms [i].kingdom.cities [j].cityAttributes.cityLevel * this.kingdoms [i].kingdom.cities [j].cityAttributes.richnessLevel;
				this.kingdoms [i].kingdom.cities [j].cityAttributes.gold += goldIncrease;
			}
		}
	}
	#endregion


	#region EXPAND EVENT
	private void TriggerExpandEvent(){
		for(int i = 0; i < this.kingdoms.Count; i++){
//			if(this.kingdoms[i].kingdom.cities.Count <= 0){
//				continue;
//			}
//			Debug.Log ("EXPAND");
			int expandPercentage = (5 + (2 * this.kingdoms[i].kingdom.ambition) + this.kingdoms[i].kingdom.performance);
			int expandChance = UnityEngine.Random.Range (0, 100);
			if(expandChance < expandPercentage){
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

		if(!kingdomTile.kingdom.isInDarkAge){
			kingdomTile.kingdom.darkAgeChance += 1;
		}
	}

//	internal void ListAdjacentKingdoms(){
//		for(int i = 0; i < this.kingdoms.Count; i++){
////			if(this.kingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
////			Debug.Log ("ListAdjacentKingdoms");
//			this.kingdoms [i].kingdom.adjacentKingdoms.Clear();
//			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
//				for(int k = 0; k < this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities.Count; k++){
//					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile != null){
//						if(this.kingdoms[i].kingdom.cities[j].cityAttributes.connectedCities[k].cityAttributes.kingdomTile.kingdom.id != this.kingdoms[i].kingdom.id){
//							this.kingdoms [i].kingdom.adjacentKingdoms.Add (this.kingdoms [i].kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile);
//						}
//					}
//				}
//			}
//			this.kingdoms [i].kingdom.adjacentKingdoms = this.kingdoms [i].kingdom.adjacentKingdoms.Distinct ().ToList();
//		}
//	}
	#endregion

	#region DECLARE WAR
	private void TriggerDeclareWarEventNew(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if(!hasAdjacency(this.kingdoms[i])){
				return;
			}
			List<KingdomRelations> targetKingdoms = new List<KingdomRelations> ();
			targetKingdoms = GetAdjacentKingdomsInOrderOfArmyNew (this.kingdoms [i]);

			int warCount = 0;
			for(int k = 0; k < this.kingdoms[i].kingdom.kingdomRelations.Count; k++){
				if(this.kingdoms[i].kingdom.kingdomRelations[k].isAtWar){
					warCount += 1;
				}
			}
			for (int j = 0; j < targetKingdoms.Count; j++) {
				if(targetKingdoms[j].isAtWar){
					continue;
				}

				float armyPercentage = (float)this.kingdoms [i].kingdom.army / (float)targetKingdoms [j].targetKingdom.kingdom.army;
				if (armyPercentage > 2f) {
					armyPercentage = 2f;
				}
				if (armyPercentage < 0.25f) {
					armyPercentage = 0.25f;
				}
				int warCounter = 7 * (4 - targetKingdoms [j].turnsAtPeace);
				if(warCounter < 0){
					warCounter = 0;
				}
				int warPercentage = ((int)(5f + ((((3f - (float)warCount) * (float)this.kingdoms [i].kingdom.ambition) - (float)this.kingdoms [i].kingdom.altruism) * armyPercentage))) - warCounter;
				int warChance = UnityEngine.Random.Range (0, 100);
				if (warChance < warPercentage) {
					DeclareWarNew (this.kingdoms [i], targetKingdoms [j].targetKingdom);
					Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED WAR WITH " + targetKingdoms [j].targetKingdom.kingdom.kingdomName);
				}
				break;
			}
		}
	}
	private void DeclareWarNew(KingdomTile currentKingdom, KingdomTile targetKingdom, bool affectDarkAge = true){
		for(int i = 0; i < currentKingdom.kingdom.kingdomRelations.Count; i++){
			if(currentKingdom.kingdom.kingdomRelations [i].targetKingdom.kingdom.id == targetKingdom.kingdom.id){
				currentKingdom.kingdom.kingdomRelations [i].isAtWar = true;
				break;
			}
		}
		for(int i = 0; i < targetKingdom.kingdom.kingdomRelations.Count; i++){
			if(targetKingdom.kingdom.kingdomRelations [i].targetKingdom.kingdom.id == currentKingdom.kingdom.id){
				targetKingdom.kingdom.kingdomRelations [i].isAtWar = true;
				break;
			}
		}

		if(affectDarkAge){
			if(!currentKingdom.kingdom.isInDarkAge){
				currentKingdom.kingdom.darkAgeChance += 3;
			}
		}
	}
	private bool hasAdjacency(KingdomTile kingdomTile){
		for (int i = 0; i < kingdomTile.kingdom.kingdomRelations.Count; i++) {
			if(kingdomTile.kingdom.kingdomRelations[i].isAdjacent){
				return true;	
			}
		}
		return false;
	}
	private List<KingdomRelations> GetAdjacentKingdomsInOrderOfArmyNew(KingdomTile kingdomTile){
		List<KingdomRelations> orderedKingdoms = new List<KingdomRelations> ();
		for (int i = 0; i < kingdomTile.kingdom.kingdomRelations.Count; i++) {
			if(kingdomTile.kingdom.kingdomRelations[i].isAdjacent){
				orderedKingdoms.Add (kingdomTile.kingdom.kingdomRelations[i]);
			}
		}
		orderedKingdoms = orderedKingdoms.OrderBy (i => i.targetKingdom.kingdom.army).ToList ();
		return orderedKingdoms;
	}
//	private void TriggerDeclareWarEvent(){
//		for (int i = 0; i < this.kingdoms.Count; i++) {
////			if(this.kingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
////			Debug.Log ("WAR");
//			if(this.kingdoms [i].kingdom.adjacentKingdoms.Count <= 0){
//				return;
//			}
//			List<KingdomTile> targetKingdoms = new List<KingdomTile> ();
//			targetKingdoms = GetAdjacentKingdomsInOrderOfArmy (this.kingdoms [i]);
//			bool isEnemy = false;
//			for (int j = 0; j < targetKingdoms.Count; j++) {
////				if(targetKingdoms[j].kingdom.cities.Count <= 0){
////					continue;
////				}
//				isEnemy = false;
//				for(int k = 0; k < targetKingdoms[j].kingdom.enemyKingdoms.Count; k++){
//					if(targetKingdoms[j].kingdom.enemyKingdoms[k].kingdom.kingdomName == this.kingdoms[i].kingdom.kingdomName){
//						isEnemy = true;
//						break;
//					}
//				}
//
//				if(isEnemy){
//					continue;
//				}
//				float armyPercentage = (float)this.kingdoms [i].kingdom.army / (float)targetKingdoms[j].kingdom.army;
//				if(armyPercentage > 2f){
//					armyPercentage = 2f;
//				}
//				if(armyPercentage < 0.25f){
//					armyPercentage = 0.25f;
//				}
//				int warCount = this.kingdoms[i].kingdom.enemyKingdoms.Count;
//				int warPercentage = (int)(5f + ((((3f - (float)warCount) * (float)this.kingdoms [i].kingdom.ambition) - (float)this.kingdoms [i].kingdom.altruism) * armyPercentage));
//				int warChance = UnityEngine.Random.Range (0, 100);
//				if (warChance < warPercentage){
//					DeclareWar (this.kingdoms[i], targetKingdoms[j]);
//					Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED WAR WITH " + targetKingdoms[j].kingdom.kingdomName);
//					break;
//				}
//			}
////			KingdomTile targetKingdom = GetAdjacentWeakestArmy (this.kingdoms [i].kingdom.adjacentKingdoms);
//		}
////		Debug.Log ("WAR");
//
//	}

//	private void DeclareWar(KingdomTile currentKingdom, KingdomTile targetKingdom){
//		currentKingdom.kingdom.enemyKingdoms.Add (targetKingdom);
//		targetKingdom.kingdom.enemyKingdoms.Add (currentKingdom);
//		currentKingdom.kingdom.citiesGained.Add (0);
//		currentKingdom.kingdom.citiesLost.Add (0);
//		targetKingdom.kingdom.citiesGained.Add (0);
//		targetKingdom.kingdom.citiesLost.Add (0);
//	}

//	private List<KingdomTile> GetAdjacentKingdomsInOrderOfArmy(KingdomTile kingdomTile){
//		List<KingdomTile> orderedKingdoms = new List<KingdomTile> ();
//		for (int i = 0; i < kingdomTile.kingdom.adjacentKingdoms.Count; i++) {
//			orderedKingdoms.Add (kingdomTile.kingdom.adjacentKingdoms[i]);
//		}
//		orderedKingdoms = orderedKingdoms.OrderBy (i => i.kingdom.army).ToList ();
//		return orderedKingdoms;
//	}
	#endregion

	#region INVADE
	private void TriggerInvadeEventNew(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if(!hasEnemies(this.kingdoms[i])){
				continue;
			}
			KingdomRelations targetKingdom = GetEnemyWeakestArmyNew (this.kingdoms [i].kingdom.kingdomRelations);
			if(targetKingdom == null){
				continue;
			}
			List <CityTile> cityTilesConnected = GetCityTilesConnected (this.kingdoms [i], targetKingdom.targetKingdom);
			int randomCityInvader = UnityEngine.Random.Range (0, cityTilesConnected.Count);

			CityTile cityInvader = cityTilesConnected [randomCityInvader];
			CityTile cityTarget = GetRandomConnectedCity(cityInvader, targetKingdom.targetKingdom);


			int invadePercentage = (int)(10f + ((4f * (10f - (float)this.kingdoms [i].kingdom.altruism)) * ((float)this.kingdoms [i].kingdom.army / (float)targetKingdom.targetKingdom.kingdom.army)));
			int invadeChance = UnityEngine.Random.Range (0, 100);
			if (invadeChance < invadePercentage) {
				InvadeNew (this.kingdoms[i], targetKingdom.targetKingdom, cityInvader, cityTarget);
			}
		}
	}
	private void InvadeNew(KingdomTile invaderKingdom, KingdomTile targetKingdom, CityTile fromCityTile, CityTile toCityTile){
		Debug.Log (invaderKingdom.kingdom.kingdomName + " ATTEMPTS TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		float invaderArmyLossPercentage = UnityEngine.Random.Range (10f, 20f);
		float targetArmyLossPercentage = UnityEngine.Random.Range (5f, 10f);

		int invaderArmyLoss = (int)((invaderArmyLossPercentage/100f) * (float)invaderKingdom.kingdom.army);
		invaderKingdom.kingdom.army -= invaderArmyLoss;

		int targetArmyLoss = (int)((targetArmyLossPercentage/100f) * (float)targetKingdom.kingdom.army);
		targetKingdom.kingdom.army -= targetArmyLoss;

		float successRate = 50f * ((float)invaderKingdom.kingdom.army / (float)targetKingdom.kingdom.army);
		int successChance = UnityEngine.Random.Range (0, 100);
		if(successChance <= successRate){
			toCityTile.cityAttributes.kingdomTile = invaderKingdom;
			toCityTile.cityAttributes.faction = fromCityTile.cityAttributes.faction;
			toCityTile.GetComponent<SpriteRenderer> ().color = invaderKingdom.kingdom.tileColor;
			invaderKingdom.kingdom.cities.Add (toCityTile);
			targetKingdom.kingdom.cities.Remove (toCityTile);
			for(int i = 0; i < invaderKingdom.kingdom.kingdomRelations.Count; i++){
				if(invaderKingdom.kingdom.kingdomRelations[i].targetKingdom.kingdom.id == targetKingdom.kingdom.id){
					invaderKingdom.kingdom.kingdomRelations[i].citiesGained += 1;
					break;
				}
			}
			for(int i = 0; i < targetKingdom.kingdom.kingdomRelations.Count; i++){
				if(targetKingdom.kingdom.kingdomRelations[i].targetKingdom.kingdom.id == invaderKingdom.kingdom.id){
					targetKingdom.kingdom.kingdomRelations[i].citiesLost += 1;
					break;
				}
			}
			if(!invaderKingdom.kingdom.isInDarkAge){
				invaderKingdom.kingdom.darkAgeChance += 1;
			}
			CheckKingdomRelations ();
			Debug.Log (invaderKingdom.kingdom.kingdomName + " SUCCESSFULLY INVADED " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		}else{
			Debug.Log (invaderKingdom.kingdom.kingdomName + " HAS FAILED TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
		}

		Debug.Log (invaderKingdom.kingdom.kingdomName + " ARMY LOSS: " + invaderArmyLoss + " (" + invaderArmyLossPercentage + "%)");
		Debug.Log (targetKingdom.kingdom.kingdomName + " ARMY LOSS: " + targetArmyLoss + " (" + targetArmyLossPercentage + "%)");

	}
	private bool hasEnemies(KingdomTile kingdomTile){
		for (int i = 0; i < kingdomTile.kingdom.kingdomRelations.Count; i++) {
			if(kingdomTile.kingdom.kingdomRelations[i].isAtWar){
				return true;	
			}
		}
		return false;
	}
	private KingdomRelations GetEnemyWeakestArmyNew(List<KingdomRelations> kingdomRelations){
		int weakestArmy = 0;
		KingdomRelations kingdomWithWeakestArmy = null;
		for (int i = 0; i < kingdomRelations.Count; i++) {
			if (kingdomRelations [i].isAtWar && kingdomRelations [i].isAdjacent) {
				weakestArmy = kingdomRelations [i].targetKingdom.kingdom.army;
				kingdomWithWeakestArmy = kingdomRelations [i];
				break;
			}
		}
		for(int i = 0; i < kingdomRelations.Count; i++){
			if(kingdomRelations[i].isAtWar && kingdomRelations[i].isAdjacent){
				if(weakestArmy > kingdomRelations[i].targetKingdom.kingdom.army){
					weakestArmy = kingdomRelations [i].targetKingdom.kingdom.army;
					kingdomWithWeakestArmy = kingdomRelations [i];
				}
			}
		}
		return kingdomWithWeakestArmy;
	}

	private List<CityTile> GetCityTilesConnected(KingdomTile kingdomTile, KingdomTile targetKingdom){
		List <CityTile> cityTilesConnected = new List<CityTile> ();
		for(int j = 0; j < kingdomTile.kingdom.cities.Count; j++){
			if(kingdomTile.kingdom.cities[j].cityAttributes.kingdomTile != null){
				for(int k = 0; k < kingdomTile.kingdom.cities[j].cityAttributes.connectedCities.Count; k++){
					if (kingdomTile.kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile != null){
						if (kingdomTile.kingdom.cities [j].cityAttributes.connectedCities [k].cityAttributes.kingdomTile.kingdom.id == targetKingdom.kingdom.id) {
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
				if (cityTile.cityAttributes.connectedCities [j].cityAttributes.kingdomTile.kingdom.id == targetKingdom.kingdom.id) {
					cityTilesConnected.Add (cityTile.cityAttributes.connectedCities [j]);
				}
			}
		}
		int randomCityTarget = UnityEngine.Random.Range (0, cityTilesConnected.Count);
		return cityTilesConnected[randomCityTarget];
	}
//	private void TriggerInvadeEvent(){
//		for (int i = 0; i < this.kingdoms.Count; i++) {
////			if(this.kingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
//			if(this.kingdoms [i].kingdom.enemyKingdoms.Count <= 0){
//				continue;
//			}
//			KingdomTile targetKingdom = GetEnemyWeakestArmy (this.kingdoms [i].kingdom.enemyKingdoms);
//			if(targetKingdom == null){
//				continue;
//			}
//			List <CityTile> cityTilesConnected = GetCityTilesConnected (this.kingdoms [i], targetKingdom);
//			int randomCityInvader = UnityEngine.Random.Range (0, cityTilesConnected.Count);
//
//			CityTile cityInvader = cityTilesConnected [randomCityInvader];
//			CityTile cityTarget = GetRandomConnectedCity(cityInvader, targetKingdom);
//
//
//			int invadePercentage = (int)(10f + ((4f * (10f - (float)this.kingdoms [i].kingdom.altruism)) * ((float)this.kingdoms [i].kingdom.army / (float)targetKingdom.kingdom.army)));
//			int invadeChance = UnityEngine.Random.Range (0, 100);
//			if (invadeChance < invadePercentage) {
//				Invade (this.kingdoms[i], targetKingdom, cityInvader, cityTarget);
//			}
//		}
////		Debug.Log ("INVADE");
//	}
//	private void Invade(KingdomTile invaderKingdom, KingdomTile targetKingdom, CityTile fromCityTile, CityTile toCityTile){
//		Debug.Log (invaderKingdom.kingdom.kingdomName + " ATTEMPTS TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
//		float invaderArmyLossPercentage = UnityEngine.Random.Range (10f, 20f);
//		float targetArmyLossPercentage = UnityEngine.Random.Range (5f, 10f);
//
//		int invaderArmyLoss = (int)((invaderArmyLossPercentage/100f) * (float)invaderKingdom.kingdom.army);
//		invaderKingdom.kingdom.army -= invaderArmyLoss;
//
//		int targetArmyLoss = (int)((targetArmyLossPercentage/100f) * (float)targetKingdom.kingdom.army);
//		targetKingdom.kingdom.army -= targetArmyLoss;
//
//		float successRate = 50f * ((float)invaderKingdom.kingdom.army / (float)targetKingdom.kingdom.army);
//		int successChance = UnityEngine.Random.Range (0, 100);
//		if(successChance >= 0 && successChance <= successRate){
//			toCityTile.cityAttributes.kingdomTile = invaderKingdom;
//			toCityTile.cityAttributes.faction = fromCityTile.cityAttributes.faction;
//			toCityTile.GetComponent<SpriteRenderer> ().color = invaderKingdom.kingdom.tileColor;
//			invaderKingdom.kingdom.cities.Add (toCityTile);
//			targetKingdom.kingdom.cities.Remove (toCityTile);
//			for(int i = 0; i < invaderKingdom.kingdom.enemyKingdoms.Count; i++){
//				if(invaderKingdom.kingdom.enemyKingdoms[i].kingdom.id == targetKingdom.kingdom.id){
//					invaderKingdom.kingdom.citiesGained [i] += 1;
//					break;
//				}
//			}
//			for(int i = 0; i < targetKingdom.kingdom.enemyKingdoms.Count; i++){
//				if(targetKingdom.kingdom.enemyKingdoms[i].kingdom.id == invaderKingdom.kingdom.id){
//					targetKingdom.kingdom.citiesLost [i] += 1;
//					break;
//				}
//			}
//			CheckEnemyKingdoms ();
//			Debug.Log (invaderKingdom.kingdom.kingdomName + " SUCCESSFULLY INVADED " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
//		}else{
//			Debug.Log (invaderKingdom.kingdom.kingdomName + " HAS FAILED TO INVADE " + targetKingdom.kingdom.kingdomName + " FROM " + fromCityTile.cityAttributes.hexTile.name + " TO " + toCityTile.cityAttributes.hexTile.name);
//		}
//
//		Debug.Log (invaderKingdom.kingdom.kingdomName + " ARMY LOSS: " + invaderArmyLoss + " (" + invaderArmyLossPercentage + "%)");
//		Debug.Log (targetKingdom.kingdom.kingdomName + " ARMY LOSS: " + targetArmyLoss + " (" + targetArmyLossPercentage + "%)");
//
//	}

//	private KingdomTile GetEnemyWeakestArmy(List<KingdomTile> enemyKingdoms){
//		int weakestArmy = 0;
//		KingdomTile kingdomWithWeakestArmy = null;
//		for(int i = 0; i < enemyKingdoms.Count; i++){
////			if(enemyKingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
//			weakestArmy = enemyKingdoms [i].kingdom.army;
//			kingdomWithWeakestArmy = enemyKingdoms [i];
//			break;
//		}
//		for(int i = 0; i < enemyKingdoms.Count; i++){
////			if(enemyKingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
//			int currentArmy = enemyKingdoms[i].kingdom.army;
//			if(weakestArmy > currentArmy){
//				weakestArmy = currentArmy;
//				kingdomWithWeakestArmy = enemyKingdoms[i];
//			}
//		}
//		return kingdomWithWeakestArmy;
//	}
	#endregion

	#region DECLARE PEACE
	private void TriggerDeclarePeaceEventNew(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(!hasEnemies(this.kingdoms[i])){
				continue;
			}
			KingdomRelations targetKingdom = GetRandomPeaceKingdomNew (this.kingdoms [i]);

			float armyPercentage = (float)targetKingdom.targetKingdom.kingdom.army / (float)this.kingdoms [i].kingdom.army;
			if(armyPercentage > 2f){
				armyPercentage = 2f;
			}
			if(armyPercentage < 0.25f){
				armyPercentage = 0.25f;
			}

			int peaceCounter = 3 * (4 - targetKingdom.turnsAtWar);
			if(peaceCounter < 0){
				peaceCounter = 0;
			}
			int peacePercentage = ((int)(5f + ((8f * ((float)targetKingdom.citiesLost + (float)targetKingdom.citiesGained)) * armyPercentage))) - peaceCounter;
			int peaceChance = UnityEngine.Random.Range (0, 100);

			if(peaceChance < peacePercentage){
				DeclarePeaceNew (this.kingdoms [i], targetKingdom.targetKingdom);
				Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED PEACE WITH " + targetKingdom.targetKingdom.kingdom.kingdomName);
			}
		}

	}
	private void DeclarePeaceNew(KingdomTile fromKingdomTile, KingdomTile toKingdomTile){
		for(int i = 0; i < fromKingdomTile.kingdom.kingdomRelations.Count; i++){
			if(fromKingdomTile.kingdom.kingdomRelations[i].targetKingdom.kingdom.id == toKingdomTile.kingdom.id){
				fromKingdomTile.kingdom.kingdomRelations[i].citiesGained = 0;
				fromKingdomTile.kingdom.kingdomRelations[i].citiesLost = 0;
				fromKingdomTile.kingdom.kingdomRelations[i].isAtWar = false;
				break;
			}
		}
		for(int i = 0; i < toKingdomTile.kingdom.kingdomRelations.Count; i++){
			if(toKingdomTile.kingdom.kingdomRelations[i].targetKingdom.kingdom.id == fromKingdomTile.kingdom.id){
				toKingdomTile.kingdom.kingdomRelations[i].citiesGained = 0;
				toKingdomTile.kingdom.kingdomRelations[i].citiesLost = 0;
				toKingdomTile.kingdom.kingdomRelations[i].isAtWar = false;
				break;
			}
		}
	}
	private KingdomRelations GetRandomPeaceKingdomNew(KingdomTile kingdomTile){
		List <KingdomRelations> enemyKingdoms = new List<KingdomRelations> ();
		for(int j = 0; j < kingdomTile.kingdom.kingdomRelations.Count; j++){
			if(kingdomTile.kingdom.kingdomRelations[j].isAtWar && kingdomTile.kingdom.kingdomRelations[j].isAdjacent){
				enemyKingdoms.Add (kingdomTile.kingdom.kingdomRelations [j]);
			}
		}
		int randomPeaceKingdom = UnityEngine.Random.Range (0, enemyKingdoms.Count);
		return enemyKingdoms[randomPeaceKingdom];
	}
//	private void TriggerDeclarePeaceEvent(){
//		for(int i = 0; i < this.kingdoms.Count; i++){
////			if(this.kingdoms[i].kingdom.cities.Count <= 0){
////				continue;
////			}
//			if(this.kingdoms[i].kingdom.enemyKingdoms.Count <= 0){
//				continue;
//			}
//			KingdomTile targetKingdom = GetRandomPeaceKingdom (this.kingdoms [i]);
//			int citiesGained = 0;
//			int citiesLost = 0;
//			for(int j = 0; j < this.kingdoms[i].kingdom.enemyKingdoms.Count; j++){
////				if(this.kingdoms[i].kingdom.enemyKingdoms[j].kingdom.cities.Count <= 0){
////					continue;
////				}
//				if(this.kingdoms[i].kingdom.enemyKingdoms[j].kingdom.id == targetKingdom.kingdom.id){
//					citiesGained = this.kingdoms [i].kingdom.citiesGained [j];
//					citiesLost = this.kingdoms [i].kingdom.citiesLost [j];
//					break;
//				}
//			}
//			float armyPercentage = (float)targetKingdom.kingdom.army / (float)this.kingdoms [i].kingdom.army;
//			if(armyPercentage > 2f){
//				armyPercentage = 2f;
//			}
//			if(armyPercentage < 0.25f){
//				armyPercentage = 0.25f;
//			}
//			int peacePercentage = (int)(5f + ((8f * ((float)citiesLost + (float)citiesGained)) * armyPercentage));
//			int peaceChance = UnityEngine.Random.Range (0, 100);
//
//			if(peaceChance < peacePercentage){
//				DeclarePeace (this.kingdoms [i], targetKingdom);
//				Debug.Log (this.kingdoms [i].kingdom.kingdomName + " DECLARED PEACE WITH " + targetKingdom.kingdom.kingdomName);
//			}
//		}
////		Debug.Log ("PEACE");
//
//	}
//	private void DeclarePeace(KingdomTile fromKingdomTile, KingdomTile toKingdomTile){
//		for(int i = 0; i < fromKingdomTile.kingdom.enemyKingdoms.Count; i++){
//			if(fromKingdomTile.kingdom.enemyKingdoms[i].kingdom.id == toKingdomTile.kingdom.id){
//				fromKingdomTile.kingdom.citiesGained.RemoveAt (i);
//				fromKingdomTile.kingdom.citiesLost.RemoveAt (i);
//				fromKingdomTile.kingdom.enemyKingdoms.RemoveAt (i);
//				break;
//			}
//		}
//		for(int i = 0; i < toKingdomTile.kingdom.enemyKingdoms.Count; i++){
//			if(toKingdomTile.kingdom.enemyKingdoms[i].kingdom.id == fromKingdomTile.kingdom.id){
//				toKingdomTile.kingdom.citiesGained.RemoveAt (i);
//				toKingdomTile.kingdom.citiesLost.RemoveAt (i);
//				toKingdomTile.kingdom.enemyKingdoms.RemoveAt (i);
//				break;
//			}
//		}
//	}
//	private KingdomTile GetRandomPeaceKingdom(KingdomTile kingdomTile){
//		List <KingdomTile> enemyKingdoms = new List<KingdomTile> ();
//		for(int j = 0; j < kingdomTile.kingdom.enemyKingdoms.Count; j++){
////			if(kingdomTile.kingdom.enemyKingdoms[j].kingdom.cities.Count <= 0){
////				continue;
////			}
//			enemyKingdoms.Add (kingdomTile.kingdom.enemyKingdoms [j]);
//		}
//		int randomPeaceKingdom = UnityEngine.Random.Range (0, enemyKingdoms.Count);
//		return enemyKingdoms[randomPeaceKingdom];
//	}
	#endregion

	#region CHECK KINGDOMS
	private void CheckEnemyKingdoms(){
		List <int> index = new List<int> ();
		List <KingdomTile> kingdoms = new List<KingdomTile> ();
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(this.kingdoms[i].kingdom.cities.Count <= 0){
				this.kingdoms[i].kingdom.isDead = true;
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
	#endregion

	#region FACTION EVENTS
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

	private void TriggerFactionEvents(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			int triggerChance = UnityEngine.Random.Range (0, 100);
			if(triggerChance < 3){
				int randomCity = UnityEngine.Random.Range (0, this.kingdoms [i].kingdom.cities.Count);
				CityTile randomCityTile = this.kingdoms [i].kingdom.cities [randomCity];

				int factionEventsChance = UnityEngine.Random.Range (0, 100);
				if(factionEventsChance < 25){ //change faction
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
			CheckFactions ();
		}
	}

	#region CHANGE FACTION
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
	#endregion

	#region INFLUENCE FACTION
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
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION / TARGET HAS ONLY ONE CITY. CHECKING CULTURE....");
				List<int> cultureIndexes = GetConnectedCitiesIndexesWithDiffCulture(fromCityTile);
				if(cultureIndexes.Count > 0){
					int randomTargetCity = UnityEngine.Random.Range (0, cultureIndexes.Count);
					CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [cultureIndexes[randomTargetCity]];
					toCityTile.cityAttributes.faction.culture = fromCityTile.cityAttributes.faction.culture;
					targetCityTile = toCityTile;
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS CULTURE TO " + toCityTile.cityAttributes.hexTile.name);
				}else{
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION OR CULTURE / TARGET HAS ONLY ONE CITY. CAN'T INFLUENCE.");
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
				Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE / TARGET HAS ONLY ONE CITY. CHECKING RELIGION....");
				List<int> religionIndexes = GetConnectedCitiesIndexesWithDiffReligion(fromCityTile);
				if(religionIndexes.Count > 0){
					int randomTargetCity = UnityEngine.Random.Range (0, religionIndexes.Count);
					CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [religionIndexes[randomTargetCity]];
					toCityTile.cityAttributes.faction.religion = fromCityTile.cityAttributes.faction.religion;
					targetCityTile = toCityTile;
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS RELIGION TO " + toCityTile.cityAttributes.hexTile.name);
				}
				else{
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE OR RELIGION / TARGET HAS ONLY ONE CITY. CAN'T INFLUENCE.");
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
				if(cityTile.cityAttributes.connectedCities[i].cityAttributes.kingdomTile.kingdom.cities.Count > 1){
					if(cityTile.cityAttributes.connectedCities[i].cityAttributes.faction.religion != cityTile.cityAttributes.faction.religion){
						indexes.Add (i);
					}
				}
			}
		}
		return indexes;
	}
	private List<int> GetConnectedCitiesIndexesWithDiffCulture(CityTile cityTile){
		List<int> indexes = new List<int> ();
		for(int i = 0; i < cityTile.cityAttributes.connectedCities.Count; i++){
			if (cityTile.cityAttributes.connectedCities [i].cityAttributes.faction != null) {
				if (cityTile.cityAttributes.connectedCities [i].cityAttributes.kingdomTile.kingdom.cities.Count > 1) {
					if (cityTile.cityAttributes.connectedCities [i].cityAttributes.faction.culture != cityTile.cityAttributes.faction.culture) {
						indexes.Add (i);
					}
				}
			}
		}
		return indexes;
	}
	#endregion

	#region SPLIT FACTION
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

			CreateKingdomRelationships (goNewKingdom.GetComponent<KingdomTile> ());
			AddKingdomRelationshipToAllKingdoms (goNewKingdom.GetComponent<KingdomTile> ());

			kingdoms.Add (goNewKingdom.GetComponent<KingdomTile>());

			kingdomTile.kingdom.factions.Remove (rebelFaction);

			for(int i = 0; i < newCityTiles.Count; i++){
				kingdomTile.kingdom.cities.Remove (newCityTiles [i]);
			}

			Debug.Log (rebelFaction.factionName + " HAS SPLIT WITH " + kingdomTile.kingdom.kingdomName);
			Debug.Log ("CREATED NEW KINGDOM: " + goNewKingdom.name);
			UpdateAdjacentKingdoms ();
			DeclareWarNew(goNewKingdom.GetComponent<KingdomTile> (), kingdomTile, false);
		}else{
			Debug.Log (kingdomTile.kingdom.kingdomName + " HAS 1 OR LOWER FACTION COUNT. CAN'T SPLIT.");
		}
	}
	#endregion
	#endregion


	private void GenerateInitialKingdomRelations(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			CreateKingdomRelationships (this.kingdoms [i]);
		}
	}
	private void CreateKingdomRelationships(KingdomTile kingdomTile){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if(kingdomTile.kingdom.id == this.kingdoms[i].kingdom.id){
				continue;
			}
			KingdomRelations kingdomRelations = new KingdomRelations ();
			kingdomRelations.targetKingdom = this.kingdoms [i];
			kingdomTile.kingdom.kingdomRelations.Add (kingdomRelations);
		}
	}
	private void AddKingdomRelationshipToAllKingdoms(KingdomTile kingdomTile){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if(kingdomTile.kingdom.id == this.kingdoms[i].kingdom.id){
				continue;
			}
			KingdomRelations kingdomRelations = new KingdomRelations ();
			kingdomRelations.targetKingdom = kingdomTile;
			this.kingdoms[i].kingdom.kingdomRelations.Add (kingdomRelations);
		}
	}
	private void CheckKingdomRelations(){
		List <KingdomRelations> kingdomRelations = new List<KingdomRelations> ();
		List <KingdomTile> kingdoms = new List<KingdomTile> ();
		for(int i = 0; i < this.kingdoms.Count; i++){	
			if(this.kingdoms[i].kingdom.cities.Count <= 0){
				this.kingdoms[i].kingdom.isDead = true;
				kingdoms.Add (this.kingdoms [i]);
			}
			kingdomRelations.Clear ();
			for(int j = 0; j < this.kingdoms[i].kingdom.kingdomRelations.Count; j++){
				if(this.kingdoms[i].kingdom.kingdomRelations[j].targetKingdom.kingdom.cities.Count <= 0){
					kingdomRelations.Add (this.kingdoms[i].kingdom.kingdomRelations[j]);
				}
			}
			for (int j = 0; j < kingdomRelations.Count; j++) {
				this.kingdoms [i].kingdom.kingdomRelations.Remove (kingdomRelations[j]);
			}
		}

		for(int i = 0; i < kingdoms.Count; i++){
			this.kingdoms.Remove (kingdoms[i]);
		}
	}
	private void CheckFactions(){
		List<Faction> factions = new List<Faction> ();
		for(int i = 0; i < this.kingdoms.Count; i++){
			factions.Clear ();
			for(int j = 0; j < this.kingdoms[i].kingdom.factions.Count; j++){
				bool hasFaction = false;
				for(int k = 0; k < this.kingdoms[i].kingdom.cities.Count; k++){
					if(this.kingdoms[i].kingdom.cities[k].cityAttributes.faction.id == this.kingdoms[i].kingdom.factions[j].id){
						hasFaction = true;
					}
				}
				if(!hasFaction){
					factions.Add (this.kingdoms[i].kingdom.factions[j]);
				}
			}

			for(int j = 0; j < factions.Count; j++){
				this.kingdoms [i].kingdom.factions.Remove (factions [j]);
			}
		}
	}
	private void CheckWarPeaceCounter(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.kingdomRelations.Count; j++){
				if(this.kingdoms[i].kingdom.kingdomRelations[j].isAtWar){
					this.kingdoms [i].kingdom.kingdomRelations [j].turnsAtWar += 1;
					this.kingdoms [i].kingdom.kingdomRelations [j].turnsAtPeace = 0;
				}else{
					this.kingdoms [i].kingdom.kingdomRelations [j].turnsAtPeace += 1;
					this.kingdoms [i].kingdom.kingdomRelations [j].turnsAtWar = 0;
				}
			}
		}
	}

	private void WorldEvents(){
		CreateNewCultureOrReligion();
		SetTrendyReligion();
		SetTrendyCulture();
		InfluenceTrendyReligion();
		InfluenceTrendyCulture();
		InfluenceReligionOrCulture();
	}
	private void CreateNewCultureOrReligion(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 5){

			Debug.Log("WORLD EVENT: CREATE NEW CULTURE OR RELIGION");

			int noOfCities = UnityEngine.Random.Range (1, 3);


			for(int i = noOfCities; i >= 0; i--){
				Debug.Log("NO OF CITIES TO BE CONVERTED: " + (i + 1));

				List<CityTile> cityTiles = GetCityListForNewCultureOrReligion(i);
				List<CityTile> chosenConnectedCityTiles = new List<CityTile> ();

				if(cityTiles.Count > 0){
					if(i > 0){
						int randomCity = UnityEngine.Random.Range (0, cityTiles.Count);
						List<CityTile> connectedCityTiles = new List<CityTile> ();
						for(int j = 0; j < cityTiles[randomCity].cityAttributes.connectedCities.Count; j++){
							if(cityTiles[randomCity].cityAttributes.connectedCities[j].cityAttributes.kingdomTile != null){
								if(cityTiles[randomCity].cityAttributes.connectedCities[j].cityAttributes.faction != null){
									if(cityTiles[randomCity].cityAttributes.connectedCities[j].cityAttributes.kingdomTile.kingdom.id == cityTiles[randomCity].cityAttributes.kingdomTile.kingdom.id){
										connectedCityTiles.Add (cityTiles [randomCity].cityAttributes.connectedCities [j]);
									}
								}
							}
						}

						chosenConnectedCityTiles.Clear ();
						for(int j = 0; j < i; j++){
							int random = UnityEngine.Random.Range (0, connectedCityTiles.Count);
							chosenConnectedCityTiles.Add (connectedCityTiles [random]);
							connectedCityTiles.Remove (connectedCityTiles [random]);
						}


						int randomNo = UnityEngine.Random.Range(0,2);

						if(randomNo == 0){ //religion
							Religion religion = ReligionGenerator.Instance.GenerateReligion();
							if(religion == null){
								Debug.Log ("NO MORE RELIGIONS. CAN'T CREATE NEW.");
								return;
							}
							cityTiles[randomCity].cityAttributes.faction.religion = religion;
							Debug.Log("RELIGION CHANGED ON CITY: " + cityTiles[randomCity].cityAttributes.hexTile.name);

							for(int j = 0; j < chosenConnectedCityTiles.Count; j++){
								chosenConnectedCityTiles[j].cityAttributes.faction.religion = religion;
								Debug.Log("RELIGION CHANGED ON CITY: " + chosenConnectedCityTiles[j].cityAttributes.hexTile.name);
							}



						}else{ //culture
							Culture culture = CultureGenerator.Instance.GenerateCulture();
							if(culture == null){
								Debug.Log ("NO MORE CULTURE. CAN'T CREATE NEW.");
								return;
							}
							cityTiles[randomCity].cityAttributes.faction.culture = culture;
							Debug.Log("CULTURE CHANGED ON CITY: " + cityTiles[randomCity].cityAttributes.hexTile.name);

							for(int j = 0; j < chosenConnectedCityTiles.Count; j++){
								chosenConnectedCityTiles[j].cityAttributes.faction.culture = culture;
								Debug.Log("CULTURE CHANGED ON CITY: " + chosenConnectedCityTiles[j].cityAttributes.hexTile.name);
							}
						}
					}else{
						int randomCity = UnityEngine.Random.Range (0, cityTiles.Count);

						int randomNo = UnityEngine.Random.Range(0,2);

						if(randomNo == 0){ //religion
							Religion religion = ReligionGenerator.Instance.GenerateReligion();
							if(religion == null){
								Debug.Log ("NO MORE RELIGIONS. CAN'T CREATE NEW.");
								return;
							}
							cityTiles[randomCity].cityAttributes.faction.religion = religion;
							Debug.Log("RELIGION CHANGED ON CITY: " + cityTiles[randomCity].cityAttributes.hexTile.name);


						}else{ //culture
							Culture culture = CultureGenerator.Instance.GenerateCulture();
							if(culture == null){
								Debug.Log ("NO MORE CULTURE. CAN'T CREATE NEW.");
								return;
							}
							cityTiles[randomCity].cityAttributes.faction.culture = culture;
							Debug.Log("CULTURE CHANGED ON CITY: " + cityTiles[randomCity].cityAttributes.hexTile.name);

						}
					}

					return;
				}else{
					continue;
				}

			}
			Debug.Log("CAN'T CHANGE RELIGION OR CULTURE. THERE IS NO CITY CAN BE CONVERTED.");
		}

	}
	private List<CityTile> GetCityListForNewCultureOrReligion(int requirement){
		List<CityTile> cityTiles = new List<CityTile> ();
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
				if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction != null){
					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction != null){
						if(CheckForNoOfCitiesConnected(this.kingdoms[i].kingdom.cities[j]) >= requirement){
							cityTiles.Add (this.kingdoms [i].kingdom.cities [j]);
						}
					}
				}
			}
		}
		return cityTiles;
	}
	private int CheckForNoOfCitiesConnected(CityTile cityTile){
		int noOfConnected = 0;
		for(int i = 0; i < cityTile.cityAttributes.connectedCities.Count; i++){
			if(cityTile.cityAttributes.connectedCities[i].cityAttributes.kingdomTile != null){
				if(cityTile.cityAttributes.connectedCities[i].cityAttributes.faction != null){
					if(cityTile.cityAttributes.connectedCities[i].cityAttributes.kingdomTile.kingdom.id == cityTile.cityAttributes.kingdomTile.kingdom.id){
						noOfConnected += 1;
					}
				}
			}
		}

		return noOfConnected;
	}

	private void SetTrendyReligion(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 15){
			Debug.Log("WORLD EVENT: SET TRENDY RELIGION");
			ReligionGenerator.Instance.trendyReligion = ReligionGenerator.Instance.allReligions[UnityEngine.Random.Range(0, ReligionGenerator.Instance.allReligions.Count)];
			Debug.Log("TRENDY RELIGION: " + ReligionGenerator.Instance.trendyReligion.religionName);
		}
	}

	private void SetTrendyCulture(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 15){
			Debug.Log("WORLD EVENT: SET TRENDY CULTURE");
			CultureGenerator.Instance.trendyCulture = CultureGenerator.Instance.allCultures[UnityEngine.Random.Range(0, CultureGenerator.Instance.allCultures.Count)];
			Debug.Log("TRENDY CULTURE: " + CultureGenerator.Instance.trendyCulture.cultureName);
		}

	}
	private void InfluenceTrendyReligion(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 25){
			Debug.Log("WORLD EVENT: SPREAD TRENDY RELIGION");
			if(ReligionGenerator.Instance.trendyReligion.religionName == string.Empty){
				Debug.Log("THERE IS NO TRENDY RELIGION. CAN'T SPREAD.");
				return;
			}
			List<CityTile> allCitiesWithTrendyReligion = new List<CityTile>();
			for(int i = 0; i < this.kingdoms.Count; i++){
				for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction != null){
						if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction.religion == ReligionGenerator.Instance.trendyReligion){
							allCitiesWithTrendyReligion.Add(this.kingdoms[i].kingdom.cities[j]);
						}
					}
				}
			}

			CityTile chosenCity = allCitiesWithTrendyReligion[UnityEngine.Random.Range(0, allCitiesWithTrendyReligion.Count)];
			List<int> religionIndexes = GetConnectedCitiesIndexesWithDiffReligion(chosenCity);
			if(religionIndexes.Count > 0){
				int randomTargetCity = UnityEngine.Random.Range (0, religionIndexes.Count);
				CityTile toCityTile = chosenCity.cityAttributes.connectedCities [religionIndexes[randomTargetCity]];
				toCityTile.cityAttributes.faction.religion = chosenCity.cityAttributes.faction.religion;
				Debug.Log (chosenCity.cityAttributes.hexTile.name + " HAS INFLUENCED TRENDY RELIGION TO " + toCityTile.cityAttributes.hexTile.name);
			}else{
				Debug.Log ("THERE IS NO CONNECTED CITY WITH DIFFERENT RELIGION. CAN'T SPREAD TRENDY RELIGION.");

			}
		}
	}
	private void InfluenceTrendyCulture(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 25){
			Debug.Log("WORLD EVENT: SPREAD TRENDY CULTURE");
			if(CultureGenerator.Instance.trendyCulture.cultureName == string.Empty){
				Debug.Log("THERE IS NO TRENDY CULTURE. CAN'T SPREAD.");
				return;
			}
			List<CityTile> allCitiesWithTrendyCulture = new List<CityTile>();
			for(int i = 0; i < this.kingdoms.Count; i++){
				for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
					if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction != null){
						if(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction.culture == CultureGenerator.Instance.trendyCulture){
							allCitiesWithTrendyCulture.Add(this.kingdoms[i].kingdom.cities[j]);
						}
					}
				}
			}

			CityTile chosenCity = allCitiesWithTrendyCulture[UnityEngine.Random.Range(0, allCitiesWithTrendyCulture.Count)];
			List<int> cultureIndexes = GetConnectedCitiesIndexesWithDiffCulture(chosenCity);
			if(cultureIndexes.Count > 0){
				int randomTargetCity = UnityEngine.Random.Range (0, cultureIndexes.Count);
				CityTile toCityTile = chosenCity.cityAttributes.connectedCities [cultureIndexes[randomTargetCity]];
				toCityTile.cityAttributes.faction.culture = chosenCity.cityAttributes.faction.culture;
				Debug.Log (chosenCity.cityAttributes.hexTile.name + " HAS INFLUENCED TRENDY CULTURE TO " + toCityTile.cityAttributes.hexTile.name);
			}else{
				Debug.Log ("THERE IS NO CONNECTED CITY WITH DIFFERENT CULTURE. CAN'T SPREAD TRENDY CULTURE.");
			}
		}
	}
	private void InfluenceReligionOrCulture(){
		int chance = UnityEngine.Random.Range(0,100);

		if(chance < 5){
			Debug.Log("WORLD EVENT: SPREAD CULTURE OR RELIGION");
			List<CityTile> allCities = new List<CityTile>();
			for(int i = 0; i < this.kingdoms.Count; i++){
				allCities.AddRange(this.kingdoms[i].kingdom.cities);
			}
			CityTile fromCityTile = allCities [UnityEngine.Random.Range (0, allCities.Count)];

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
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION / TARGET HAS ONLY ONE CITY. CHECKING CULTURE....");
					List<int> cultureIndexes = GetConnectedCitiesIndexesWithDiffCulture(fromCityTile);
					if(cultureIndexes.Count > 0){
						int randomTargetCity = UnityEngine.Random.Range (0, cultureIndexes.Count);
						CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [cultureIndexes[randomTargetCity]];
						toCityTile.cityAttributes.faction.culture = fromCityTile.cityAttributes.faction.culture;
						targetCityTile = toCityTile;
						Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS CULTURE TO " + toCityTile.cityAttributes.hexTile.name);
					}else{
						Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT RELIGION OR CULTURE / TARGET HAS ONLY ONE CITY. CAN'T INFLUENCE.");
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
					Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE / TARGET HAS ONLY ONE CITY. CHECKING RELIGION....");
					List<int> religionIndexes = GetConnectedCitiesIndexesWithDiffReligion(fromCityTile);
					if(religionIndexes.Count > 0){
						int randomTargetCity = UnityEngine.Random.Range (0, religionIndexes.Count);
						CityTile toCityTile = fromCityTile.cityAttributes.connectedCities [religionIndexes[randomTargetCity]];
						toCityTile.cityAttributes.faction.religion = fromCityTile.cityAttributes.faction.religion;
						targetCityTile = toCityTile;
						Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS INFLUENCED ITS RELIGION TO " + toCityTile.cityAttributes.hexTile.name);
					}
					else{
						Debug.Log (fromCityTile.cityAttributes.hexTile.name + " HAS NO ADJACENT CITIES WITH DIFFERENT CULTURE OR RELIGION / TARGET HAS ONLY ONE CITY. CAN'T INFLUENCE.");
					}
				}

			}
	//		if(targetCityTile != null){
	//			Faction faction = FactionStorage.Instance.CheckFaction (targetCityTile.cityAttributes.faction);
	//			if(faction == null){
	//				Debug.Log ("FACTION NON EXISTENT. CREATING NEW FACTION....");
	//				targetCityTile.cityAttributes.faction = CreateNewFaction (targetCityTile.cityAttributes.faction.race, targetCityTile.cityAttributes.faction.religion, targetCityTile.cityAttributes.faction.culture);
	//				targetCityTile.cityAttributes.kingdomTile.kingdom.factions.Add (targetCityTile.cityAttributes.faction);
	//				FactionStorage.Instance.AddFaction (targetCityTile.cityAttributes.faction);
	//			}else{
	//				Debug.Log ("FACTION ALREADY EXISTS. RETRIEVING FACTION....");
	//				targetCityTile.cityAttributes.faction = faction;
	//			}
	//		}
		}
	}
	private void DarkAge(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(this.kingdoms[i].kingdom.isInDarkAge){
				this.kingdoms[i].kingdom.darkAgeCounter += 1;
				if(this.kingdoms[i].kingdom.darkAgeCounter >= 10){
					this.kingdoms[i].kingdom.darkAgeCounter = 0;
					this.kingdoms[i].kingdom.isInDarkAge = false;
					this.kingdoms[i].kingdom.performance = this.kingdoms[i].kingdom.performanceStorage;

				}
				continue;
			}
			int chance = UnityEngine.Random.Range(0,100);
			if(chance < this.kingdoms[i].kingdom.darkAgeChance){
				Debug.Log(this.kingdoms[i].kingdom.kingdomName + " ENTERS THE DARK AGE! Dun dun dun dun....");
				this.kingdoms[i].kingdom.isInDarkAge = true;
				this.kingdoms[i].kingdom.isInGoldenAge = false;
				this.kingdoms[i].kingdom.darkAgeChance = this.kingdoms[i].kingdom.defaultDarkAgeChance;

				this.kingdoms[i].kingdom.performance = this.kingdoms[i].kingdom.performanceStorage;
				this.kingdoms[i].kingdom.performance -= 4;
				if(this.kingdoms[i].kingdom.performance < 1){
					this.kingdoms[i].kingdom.performance = 1;
				}
			}
		}
	}
	private void GoldenAge(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(this.kingdoms[i].kingdom.isInGoldenAge){
				this.kingdoms[i].kingdom.goldenAgeCounter += 1;
				if(this.kingdoms[i].kingdom.goldenAgeCounter >= 10){
					this.kingdoms[i].kingdom.goldenAgeCounter = 0;
					this.kingdoms[i].kingdom.isInGoldenAge = false;
					this.kingdoms[i].kingdom.performance = this.kingdoms[i].kingdom.performanceStorage;
				}
				continue;
			}

			this.kingdoms[i].kingdom.goldenAgeIncreaseCounter += 1;
			if(this.kingdoms[i].kingdom.goldenAgeIncreaseCounter >= 4){
				this.kingdoms[i].kingdom.goldenAgeIncreaseCounter = 0;
				this.kingdoms[i].kingdom.goldenAgeChance += 1;
			}

			int chance = UnityEngine.Random.Range(0,100);
			if(chance < this.kingdoms[i].kingdom.goldenAgeChance){
				Debug.Log(this.kingdoms[i].kingdom.kingdomName + " ENTERS THE GOLDEN AGE! Weeeeeeeeeeeeeeeeee....");
				this.kingdoms[i].kingdom.isInDarkAge = false;
				this.kingdoms[i].kingdom.isInGoldenAge = true;

				this.kingdoms[i].kingdom.goldenAgeIncreaseCounter = 0;
				for(int j = 0; j < this.kingdoms.Count; j++){
					this.kingdoms[j].kingdom.goldenAgeChance = this.kingdoms[j].kingdom.defaultGoldenAgeChance;
				}

				this.kingdoms[i].kingdom.performance = this.kingdoms[i].kingdom.performanceStorage;
				this.kingdoms[i].kingdom.performance += 4;
				if(this.kingdoms[i].kingdom.performance > 10){
					this.kingdoms[i].kingdom.performance = 10;
				}
			}
		}
	}

	private void CreateNewFactionFromExistingOnes(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			int chance = UnityEngine.Random.Range(0,100);
			if(chance < 15){
				Debug.Log(this.kingdoms[i].kingdom.kingdomName + ": CREATE NEW FACTION EVENT!");
				if(this.kingdoms[i].kingdom.cities.Count >= 4){
					List<CityTile> allCitiesWithoutFaction = new List<CityTile>();
					for(int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++){
						if(FactionStorage.Instance.CheckFaction(this.kingdoms[i].kingdom.cities[j].cityAttributes.faction) == null){
							allCitiesWithoutFaction.Add(this.kingdoms[i].kingdom.cities[j]);
						}
					}
					if(allCitiesWithoutFaction.Count > 0){
						List<List<CityTile>> groupedCities = GroupCitiesByCultureAndReligion(allCitiesWithoutFaction);
						if(groupedCities.Count > 0){
							Debug.Log ("CREATING NEW FACTION....");
							List<CityTile> chosenCities = groupedCities[UnityEngine.Random.Range(0,groupedCities.Count)];
							Faction faction = CreateNewFaction (chosenCities[0].cityAttributes.faction.race, chosenCities[0].cityAttributes.faction.religion, chosenCities[0].cityAttributes.faction.culture);

							for(int j = 0; j < chosenCities.Count; j++){
								chosenCities[j].cityAttributes.faction = faction;
							}

							this.kingdoms[i].kingdom.factions.Add (faction);
							FactionStorage.Instance.AddFaction (faction);
						}else{
							Debug.Log("THERE ARE NO 2 OR MORE FACTIONLESS CITIES HAVE THE SAME CULTURE, RELIGION, AND RACE.");
						}
					}else{
						Debug.Log("ALL CITIES HAVE EXSTING FACTIONS. CAN'T CREATE NEW FACTION.");
					}
				}else{
					Debug.Log("LOW CITY COUNT. CAN'T CREATE NEW FACTION.");
				}
			}
		}
	}

	private List<List<CityTile>> GroupCitiesByCultureAndReligion (List<CityTile> cityTiles){
		List<List<CityTile>> groupedCities = new List<List<CityTile>>();
		for(int i = 0; i < cityTiles.Count; i++){
			List<CityTile> subList = new List<CityTile>();
			for(int j = 0; j < cityTiles.Count; j++){
				if(cityTiles[i].cityAttributes.faction.culture == cityTiles[j].cityAttributes.faction.culture && cityTiles[i].cityAttributes.faction.religion == cityTiles[j].cityAttributes.faction.religion
					&& cityTiles[i].cityAttributes.faction.race == cityTiles[j].cityAttributes.faction.race){
					subList.Add(cityTiles[j]);
				}
			}

			if(subList.Count > 1){
				groupedCities.Add(subList);
			}
		}

		return groupedCities;
	}

	private void SplitKingdoms(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			int chance = UnityEngine.Random.Range(0,100);
			if(chance < 4){
				Debug.Log(this.kingdoms[i].kingdom.kingdomName + ": SPLIT KINGDOMS EVENT!");
				SplitFaction(this.kingdoms[i]);
			}
		}
	}
}
