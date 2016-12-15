using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {

	public static LineDrawer Instance = null;

	public Material lineMat;
	public GameObject linePrefab;
	List<Vector3> startPoints;
	List<Vector3> endPoints;

	void Awake(){
		Instance = this;
		startPoints = new List<Vector3>();
		endPoints = new List<Vector3>();
	}

	void RenderLines(){
		if (startPoints != null) {
			for (int i = 0; i < startPoints.Count; ++i) {
				if (startPoints [i] == null) {
					break;
				}
				GL.Begin (GL.LINES);
				lineMat.SetPass (0);
				GL.Color (new Color (lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
				GL.Vertex (startPoints [i]);
				GL.Vertex (endPoints [i]);
				GL.End ();
			}
		}
	}

	public GameObject DrawLine(Transform parentObj, Vector3 startPoint, Vector3 endPoint){
//		startPoints.Add (startPoint);
//		endPoints.Add (endPoint);

		GameObject lineGO = Instantiate(linePrefab);
		lineGO.transform.parent = parentObj;
		lineGO.transform.localPosition = Vector3.zero;
		Vector3 diff =  lineGO.transform.position - endPoint;
		diff.Normalize ();
		float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
		lineGO.transform.rotation = Quaternion.Euler (0f, 0f, rot_z);

		Vector3 newSize = Vector3.one;
		newSize.x = Vector3.Distance(endPoint, startPoint)/2.45f;
		lineGO.transform.localScale = newSize;

		lineGO.GetComponent<Line>().startPoint = startPoint;
		lineGO.GetComponent<Line> ().endPoint = endPoint;

		return lineGO;
	}

	// To show the lines in the game window whne it is running
//	void OnPostRender() {
//		RenderLines();
//	}

	// To show the lines in the editor
//	void OnDrawGizmos() {
//		RenderLines();
//	}
}
