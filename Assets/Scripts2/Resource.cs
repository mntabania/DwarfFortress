using UnityEngine;
using System.Collections;

[System.Serializable]
public class Resource {
	public RESOURCE resourceType;
	public int resourceQuantity;

	public Resource(RESOURCE resourceType, int resourceQuantity){
		this.resourceType = resourceType;
		this.resourceQuantity = resourceQuantity;
	}
}
