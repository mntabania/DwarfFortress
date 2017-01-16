using UnityEngine;
using System.Collections;

[System.Serializable]
public class ResourceStatus {

	public RESOURCE resource;
	public RESOURCE_STATUS status;

	public ResourceStatus(RESOURCE resource, RESOURCE_STATUS status){
		this.resource = resource;
		this.status = status;
	}
}
