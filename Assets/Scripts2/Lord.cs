using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Lord {
	public int id;
	public string name;
	public int predictability;
	public int persistence;
	public int trustworthiness;
	public int selflessness;
	public int skill;
	public int racism;
	public int religiousTolerance;
	public int likeCitizen;
	public int accumulatedScore;
	public int dealings;
	public KingdomTest kingdom;
	public LORD_PERSONALITY personality;
	public LordInternalPersonality internalPersonality;
	public CHARACTER character;
	public INTELLIGENCE intelligence;
	public AGGRESSIVENESS aggressiveness;
	public List<GOALS> goals;
	public List<List<string>> tasks;
	public List<PUBLIC_IMAGE> publicImages;
	public List<Relationship> relationshipKings;
	public List<Relationship> relationshipLords;
	public List<WAR_REASONS> lordWarReasons;
	public List<PEACE_REASONS> lordPeaceReasons;
	public MIGHT_TRAIT lordMightTrait;
	public RELATIONSHIP_TRAIT lordRelationshipTrait;
	public List<MilitaryData> militaryData;

	public int daysWithoutWar = -1;

	internal bool hasExperiencedWar = false;

	protected List<Lord> candidatesForWar;

	protected List<Relationship> targetableLords{
		get { return relationshipLords.Where (x => x.isAdjacent && !x.isAtWar).ToList();}
	}

	internal List<Relationship> currentWars{
		get { return relationshipLords.Where (x => x.isAtWar).ToList();}
	}

	int defaultWarChance = 5;
	int currentWarChance = 0;
	public Lord(KingdomTest kingdom){
		this.id = 1 + GetID ();
		this.name = RandomNameGenerator.GenerateRandomName();
		this.predictability = UnityEngine.Random.Range (0, 10);
		this.persistence = UnityEngine.Random.Range (0, 10);
		this.trustworthiness = UnityEngine.Random.Range (0, 10);
		this.selflessness = UnityEngine.Random.Range (0, 10);
		this.skill = UnityEngine.Random.Range (0, 10);
		this.racism = UnityEngine.Random.Range (0, 10);
		this.religiousTolerance = UnityEngine.Random.Range (0, 10);		
//		this.personality = (LORD_PERSONALITY)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(LORD_PERSONALITY)).Length));
		this.personality = LORD_PERSONALITY.TIT_FOR_TAT;
		this.intelligence = (INTELLIGENCE)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(INTELLIGENCE)).Length));
		this.aggressiveness = (AGGRESSIVENESS)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(AGGRESSIVENESS)).Length));
		this.internalPersonality = new LordInternalPersonality ("");
		this.character = (CHARACTER)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CHARACTER)).Length));		
		this.likeCitizen = 0;
		this.kingdom = kingdom;
		this.goals = new List<GOALS> ();
		this.tasks = new List<List<string>> ();
		this.publicImages = new List<PUBLIC_IMAGE> ();
		this.relationshipKings = new List<Relationship> ();
		this.relationshipLords = new List<Relationship> ();
		this.lordWarReasons = GenerateWarReasons();
		this.lordMightTrait = GenerateMightTrait();
		this.lordRelationshipTrait = GenerateRelationshipTrait();
		this.lordPeaceReasons = GeneratePeaceReasons();
		this.currentWarChance = this.defaultWarChance;
		this.militaryData = new List<MilitaryData> ();
		SetLastID (this.id);
	}

	internal void UpdateAdjacentLords(){
		List<int> adjacentLordIDs = new List<int>();
		for (int i = 0; i < this.kingdom.cities.Count; i++) {
			for (int j = 0; j < this.kingdom.cities[i].cityAttributes.connectedCities.Count; j++) {
				if (this.kingdom.cities [i].cityAttributes.connectedCities [j].cityAttributes.kingdomTile) {
					if (this.kingdom.cities [i].cityAttributes.connectedCities [j].cityAttributes.kingdomTile.kingdom.id != this.kingdom.id) {
						if (!adjacentLordIDs.Contains (this.kingdom.cities [i].cityAttributes.connectedCities [j].cityAttributes.kingdomTile.kingdom.lord.id)) {
							adjacentLordIDs.Add (this.kingdom.cities [i].cityAttributes.connectedCities [j].cityAttributes.kingdomTile.kingdom.lord.id);
						}
					}
				}
			}
		}

		for (int i = 0; i < this.relationshipLords.Count; i++) {
			if (adjacentLordIDs.Contains (this.relationshipLords[i].lord.id)) {
				this.relationshipLords [i].isAdjacent = true;
			} else {
				this.relationshipLords [i].isAdjacent = false;
			}
		}
	}

	private int GetID(){
		return Utilities.lastLordId;
	}
	private void SetLastID(int id){
		Utilities.lastLordId = id;
	}

//	internal void SetLordToWar(int id){
//		for (int i = 0; i < this.relationshipLords.Count; i++) {
//			if (this.relationshipLords[i].id == id) {
//				this.relationshipLords [i].isAtWar = true;
//			}
//		}
//	}

	internal void CreateInitialRelationshipsToLords(){
		for (int i = 0; i < GameManager.Instance.kingdoms.Count; i++) {
			KingdomTest otherKingdom = GameManager.Instance.kingdoms[i].kingdom;
			if (otherKingdom.id != this.kingdom.id) {
				this.relationshipLords.Add (new Relationship (otherKingdom.lord, DECISION.NEUTRAL, 0));
			}
		}
	}

	List<WAR_REASONS> GenerateWarReasons(){
		int totalChances = 0;
		Dictionary<WAR_REASONS, int> traitsDict = new Dictionary<WAR_REASONS, int>(Utilities.lordWarReasons[this.personality]); 
		List<WAR_REASONS> generatedReasons = new List<WAR_REASONS>();
		while(generatedReasons.Count != 2){
			for (int i = 0; i < traitsDict.Keys.Count; i++) {
				totalChances += traitsDict[traitsDict.Keys.ElementAt(i)];
			}

			int chance = Random.Range (0, totalChances);
			int upperBound = 0;
			int lowerBound = 0;

			for (int i = 0; i < traitsDict.Keys.Count; i++) {
				WAR_REASONS currentReason = traitsDict.Keys.ElementAt(i);
				int currentTraitChance = traitsDict[currentReason];

				upperBound += currentTraitChance;
				if (chance >= lowerBound && chance < upperBound) {
					generatedReasons.Add(currentReason);
					traitsDict.Remove(currentReason);
					break;
				}
				lowerBound = upperBound;
			}
		}
		return generatedReasons;
	}

	List<PEACE_REASONS> GeneratePeaceReasons(){
		List<PEACE_REASONS> peaceReasons = new List<PEACE_REASONS>();
		peaceReasons.Add(PEACE_REASONS.DEFEATED);
		if (this.lordWarReasons.Contains(WAR_REASONS.COVETOUS) || this.lordWarReasons.Contains(WAR_REASONS.IMPOTENT) || 
			this.lordWarReasons.Contains(WAR_REASONS.PARANOID) || this.lordWarReasons.Contains(WAR_REASONS.COMPETITIVE)) {
			peaceReasons.Add(PEACE_REASONS.GOAL_REACHED);
		}

		int chance = Random.Range(0, 100);

		PEACE_REASONS[] randomPeaceReasons = new PEACE_REASONS[] {
			PEACE_REASONS.MANY_WARS,
			PEACE_REASONS.OFFER_ALLIANCE,
			PEACE_REASONS.ENEMY_OF_ENEMY
		};

		if (chance < 2) {
			peaceReasons.Add (randomPeaceReasons[Random.Range(0, randomPeaceReasons.Length)]);
		}
		return peaceReasons;
	}

	MIGHT_TRAIT GenerateMightTrait(){
		Dictionary<MIGHT_TRAIT, int> mightDict = new Dictionary<MIGHT_TRAIT, int>(Utilities.lordMightChecks[this.personality]); 
		int chance = Random.Range (0, 100);
		int upperBound = 0;
		int lowerBound = 0;

		for (int i = 0; i < mightDict.Keys.Count; i++) {
			MIGHT_TRAIT currentMight = mightDict.Keys.ElementAt(i);
			int currentTraitChance = mightDict[currentMight];

			upperBound += currentTraitChance;
			if (chance >= lowerBound && chance < upperBound) {
				return currentMight;
			}
			lowerBound = upperBound;
		}
		return MIGHT_TRAIT.NORMAL;
	}

	RELATIONSHIP_TRAIT GenerateRelationshipTrait(){
		Dictionary<RELATIONSHIP_TRAIT, int> relationshipDict = new Dictionary<RELATIONSHIP_TRAIT, int>(Utilities.lordRelationshipChecks[this.personality]); 
		int chance = Random.Range (0, 100);
		int upperBound = 0;
		int lowerBound = 0;

		for (int i = 0; i < relationshipDict.Keys.Count; i++) {
			RELATIONSHIP_TRAIT currentRelationshipTrait = relationshipDict.Keys.ElementAt(i);
			int currentTraitChance = relationshipDict[currentRelationshipTrait];

			upperBound += currentTraitChance;
			if (chance >= lowerBound && chance < upperBound) {
				return currentRelationshipTrait;
			}
			lowerBound = upperBound;
		}
		return RELATIONSHIP_TRAIT.NORMAL;
	}

	#region DECISION-MAKING
	internal void AdjustLikeness(Lord targetLord, DECISION sourceDecision, DECISION targetDecision, LORD_EVENTS eventType, bool isSender){
		Relationship relationship = SearchRelationship (targetLord);
		relationship.previousInteraction = eventType;
		int eventEffect = EventEffect (eventType, sourceDecision, targetDecision);
		this.accumulatedScore += eventEffect;
		this.dealings++;
		int multiplier = 0;
		int results = 0;
		relationship.previousDecision = targetDecision;
		switch (this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				multiplier = 10;
				if(sourceDecision == DECISION.RUDE){
					multiplier = multiplier / 2;
				}

				results = eventEffect * multiplier;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.TRADE){
				int addedLike = 0;

				if(isSender){
					addedLike = 2;
					if(targetDecision == DECISION.RUDE){
						addedLike = -1;
					}
				}
				results = addedLike;
				relationship.like += results;
				
			}else if(eventType == LORD_EVENTS.HELP){
				int addedLike = 0;

				if(isSender){
					addedLike = 10;
					if(targetDecision == DECISION.RUDE){
						addedLike = -5;
					}
				}else{
					if(sourceDecision == DECISION.NICE){
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}else if(eventType == LORD_EVENTS.GIFT){
				int addedLike = 0;
				if (isSender) {
					addedLike = 10;
					if (targetDecision == DECISION.RUDE) {
						addedLike = -5;
					}
				}
				results = addedLike;
				relationship.like += results;
			}

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + results + ".\n\n";

			break;
		case LORD_PERSONALITY.EMOTIONAL:
			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				multiplier = 10;
				if(eventEffect < 0){
					multiplier = 20;
				}
				if(sourceDecision == DECISION.RUDE){
					multiplier = multiplier / 2;
				}
				results = eventEffect * multiplier;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.TRADE){
				int addedLike = 0;

				if(isSender){
					addedLike = 2;
					if(targetDecision == DECISION.RUDE){
						addedLike = -1;
					}
				}
				results = addedLike;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.HELP){
				int addedLike = 0;

				if(isSender){
					addedLike = 10;
					if(targetDecision == DECISION.RUDE){
						addedLike = -5;
					}
				}else{
					if(sourceDecision == DECISION.NICE){
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}else if(eventType == LORD_EVENTS.GIFT){
				int addedLike = 0;
				if (isSender) {
					addedLike = 10;
					if (targetDecision == DECISION.RUDE) {
						addedLike = -5;
					}
				}
				results = addedLike;
				relationship.like += results;
			}

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + results + ".\n\n";

			break;
		case LORD_PERSONALITY.RATIONAL:
			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				multiplier = 5;
				if(sourceDecision == DECISION.RUDE){
					multiplier = 0;
				}
				results = eventEffect * multiplier;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.TRADE){
				int addedLike = 0;

				if(isSender){
					addedLike = 2;
					if(targetDecision == DECISION.RUDE){
						addedLike = -1;
					}
				}
				results = addedLike;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.HELP){
				int addedLike = 0;

				if(isSender){
					addedLike = 5;
					if(targetDecision == DECISION.RUDE){
						addedLike = -2;
					}
				}else{
					if(sourceDecision == DECISION.NICE){
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}else if(eventType == LORD_EVENTS.GIFT){
				int addedLike = 0;
				if (isSender) {
					addedLike = 5;
					if (targetDecision == DECISION.RUDE) {
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}
//			multiplier = 5;
//			if(eventEffect < 0){
//				multiplier = 25;
//			}
//			relationship.like += (eventEffect * multiplier);

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + results + ".\n\n";

			break;
		case LORD_PERSONALITY.NAIVE:
			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				if(relationship.like < 0 && eventEffect >= 0){
					relationship.like = 0;
				}
				multiplier = 10;
				if(sourceDecision == DECISION.RUDE){
					multiplier = multiplier / 2;
				}
				results = eventEffect * multiplier;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.TRADE){
				int addedLike = 0;

				if(isSender){
					addedLike = 2;
					if(targetDecision == DECISION.RUDE){
						addedLike = -1;
					}
				}
				results = addedLike;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.HELP){
				int addedLike = 0;

				if(isSender){
					addedLike = 10;
					if(targetDecision == DECISION.RUDE){
						addedLike = -5;
					}
				}else{
					if(sourceDecision == DECISION.NICE){
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}else if(eventType == LORD_EVENTS.GIFT){
				int addedLike = 0;
				if (isSender) {
					addedLike = 10;
					if (targetDecision == DECISION.RUDE) {
						addedLike = -5;
					}
				}
				results = addedLike;
				relationship.like += results;
			}


			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + results + ".\n\n";
			break;

		case LORD_PERSONALITY.HATER:
			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				multiplier = 10;
				if(sourceDecision == DECISION.RUDE){
					multiplier = multiplier / 2;
				}
				results = eventEffect * multiplier;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.TRADE){
				int addedLike = 0;

				if(isSender){
					addedLike = 2;
					if(targetDecision == DECISION.RUDE){
						addedLike = -1;
					}
				}
				results = addedLike;
				relationship.like += results;

			}else if(eventType == LORD_EVENTS.HELP){
				int addedLike = 0;

				if(isSender){
					addedLike = 10;
					if(targetDecision == DECISION.RUDE){
						addedLike = -5;
					}
				}else{
					if(sourceDecision == DECISION.NICE){
						addedLike = -2;
					}
				}
				results = addedLike;
				relationship.like += results;
			}else if(eventType == LORD_EVENTS.GIFT){
				int addedLike = 0;
				if (isSender) {
					addedLike = 10;
					if (targetDecision == DECISION.RUDE) {
						addedLike = -5;
					}
				}
				results = addedLike;
				relationship.like += results;
			}


			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + results + ".\n\n";
			break;
		}
		if(relationship.like > 100){
			relationship.like = 100;
		} else if (relationship.like < -100){
			relationship.like = -100;
		}
		relationship.lordRelationship = GetLordRelationship (relationship.like);
//		relationship.previousDecision = targetDecision;


	}
	internal DECISION ComputeDecisionBasedOnPersonality(LORD_EVENTS eventType, Lord targetLord){
		Relationship relationshipWithOtherLord = this.SearchRelationship (targetLord);
		Relationship relationshipFromOtherLord = targetLord.SearchRelationship (this);
		DECISION decision = DECISION.NEUTRAL;
		switch(this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			decision = TitForTat (relationshipWithOtherLord, targetLord);
			break;
//			return TitForTat (relationshipWithOtherLord);
		case LORD_PERSONALITY.EMOTIONAL:
			decision = Emotional (relationshipWithOtherLord, targetLord);
			break;
//			return Emotional (relationshipWithOtherLord);
		case LORD_PERSONALITY.RATIONAL:
			decision = Rational (eventType, relationshipWithOtherLord, relationshipFromOtherLord);
			break;
//			return Rational (eventType, relationshipWithOtherLord, relationshipFromOtherLord);
		case LORD_PERSONALITY.NAIVE:
			decision = Naive (relationshipWithOtherLord, targetLord);
			break;
//			return Naive (relationshipWithOtherLord);
		case LORD_PERSONALITY.HATER:
			decision = Hater (relationshipWithOtherLord, targetLord);
			break;
//			return Hateful (relationshipWithOtherLord);
		}

		if (relationshipWithOtherLord.isFirstEncounter && (eventType == LORD_EVENTS.TRADE || eventType == LORD_EVENTS.HELP || eventType == LORD_EVENTS.GIFT)) {
			relationshipFromOtherLord.isFirstEncounter = false;
		} else {
			relationshipWithOtherLord.isFirstEncounter = false;
		}

		return decision;
	}
	private DECISION TitForTat(Relationship relationshipWithOtherLord, Lord targetLord){
		if(relationshipWithOtherLord.isFirstEncounter){

			if (this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace) {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of same race.\n\n";
			} else {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of diff race.\n\n";
			}

			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, true);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
//			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
//			int chance = UnityEngine.Random.Range (0, 100);
//			if(this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace){
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 90% chance to choose NICE on first encounter of same race.\n\n";
//
//				if(chance < 90){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}else{
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 50% chance to choose NICE on first encounter of different race.\n\n";
//
//				if(chance < 50){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}

		}else{
			int chance = UnityEngine.Random.Range (0, 100);

			if (chance < 80) {
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has 80% chance to choose "
					+ relationshipWithOtherLord.previousDecision.ToString () + " because " + relationshipWithOtherLord.lord.name + " chose " 
					+ relationshipWithOtherLord.previousDecision.ToString () + " during their latest encounter.\n\n";

				return relationshipWithOtherLord.previousDecision;
			} else {
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
					+ this.name + " has 20% chance to choose NICE/RUDE based on how much he likes " + relationshipWithOtherLord.lord.name + ".\n\n";

				int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, false);
				int randomChance = UnityEngine.Random.Range (0, 100);
				if(randomChance < niceChance){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}

		}
	}
	private DECISION Emotional(Relationship relationshipWithOtherLord, Lord targetLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			if (this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace) {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of same race.\n\n";
			} else {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of diff race.\n\n";
			}

			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, true);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
//			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
//			int chance = UnityEngine.Random.Range (0, 100);
//			if(this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace){
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 90% chance to choose NICE on first encounter of same race.\n\n";
//				if(chance < 90){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}else{
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 50% chance to choose NICE on first encounter of different race.\n\n";
//				if(chance < 50){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}
		}else{
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
				+ this.name + " will choose NICE/RUDE based on how much he likes " + relationshipWithOtherLord.lord.name + ".\n\n";
			
			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, false);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
//			if(relationshipWithOtherLord.like >= 0){
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE because he likes " + relationshipWithOtherLord.name + ".\n\n";
//				int chance = UnityEngine.Random.Range (0, 100);
//				if(chance < 90){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}else{
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose RUDE because he dislikes " + relationshipWithOtherLord.name + ".\n\n";
//				int chance = UnityEngine.Random.Range (0, 100);
//				if(chance < 90){
//					return DECISION.RUDE;
//				}
//				return DECISION.NICE;
//			}

		}
	}
	private DECISION Rational(LORD_EVENTS eventType, Relationship relationshipWithOtherLord, Relationship relationshipFromOtherLord){
		int niceEffect = 0;
		int rudeEffect = 0;

		if(relationshipWithOtherLord.isFirstEncounter){
			List<int> effects = new List<int> {EventEffect (eventType, DECISION.NICE, DECISION.NICE), EventEffect (eventType, DECISION.RUDE, DECISION.NICE)
				, EventEffect (eventType, DECISION.NICE, DECISION.RUDE), EventEffect (eventType, DECISION.RUDE, DECISION.RUDE)};
//			if (relationshipWithOtherLord.like >= 0){
//				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.NICE);
//				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.NICE);
//			}else{
//				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.RUDE);
//				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.RUDE);
//			}
			int index = effects.IndexOf(effects.Max());
			if(index == 0){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 100% chance to choose " + DECISION.NICE.ToString() + " during first encounter because it has " 
					+ effects[index] + " max effect.\n\n";
				return DECISION.NICE;
			}else if(index == 1){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 100% chance to choose " + DECISION.RUDE.ToString() + " during first encounter because it has " 
					+ effects[index] + " max effect.\n\n";
				return DECISION.RUDE;
			}else if(index == 2){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 100% chance to choose " + DECISION.NICE.ToString() + " during first encounter because it has " 
					+ effects[index] + " max effect.\n\n";
				return DECISION.NICE;
			}else{
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 100% chance to choose " + DECISION.RUDE.ToString() + " during first encounter because it has " 
					+ effects[index] + " max effect.\n\n";
				return DECISION.RUDE;
			}
		}else{
			
			int chance = UnityEngine.Random.Range (0, 100);
			if(relationshipFromOtherLord.like < -20){
				
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.RUDE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.RUDE);


				if(niceEffect >= rudeEffect){
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name 
						+ " has 70% chance to choose NICE because he assumed " + relationshipWithOtherLord.lord.name + " will choose RUDE.\n\n";

				}else{
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name 
						+ " has 70% chance to choose RUDE because he assumed " + relationshipWithOtherLord.lord.name + " will choose RUDE.\n\n";
				}

				if(chance < 30){
					return DECISION.NICE;
				}else{
					if(niceEffect >= rudeEffect){
						return DECISION.NICE;

					}else{
						return DECISION.RUDE;
					}
				}
				
			}else if(relationshipFromOtherLord.like > 20){
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.NICE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.NICE);


				if(niceEffect >= rudeEffect){
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name 
						+ " has 70% chance to choose NICE because he assumed " + relationshipWithOtherLord.lord.name + " will choose NICE.\n\n";

				}else{
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name 
						+ " has 70% chance to choose RUDE because he assumed " + relationshipWithOtherLord.lord.name + " will choose NICE.\n\n";
				}

				if(chance < 30){
					return DECISION.NICE;
				}else{
					if(niceEffect >= rudeEffect){
						return DECISION.NICE;

					}else{
						return DECISION.RUDE;
					}
				}
			}else{
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name 
					+ " has 90% chance to choose NICE because he wants " + relationshipWithOtherLord.lord.name + " to warm up to him.\n\n";
				if(chance < 90){
					return DECISION.NICE;
				}else{
					return DECISION.RUDE;
				}
			}
		}
	}
	private DECISION Naive(Relationship relationshipWithOtherLord, Lord targetLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			if (this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace) {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of same race.\n\n";
			} else {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of diff race.\n\n";
			}

			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, true);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
//			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
//			int chance = UnityEngine.Random.Range (0, 100);
//			if(this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace){
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 90% chance to choose NICE on first encounter of same race.\n\n";
//				if(chance < 90){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}else{
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
//					+ this.name + " has 70% chance to choose NICE on first encounter of different race.\n\n";
//				if(chance < 70){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}
		}else{
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
				+ this.name + " will choose NICE/RUDE based on how much he likes " + relationshipWithOtherLord.lord.name + ".\n\n";
			
			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, false);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
//			if(relationshipWithOtherLord.like >= 0){
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE because he likes " + relationshipWithOtherLord.name + ".\n\n";
//
//				int chance = UnityEngine.Random.Range (0, 100);
//				if(chance < 90){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}else{
//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose NICE because he dislikes " + relationshipWithOtherLord.name + ".\n\n";
//
//				int chance = UnityEngine.Random.Range (0, 100);
//				if(chance < 50){
//					return DECISION.NICE;
//				}
//				return DECISION.RUDE;
//			}

		}
	}
	private DECISION Hater(Relationship relationshipWithOtherLord, Lord targetLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			if (this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace) {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of same race.\n\n";
			} else {
				UserInterfaceManager.Instance.externalAffairsLogList [UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": "
					+ this.name + " has chance to choose NICE during their first encounter based on how much he likes " + relationshipWithOtherLord.lord.name + " of diff race.\n\n";
			}

			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, true);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
			//			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
			//			int chance = UnityEngine.Random.Range (0, 100);
			//			if(this.kingdom.kingdomRace == targetLord.kingdom.kingdomRace){
			//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
			//					+ this.name + " has 90% chance to choose NICE on first encounter of same race.\n\n";
			//				if(chance < 90){
			//					return DECISION.NICE;
			//				}
			//				return DECISION.RUDE;
			//			}else{
			//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
			//					+ this.name + " has 70% chance to choose NICE on first encounter of different race.\n\n";
			//				if(chance < 70){
			//					return DECISION.NICE;
			//				}
			//				return DECISION.RUDE;
			//			}
		}else{
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " 
				+ this.name + " has 60% chance to choose NICE because he likes " + relationshipWithOtherLord.lord.name + ".\n\n";

			int niceChance = NiceChanceBasedOnLordRelationship (relationshipWithOtherLord.lordRelationship, targetLord, false);
			int randomChance = UnityEngine.Random.Range (0, 100);
			if(randomChance < niceChance){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
			//			if(relationshipWithOtherLord.like >= 0){
			//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE because he likes " + relationshipWithOtherLord.name + ".\n\n";
			//
			//				int chance = UnityEngine.Random.Range (0, 100);
			//				if(chance < 90){
			//					return DECISION.NICE;
			//				}
			//				return DECISION.RUDE;
			//			}else{
			//				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose NICE because he dislikes " + relationshipWithOtherLord.name + ".\n\n";
			//
			//				int chance = UnityEngine.Random.Range (0, 100);
			//				if(chance < 50){
			//					return DECISION.NICE;
			//				}
			//				return DECISION.RUDE;
			//			}

		}
	}
	internal Relationship SearchRelationship(Lord targetLord){
		for(int i = 0; i < this.relationshipLords.Count; i++){
			if(this.relationshipLords[i].lord.id == targetLord.id){
				return this.relationshipLords[i];
			}
		}
		return null;
	}
	private int NiceChanceBasedOnLordRelationship(LORD_RELATIONSHIP lordRelationship, Lord targetLord, bool isFirstEncounter){
		if(this.personality == LORD_PERSONALITY.TIT_FOR_TAT){
			switch(lordRelationship){
			case LORD_RELATIONSHIP.RIVAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.ENEMY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 15;
			case LORD_RELATIONSHIP.COLD:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 25;
			case LORD_RELATIONSHIP.NEUTRAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 50;
					}
				}else{
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 60;
					}
				}
				return 40;
			case LORD_RELATIONSHIP.WARM:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 80;
					}
				}
				return 70;
			case LORD_RELATIONSHIP.FRIEND:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 80;
					}
				}
				return 85;
			case LORD_RELATIONSHIP.ALLY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 80;
					}
				}
				return 95;
			}
		}else if(this.personality == LORD_PERSONALITY.EMOTIONAL){
			switch(lordRelationship){
			case LORD_RELATIONSHIP.RIVAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.ENEMY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 15;
			case LORD_RELATIONSHIP.COLD:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 25;
			case LORD_RELATIONSHIP.NEUTRAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 50;
					}
				}else{
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 60;
					}
				}

				return 40;
			case LORD_RELATIONSHIP.WARM:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 80;
					}
				}
				return 70;
			case LORD_RELATIONSHIP.FRIEND:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 80;
					}
				}
				return 85;
			case LORD_RELATIONSHIP.ALLY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 80;
					}
				}
				return 95;
			}
		}
		else if(this.personality == LORD_PERSONALITY.RATIONAL){ //not used
			switch(lordRelationship){
			case LORD_RELATIONSHIP.RIVAL:
				return 5;
			case LORD_RELATIONSHIP.ENEMY:
				return 15;
			case LORD_RELATIONSHIP.COLD:
				return 25;
			case LORD_RELATIONSHIP.NEUTRAL:
				return 60;
			case LORD_RELATIONSHIP.WARM:
				return 70;
			case LORD_RELATIONSHIP.FRIEND:
				return 85;
			case LORD_RELATIONSHIP.ALLY:
				return 95;
			}
		}
		else if(this.personality == LORD_PERSONALITY.NAIVE){
			switch(lordRelationship){
			case LORD_RELATIONSHIP.RIVAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 30;
					}else{
						return 30;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.ENEMY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 30;
					}else{
						return 30;
					}
				}
				return 25;
			case LORD_RELATIONSHIP.COLD:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 30;
					}else{
						return 30;
					}
				}
				return 25;
			case LORD_RELATIONSHIP.NEUTRAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 70;
					}
				}else{
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}
				}

				return 60;
			case LORD_RELATIONSHIP.WARM:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 90;
					}else{
						return 90;
					}
				}
				return 95;
			case LORD_RELATIONSHIP.FRIEND:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 95;
					}
				}
				return 95;
			case LORD_RELATIONSHIP.ALLY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}else{
						return 95;
					}
				}
				return 95;
			}
		}else if(this.personality == LORD_PERSONALITY.HATER){
			switch(lordRelationship){
			case LORD_RELATIONSHIP.RIVAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.ENEMY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.COLD:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 10;
					}else{
						return 10;
					}
				}
				return 5;
			case LORD_RELATIONSHIP.NEUTRAL:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 50;
					}else{
						return 20;
					}
				}else{
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 95;
					}
				}

				return 35;
			case LORD_RELATIONSHIP.WARM:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 80;
					}else{
						return 50;
					}
				}
				return 60;
			case LORD_RELATIONSHIP.FRIEND:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 80;
					}else{
						return 50;
					}
				}
				return 70;
			case LORD_RELATIONSHIP.ALLY:
				if(isFirstEncounter){
					if (targetLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
						return 80;
					}else{
						return 95;
					}
				}
				return 95;
			}
		}
		return 0;
	}
	private int EventEffect(LORD_EVENTS eventType, DECISION decisionToOtherLord, DECISION decisionOfOtherLord){
		switch (eventType){
		case LORD_EVENTS.TRADE:
			if(decisionToOtherLord == DECISION.NICE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 1;
				}
			}else if(decisionToOtherLord == DECISION.RUDE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return 1;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}

			break;
		case LORD_EVENTS.HELP:
			if(decisionToOtherLord == DECISION.NICE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return -1;
				}
			}else if(decisionToOtherLord == DECISION.RUDE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return 2;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}

		case LORD_EVENTS.GIFT:
			if(decisionToOtherLord == DECISION.NICE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 1;
				}
			}else if(decisionToOtherLord == DECISION.RUDE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return -1;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}
		case LORD_EVENTS.COOPERATE1:
			if(decisionToOtherLord == DECISION.NICE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 1;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return -3;
				}else{
					return 0;
				}
			}else if(decisionToOtherLord == DECISION.RUDE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 2;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return -1;
				}else{
					return 0;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}
		case LORD_EVENTS.COOPERATE2:
			if(decisionToOtherLord == DECISION.NICE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 2;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return -1;
				}else{
					return 0;
				}
			}else if(decisionToOtherLord == DECISION.RUDE){
				if(decisionOfOtherLord == DECISION.NICE){
					return 3;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return -2;
				}else{
					return 0;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return 0;
				}else{
					return 0;
				}
			}
		}
		return 0;
	}

	internal LORD_RELATIONSHIP GetLordRelationship(int likeness){
		if(likeness <= -81){
			return LORD_RELATIONSHIP.RIVAL;
		}else if(likeness >= -80 && likeness <= -41){
			return LORD_RELATIONSHIP.ENEMY;
		}else if(likeness >= -40 && likeness <= -21){
			return LORD_RELATIONSHIP.COLD;
		}else if(likeness >= -20 && likeness <= 20){
			return LORD_RELATIONSHIP.NEUTRAL;
		}else if(likeness >= 21 && likeness <= 40){
			return LORD_RELATIONSHIP.WARM;
		}else if(likeness >= 41 && likeness <= 80){
			return LORD_RELATIONSHIP.FRIEND;
		}else{
			return LORD_RELATIONSHIP.ALLY;
		}
	}
	#endregion

	#region INTERNAL PERSONALITY
	internal void PositiveInternalPersonalityEvents(){
		CityTest randomCity = this.kingdom.cities [UnityEngine.Random.Range (0, this.kingdom.cities.Count)].cityAttributes;
		switch(internalPersonality.goodPersonality){
		case LORD_INTERNAL_PERSONALITY_POSITIVE.GALLANT:
			Gallant (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_POSITIVE.GREEN_THUMB:
			GreenThumb (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_POSITIVE.MONEYMAKER:
			Moneymaker (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_POSITIVE.INSPIRING:
			Inspiring (randomCity);
			break;
		}
	}
	private void Gallant(CityTest chosenCity){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < internalPersonality.gallantChance){
			if(chosenCity.goldCount < 500 || chosenCity.unrest <= 0){
				chosenCity.goldCount -= 500;
				chosenCity.unrest -= 10;
				if(chosenCity.unrest < 0){
					chosenCity.unrest = 0;
				}
				Debug.Log (chosenCity.cityName + ": GALLANT TRIGGERED!");

			}else{
				Debug.Log ("GALLANT: NOT ENOUGH GOLD OR UNREST IS ZERO(0).");
			}
		}
	}
	private void GreenThumb(CityTest chosenCity){
		if(GameManager.Instance.daysUntilNextHarvest <= 1){
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < internalPersonality.greenthumbChance){
				chosenCity.farmerMultiplier = 3f;
				Debug.Log (chosenCity.cityName + ": GREEN THUMB TRIGGERED!");
			}else{
				chosenCity.farmerMultiplier = 2f;
			}
		}
	}
	private void Moneymaker(CityTest chosenCity){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < internalPersonality.moneymakerChance){
			chosenCity.goldMultiplier = 2f;
			Debug.Log (chosenCity.cityName + ": MONEYMAKER TRIGGERED!");
		}else{
			chosenCity.goldMultiplier = 1f;
		}
	}
	private void Inspiring(CityTest chosenCity){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < internalPersonality.inspiringChance){
			chosenCity.farmerMultiplier = 3f;
			chosenCity.hunterMultiplier = 3f;
			chosenCity.woodsmanMultiplier = 3f;
			chosenCity.quarrymanMultiplier = 3f;
			chosenCity.minerMultiplier = 3f;
			chosenCity.alchemistMultiplier = 3f;

			chosenCity.unrest -= 5;
			if(chosenCity.unrest < 0){
				chosenCity.unrest = 0;
			}
			Debug.Log (chosenCity.cityName + ": INSPIRING TRIGGERED!");
		}else{
			chosenCity.farmerMultiplier = 2f;
			chosenCity.hunterMultiplier = 2f;
			chosenCity.woodsmanMultiplier = 2f;
			chosenCity.quarrymanMultiplier = 2f;
			chosenCity.minerMultiplier = 2f;
			chosenCity.alchemistMultiplier = 2f;		
		}
	}


	internal void NegativeInternalPersonalityEvents(){
		CityTest randomCity = this.kingdom.cities [UnityEngine.Random.Range (0, this.kingdom.cities.Count)].cityAttributes;
		switch(internalPersonality.badPersonality){
		case LORD_INTERNAL_PERSONALITY_NEGATIVE.CORRUPT:
			Corrupt (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_NEGATIVE.VIOLENT:
			Violent (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_NEGATIVE.TYRANT:
			Tyrant (randomCity);
			break;
		case LORD_INTERNAL_PERSONALITY_NEGATIVE.DESTRUCTIVE:
			Destructive (randomCity);
			break;
		}
	}

	private void Corrupt(CityTest chosenCity){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < internalPersonality.corruptChance){
			float goldStealPercentage = UnityEngine.Random.Range (2, 6) / 100f;
			int stolenGold = (int)((float)chosenCity.goldCount * goldStealPercentage);

			if(chosenCity.goldCount >= stolenGold){
				chosenCity.goldCount -= stolenGold;
				chosenCity.unrest += 5;
				if(chosenCity.unrest > 100){
					chosenCity.unrest = 100;
				}
				Debug.Log (chosenCity.cityName + ": CORRUPT TRIGGERED!");
				Debug.Log ("Gold Steal Percentage: " + goldStealPercentage);
				Debug.Log ("Lord has stole: " + stolenGold + " gold.");
			}else{
				Debug.Log (chosenCity.cityName + ": CORRUPT! Lord can't steal gold. Not enough gold.");
			}
		}
	}
	private void Violent(CityTest chosenCity){
		float chance = UnityEngine.Random.Range (0f, 100f);
		if(chance < internalPersonality.voilentChance){
			Citizen citizenToBeKilled = chosenCity.citizens [UnityEngine.Random.Range (0, chosenCity.citizens.Count)];

			chosenCity.unrest += 10;
			if(chosenCity.unrest > 100){
				chosenCity.unrest = 100;
			}

			Debug.Log (chosenCity.cityName + ": VIOLENT TRIGGERED!");
			Debug.Log (chosenCity.cityName + ": Lord has killed " + citizenToBeKilled.name + ", " + citizenToBeKilled.job.jobType.ToString() + ".");
		}
	}
	private void Tyrant(CityTest chosenCity){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < internalPersonality.tyrantChance){
			chosenCity.farmerMultiplier = 1f;
			chosenCity.hunterMultiplier = 1f;
			chosenCity.woodsmanMultiplier = 1f;
			chosenCity.quarrymanMultiplier = 1f;
			chosenCity.minerMultiplier = 1f;
			chosenCity.alchemistMultiplier = 1f;

			chosenCity.unrest += 2;
			if(chosenCity.unrest > 100){
				chosenCity.unrest = 100;
			}

			Debug.Log (chosenCity.cityName + ": TYRANT TRIGGERED!");
		}else{
			chosenCity.farmerMultiplier = 2f;
			chosenCity.hunterMultiplier = 2f;
			chosenCity.woodsmanMultiplier = 2f;
			chosenCity.quarrymanMultiplier = 2f;
			chosenCity.minerMultiplier = 2f;
			chosenCity.alchemistMultiplier = 2f;		
		}
	}

	private void Destructive(CityTest chosenCity){
		float chance = UnityEngine.Random.Range (0f, 100f);
		if(chance < internalPersonality.destructiveChance){
			int randomDecrease = UnityEngine.Random.Range (2, 6);
			if(chosenCity.ownedBiomeTiles.Count <= 0){
				Debug.Log ("NO OWNED OTHER TILES EXCEPT ITS OWN!");
				return;
			}

			HexTile randomTile = chosenCity.ownedBiomeTiles [UnityEngine.Random.Range (1, chosenCity.ownedBiomeTiles.Count)];
			List<string> hexNames = new List<string> {
				"farm",
				"hunt",
				"wood",
				"stone",
				"mana",
				"metal",
				"gold"
			};
			hexNames = Utilities.Shuffle (hexNames);
			Debug.Log (chosenCity.cityName + ": DESTRUCTIVE TRIGGERED ON TILE - " + randomTile.name);
			for(int i = 0; i < hexNames.Count; i++){
				if(hexNames[i] == "farm"){
					if(randomTile.farmingValue == 0){
						continue;
					}else{
						randomTile.farmingValue -= randomDecrease;
						if(randomTile.farmingValue <= 0){
							randomTile.farmingValue = 0;
						}
						Debug.Log (randomTile.name + ": FARMING VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else if(hexNames[i] == "hunt"){
					if(randomTile.huntingValue == 0){
						continue;
					}else{
						randomTile.huntingValue -= randomDecrease;
						if(randomTile.huntingValue <= 0){
							randomTile.huntingValue = 0;
						}
						Debug.Log (randomTile.name + ": HUNTING VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else if(hexNames[i] == "wood"){
					if(randomTile.woodValue == 0){
						continue;
					}else{
						randomTile.woodValue -= randomDecrease;
						if(randomTile.woodValue <= 0){
							randomTile.woodValue = 0;
						}
						Debug.Log (randomTile.name + ": WOOD VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else if(hexNames[i] == "stone"){
					if(randomTile.stoneValue == 0){
						continue;
					}else{
						randomTile.stoneValue -= randomDecrease;
						if(randomTile.stoneValue <= 0){
							randomTile.stoneValue = 0;
						}
						Debug.Log (randomTile.name + ": STONE VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else if(hexNames[i] == "mana"){
					if(randomTile.manaValue == 0){
						continue;
					}else{
						randomTile.manaValue -= randomDecrease;
						if(randomTile.manaValue <= 0){
							randomTile.manaValue = 0;
						}
						Debug.Log (randomTile.name + ": MANA VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else if(hexNames[i] == "metal"){
					if(randomTile.metalValue == 0){
						continue;
					}else{
						randomTile.metalValue -= randomDecrease;
						if(randomTile.metalValue <= 0){
							randomTile.metalValue = 0;
						}
						Debug.Log (randomTile.name + ": METAL VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}

				else {
					if(randomTile.goldValue == 0){
						continue;
					}else{
						randomTile.goldValue -= randomDecrease;
						if(randomTile.goldValue <= 0){
							randomTile.goldValue = 0;
						}
						Debug.Log (randomTile.name + ": GOLD VALUE HAS DECREASED BY " + randomDecrease);
						break;
					}
				}


			}


			chosenCity.unrest += 5;
			if(chosenCity.unrest > 100){
				chosenCity.unrest = 100;
			}
				
		}
	}

	#endregion

	internal void CheckForWars(){
		if (IsReasonForWarSatisfied()) {
			if(IsMightCheckSatisfied() && IsRelationshipCheckSatisfied()) {
				int chance = Random.Range (0, 100);
				if (chance < this.currentWarChance) {
					for (int i = 0; i < this.candidatesForWar.Count; i++) {
						GoToWarWith (this.candidatesForWar [i]);
						this.candidatesForWar [i].GoToWarWith (this);
						this.currentWarChance = this.defaultWarChance;
					}
				} else {
					this.currentWarChance += 1;
				}
			}
		}
	}

	internal void GoToWarWith(Lord lord){
		Debug.LogError (this.id + "-" + this.name + "-" + this.kingdom.ComputeMilitaryStrength() + " DECLARES WAR ON : " + lord.id + "-" + lord.name + "-" + lord.kingdom.ComputeMilitaryStrength());
		this.daysWithoutWar = 0;
		this.hasExperiencedWar = true;
		for (int i = 0; i < this.relationshipLords.Count; i++) {
			if (this.relationshipLords[i].lord.id == lord.id) {
				this.relationshipLords [i].isAtWar = true;
			}
		}
		Relationship relOfThisLord = this.GetRelationshipByLordID (lord.id);
		if (relOfThisLord.like > -50) {
			relOfThisLord.like = -50;
			relOfThisLord.lordRelationship = GetLordRelationship (relOfThisLord.like);
		}

		for(int i = 0; i < lord.kingdom.cities.Count; i++){
			this.militaryData.Add (new MilitaryData (lord.kingdom.cities [i].cityAttributes, null, 0, BATTLE_MOVE.ATTACK));
		}
		this.UpdateMilitaryData ();
		GameManager.Instance.turnEnded += relOfThisLord.IncreaseWartime;
	}



	bool IsReasonForPeaceSatisfied(){
		bool result = false;
		for (int i = 0; i < this.lordPeaceReasons.Count; i++) {
			switch (this.lordPeaceReasons[i]) {
			case PEACE_REASONS.DEFEATED:
				result = IsDefeatedSatisfied();
				break;
			case PEACE_REASONS.MANY_WARS:
				result = IsTooManyWarsSatisfied();
				break;
			}
			if (result) {
				return result;
			}
		}
		return result;

	}

	bool IsReasonForWarSatisfied(){
		bool result = false;
		if (this.targetableLords.Count <= 0) {
			return result;
		}

		candidatesForWar = new List<Lord>();
		for (int i = 0; i < this.lordWarReasons.Count; i++) {
			switch(this.lordWarReasons[i]){
			case WAR_REASONS.COMPETITIVE:
				result = IsCompetetiveSatisfied ();
				break;
			case WAR_REASONS.COVETOUS:
				result = IsCovetousSatisfied();
				break;
			case WAR_REASONS.DEFENDER:
				result = IsDefenderSatisfied();
				break;
			case WAR_REASONS.IMPOTENT:
				result = IsImpotentSatisfied();
				break;
			case WAR_REASONS.MONEY_GRUBBER:
				result = IsMoneyGrubberSatisfied();
				break;
			case WAR_REASONS.ONE_TRUE_KING:
				result = IsOneTrueKingSatisfied();
				break;
			case WAR_REASONS.OPPORTUNIST:
				result = IsOpportunistSatisfied();
				break;
			case WAR_REASONS.PARANOID:
				result = IsParanoidSatisfied();
				break;
			case WAR_REASONS.RACIST:
				result = IsRacistSatisfied();
				break;
			case WAR_REASONS.SCAVENGER:
				result = IsScavengerSatisfied();
				break;
			case WAR_REASONS.SNEAKY:
				result = IsSneakySatisfied();
				break;
			case WAR_REASONS.SUSPICIOUS:
				result = IsSuspiciousSatisfied();
				break;
			case WAR_REASONS.TRADER:
				result = IsTraderSatisfied();
				break;
			case WAR_REASONS.TRAITOR:
				result = IsTraitorSatisfied();
				break;
			}
		}
		candidatesForWar = candidatesForWar.Distinct().ToList();
		return result;
	}

	bool IsMightCheckSatisfied(){
		if (this.kingdom.ComputeMilitaryStrength () <= 0 || candidatesForWar.Count <= 0) {
			return false;
		}
		switch (this.lordMightTrait) {
		case MIGHT_TRAIT.BULLY:
			return IsBullySatisfied();
		case MIGHT_TRAIT.NORMAL:
			return IsNormalMightSatisfied();
		case MIGHT_TRAIT.UNDERDOG:
			return IsUnderdogSatisfied();
		}
		return false;
	}

	bool IsRelationshipCheckSatisfied(){
		if ( candidatesForWar.Count <= 0) {
			return false;
		}
		switch (this.lordRelationshipTrait) {
		case RELATIONSHIP_TRAIT.NORMAL:
			return IsNormalRelationshipSatisfied();
		case RELATIONSHIP_TRAIT.PEACEFUL:
			return IsPeacefulSatisfied();
		case RELATIONSHIP_TRAIT.WARMONGER:
			return IsWarmongerSatisfied();
		}
		return false;
	}



	#region peace reason booleans
	bool IsDefeatedSatisfied(){
		if (this.kingdom.ComputeMilitaryStrength () <= 0) {
			return true;
		}
		return false;
	}

	bool IsTooManyWarsSatisfied(){
		if (this.currentWars.Count > 1) {
			return true;
		}
		return false;
	}
	#endregion

	#region war reason booleans
	bool IsCompetetiveSatisfied(){
		bool result = false;
		for (int j = 0; j < this.relationshipLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.cities.Count > this.kingdom.cities.Count) {
				result = true;
			}
		}

		if (result) {
			this.targetableLords.ForEach(x => this.candidatesForWar.Add(x.lord));
		}

		return result;
	}

	bool IsCovetousSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.cities.Count > this.kingdom.cities.Count) {
				candidatesForWar.Add (otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsDefenderSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			if (this.targetableLords[j].lordRelationship == LORD_RELATIONSHIP.WARM || this.targetableLords[j].lordRelationship == LORD_RELATIONSHIP.FRIEND || 
				this.targetableLords[j].lordRelationship == LORD_RELATIONSHIP.ALLY) {
				continue;
			}

			Lord otherLord = this.targetableLords[j].lord;

			for (int k = 0; k < otherLord.relationshipLords.Count; k++) {
				if (otherLord.relationshipLords[k].isAtWar) {
					for (int l = 0; l < this.relationshipLords.Count; l++) {
						if (otherLord.relationshipLords [k].lord.id == this.relationshipLords [l].lord.id) {
							if (this.relationshipLords [l].lordRelationship == LORD_RELATIONSHIP.FRIEND || 
								this.relationshipLords [l].lordRelationship == LORD_RELATIONSHIP.ALLY) {
								//Check if combined military strength is greater than or equal otherLord
//								Lord friendLord = GameManager.Instance.SearchLordById(this.relationshipLords[l].id);
//								if((friendLord.kingdom.ComputeMilitaryStrength() + this.kingdom.ComputeMilitaryStrength()) >= (otherLord.kingdom.ComputeMilitaryStrength()*1.1f)){
									candidatesForWar.Add(otherLord);
									result = true;
//								}
								break;
							}
							break;
						}
					}
					
				}
			}
		}
		return result;
	}

	bool IsImpotentSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.ComputeTotalCitizenCount() > this.kingdom.ComputeTotalCitizenCount()) {
				candidatesForWar.Add(otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsMoneyGrubberSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			KingdomTest adjacentKingdom = otherLord.kingdom;
			for (int k = 0; k < adjacentKingdom.cities.Count; k++) {
				CityTileTest currentCity = adjacentKingdom.cities[k];
				if (currentCity.cityAttributes.goldCount >= 2000) {
					candidatesForWar.Add(otherLord);
					result = true;
				}
			}
		}
		return result;
	}

	bool IsOneTrueKingSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.kingdomRace == this.kingdom.kingdomRace) {
				candidatesForWar.Add(otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsOpportunistSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			for(int k = 0; k < otherLord.relationshipLords.Count; k++){
				if(otherLord.relationshipLords[k].isAtWar){
//					Lord allyLord = GameManager.Instance.SearchLordById(otherLord.relationshipLords[k].id);
//					if ((allyLord.kingdom.ComputeMilitaryStrength() + this.kingdom.ComputeMilitaryStrength()) >= (otherLord.kingdom.ComputeMilitaryStrength()*1.1f)) {
						candidatesForWar.Add(otherLord);
						result = true;
						break;
//					}
				}
			}
		}
		return result;
	}

	bool IsParanoidSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.ComputeMilitaryStrength() >= this.kingdom.ComputeMilitaryStrength()) {
				candidatesForWar.Add(otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsRacistSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.kingdom.kingdomRace != this.kingdom.kingdomRace) {
				candidatesForWar.Add(otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsScavengerSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			if (otherLord.daysWithoutWar > 0 && otherLord.daysWithoutWar <= 10) {
				candidatesForWar.Add(otherLord);
				result = true;
			}
		}
		return result;
	}

	bool IsSneakySatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			for (int k = 0; k < otherLord.relationshipLords.Count; k++) {
				if (otherLord.relationshipLords[k].isAtWar) {
					Lord lordWithWar = otherLord.relationshipLords[k].lord;
					if (otherLord.kingdom.ComputeMilitaryStrength () > lordWithWar.kingdom.ComputeMilitaryStrength ()) {
						candidatesForWar.Add(otherLord);
						result = true;
					}
				}
			}
		}
		return result;
	}

	bool IsSuspiciousSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			Lord otherLord = this.targetableLords[j].lord;
			for (int k = 0; k < otherLord.relationshipLords.Count; k++) {
				if (otherLord.relationshipLords[k].lord.id == this.id) {
					if (otherLord.relationshipLords [k].lordRelationship == LORD_RELATIONSHIP.ENEMY ||
					   otherLord.relationshipLords [k].lordRelationship == LORD_RELATIONSHIP.RIVAL) {
						candidatesForWar.Add(otherLord);
						result = true;
					}
					break;
				}
			}
		}
		return result;
	}

	bool IsTraderSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			if (this.targetableLords[j].previousInteraction == LORD_EVENTS.TRADE && 
				this.targetableLords[j].previousDecision == DECISION.RUDE) {
				candidatesForWar.Add(this.targetableLords[j].lord);
				result = true;
			}
		}
		return result;
	}

	bool IsTraitorSatisfied(){
		bool result = false;
		for (int j = 0; j < this.targetableLords.Count; j++) {
			if (this.targetableLords[j].previousInteraction == LORD_EVENTS.GIFT ||
				this.targetableLords[j].previousInteraction == LORD_EVENTS.HELP) {
				if (this.targetableLords[j].previousDecision == DECISION.NICE) {
					candidatesForWar.Add(this.targetableLords[j].lord);
					result = true;
				}
			}
		}
		return result;
	}
	#endregion

	#region might check booleans
	bool IsBullySatisfied(){
		bool result = false;
		for (int j = 0; j < this.candidatesForWar.Count; j++) {
			if ((this.kingdom.ComputeMilitaryStrength()/2) > this.candidatesForWar[j].kingdom.ComputeMilitaryStrength()) {
				result = true;
			} else {
				this.candidatesForWar.Remove(this.candidatesForWar[j]);
			}
		}
		return result;
	}

	bool IsNormalMightSatisfied(){
		bool result = false;
		for (int j = 0; j < this.candidatesForWar.Count; j++) {
			if ((this.kingdom.ComputeMilitaryStrength()*0.9f) >= this.candidatesForWar[j].kingdom.ComputeMilitaryStrength() || 
				this.kingdom.ComputeMilitaryStrength() >= (this.candidatesForWar[j].kingdom.ComputeMilitaryStrength()*0.9f)) {
				result = true;
			} else {
				this.candidatesForWar.Remove(this.candidatesForWar[j]);
			}
		}
		return result;
	}

	bool IsUnderdogSatisfied(){
		bool result = false;
		for (int j = 0; j < this.candidatesForWar.Count; j++) {
			if ((this.kingdom.ComputeMilitaryStrength()*1.3f) < this.candidatesForWar[j].kingdom.ComputeMilitaryStrength()) {
				result = true;
			} else {
				this.candidatesForWar.Remove(this.candidatesForWar[j]);
			}
		}
		return result;
	}
	#endregion

	#region relationship check booleans
	bool IsNormalRelationshipSatisfied(){
		bool result = false;
		for (int j = 0; j < this.candidatesForWar.Count; j++) {
			Relationship relationshipWithLord = GetRelationshipByLordID(this.candidatesForWar[j].id);
			if (relationshipWithLord.lordRelationship == LORD_RELATIONSHIP.ENEMY ||
			    relationshipWithLord.lordRelationship == LORD_RELATIONSHIP.RIVAL) {
				result = true;
			} else {
				this.candidatesForWar.Remove(this.candidatesForWar[j]);
			}
		}
		return result;
	}

	bool IsPeacefulSatisfied(){
		bool result = false;
		for (int j = 0; j < this.candidatesForWar.Count; j++) {
			Relationship relationshipWithLord = GetRelationshipByLordID(this.candidatesForWar[j].id);
			if (relationshipWithLord.lordRelationship == LORD_RELATIONSHIP.RIVAL) {
				result = true;
			} else {
				this.candidatesForWar.Remove(this.candidatesForWar[j]);
			}
		}
		return result;
	}

	bool IsWarmongerSatisfied(){
		if (this.candidatesForWar.Count > 0) {
			return true;
		}
		return false;
	}
	#endregion

	Relationship GetRelationshipByLordID(int id){
		for (int i = 0; i < this.relationshipLords.Count; i++) {
			if (this.relationshipLords[i].lord.id == id) {
				return this.relationshipLords[i];
			}
		}
		return null;
	}

	internal void MilitaryData(){
		if(!hasEnemies()){
			return;
		}
		for(int i = 0; i < this.kingdom.cities.Count; i++){
			
		}
	}
	internal void TriggerAttack(){
		if(!hasEnemies()){
			return;
		}
		for(int i = 0; i < this.relationshipLords.Count; i++){
			if(this.relationshipLords[i].isAtWar && this.relationshipLords[i].isAdjacent){
				KingdomTest attackerKingdom = this.kingdom;
				KingdomTest defenderKingdom = this.relationshipLords [i].lord.kingdom;

				List<CityTest> yourConnectedCities = GetConnectedCities (attackerKingdom, defenderKingdom);
				List<CityTest> enemyConnectedCities = GetConnectedCities (defenderKingdom, attackerKingdom);

				int enemyWeakestArmy = GetWeakestArmy (enemyConnectedCities);

				CityTest attackerCity = GetCityWithStrongestArmy (yourConnectedCities);

				if(attackerCity != null){
					CityTest defenderCity = GetCityWithWeakestArmy (attackerCity, enemyConnectedCities, enemyWeakestArmy);
//					attackerCity.TriggerCityAttack (defenderCity);
				}else{
					Debug.Log ("ALL CITIES ALREADY HAVE A TARGET!");
				}
			}
		}

	}
	internal CityTest GetCityWithStrongestArmy(List<CityTest> cities){
		int strongestArmy = cities[0].GetArmyStrength ();
		List<CityTest> citiesStrongestArmy = new List<CityTest>();
		for (int i = 0; i < cities.Count; i++) {
			if(cities[i].targetCity == null){
				int currentArmy = cities [i].GetArmyStrength ();
				if(currentArmy > strongestArmy){
					strongestArmy = currentArmy;
				}
			}
		}
		for (int i = 0; i < cities.Count; i++) {
			if (cities [i].targetCity == null) {
				int currentArmy = cities [i].GetArmyStrength ();
				if (currentArmy == strongestArmy) {
					citiesStrongestArmy.Add (cities [i]);
				}
			}
		}
		if(citiesStrongestArmy.Count <= 0){
			return null;
		}
		return citiesStrongestArmy[UnityEngine.Random.Range(0, citiesStrongestArmy.Count)];
	}
	internal List<CityTest> GetConnectedCities(KingdomTest sourceKingdom, KingdomTest targetKingdom){
		List<CityTest> connectedCities = new List<CityTest> ();
		for(int i = 0; i < sourceKingdom.cities.Count; i++){
			for(int j = 0; j < sourceKingdom.cities[i].cityAttributes.connectedCities.Count; j++){
				if(sourceKingdom.cities[i].cityAttributes.connectedCities[j].cityAttributes.kingdomTile.kingdom.id == targetKingdom.id){
					connectedCities.Add (sourceKingdom.cities [i].cityAttributes);
					break;
				}
			}
		}
		return connectedCities;
	}
	internal CityTest GetCityWithWeakestArmy(CityTest yourCity, List<CityTest> cities, int weakestArmy){
		if(cities.Count <= 0){
			return null;
		}
		List<CityTest> citiesWeakestArmy = new List<CityTest>();

		if(this.intelligence == INTELLIGENCE.SIMPLE){
			return cities [UnityEngine.Random.Range (0, cities.Count)];
		}else{
			for (int i = 0; i < cities.Count; i++) {
				int currentArmy = cities [i].GetArmyStrength ();
				if(currentArmy == weakestArmy){
					citiesWeakestArmy.Add (cities [i]);
				}
			}

			if(this.intelligence == INTELLIGENCE.SMART){
				float shortestDistance = Vector3.Distance (yourCity.hexTile.gameObject.transform.position, citiesWeakestArmy [0].hexTile.gameObject.transform.position);
				CityTest cityShortestDistance = citiesWeakestArmy [0];
				for(int i = 0; i < citiesWeakestArmy.Count; i++){
					float currentDistance = Vector3.Distance (yourCity.hexTile.gameObject.transform.position, citiesWeakestArmy [i].hexTile.gameObject.transform.position);
					if(currentDistance < shortestDistance){
						shortestDistance = currentDistance;
						cityShortestDistance = citiesWeakestArmy [i];
					}
				}
				return cityShortestDistance;
			}else{
				return citiesWeakestArmy [UnityEngine.Random.Range (0, citiesWeakestArmy.Count)];
			}
		}

//		return citiesWeakestArmy;
	}
	internal int GetWeakestArmy(List<CityTest> cities){
		if(cities.Count <= 0){
			return 0;
		}
		int weakestArmy = cities[0].GetArmyStrength ();
//		List<CityTest> citiesWeakestArmy = new List<CityTest>();

		for (int i = 0; i < cities.Count; i++) {
			int currentArmy = cities [i].GetArmyStrength ();
			if(currentArmy < weakestArmy){
				weakestArmy = currentArmy;
			}
		}
		return weakestArmy;
//		for (int i = 0; i < cities.Count; i++) {
//			int currentArmy = cities [i].GetArmyStrength (true);
//			if(currentArmy == weakestArmy){
//				citiesWeakestArmy.Add (cities [i]);
//			}
//		}
//		return citiesWeakestArmy;
	}
	internal bool hasEnemies(){
		for(int i = 0; i < this.relationshipLords.Count; i++){
			if(this.relationshipLords[i].isAtWar && this.relationshipLords[i].isAdjacent){
				return true;
			}
		}
		return false;
	}
//	internal void ProvideBattleHelp(CityTest neededHelpCity, int neededArmyStrength){
//		int armyStrength = 0;
//		bool hasProvidedHelp = false;
//		for(int i = 0; i < this.kingdom.cities.Count; i++){
//			if(this.kingdom.cities[i].cityAttributes.id != neededHelpCity.id){
//				if(this.kingdom.cities[i].cityAttributes.GetArmyStrength() > neededArmyStrength){
//					if(this.kingdom.cities[i].cityAttributes.enemyGenerals.Count <= 0){
//						if(this.kingdom.cities[i].cityAttributes.generals.Count > 1){
//							List<General> generalOrderByArmyStrength = this.kingdom.cities [i].cityAttributes.generals.OrderByDescending (x => x.ArmyStrength ()).ToList();
//							for(int j = 0; j < generalOrderByArmyStrength.Count - 1; j++){
//								armyStrength += generalOrderByArmyStrength[j].ArmyStrength();
//								neededHelpCity.helpGenerals.Add (new DeployedGenerals (generalOrderByArmyStrength [j], 0));
//								this.kingdom.cities [i].cityAttributes.deployedGenerals.Add (generalOrderByArmyStrength [j]);
//								if(armyStrength >= neededArmyStrength){
//									hasProvidedHelp = true;
//									break;
//								}
//							}
//						}else{
//							//NO OFFENSE GENERAL, MUST NOT LEAVE CITY DEFENSELESS
//						}
//					}else{
//						//MUST DEFEND CAN'T PROVIDE HELP
//					}
//				}else{
//					//CAN'T HELP. STRENGTH IS LOWER THAN NEEDED
//				}
//
//			}
//			if(armyStrength >= neededArmyStrength){
//				hasProvidedHelp = true;
//				break;
//			}
//
//		}
//
//		if(!hasProvidedHelp){
//			//CREATE GENERALS OR INCREASE UPGRADE CITY CHANCE
//			CreateGeneralsForPreparation(neededHelpCity);
//		}
//	}

	internal void CreateGeneralsForPreparation(CityTest neededHelpCity){
		if(neededHelpCity.generals.Count >= neededHelpCity.generalsLimit){
			neededHelpCity.cityActionChances.increaseHousingChance += 50;
		}else{
			neededHelpCity.CreateGeneral ();
		}
	}
	internal bool CheckForSpecificWar(Lord lord){
		for(int i = 0; i < this.relationshipLords.Count; i++){
			if(this.relationshipLords[i].lord.id == lord.id){
				if(this.relationshipLords[i].isAtWar){
					return true;
				}
			}
		}
		return false;
	}

	internal void UpdateMilitaryData(){
//		this.militaryData.RemoveAll (x => x.isResolved);
		List<MilitaryData> allDefense = this.militaryData.Where(x => x.battleMove == BATTLE_MOVE.DEFEND).ToList();
		List<MilitaryData> allOffense = this.militaryData.Where(x => x.battleMove == BATTLE_MOVE.ATTACK).ToList();
		allDefense = allDefense.OrderBy (x => x.enemyGeneral.daysBeforeArrival).ToList();
		allOffense = allOffense.OrderBy (x => x.enemyCity.GetArmyStrength ()).ToList();

		List<MilitaryData> allData = new List<MilitaryData> ();
		allData.AddRange(allDefense);
		allData.AddRange (allOffense);


		this.militaryData = new List<MilitaryData>(allData);
	}

	internal MilitaryData SearchForDefenseMilitaryData(CityTest city){
		List<MilitaryData> milData = this.militaryData.Where(x => x.battleMove == BATTLE_MOVE.DEFEND).ToList();
		if(milData == null){
			return null;
		}
		for (int i = 0; i < milData.Count; i++) {
			if(!milData[i].isResolved){
				if(milData[i].enemyGeneral.targetCity.id == city.id){
					return milData [i];
				}
			}
		}
		return null;
	}
	internal MilitaryData RemoveMilitaryData(BATTLE_MOVE battleMove, CityTest city, General general){
		List<MilitaryData> milData = this.militaryData.Where (x => x.battleMove == battleMove).ToList ();

		if (battleMove == BATTLE_MOVE.ATTACK) {
			for (int i = 0; i < milData.Count; i++) {
				if (milData [i].enemyCity.id == city.id) {
					milData.RemoveAt (i);
					break;
				}

			}
		} else {
			for (int i = 0; i < milData.Count; i++) {
				if (milData [i].enemyGeneral.id == general.id) {
					milData.RemoveAt (i);
					break;
				}
			}
		}
		return null;
	}
}
