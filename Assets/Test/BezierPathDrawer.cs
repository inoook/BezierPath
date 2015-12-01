using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bezier;

public class BezierPathDrawer : MonoBehaviour
{
	public BezierPath path;
	public List<BezierPt> bPoints;
	
	// Use this for initialization
	void Start ()
	{

	}

	
	public Color drawColor = Color.yellow;
	public int split = 30;

	// Update is called once per frame
	void OnDrawGizmos ()
	{
		List<Quaternion> rotList = new List<Quaternion> ();
		for (int i = 0; i < bPoints.Count; i++) {
			BezierPt b = bPoints [i];
			rotList.Add (b.transform.rotation);
		}

		List<Vector3> nodes = new List<Vector3> ();
		for (int i = 0; i < bPoints.Count-1; i++) {
			BezierPt start = bPoints [i];
			BezierPt end = bPoints [i + 1];
			
			nodes.Add (start.transform.position);
			nodes.Add (end.inTrans.position);
			nodes.Add (start.outTrans.position);
			nodes.Add (end.transform.position);
		}

		path = new BezierPath (nodes.ToArray (), rotList.ToArray ());

		// draw
		float delta = 1.0f / split;
		
		Gizmos.color = drawColor;
		for (int i = 0; i < split-1; i++) {
			float t0 = delta * i;
			float t1 = delta * (i + 1);
			Vector3 pos0 = this.GetPointAt (t0);
			Vector3 pos1 = this.GetPointAt (t1);

			Gizmos.DrawLine (pos0, pos1);
		}

		for (int i = 0; i < split; i++) {

			float v = (float)i / split;
			
			BezierPointInfo pInfo = path.GetBezierPointInfo(v);
			
			Vector3 pt = pInfo.point;
			Gizmos.color = drawColor;
			Gizmos.DrawWireSphere (pt, 0.25f);

			// tangent
			Gizmos.color = Color.blue;
			Vector3 tangent = pInfo.tangent;
			Gizmos.DrawRay (pt, tangent);

			Quaternion quat = pInfo.rotation;
			Gizmos.color = Color.green;
			Gizmos.DrawRay (pt, quat* Vector3.up);
			Gizmos.color = Color.red;
			Gizmos.DrawRay (pt, quat* Vector3.right);
		}
	}
	
	public float length {
		get {
			if (path != null) {
				return path.length;
			} else {
				return 0;
			}
		}
	}

	public Vector3 GetPointAt (float p)
	{
		if (path != null) {
			return path.point (p);
		} else {
			return Vector3.zero;
		}
	}
//
//	public Vector3 GetTangentAt (float p, float offset = 0.0001f)
//	{
//		if (path != null) {
//			return (path.point (p + offset) - path.point (p)).normalized;
//		} else {
//			return Vector3.zero;
//		}
//	}
//
//	private Quaternion GetRotAt (float p)
//	{
//		if (path != null) {
//			return path.rot (p);
//		} else {
//			return Quaternion.identity;
//		}
//	}
//
//	public Quaternion GetQuaternionAt (float v)
//	{
//		Quaternion q = this.GetRotAt (v);
//		Vector3 tangent = this.GetTangentAt (v);
//		Vector3 up = Vector3.Cross (tangent, q * Vector3.right);
//		return Quaternion.LookRotation(tangent, up); 
//	}
}
