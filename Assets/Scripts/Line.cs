using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

	public bool isIntersecting;

	public Vector3 startPoint;
	public Vector3 endPoint;

	void OnTriggerEnter2D(Collider2D other){
		isIntersecting = true;
	}

	void OnTriggerStay2D(Collider2D other){
		isIntersecting = true;
	}
}
