using UnityEngine;
using System.Collections;

namespace THREE
{
/**************************************************************
 *	Line
 **************************************************************/
	public class LineCurve : Curve
	{

		public Vector3 v1;
		public Vector3 v2;

		public LineCurve (Vector3 v1, Vector3 v2)
		{
		
			this.v1 = v1;
			this.v2 = v2;
		}

		public override Vector3 getPoint (float t)
		{
		
			Vector3 point = this.v2 - this.v1;
			point = (point * t) + (this.v1);
		
			return point;
		}
		// Line curve is linear, so we can overwrite default getPointAt
	
		public override Vector3 getPointAt (float u)
		{
			return this.getPoint (u);
		}
	
		public override Vector3 getTangent (float t)
		{
		
			Vector3 tangent = this.v2 - (this.v1);
		
			return tangent.normalized;
		}
	}
}
