using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	public delegate void TurnEndedDelegate(int currentDay);
	public TurnEndedDelegate turnEnded;

	public Sprite grasslandSprite;
	public Sprite woodlandSprite;
	public Sprite forestSprite;
	public Sprite desertSprite;
	public Sprite tundraSprite;
	public Sprite snowSprite;

	public List<GameObject> cities;
	public GameObject kingdomTilePrefab;
	public List<KingdomTileTest> kingdoms;

	public List<CooperateEvents> pendingCooperateEvents = new List<CooperateEvents>();

	public int currentDay = 0;

	public bool isDayPaused = false;

	public int daysUntilNextHarvest = 30;
	public bool harvestTime;


	void Awake(){
		Instance = this;
		this.cities = new List<GameObject>();

	}

	void Start(){
		turnEnded += IncrementDaysOnTurn;
		turnEnded += WaitForHarvest;
		MapGenerator();
		GenerateCities();
		GenerateBiomes ();
		GenerateInitialKingdoms();
		CreateInitialRelationshipsToLords();
//		AssignCitiesToKingdoms();
		GenerateCityConnections();
		UpdateLordAdjacency();
//		GenerateInitialCitizens ();
		StartResourceProductions ();
		UserInterfaceManager.Instance.SetCityInfoToShow (this.cities [0].GetComponent<CityTileTest> ());
	}


	/*
	 * Get List of tiles (Path) that will connect 2 city tiles
	 * */
	public IEnumerable<Tile> GetPath(Tile startingTile, Tile destinationTile, bool forCreatingRoads){
		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destinationTile.X, 2) + Math.Pow(t.Y - destinationTile.Y, 2));
		var path = PathFind.PathFind.FindPath(startingTile, destinationTile, distance, estimate, forCreatingRoads);
		return path;
	}

	void MapGenerator(){
		GridMap.Instance.GenerateGrid();
	}

	void GenerateCities(){
		CreateCity(GridMap.Instance.listHexes [293].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [1244].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [2094].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [2127].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [1222].GetComponent<HexTile>());
		CreateCity(GridMap.Instance.listHexes [276].GetComponent<HexTile>());
//		CreateCity(GridMap.Instance.listHexes [705].GetComponent<HexTile>());
//		CreateCity(GridMap.Instance.listHexes [1706].GetComponent<HexTile>());
	}

	void GenerateCityConnections(){
		for (int i = 0; i < this.cities.Count; i++) {
			CityTileTest currentCityTile = this.cities[i].GetComponent<CityTileTest>();
			int nextCityIndex = i + 1;
			int previousCityIndex = i - 1;
			if (nextCityIndex >= this.cities.Count) {
				nextCityIndex = 0;
			}
			if (previousCityIndex < 0) {
				previousCityIndex = 5;
			}

			currentCityTile.cityAttributes.AddCityAsConnected(this.cities [nextCityIndex].GetComponent<CityTileTest>());
			currentCityTile.cityAttributes.AddCityAsConnected(this.cities [previousCityIndex].GetComponent<CityTileTest>());

			Tile thisCity = currentCityTile.GetComponent<HexTile>().tile;
			for (int j = 0; j < currentCityTile.cityAttributes.connectedCities.Count; j++) {
				Tile tileToConnectTo = currentCityTile.cityAttributes.connectedCities[j].hexTile.tile;
				thisCity.canPass = true;
				tileToConnectTo.canPass = true;
				List<Tile> roads = GetPath(thisCity, tileToConnectTo, true).ToList();
				for (int k = 0; k < roads.Count; k++) {
					roads [k].hexTile.isRoad = true;
					if (!roads [k].hexTile.isCity && !roads [k].hexTile.isOccupied) {
						roads [k].hexTile.SetTileColor (Color.gray);
					}
				}
				thisCity.canPass = false;
				tileToConnectTo.canPass = false;
			}
		}







	}

	void GenerateBiomes(){
		for (int i = 0; i < this.cities.Count; i++) {
			HexTile currentHexTile = this.cities [i].GetComponent<HexTile> ();
			HexTile[] neighbours = currentHexTile.GetTilesInRange(5);
			BIOMES targetBiomeType = BIOMES.BARE;

			Sprite biomeSprite = null;
			if (i == 0) {
				targetBiomeType = BIOMES.GRASSLAND;
				biomeSprite = grasslandSprite;
			} else if (i == 1) {
				targetBiomeType = BIOMES.DESERT;
				biomeSprite = desertSprite;
			} else if (i == 2) {
				targetBiomeType = BIOMES.WOODLAND;
				biomeSprite = woodlandSprite;
			} else if (i == 3) {
				targetBiomeType = BIOMES.GRASSLAND;
				biomeSprite = grasslandSprite;
			} else if (i == 4) {
				targetBiomeType = BIOMES.FOREST;
				biomeSprite = forestSprite;
			} else if (i == 5) {
				targetBiomeType = BIOMES.WOODLAND;
				biomeSprite = woodlandSprite;
			}else if (i == 6) {
				targetBiomeType = BIOMES.SNOW;
				biomeSprite = snowSprite;
			}else if (i == 7) {
				targetBiomeType = BIOMES.TUNDRA;
				biomeSprite = tundraSprite;
			}

			for (int j = 0; j < neighbours.Length; j++) {
				neighbours[j].SetTileBiomeType(targetBiomeType);
				neighbours[j].GenerateResourceValues ();
				neighbours[j].GetComponent<SpriteRenderer> ().sprite = biomeSprite;
				neighbours [j].elevationType = ELEVATION.PLAIN;
				switch (targetBiomeType) {
				case BIOMES.GRASSLAND:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.WOODLAND:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				case BIOMES.FOREST:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.DESERT:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				case BIOMES.TUNDRA:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.LUMBER;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.MANA;
					break;
				case BIOMES.SNOW:
					neighbours[j].primaryResourceToPurchaseTile = RESOURCE.STONE;
					neighbours[j].secondaryResourceToPurchaseTile = RESOURCE.METAL;
					break;
				}
			}
		}
	}

	void GenerateInitialKingdoms(){
		List<GameObject> tempCities = new List<GameObject> ();
		tempCities.AddRange (cities);

		GameObject goKingdom1 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom1.transform.parent = this.transform;
		goKingdom1.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){this.cities[0].GetComponent<CityTileTest>()}, new Color(255f/255f, 0f/255f, 206f/255f));
		goKingdom1.name = goKingdom1.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		this.kingdoms.Add (goKingdom1.GetComponent<KingdomTileTest>());

		GameObject goKingdom2 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom2.transform.parent = this.transform;
		goKingdom2.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, new List<CityTileTest>(){this.cities[5].GetComponent<CityTileTest>()}, new Color(40f/255f, 255f/255f, 0f/255f));
		goKingdom2.name = goKingdom2.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		kingdoms.Add (goKingdom2.GetComponent<KingdomTileTest>());

//		GameObject goKingdom3 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
//		goKingdom3.transform.parent = this.transform;
//		goKingdom3.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.HUMANS, new List<CityTileTest>(){this.cities[2].GetComponent<CityTileTest>()}, new Color(0f/255f, 234f/255f, 255f/255f));
//		goKingdom3.name = goKingdom3.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
//		kingdoms.Add (goKingdom3.GetComponent<KingdomTileTest>());
//
//
//		GameObject goKingdom4 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
//		goKingdom4.transform.parent = this.transform;
//		goKingdom4.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.MINGONS, new List<CityTileTest>(){this.cities[3].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
//		goKingdom4.name = goKingdom4.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
//		kingdoms.Add (goKingdom4.GetComponent<KingdomTileTest>());
//
//		GameObject goKingdom5 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
//		goKingdom5.transform.parent = this.transform;
//		goKingdom5.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.CROMADS, new List<CityTileTest>(){this.cities[4].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
//		goKingdom5.name = goKingdom5.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
//		kingdoms.Add (goKingdom5.GetComponent<KingdomTileTest>());
//
//		GameObject goKingdom6 = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
//		goKingdom6.transform.parent = this.transform;
//		goKingdom6.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.MINGONS, new List<CityTileTest>(){this.cities[5].GetComponent<CityTileTest>()}, new Color(157f/255f, 0f/255f, 255f/255f));
//		goKingdom6.name = goKingdom6.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
//		kingdoms.Add (goKingdom6.GetComponent<KingdomTileTest>());
	}

	public KingdomTileTest CreateNewKingdom(RACE race, List<CityTileTest> initialCities){
		GameObject goKingdom = (GameObject)GameObject.Instantiate(kingdomTilePrefab);
		goKingdom.transform.parent = this.transform;
		goKingdom.GetComponent<KingdomTileTest>().CreateKingdom (5f, race, initialCities, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
		goKingdom.name = goKingdom.GetComponent<KingdomTileTest>().kingdom.kingdomName;
		this.kingdoms.Add (goKingdom.GetComponent<KingdomTileTest>());
		goKingdom.GetComponent<KingdomTileTest>().kingdom.lord.CreateInitialRelationshipsToLords();
		AddRelationshipToOtherLords (goKingdom.GetComponent<KingdomTileTest> ().kingdom.lord);
		GenerateCityConnections();
		UpdateLordAdjacency();
		return goKingdom.GetComponent<KingdomTileTest>();
	}

	public void AddRelationshipToOtherLords(Lord newLord){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if (this.kingdoms[i].kingdom.id != newLord.kingdom.id) {
				this.kingdoms[i].kingdom.lord.relationshipLords.Add (new Relationship(newLord, DECISION.NEUTRAL, 0));
			}
		}
	}

	public void RemoveRelationshipToOtherLords(Lord lordToRemove){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			for (int j = 0; j < this.kingdoms[i].kingdom.lord.relationshipLords.Count; j++) {
//				Relationship currRelationship = this.kingdoms[i].kingdom.lord.relationshipLords[j];
				if (this.kingdoms[i].kingdom.lord.relationshipLords[j].lord.id == lordToRemove.id) {
					this.kingdoms[i].kingdom.lord.relationshipLords.Remove(this.kingdoms[i].kingdom.lord.relationshipLords[j]);
				}
			}
		}
	}

	public void UpdateLordAdjacency(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			this.kingdoms[i].kingdom.lord.UpdateAdjacentLords();
		}
	}

	internal void CreateInitialRelationshipsToLords(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			this.kingdoms [i].kingdom.lord.CreateInitialRelationshipsToLords ();
//			CityTest otherCity = this.city.kingdomTile.kingdom.cities [i].cityAttributes;
//			if (otherCity.id != this.city.id) {
//				this.relationshipLords.Add (new Relationship (otherCity.cityLord.id, otherCity.cityLord.name, DECISION.NEUTRAL, 0));
//			}
		}
	}
	void AssignCitiesToKingdoms(){
		for (int i = 0; i < cities.Count; i++) {
			KingdomTileTest randomKingdom = kingdoms [UnityEngine.Random.Range (0, kingdoms.Count)];
			randomKingdom.AddCityToKingdom (cities [i].GetComponent<CityTileTest> ());
		}
	}

	void CreateCity(HexTile tile){
		tile.SetTileColor(Color.black);
		tile.isCity = true;
//		tile.isOccupied = true;
		tile.tile.canPass = false;
		tile.gameObject.AddComponent<CityTileTest>();
		tile.gameObject.GetComponent<CityTileTest>().hexTile = tile;
		tile.gameObject.GetComponent<CityTileTest>().cityAttributes = new CityTest(tile, null);
		this.cities.Add(tile.gameObject);
	}

	void StartResourceProductions(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				this.kingdoms[i].kingdom.cities[j].SetCityAsActiveAndSetProduction ();
//				this.kingdoms [i].kingdom.cities [j].hexTile.isOccupied = true;
			}
		}
		turnEnded += TriggerCooperateEvents;
		turnEnded += CheckCooperateEvents;
		turnEnded += GeneralAI.TriggerCheckTask;
		turnEnded += GeneralAI.TriggerMove;

		for (int i = 0; i < this.kingdoms.Count; i++) {
			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
				turnEnded += this.kingdoms[i].kingdom.cities[j].CheckVisitingGenerals;
			}
		}
		ActivateProducationCycle();
	}

	public void ActivateProducationCycle(){
		InvokeRepeating("EndTurn", 0f, 0.5f);
	}
		
	public void EndTurn(){
		if (isDayPaused) {
			return;
		}
		turnEnded(this.currentDay);
		if(this.kingdoms.Count > 0){
			for (int i = 0; i < this.kingdoms.Count; i++) {
				for (int j = 0; j < this.kingdoms[i].kingdom.lord.relationshipLords.Count; j++) {
					if(this.kingdoms[i].kingdom.lord.relationshipLords[j].lord.kingdom.cities.Count <= 0){
						this.kingdoms [i].kingdom.lord.relationshipLords.RemoveAt (j);
						j--;
					}	
				}
			}
			for (int i = 0; i < this.kingdoms.Count; i++) {
				if(this.kingdoms[i].kingdom.cities.Count <= 0){
					this.kingdoms [i].kingdom.isDead = true;
					turnEnded -= this.kingdoms [i].TurnActions;
					Destroy (this.kingdoms [i].gameObject);
					this.kingdoms.RemoveAt (i);
					i--;
				}else{
				}
			}
		}

		if((this.currentDay % 500) == 0){
			Debug.Log ("-----------------500 DAYS!------------------");
			Debug.Log ("TRADE COUNT: " + Utilities.tradeCount);
			Debug.Log ("HELP COUNT: " + Utilities.helpCount);
			Debug.Log ("GIFT COUNT: " + Utilities.giftCount);
			Debug.Log ("COOPERATE 1 COUNT: " + Utilities.cooperate1Count);
			Debug.Log ("COOPERATE 2 COUNT: " + Utilities.cooperate2Count);

		}
//		for (int i = 0; i < this.kingdoms.Count; i++) {
//			for (int j = 0; j < this.kingdoms [i].kingdom.cities.Count; j++) {
//				if (currentDay % 7 == 0) { //Select a new Citizen to create(Only occurs every 7 days)
//					this.kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenForCreation(true); //assign citizen type to the City's newCitizenTarget
//				}
//
////				if(kingdoms [i].kingdom.cities [j].cityAttributes.upgradeCitizenTarget == null){
////					kingdoms [i].kingdom.cities [j].cityAttributes.SelectCitizenToUpgrade();
////				}
//
//			}
//		}
	}

	public void TogglePause(){
		isDayPaused = !isDayPaused;
	}

	void IncrementDaysOnTurn(int currentDay){
		this.currentDay++;
		UserInterfaceManager.Instance.UpdateDayCounter(this.currentDay);
		if((currentDay % 50) == 0){
			UserInterfaceManager.Instance.externalAffairsLogList.Add (string.Empty);
			UserInterfaceManager.Instance.currentIndex = UserInterfaceManager.Instance.externalAffairsLogList.Count - 1;
		}
		CheckLordsInternalPersonality ();
	}
	void WaitForHarvest(int currentDay){
		if (daysUntilNextHarvest <= 1) {
			for (int i = 0; i < this.kingdoms.Count; i++) {
				for (int j = 0; j < this.kingdoms[i].kingdom.cities.Count; j++) {
					this.kingdoms[i].kingdom.cities[j].cityAttributes.TriggerFoodHarvest();
				}
			}
			daysUntilNextHarvest = 30;
			harvestTime = true;
		} else {
			daysUntilNextHarvest--;
			harvestTime = false;
		}

	}
	internal void CheckLordsInternalPersonality(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			this.kingdoms [i].kingdom.lord.PositiveInternalPersonalityEvents ();
			this.kingdoms [i].kingdom.lord.NegativeInternalPersonalityEvents ();
		}
	}
	internal void TriggerCooperateEvents(int currentDay){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < 15){
			int randomEvent = UnityEngine.Random.Range (0, 2);
			if(randomEvent == 0){
				CooperateEvent1 ();
			}else{
				CooperateEvent2 ();
			}
		}
	}
	internal void CooperateEvent1(){
		List<KingdomTileTest> kingdoms = new List<KingdomTileTest> ();
		kingdoms.AddRange (this.kingdoms);


		if(kingdoms.Count > 1){
			kingdoms = Utilities.Shuffle (kingdoms);
			/*
			A huge monster guarding some treasure is discovered. 
			The Lords must cooperate to defeat it. If they cooperate, the monster is defeated and they are able to split the treasure. 
			One can be betrayed by unleashing the monster onto the other Lord's realm while the other takes all the treasure. 
			If both attempt to do this, the monster stays and the treasure remains guarded.
			*/

			int randomLord1 = UnityEngine.Random.Range (0, kingdoms.Count);
			Lord lord1 = kingdoms [randomLord1].kingdom.lord;
			kingdoms.RemoveAt (randomLord1);

			int randomLord2 = UnityEngine.Random.Range (0, kingdoms.Count);
			Lord lord2 = kingdoms [randomLord2].kingdom.lord;
			kingdoms.RemoveAt (randomLord2);

			DECISION lord1Decision = lord1.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE1, lord2);
			DECISION lord2Decision = lord2.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE1, lord1);

			if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.NICE){
				
			}else if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.RUDE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.RUDE){

			}
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += this.currentDay.ToString () + ": COOPERATE EVENT 1: " + lord1.name + " decided to be " + lord1Decision.ToString() 
				+ ". " + lord2.name + " decided to be " + lord2Decision.ToString() + ".\n\n";

			pendingCooperateEvents.Add(new CooperateEvents(lord1.id, lord1Decision, lord2.id, lord2Decision, LORD_EVENTS.COOPERATE1, (currentDay + 5)));

			Utilities.cooperate1Count++;


		}else{
			Debug.Log ("THERE IS NOT ENOUGH KINGDOMS! CAN'T COOPERATE (1)");
		}
	}

	internal void CooperateEvent2(){
		List<KingdomTileTest> kingdoms = new List<KingdomTileTest> ();
		kingdoms.AddRange (this.kingdoms);


		if(kingdoms.Count > 1){
			kingdoms = Utilities.Shuffle (kingdoms);
			/*
			A huge monster guarding some treasure is discovered. 
			The Lords must cooperate to defeat it. If they cooperate, the monster is defeated and they are able to split the treasure. 
			One can be betrayed by unleashing the monster onto the other Lord's realm while the other takes all the treasure. 
			If both attempt to do this, the monster stays and the treasure remains guarded.
			*/

			int randomLord1 = UnityEngine.Random.Range (0, kingdoms.Count);
			Lord lord1 = kingdoms [randomLord1].kingdom.lord;
			kingdoms.RemoveAt (randomLord1);

			int randomLord2 = UnityEngine.Random.Range (0, kingdoms.Count);
			Lord lord2 = kingdoms [randomLord2].kingdom.lord;
			kingdoms.RemoveAt (randomLord2);

			DECISION lord1Decision = lord1.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE2, lord2);
			DECISION lord2Decision = lord2.ComputeDecisionBasedOnPersonality (LORD_EVENTS.COOPERATE2, lord1);

			if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.NICE && lord2Decision == DECISION.RUDE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.NICE){

			}else if(lord1Decision == DECISION.RUDE && lord2Decision == DECISION.RUDE){

			}

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += this.currentDay.ToString () + ": COOPERATE EVENT 2: " + lord1.name + " decided to be " + lord1Decision.ToString() 
				+ ". " + lord2.name + " decided to be " + lord2Decision.ToString() + ".\n\n";
			
			pendingCooperateEvents.Add(new CooperateEvents(lord1.id, lord1Decision, lord2.id, lord2Decision, LORD_EVENTS.COOPERATE2, (currentDay + 5)));

			Utilities.cooperate2Count++;

		}else{
			Debug.Log ("THERE IS NOT ENOUGH KINGDOMS! CAN'T COOPERATE (2)");
		}
	}
	internal void CheckCooperateEvents(int currentDay){
		if(this.pendingCooperateEvents.Count > 0){
			for(int i = 0; i < this.pendingCooperateEvents.Count; i++){
				if(currentDay == this.pendingCooperateEvents[i].daysLeft){
					Lord lord1 = SearchLordById (this.pendingCooperateEvents [i].lord1Id);
					Lord lord2 = SearchLordById (this.pendingCooperateEvents [i].lord2Id);

					if(lord1 == null || lord2 == null){
						Debug.Log ("CAN'T ADJUST LIKENESS. AT LEAST 1 LORD IS DEAD.");
						continue;
					}

					lord1.AdjustLikeness (lord2, this.pendingCooperateEvents [i].lord1Decision, this.pendingCooperateEvents [i].lord2Decision, this.pendingCooperateEvents [i].eventType, false);
					lord2.AdjustLikeness (lord1, this.pendingCooperateEvents [i].lord2Decision, this.pendingCooperateEvents [i].lord1Decision, this.pendingCooperateEvents [i].eventType, false);


					this.pendingCooperateEvents.RemoveAt (i);
					break;

				}
			}
		}
	}

	internal Lord SearchLordById(int id){
		for(int i = 0; i < this.kingdoms.Count; i++){
			if(this.kingdoms[i].kingdom.lord.id == id){
				return this.kingdoms [i].kingdom.lord;
			}
		}

		return null;
	}
}
