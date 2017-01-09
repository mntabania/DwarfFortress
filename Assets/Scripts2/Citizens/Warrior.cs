using UnityEngine;
using System.Collections;

public class Warrior {

	public static CitizenUpgradeRequirements req;

	static Warrior(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 0));
	}
}
