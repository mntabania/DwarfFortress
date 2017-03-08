using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliticsPrototypeManager : MonoBehaviour {

	public static PoliticsPrototypeManager Instance = null;

	public delegate void TurnEndedDelegate();
	public TurnEndedDelegate turnEnded;

	public int month;
	public int week;
	public int year;

	public GameObject kingdomTilePrefab;
	public GameObject kingdomsParent;
	public List<KingdomTileTest> kingdoms;

	public static Dictionary<int, List<Royalty>> fullGenealogy;

	public bool isDayPaused = false;

	void Awake(){
		Instance = this;
		fullGenealogy = new Dictionary<int, List<Royalty>>();
	}

	void Start(){
		GridMap.Instance.GenerateGrid();
		EquatorGenerator.Instance.GenerateEquator();
		Biomes.Instance.GenerateElevation();
		Biomes.Instance.GenerateBiome();
		Biomes.Instance.GenerateTileDetails ();
		CityGenerator.Instance.GenerateCities();

		for (int i = 0; i < CityGenerator.Instance.cities.Count; i++) {
//			Destroy(CityGenerator.Instance.cities[i].gameObject.GetComponent<CityTile>());
			CityGenerator.Instance.cities[i].gameObject.AddComponent<CityTileTest>();
			CityGenerator.Instance.cities[i].gameObject.GetComponent<CityTileTest>().hexTile = CityGenerator.Instance.cities[i];
		}

		this.GenerateKingdoms();
		CreateInitialRelationshipsToKingdoms ();
		PoliticsPrototypeUIManager.Instance.LoadKingdoms();
		StartWeekProgression();
	}

	void StartWeekProgression(){
		InvokeRepeating("StartTime", 0f, 0.2f);
	}

	void StartTime(){
		if (isDayPaused) {
			return;
		}
		this.week += 1;
		if (week > 4) {
			this.week = 1;
			this.month += 1;
			if (this.month > 12) {
				this.month = 1;
				this.year += 1;
				RoyaltyEventDelegate.TriggerIncreaseIllnessAndAccidentChance ();
			}
		}
		if (turnEnded != null) {
			turnEnded ();
		}

		if(this.kingdoms.Count > 0){
//			for (int i = 0; i < this.kingdoms.Count; i++) {
//				for (int j = 0; j < this.kingdoms[i].kingdom.lord.relationshipLords.Count; j++) {
//					if(this.kingdoms[i].kingdom.lord.relationshipLords[j].lord.kingdom.cities.Count <= 0){
//						this.kingdoms [i].kingdom.lord.relationshipLords.RemoveAt (j);
//						j--;
//					}	
//				}
//			}
			for (int i = 0; i < this.kingdoms.Count; i++) {
				if(this.kingdoms[i].kingdom.cities.Count <= 0){
					this.kingdoms [i].kingdom.isDead = true;
//					turnEnded -= this.kingdoms [i].TurnActions;
//					Destroy (this.kingdoms [i].gameObject);
					this.kingdoms.RemoveAt (i);
					i--;
				}
			}
		}

	}

	void GenerateKingdoms(){
		List<HexTile> elligibleCities = new List<HexTile>(CityGenerator.Instance.cities);

		int numOfKingdomsToCreate = 4;
		for (int i = 0; i < numOfKingdomsToCreate; i++) {
			
			List<CityTileTest> citiesForKingdom = new List<CityTileTest>();
			for (int j = 0; j < 4; j++) {
				int randomIndex = Random.Range(0, elligibleCities.Count);
				citiesForKingdom.Add(elligibleCities[randomIndex].GetComponent<CityTileTest>());
				elligibleCities.RemoveAt(randomIndex);
			}
			CreateNewKingdom(citiesForKingdom);
		}
	}

	public KingdomTest CreateNewKingdom(List<CityTileTest> cities){
		GameObject goKingdom = (GameObject)GameObject.Instantiate (kingdomTilePrefab);
		goKingdom.transform.parent = kingdomsParent.transform;
		goKingdom.GetComponent<KingdomTileTest>().CreateKingdom (5f, RACE.ELVES, cities,  Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
		goKingdom.name = goKingdom.GetComponent<KingdomTileTest> ().kingdom.kingdomName;
		this.kingdoms.Add (goKingdom.GetComponent<KingdomTileTest>());
		return goKingdom.GetComponent<KingdomTileTest>().kingdom;
	}

	#region family tree
	public void RegisterRoyalty(Royalty royalty){
		if (fullGenealogy.ContainsKey (royalty.generation)) {
			List<Royalty> allRoyaltiesInGeneration = fullGenealogy [royalty.generation];
			allRoyaltiesInGeneration.Add(royalty);
		} else {
			fullGenealogy.Add (royalty.generation, new List<Royalty> (){ royalty });
		}
	}
	#endregion

	internal void CreateInitialHatred(){
		for(int i = 0; i < this.kingdoms.Count; i++){
			for(int j = 0; j < this.kingdoms[i].kingdom.royaltyList.allRoyalties.Count; j++){
				this.kingdoms [i].kingdom.royaltyList.allRoyalties [j].ChangeHatred ();
			}
		}
	}
	public void AddRelationshipToOtherKingdoms(KingdomTest newKingdom){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if (this.kingdoms[i].kingdom.id != newKingdom.id) {
				this.kingdoms[i].kingdom.relationshipKingdoms.Add (new RelationshipKingdoms(newKingdom, DECISION.NEUTRAL, 0));
			}
		}
	}

	public KingdomTest GetKingdomByName(string name){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			if (this.kingdoms [i].kingdom.kingdomName == name) {
				return this.kingdoms[i].kingdom;
			}
		}
		return null;
	}

	internal void CreateInitialRelationshipsToKingdoms(){
		for (int i = 0; i < this.kingdoms.Count; i++) {
			this.kingdoms [i].kingdom.CreateInitialRelationshipsToKingdoms ();
		}
	}

	internal void TogglePause(){
		isDayPaused = !isDayPaused;
	}
}
