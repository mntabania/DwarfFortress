using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer {

	public static CitizenUpgradeRequirements req;

	static Farmer(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.FOOD, 0));
		req.resource.Add (new Resource(RESOURCE.LUMBER, 0));
	}
}
