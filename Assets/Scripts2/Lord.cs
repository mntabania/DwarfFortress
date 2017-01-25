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
	public KingdomTest kingdom;
	public LORD_PERSONALITY personality;
	public LordInternalPersonality internalPersonality;
	public CHARACTER character;
	public List<GOALS> goals;
	public List<List<string>> tasks;
	public List<PUBLIC_IMAGE> publicImages;
	public List<Relationship> relationshipKings;
	public List<Relationship> relationshipLords;


	public Lord(KingdomTest kingdom){
		this.id = 1 + GetID ();
		this.name = "LORD" + this.id.ToString();
		this.predictability = UnityEngine.Random.Range (0, 10);
		this.persistence = UnityEngine.Random.Range (0, 10);
		this.trustworthiness = UnityEngine.Random.Range (0, 10);
		this.selflessness = UnityEngine.Random.Range (0, 10);
		this.skill = UnityEngine.Random.Range (0, 10);
		this.racism = UnityEngine.Random.Range (0, 10);
		this.religiousTolerance = UnityEngine.Random.Range (0, 10);		
		this.personality = (LORD_PERSONALITY)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(LORD_PERSONALITY)).Length));
		this.internalPersonality = new LordInternalPersonality ("");
		this.character = (CHARACTER)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CHARACTER)).Length));		
		this.likeCitizen = 0;
		this.kingdom = kingdom;
		this.goals = new List<GOALS> ();
		this.tasks = new List<List<string>> ();
		this.publicImages = new List<PUBLIC_IMAGE> ();
		this.relationshipKings = new List<Relationship> ();
		this.relationshipLords = new List<Relationship> ();

		SetLastID (this.id);
	}
	private int GetID(){
		return Utilities.lastLordId;
	}
	private void SetLastID(int id){
		Utilities.lastLordId = id;
	}

	internal void CreateInitialRelationshipsToLords(){
		for (int i = 0; i < GameManager.Instance.kingdoms.Count; i++) {
			KingdomTest otherKingdom = GameManager.Instance.kingdoms[i].kingdom;
			if (otherKingdom.id != this.kingdom.id) {
				this.relationshipLords.Add (new Relationship (otherKingdom.lord.id, otherKingdom.lord.name, DECISION.NEUTRAL, 0));
			}
		}
	}

	#region DECISION-MAKING
	internal void AdjustLikeness(Lord targetLord, DECISION sourceDecision, DECISION targetDecision, LORD_EVENTS eventType){
		Relationship relationship = SearchRelationship (targetLord);
		int eventEffect = EventEffect (eventType, sourceDecision, targetDecision);

		switch (this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			relationship.like += (eventEffect * 5);

			if(eventType == LORD_EVENTS.COOPERATE1 || eventType == LORD_EVENTS.COOPERATE2){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
					+ targetLord.name + " by " + (eventEffect * 5) + ".\n\n";
			}

			break;
		case LORD_PERSONALITY.VENGEFUL:
			int multiplier = 5;
			if(eventEffect < 0){
				multiplier = 25;
			}
			relationship.like += (eventEffect * multiplier);

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + (eventEffect * multiplier) + ".\n\n";

			break;
		case LORD_PERSONALITY.RATIONAL:
			relationship.like += (eventEffect * 10);

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + (eventEffect * 10) + ".\n\n";

			break;
		case LORD_PERSONALITY.NAIVE:
			if(relationship.like < 0 && eventEffect >= 0){
				relationship.like = 0;
			}
			relationship.like += (eventEffect * 10);

			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + eventType.ToString() + " EVENT RESULTS: " + this.name + " has increased/decreased his likeness towards "
				+ targetLord.name + " by " + (eventEffect * 10) + ".\n\n";
			break;
		}
		if(relationship.like > 100){
			relationship.like = 100;
		} else if (relationship.like < -100){
			relationship.like = -100;
		}
//		relationship.previousDecision = targetDecision;


	}
	internal DECISION ComputeDecisionBasedOnPersonality(LORD_EVENTS eventType, Lord targetLord){

		Relationship relationshipWithOtherLord = this.SearchRelationship (targetLord);
		Relationship relationshipFromOtherLord = targetLord.SearchRelationship (this);
		DECISION decision = DECISION.NEUTRAL;
		relationshipFromOtherLord.isFirstEncounter = false;
		switch(this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			decision = TitForTat (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
//			return TitForTat (relationshipWithOtherLord);
		case LORD_PERSONALITY.VENGEFUL:
			decision = Vengeful (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
//			return Vengeful (relationshipWithOtherLord);
		case LORD_PERSONALITY.RATIONAL:
			decision = Rational (eventType, relationshipWithOtherLord, relationshipFromOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
//			return Rational (eventType, relationshipWithOtherLord, relationshipFromOtherLord);
		case LORD_PERSONALITY.NAIVE:
			decision = Naive (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
//			return Naive (relationshipWithOtherLord);
		}
		return DECISION.NEUTRAL;
	}
	private DECISION TitForTat(Relationship relationshipWithOtherLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			int chance = UnityEngine.Random.Range (0, 100);
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 80% chance to choose "
				+ relationshipWithOtherLord.previousDecision.ToString () + " because " + relationshipWithOtherLord.name + " chose " + relationshipWithOtherLord.previousDecision.ToString () + " during their latest encounter.\n\n";
			if(chance < 80){
				return relationshipWithOtherLord.previousDecision;
			}else{
				if(relationshipWithOtherLord.like >= 0){
					return DECISION.NICE;
				}else{
					return DECISION.RUDE;
				}
			}

		}
	}
	private DECISION Vengeful(Relationship relationshipWithOtherLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			if(relationshipWithOtherLord.like >= 0){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE because he likes " + relationshipWithOtherLord.name + ".\n\n";
				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 90){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}else{
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose RUDE because he dislikes " + relationshipWithOtherLord.name + ".\n\n";
				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 90){
					return DECISION.RUDE;
				}
				return DECISION.NICE;
			}

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
			if(relationshipFromOtherLord.like < 0){
				
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.RUDE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.RUDE);


				if(niceEffect >= rudeEffect){
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose NICE because " 
						+ relationshipWithOtherLord.name + " dislikes him and it has " + niceEffect + " max effect if " + relationshipWithOtherLord.name + " chooses RUDE.\n\n";

				}else{
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose RUDE because " 
						+ relationshipWithOtherLord.name + " dislikes him and it has " + rudeEffect + " max effect if " + relationshipWithOtherLord.name + " chooses RUDE.\n\n";
				}

				if(chance < 50){
					return DECISION.NICE;
				}else{
					if(niceEffect >= rudeEffect){
						return DECISION.NICE;

					}else{
						return DECISION.RUDE;
					}
				}
				
			}else{
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.NICE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.NICE);


				if(niceEffect >= rudeEffect){
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose NICE because " 
						+ relationshipWithOtherLord.name + " likes him and it has " + niceEffect + " max effect if " + relationshipWithOtherLord.name + " chooses NICE.\n\n";

				}else{
					UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose RUDE because " 
						+ relationshipWithOtherLord.name + " likes him and it has " + rudeEffect + " max effect if " + relationshipWithOtherLord.name + " chooses NICE.\n\n";
				}

				if(chance < 50){
					return DECISION.NICE;
				}else{
					niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.NICE);
					rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.NICE);
					if(niceEffect >= rudeEffect){
						return DECISION.NICE;

					}else{
						return DECISION.RUDE;
					}
				}
			}
		}
	}
	private DECISION Naive(Relationship relationshipWithOtherLord){
		if(relationshipWithOtherLord.isFirstEncounter){
			UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE on first encounter.\n\n";
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			if(relationshipWithOtherLord.like >= 0){
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 90% chance to choose NICE because he likes " + relationshipWithOtherLord.name + ".\n\n";

				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 90){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}else{
				UserInterfaceManager.Instance.externalAffairsLogList[UserInterfaceManager.Instance.externalAffairsLogList.Count - 1] += GameManager.Instance.currentDay.ToString () + ": " + this.name + " has 50% chance to choose NICE because he dislikes " + relationshipWithOtherLord.name + ".\n\n";

				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 50){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}

		}
	}
	internal Relationship SearchRelationship(Lord targetLord){
		for(int i = 0; i < this.relationshipLords.Count; i++){
			if(this.relationshipLords[i].id == targetLord.id){
				return this.relationshipLords[i];
			}
		}
		return null;
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
					return -1;
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
					return 0;
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
					return -1;
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
					return 1;
				}
			}else{
				if(decisionOfOtherLord == DECISION.NICE){
					return 0;
				}else if(decisionOfOtherLord == DECISION.RUDE){
					return -1;
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
		float chance = UnityEngine.Random.Range (0f, 100f);
		if(chance < internalPersonality.tyrantChance){
			chosenCity.farmerMultiplier = 1f;
			chosenCity.hunterMultiplier = 1f;
			chosenCity.woodsmanMultiplier = 1f;
			chosenCity.quarrymanMultiplier = 1f;
			chosenCity.minerMultiplier = 1f;
			chosenCity.alchemistMultiplier = 1f;

			chosenCity.unrest += 5;
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
}
