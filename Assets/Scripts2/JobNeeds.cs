using UnityEngine;
using System.Collections;

public class JobNeeds {

	public JOB_TYPE jobType;
	public Resource resource;

	public JobNeeds(JOB_TYPE jobType, Resource resource){
		this.jobType = jobType;
		this.resource = resource;
	}
}
