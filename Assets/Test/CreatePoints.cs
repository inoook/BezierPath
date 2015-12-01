using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatePoints : MonoBehaviour {

	public ShapePathDrawer shapePath;
	public ThreePathDrawer threeCurve;

	public ShapePathLineRenderer shapePathLineRenderer;

	List<Vector3> randomPoints;
	public List<Transform> transList;
	// Use this for initialization
	void Awake () {
		
//		randomPoints = new List<Vector3>();
//		for (int i = 0; i < 4*3; i ++) {
//			randomPoints.Add(new Vector3((i) * 10, Random.Range(- 50.0f, 50.0f), Random.Range(- 50.0f, 50.0f)));
//		}


	}
	
	// Update is called once per frame
	void Update () {
		randomPoints = new List<Vector3>();
		for (int i = 0; i < transList.Count; i ++) {
			randomPoints.Add(transList[i].position);
		}
		List<Quaternion> rots = new List<Quaternion>();
		for (int i = 0; i < transList.Count; i ++) {
			rots.Add(transList[i].rotation);
		}

		// ShapePathDrawer
		shapePath.points = randomPoints;
		shapePath.rots = rots;

		// ThreePathDrawer
		threeCurve.points = randomPoints;

		// ShapePathLineRenderer
		shapePathLineRenderer.path = randomPoints.ToArray();
	}
	void OnDrawGizmos()
	{
		if (randomPoints != null) {
			Gizmos.color = Color.cyan;
			for (int i = 0; i < randomPoints.Count; i++) {
				Gizmos.DrawWireSphere(randomPoints[i], 1.5f);
			}
			//			Gizmos.color = Color.magenta;
			//			for (int i = 0; i < randomPoints.Count-1; i++) {
			//				Gizmos.DrawWireSphere(Vector3.Lerp(randomPoints[i + 1], randomPoints[i], 0.5f), 1.5f);
			//			}
		}
	}
}
