using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THREE;

public class ThreePathDrawer : MonoBehaviour
{

	THREE.CurvePath curve;
//	Curve spline;

	public List<Vector3> points;

	public Color color;

	// Use this for initialization
	void Start()
	{


		// Spline3
//		spline = new THREE.SplineCurve3(points);
	}

	public int num = 10;
	public bool useGetPointAt = false;
	// Update is called once per frame
	void OnDrawGizmos()
	{
		// BezierCurve3
		curve = new THREE.CurvePath();

		// BezierCurve3
		if (curve != null) {

			Vector3 centerPosPre;
			Vector3 centerPosNext = Vector3.Lerp(points[1], points[2], 0.5f);
			
			THREE.QuadraticBezierCurve3 curveSegment = new THREE.QuadraticBezierCurve3(points[0], points[1], centerPosNext);
			curve.add(curveSegment);
			
			for (int i = 2; i < points.Count-1; i++) {
				centerPosPre = Vector3.Lerp(points[i - 1], points[i], 0.5f);
				centerPosNext = Vector3.Lerp(points[i], points[i + 1], 0.5f);
				THREE.QuadraticBezierCurve3 curveSegment1 = new THREE.QuadraticBezierCurve3(centerPosPre, points[i], centerPosNext);
				curve.add(curveSegment1);
			}
			//
			Gizmos.color = color;
			for (int i = 0; i < num; i++) {
				float v = (float)i / num;
				Vector3 pt;
				if (useGetPointAt) {
					pt = curve.getPointAt(v);
				} else {
					pt = curve.getPoint(v);
				}
				Vector3 tangent = curve.getTangent(v);
				Gizmos.DrawRay(pt, tangent);
				Gizmos.DrawWireSphere(pt, 0.25f);
			}
		}

		// Spline3
		//		if (spline != null) {
		//			Gizmos.color = Color.green;
		//			for (int i = 0; i < num; i++) {
		//				float v = (float)i / num;
		//				Vector3 pt;
		//				if (useGetPointAt) {
		//					pt = spline.getPointAt(v);
		//				} else {
		//					pt = spline.getPoint(v);
		//				}
		//				Gizmos.DrawWireSphere(pt, 0.25f);
		//			}
		//		}
	}
}