using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	internal void AdjustLikeness(Lord targetLord, DECISION sourceDecision, DECISION targetDecision, LORD_EVENTS eventType){
		Relationship relationship = SearchRelationship (targetLord);
		int eventEffect = EventEffect (eventType, sourceDecision, targetDecision);

		switch (this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			relationship.like += (eventEffect * 5);
			break;
		case LORD_PERSONALITY.VENGEFUL:
			int multiplier = 5;
			if(eventEffect < 0){
				multiplier = 25;
			}
			relationship.like += (eventEffect * multiplier);
			break;
		case LORD_PERSONALITY.RATIONAL:
			relationship.like += (eventEffect * 10);
			break;
		case LORD_PERSONALITY.NAIVE:
			if(relationship.like < 0 && eventEffect >= 0){
				relationship.like = 0;
			}
			relationship.like += (eventEffect * 10);
			break;
		}
		if(relationship.like > 100){
			relationship.like = 100;
		} else if (relationship.like < -100){
			relationship.like = -100;
		}
		relationship.previousDecision = targetDecision;
	}
	internal DECISION ComputeDecisionBasedOnPersonality(LORD_EVENTS eventType, Lord targetLord){

		Relationship relationshipWithOtherLord = this.SearchRelationship (targetLord);
		Relationship relationshipFromOtherLord = targetLord.SearchRelationship (this);
		DECISION decision = DECISION.NEUTRAL;
		switch(this.personality){
		case LORD_PERSONALITY.TIT_FOR_TAT:
			decision = TitForTat (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
		case LORD_PERSONALITY.VENGEFUL:
			decision = Vengeful (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
		case LORD_PERSONALITY.RATIONAL:
			decision = Rational (eventType, relationshipWithOtherLord, relationshipFromOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
		case LORD_PERSONALITY.NAIVE:
			decision = Naive (relationshipWithOtherLord);
			relationshipFromOtherLord.previousDecision = decision;
			return decision;
		}
		return decision;
	}
	private DECISION TitForTat(Relationship relationshipWithOtherLord){
		if(relationshipWithOtherLord.previousDecision == DECISION.NEUTRAL){
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			int chance = UnityEngine.Random.Range (0, 100);
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
		if(relationshipWithOtherLord.previousDecision == DECISION.NEUTRAL){
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			if(relationshipWithOtherLord.like >= 0){
				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 90){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}else{
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

		if(relationshipWithOtherLord.previousDecision == DECISION.NEUTRAL){
			if(relationshipWithOtherLord.like >= 0){
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.NICE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.NICE);
			}else{
				niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.RUDE);
				rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.RUDE);
			}

			if(niceEffect >= rudeEffect){
				return DECISION.NICE;
			}else{
				return DECISION.RUDE;
			}
		}else{
			int chance = UnityEngine.Random.Range (0, 100);
			if(relationshipFromOtherLord.like < 0){
				if(chance < 50){
					return DECISION.NICE;
				}else{
					niceEffect = EventEffect (eventType, DECISION.NICE, DECISION.RUDE);
					rudeEffect = EventEffect (eventType, DECISION.RUDE, DECISION.RUDE);
					if(niceEffect >= rudeEffect){
						return DECISION.NICE;

					}else{
						return DECISION.RUDE;
					}
				}
				
			}else{
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
		if(relationshipWithOtherLord.previousDecision == DECISION.NEUTRAL){
			int chance = UnityEngine.Random.Range (0, 100);
			if(chance < 90){
				return DECISION.NICE;
			}
			return DECISION.RUDE;
		}else{
			if(relationshipWithOtherLord.like >= 0){
				int chance = UnityEngine.Random.Range (0, 100);
				if(chance < 90){
					return DECISION.NICE;
				}
				return DECISION.RUDE;
			}else{
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
}
