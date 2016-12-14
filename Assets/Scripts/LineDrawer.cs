using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {

	public static LineDrawer Instance = null;

	public Material lineMat;
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

	public void DrawLine(Vector3 startPoint, Vector3 endPoint){
		startPoints.Add (startPoint);
		endPoints.Add (endPoint);
		Debug.Log ("DRAW LINEE!");
	}

	// To show the lines in the game window whne it is running
	void OnPostRender() {
		RenderLines();
	}

	// To show the lines in the editor
	void OnDrawGizmos() {
		RenderLines();
	}
}
