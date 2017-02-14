using UnityEngine;
using System.Collections;

[System.Serializable]
public class DeployedGenerals {

	public General general;
	public int daysBeforeArrival;

	public DeployedGenerals(General general, int daysBeforeArrival){
		this.general = general;
		this.daysBeforeArrival = daysBeforeArrival;
	}
}
