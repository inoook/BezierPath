using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class SplineCurve3 : Curve
	{

		List<Vector3> points;
	
		public SplineCurve3 (List<Vector3> points)
		{
		
			this.points = (points == null) ? new List<Vector3> () : points;
		}

		public override Vector3 getPoint (float t)
		{
			Vector3 v = new Vector3 ();
			int[] c = new int[4];
			List<Vector3> points = this.points;

			int intPoint;
			float weight;
			float point = (float)(points.Count - 1) * t;
		
			intPoint = Mathf.FloorToInt (point);
			weight = point - intPoint;
		
			c [0] = intPoint == 0 ? intPoint : intPoint - 1;
			c [1] = intPoint;
			c [2] = intPoint > points.Count - 2 ? points.Count - 1 : intPoint + 1;
			c [3] = intPoint > points.Count - 3 ? points.Count - 1 : intPoint + 2;
		
			Vector3 pt0 = points [c [0]],
			pt1 = points [c [1]],
			pt2 = points [c [2]],
			pt3 = points [c [3]];
		
			v.x = Curve.Utils.interpolate (pt0.x, pt1.x, pt2.x, pt3.x, weight);
			v.y = Curve.Utils.interpolate (pt0.y, pt1.y, pt2.y, pt3.y, weight);
			v.z = Curve.Utils.interpolate (pt0.z, pt1.z, pt2.z, pt3.z, weight);
		
			return v;
		
		}

	}
}
