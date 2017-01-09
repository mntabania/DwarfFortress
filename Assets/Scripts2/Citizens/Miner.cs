using UnityEngine;
using System.Collections;

public class Miner {

	public static CitizenUpgradeRequirements req;

	static Miner(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
		req.resource.Add (new Resource(RESOURCE.MANA_STONE, 0));
	}
}
