using UnityEngine;
using System.Collections;

[System.Serializable]
public class ResourceStatus {

	public RESOURCE resource;
	public RESOURCE_STATUS status;
	public int amount;
	public int daysToReachGoal;

	public ResourceStatus(RESOURCE resource, RESOURCE_STATUS status, int amount, int daysToReachGoal){
		this.resource = resource;
		this.status = status;
		this.amount = amount;
		this.daysToReachGoal = daysToReachGoal;
	}
}
