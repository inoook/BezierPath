using UnityEngine;
using System.Collections;

namespace THREE
{
	public class QuadraticBezierCurve3 : Curve
	{

		Vector3 v0;
		Vector3 v1;
		Vector3 v2;

		public QuadraticBezierCurve3 (Vector3 v0, Vector3 v1, Vector3 v2)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.v2 = v2;
		}

		public override Vector3 getPoint (float t)
		{
		
			float tx, ty, tz;
		
			tx = Shape.Utils.b2 (t, this.v0.x, this.v1.x, this.v2.x);
			ty = Shape.Utils.b2 (t, this.v0.y, this.v1.y, this.v2.y);
			tz = Shape.Utils.b2 (t, this.v0.z, this.v1.z, this.v2.z);
		
			return new Vector3 (tx, ty, tz);
		}
	}
}
