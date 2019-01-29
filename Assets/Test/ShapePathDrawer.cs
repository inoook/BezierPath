using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THREE;

public class ShapePathDrawer : MonoBehaviour
{

	public List<Vector3> points;
	public List<Quaternion> rots;
	
	ShapePath shapePath;

	public Color color;

	// ShapePath
	PointList pointList;

	// Use this for initialization
	void Start()
	{

	}

	public int num = 10;
	public bool useGetPointAt = false;
	// Update is called once per frame
	void OnDrawGizmos()
	{
		if(points == null || points.Count < 1){ return; }

		// ShapePath
		pointList = new PointList();
		
		// ShapePath
		if (pointList != null) {
			shapePath = pointList.GetShapePath(points.ToArray(), rots.ToArray());
		
			for (int i = 0; i < num; i++) {
				float v = (float)i / num;

				Vector3 pt;
//				pt = shapePath.getPointInfoVec3Percent(v);
				PathVector pv = shapePath.getPathInfo(v);
				pt = new Vector3(pv.x, pv.y, pv.z);

				Gizmos.color = color;
				Gizmos.DrawWireSphere(pt, 0.25f);

				Quaternion rot = pv.rot;

				//
				Gizmos.color = new Color(0, 0, 0, 0.25f);
				Gizmos.DrawRay(pt, rot * Vector3.up * 2);
				Gizmos.DrawRay(pt, rot * Vector3.right * 2);
				
				Quaternion q2 = GetQuaternionAt(v);
				Gizmos.color = Color.green;
				Gizmos.DrawRay (pt, q2* Vector3.up);
				Gizmos.color = Color.red;
				Gizmos.DrawRay (pt, q2* Vector3.right);
			}
		}
	}

	public Vector3 GetTangentAt (float p)
	{
		if (shapePath != null) {
			return (shapePath.getPointInfoVec3Percent (p + 0.01f) - shapePath.getPointInfoVec3Percent (p)).normalized;
		} else {
			return Vector3.zero;
		}
	}

	public Quaternion GetRotAt (float p)
	{
		if (shapePath != null) {
			PathVector pv = shapePath.getPathInfo(p);
			Quaternion rot = pv.rot;
			return rot;
		} else {
			return Quaternion.identity;
		}
	}

	public Quaternion GetQuaternionAt (float v)
	{
		Quaternion q = this.GetRotAt (v);
		Vector3 tangent = this.GetTangentAt (v);
		Vector3 up = Vector3.Cross (tangent, q * Vector3.right);
		return Quaternion.LookRotation(tangent, up); 
	}
}