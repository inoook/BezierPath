// https://github.com/mrdoob/three.js/blob/master/src/extras/curves/ClosedSplineCurve3.js
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class ClosedSplineCurve3 : Curve
	{

		List<Vector3> points;

		// Use this for initialization
		public ClosedSplineCurve3 (List<Vector3> points)
		{
			if (points == null) {
				this.points = new List<Vector3> ();
			} else {
				this.points = points;
			}
		}

		//function ( t ) {
		public override Vector3 getPoint (float t)
		{
		
			Vector3 v = new Vector3 ();
			int[] c = new int[4];
			List<Vector3> points = this.points;
			float point, intPoint, weight;
			point = (points.Count - 0) * t;
			// This needs to be from 0-length +1
		
			intPoint = Mathf.Floor (point);
			weight = point - intPoint;
		
			intPoint += intPoint > 0 ? 0 : (Mathf.Floor (Mathf.Abs (intPoint) / points.Count) + 1) * points.Count;
			c [0] = (int)((intPoint - 1) % points.Count);
			c [1] = (int)((intPoint) % points.Count);
			c [2] = (int)((intPoint + 1) % points.Count);
			c [3] = (int)((intPoint + 2) % points.Count);
		
//			v.x = Curve.Utils.interpolate (points [c [0]].x, points [c [1]].x, points [c [2]].x, points [c [3]].x, weight);
//			v.y = Curve.Utils.interpolate (points [c [0]].y, points [c [1]].y, points [c [2]].y, points [c [3]].y, weight);
//			v.z = Curve.Utils.interpolate (points [c [0]].z, points [c [1]].z, points [c [2]].z, points [c [3]].z, weight);
			
			Vector3 pt0 = points [c [0]],
			pt1 = points [c [1]],
			pt2 = points [c [2]],
			pt3 = points [c [3]];
			
			v.x = Curve.Utils.interpolate (pt0.x, pt1.x, pt2.x, pt3.x, weight);
			v.y = Curve.Utils.interpolate (pt0.y, pt1.y, pt2.y, pt3.y, weight);
			v.z = Curve.Utils.interpolate (pt0.z, pt1.z, pt2.z, pt3.z, weight);
		
			return v;
		
		}



		/*
	public override Vector3 getSpacedPoints(float t){
		return getPoint(t);
	}
	*/
//

	}
}
