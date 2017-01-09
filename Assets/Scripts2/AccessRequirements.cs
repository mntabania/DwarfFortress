using UnityEngine;
using System.Collections;

public class AccessRequirements {

	public static CitizenUpgradeRequirements AccessCitizenUpgradeRequirements(CITIZEN_TYPE citizenType, int level){
		CitizenUpgradeRequirements req = new CitizenUpgradeRequirements ();
		switch (citizenType) {
		case CITIZEN_TYPE.ALCHEMIST:
			req = GetCitizenUpgradeRequirements (Alchemist.req, level);
			break;
		case CITIZEN_TYPE.ARCHER:
			break;
		case CITIZEN_TYPE.ARTISAN:
			break;
		case CITIZEN_TYPE.DIPLOMAT:
			break;
		case CITIZEN_TYPE.ENCHANTER:
			break;
		case CITIZEN_TYPE.FARMER:
			req = GetCitizenUpgradeRequirements (Farmer.req, level);
			break;
		case CITIZEN_TYPE.HEALER:
			break;
		case CITIZEN_TYPE.HUNTER:
			req = GetCitizenUpgradeRequirements (Hunter.req, level);
			break;
		case CITIZEN_TYPE.MAGE:
			break;
		case CITIZEN_TYPE.MERCHANT:
			break;
		case CITIZEN_TYPE.MINER:
			req = GetCitizenUpgradeRequirements (Miner.req, level);
			break;
		case CITIZEN_TYPE.SPY:
			break;
		case CITIZEN_TYPE.WARRIOR:
			req = GetCitizenUpgradeRequirements (Warrior.req, level);
			break;
		case CITIZEN_TYPE.WOODSMAN:
			req = GetCitizenUpgradeRequirements (Woodsman.req, level);
			break;
		}
		return req;
	}

	private static CitizenUpgradeRequirements GetCitizenUpgradeRequirements(CitizenUpgradeRequirements upgradeRequirements, int currentLevel){
		for (int i = 0; i < upgradeRequirements.resource.Count; i++) {
			upgradeRequirements.resource [i].resourceQuantity = 100 * currentLevel;
		}
		return upgradeRequirements;
	}

}
