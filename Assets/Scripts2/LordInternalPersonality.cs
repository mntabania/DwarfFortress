using UnityEngine;
using System.Collections;

[System.Serializable]
public class LordInternalPersonality {

	public LORD_INTERNAL_PERSONALITY_POSITIVE goodPersonality;
	public LORD_INTERNAL_PERSONALITY_NEGATIVE badPersonality;

	public int corruptChance;
	public float voilentChance;
	public int tyrantChance;
	public float destructiveChance;
	private const int defaultCorruptChance = 1;
	private const float defaultVoilentChance = 0.3f;
	private const int defaultTyrantChance = 2;
	private const float defaultDestructiveChance = 0.3f;

	public int gallantChance;
	public int greenthumbChance;
	public int moneymakerChance;
	public int inspiringChance;
	private const int defaultGallantChance = 1;
	private const int defaultGreenthumbChance = 8;
	private const int defaultMoneymakerChance = 2;
	private const int defaultInspiringChance = 2;


	public LordInternalPersonality(LORD_INTERNAL_PERSONALITY_POSITIVE goodPersonality, LORD_INTERNAL_PERSONALITY_NEGATIVE badPersonality){
		this.goodPersonality = goodPersonality;
		this.badPersonality = badPersonality;

		this.corruptChance = defaultCorruptChance;
		this.voilentChance = defaultVoilentChance;
		this.tyrantChance = defaultTyrantChance;
		this.destructiveChance = defaultDestructiveChance;

		this.gallantChance = defaultGallantChance;
		this.greenthumbChance = defaultGreenthumbChance;
		this.moneymakerChance = defaultMoneymakerChance;
		this.inspiringChance = defaultInspiringChance;

	}

	public LordInternalPersonality(string dummy){
		this.goodPersonality = (LORD_INTERNAL_PERSONALITY_POSITIVE)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(LORD_INTERNAL_PERSONALITY_POSITIVE)).Length));
		this.badPersonality = (LORD_INTERNAL_PERSONALITY_NEGATIVE)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(LORD_INTERNAL_PERSONALITY_NEGATIVE)).Length));

		this.corruptChance = defaultCorruptChance;
		this.voilentChance = defaultVoilentChance;
		this.tyrantChance = defaultTyrantChance;
		this.destructiveChance = defaultDestructiveChance;

		this.gallantChance = defaultGallantChance;
		this.greenthumbChance = defaultGreenthumbChance;
		this.moneymakerChance = defaultMoneymakerChance;
		this.inspiringChance = defaultInspiringChance;
	}

	internal bool TriggerPositivePersonality(){
		switch(this.goodPersonality){
		case LORD_INTERNAL_PERSONALITY_POSITIVE.GALLANT:
			return TriggerGallant ();
		case LORD_INTERNAL_PERSONALITY_POSITIVE.GREEN_THUMB:
			return TriggerGreenThumb ();
		case LORD_INTERNAL_PERSONALITY_POSITIVE.MONEYMAKER:
			return TriggerMoneymaker ();
		case LORD_INTERNAL_PERSONALITY_POSITIVE.INSPIRING:
			return TriggerInspiring ();
		}
		return false;
	}

	internal bool TriggerGallant(){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < this.gallantChance){
//			this.gallantChance = defaultGallantChance;
			return true;
		}
		return false;

	}
	internal bool TriggerGreenThumb(){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < this.greenthumbChance){
//			this.greenthumbChance = defaultGreenthumbChance;
			return true;
		}
		return false;

	}
	internal bool TriggerMoneymaker(){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < this.moneymakerChance){
//			this.moneymakerChance = defaultMoneymakerChance;
			return true;
		}
		return false;

	}
	internal bool TriggerInspiring(){
		int chance = UnityEngine.Random.Range (0, 100);
		if(chance < this.inspiringChance){
//			this.inspiringChance = defaultInspiringChance;
			return true;
		}
		return false;
	}
}
