using UnityEngine;
using System.Collections;

namespace THREE
{
	public class LineCurve3 : Curve
	{
/**************************************************************
 *	Line3D
 **************************************************************/
	
		public Vector3 v1;
		public Vector3 v2;
	
		public LineCurve3 (Vector3 v1, Vector3 v2)
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
	}
}

